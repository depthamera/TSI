using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace TSI.Core.Scene
{
    public class StandardCustomSceneManager : ICustomSceneManager
    {
        private readonly Dictionary<SceneHandle, SceneNode> _nodes = new();
        private readonly Dictionary<SceneRef, int> _instanceCounters = new();

        public async UniTask<SceneHandle> LoadScene(SceneRef target, SceneHandle parent, SceneTransitionOptions options = default)
        {
            // 1) 대상 씬 로드 시작 (메모리 적재만 진행)
            var loadOp = Addressables.LoadSceneAsync(target.ScenePath, activateOnLoad: false, loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);

            AsyncOperationHandle<SceneInstance> loadingSceneHandle = default;
            bool hasLoadingScreen = options.LoadingSceneProfile != null;

            try
            {
                // 로딩 씬 처리 시작
                if (hasLoadingScreen)
                {
                    loadingSceneHandle = Addressables.LoadSceneAsync(
                        options.LoadingSceneProfile.SceneRef.ScenePath,
                        activateOnLoad: true,
                        loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);

                    var loadingSceneInstance = await loadingSceneHandle.ToUniTask();
                    var loadingScreen = FindLoadingScreen(loadingSceneInstance.Scene);

                    // 로딩창 등장 연출 완료 대기
                    await loadingScreen.Show();

                    // 대상 씬 로딩 루프
                    while (!loadOp.IsDone)
                    {
                        // 팁: activateOnLoad가 false일 때 PercentComplete는 최대 0.9까지만 올라갑니다.
                        // 유저에게 90%가 아닌 100%로 보이기 위해 비율을 보정합니다.
                        float progress = Mathf.Clamp01(loadOp.PercentComplete / 0.9f);
                        loadingScreen.SetProgress(progress);

                        await UniTask.Yield();
                    }
                }
                else
                {
                    // 로딩창이 없다면 백그라운드 로드 완료까지 단순히 대기
                    await loadOp.ToUniTask();
                }

                // 2) 핸들 생성 & 트리 등록 (로드가 완료되어 데이터가 확보된 시점)
                var nextId = _instanceCounters.TryGetValue(target, out var count) ? count + 1 : 1;
                _instanceCounters[target] = nextId;

                var sceneHandle = new SceneHandle(target.ScenePath, nextId);
                _nodes[sceneHandle] = new SceneNode(loadOp);
                RegisterNode(parent, sceneHandle);

                // 3) ★활성화 (로딩창이 눈앞을 가리고 있는 안전한 상태에서 활성화 수행)
                var loadedSceneInstance = loadOp.Result;
                await loadedSceneInstance.ActivateAsync().ToUniTask();

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
                Debug.LogError($"씬 로드 중 오류 발생: {ex.Message}");
                throw;
            }
            finally
            {
                // 5) 안전장치: 성공하든 실패하든 생성된 로딩 씬 핸들은 여기서 확실하게 언로드
                if (hasLoadingScreen && loadingSceneHandle.IsValid())
                {
                    await Addressables.UnloadSceneAsync(loadingSceneHandle);
                }
            }
        }

        public UniTask<SceneHandle> ReplaceScene(SceneRef target, SceneHandle toReplace, SceneTransitionOptions options = default)
        {
            throw new System.NotImplementedException();
        }

        public UniTask ResumeScene(SceneHandle target)
        {
            throw new System.NotImplementedException();
        }

        public UniTask SuspendScene(SceneHandle target)
        {
            throw new System.NotImplementedException();
        }

        public UniTask UnloadScene(SceneHandle target)
        {
            throw new System.NotImplementedException();
        }
        public SceneHandle RegisterRootScene(UnityEngine.SceneManagement.Scene bootstrapScene)
        {
            var handle = new SceneHandle(bootstrapScene.path, 0);
            _nodes[handle] = new SceneNode(bootstrapScene);
            return handle;
        }

        private void RegisterNode(SceneHandle parent, SceneHandle child)
        {
            _nodes[parent].Children.Add(child);
            _nodes[child].Parent = parent;
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
    }
}
