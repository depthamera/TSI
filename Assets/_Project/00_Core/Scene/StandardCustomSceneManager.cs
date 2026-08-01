using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer;
using VContainer.Unity;

namespace TSI.Core.Scene
{
    public class StandardCustomSceneManager : ICustomSceneManager
    {
        private readonly Dictionary<SceneHandle, SceneNode> _nodes = new();
        private readonly Dictionary<SceneProfile, int> _instanceCounters = new();
        private readonly Dictionary<UnityEngine.SceneManagement.Scene, SceneHandle> _sceneToHandle = new();

        public async UniTask<SceneHandle> LoadScene(SceneProfile target, SceneHandle parent, SceneTransitionOptions options = default, Action<IContainerBuilder> extraRegistrations = null)
        {
            if (target == null) throw new System.ArgumentNullException(nameof(target));

            var parentNode = _nodes[parent];
            AsyncOperationHandle<SceneInstance> loadingSceneHandle = default;
            bool hasLoadingScreen = options.LoadingScene != null;

            try
            {
                AsyncOperationHandle<SceneInstance> loadOp;
                if (hasLoadingScreen)
                {
                    // 1) 로딩 씬 로드 (activateOnLoad: true)
                    loadingSceneHandle = Addressables.LoadSceneAsync(
                        options.LoadingScene.ScenePath,
                        activateOnLoad: true,
                        loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);

                    var loadingSceneInstance = await loadingSceneHandle.ToUniTask();
                    var loadingScreen = FindLoadingScreen(loadingSceneInstance.Scene);

                    // 로딩창 등장 연출 완료 대기
                    await loadingScreen.Show();

                    // 2) 타겟 씬 로드 시작 (activateOnLoad: false)
                    loadOp = Addressables.LoadSceneAsync(
                        target.ScenePath,
                        activateOnLoad: false,
                        loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);

                    // 3) 대상 씬 로딩 루프 (최소 로딩 시간 적용)
                    float minimumTime = options.MinimumLoadingTime;
                    float elapsed = 0f;

                    while (!loadOp.IsDone || (minimumTime > 0f && elapsed < minimumTime))
                    {
                        // 로드 중 에러 발생 시 무한 루프 탈출
                        if (loadOp.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
                        {
                            throw new System.Exception($"Scene load failed: {target.ScenePath}");
                        }

                        elapsed += UnityEngine.Time.unscaledDeltaTime;

                        // activateOnLoad가 false일 때 PercentComplete는 최대 0.9까지만 올라감
                        float loadProgress = Mathf.Clamp01(loadOp.PercentComplete / 0.9f);

                        if (minimumTime > 0f)
                        {
                            float timeProgress = Mathf.Clamp01(elapsed / minimumTime);
                            loadingScreen.SetProgress(Mathf.Min(loadProgress, timeProgress));
                        }
                        else
                        {
                            loadingScreen.SetProgress(loadProgress);
                        }

                        await UniTask.Yield(PlayerLoopTiming.Update);
                    }
                }
                else
                {
                    // 로딩창이 없다면 대상 씬 로드 완료까지 단순히 대기
                    loadOp = Addressables.LoadSceneAsync(
                        target.ScenePath,
                        activateOnLoad: false,
                        loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);

                    await loadOp.ToUniTask();

                    if (loadOp.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
                    {
                        throw new System.Exception($"Scene load failed: {target.ScenePath}");
                    }
                }

                // 로드가 완료되어 데이터가 확보된 시점
                var loadedSceneInstance = loadOp.Result;

                // 핸들 생성 & 트리 등록
                var nextId = _instanceCounters.TryGetValue(target, out var count) ? count + 1 : 1;
                _instanceCounters[target] = nextId;

                var sceneHandle = new SceneHandle(target.ScenePath, nextId);
                _nodes[sceneHandle] = new SceneNode(loadOp);
                RegisterNode(parent, sceneHandle, loadedSceneInstance.Scene);

                // ====================================================================
                // 3) ★ 타겟 씬 활성화 & VContainer 의존성 주입 (가장 중요한 부분)
                // ====================================================================
                // 이 using 블록 안에서 오직 '타겟 씬의 Awake()'만 실행되도록 타이트하게 감쌉니다.
                using (LifetimeScope.EnqueueParent(parentNode.LifetimeScope))
                using (LifetimeScope.Enqueue(extraRegistrations ?? (_ => { })))
                {
                    await loadedSceneInstance.ActivateAsync().ToUniTask();
                }
                // ====================================================================

                // ActivateAsync 이후 씬에 생성된 LifetimeScope를 노드에 등록
                _nodes[sceneHandle].LifetimeScope = FindLifetimeScope(loadedSceneInstance.Scene);

                // 4) 로딩창 연출 종료
                if (hasLoadingScreen && loadingSceneHandle.IsValid())
                {
                    var loadingScreen = FindLoadingScreen(loadingSceneHandle.Result.Scene);
                    loadingScreen.SetProgress(1f); // 강제로 100% 채움

                    // 로딩창 아웃 연출 완료 대기 (페이드아웃 등)
                    await loadingScreen.Hide();
                }

                return sceneHandle;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"씬 로드 중 오류 발생: {ex}");
                throw;
            }
            finally
            {
                // 5) 안전장치: 성공하든 실패하든 생성된 로딩 씬 핸들은 여기서 확실하게 언로드
                if (hasLoadingScreen && loadingSceneHandle.IsValid())
                {
                    // UnloadSceneAsync도 비동기이므로 확실히 대기
                    await Addressables.UnloadSceneAsync(loadingSceneHandle, true).ToUniTask();
                }
            }
        }

        public async UniTask<SceneHandle> ReplaceScene(SceneProfile target, SceneHandle toReplace, SceneTransitionOptions options = default, Action<IContainerBuilder> extraRegistrations = null)
        {
            if (!_nodes.TryGetValue(toReplace, out var node))
            {
                Debug.LogWarning($"Scene not found for replace: {toReplace}");
                return default;
            }

            var parent = node.Parent;

            await UnloadScene(toReplace);
            return await LoadScene(target, parent, options, extraRegistrations);
        }

        public UniTask ResumeScene(SceneHandle target)
        {
            throw new System.NotImplementedException();
        }

        public UniTask SuspendScene(SceneHandle target)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask UnloadScene(SceneHandle target)
        {
            if (!_nodes.TryGetValue(target, out var node))
            {
                Debug.LogWarning($"Scene already unloaded: {target}");
                return;
            }

            var childrenSnapshot = new List<SceneHandle>(node.Children);
            foreach (var child in childrenSnapshot)
            {
                await UnloadScene(child);
            }

            UnregisterNode(target);

            await Addressables.UnloadSceneAsync(node.AddressableHandle, true);
        }
        public SceneHandle Resolve(UnityEngine.SceneManagement.Scene scene)
        {
            return _sceneToHandle[scene];
        }
        public SceneHandle GetParent(SceneHandle handle)
        {
            return _nodes[handle].Parent;
        }

        public SceneHandle RegisterRootScene(UnityEngine.SceneManagement.Scene bootstrapScene)
        {
            var scope = FindLifetimeScope(bootstrapScene);

            var handle = new SceneHandle(bootstrapScene.path, 0);
            _nodes[handle] = new SceneNode(bootstrapScene, scope);
            _sceneToHandle[bootstrapScene] = handle;
            return handle;
        }

        private void RegisterNode(SceneHandle parent, SceneHandle child, UnityEngine.SceneManagement.Scene scene)
        {
            _nodes[parent].Children.Add(child);
            _nodes[child].Parent = parent;
            _sceneToHandle[scene] = child;
        }

        private void UnregisterNode(SceneHandle handle)
        {
            if (!_nodes.TryGetValue(handle, out var node)) return;

            _nodes.Remove(handle);
            _sceneToHandle.Remove(node.AddressableHandle.Result.Scene);

            if (_nodes.TryGetValue(node.Parent, out var parentNode))
            {
                parentNode.Children.Remove(handle);
            }
        }

        private ILoadingScreen FindLoadingScreen(UnityEngine.SceneManagement.Scene scene)
        {
            foreach(var go in scene.GetRootGameObjects())
            {
                var loadingScreen = go.GetComponentInChildren<ILoadingScreen>();
                if (loadingScreen != null)
                    return loadingScreen;
            }

            return null;
        }

        private LifetimeScope FindLifetimeScope(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                var lifetimeScope = go.GetComponentInChildren<LifetimeScope>();
                if (lifetimeScope != null)
                    return lifetimeScope;
            }

            return null;
        }
    }
}
