using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TSI.Editor
{
    /// <summary>
    /// 에디터에서 Play 버튼을 누르면 항상 Bootstrap 씬부터 시작하도록 강제합니다.
    /// Play 모드 종료 시 원래 편집 중이던 씬으로 자동 복귀합니다.
    ///
    /// 사용법: 메뉴 TSI > Always Start From Bootstrap 토글
    /// </summary>
    [InitializeOnLoad]
    internal static class BootstrapPlayMode
    {
        private const string MenuPath = "TSI/Always Start From Bootstrap";
        private const string EnabledPrefKey = "TSI_AlwaysStartFromBootstrap";
        private const string PreviousScenePrefKey = "TSI_PreviousScenePath";

        static BootstrapPlayMode()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(EnabledPrefKey, false);
            set => EditorPrefs.SetBool(EnabledPrefKey, value);
        }

        [MenuItem(MenuPath, priority = 100)]
        private static void ToggleAlwaysStartFromBootstrap()
        {
            IsEnabled = !IsEnabled;

            if (IsEnabled)
            {
                var bootstrapScene = FindBootstrapSceneAsset();
                if (bootstrapScene != null)
                {
                    EditorSceneManager.playModeStartScene = bootstrapScene;
                    Debug.Log($"[BootstrapPlayMode] 활성화 — Play 시 항상 '{bootstrapScene.name}' 씬부터 시작합니다.");
                }
                else
                {
                    Debug.LogWarning("[BootstrapPlayMode] Bootstrap 씬을 찾을 수 없습니다. 프로젝트에 'Bootstrap.unity'가 있는지 확인하세요.");
                    IsEnabled = false;
                }
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
                Debug.Log("[BootstrapPlayMode] 비활성화 — 현재 열린 씬에서 Play합니다.");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, IsEnabled);
            return true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!IsEnabled) return;

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    HandleExitingEditMode();
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    HandleEnteredEditMode();
                    break;
            }
        }

        /// <summary>
        /// Play 모드 진입 직전: 현재 씬 경로를 저장하고 Bootstrap 씬 시작을 보장합니다.
        /// </summary>
        private static void HandleExitingEditMode()
        {
            // 현재 편집 중인 씬 경로 저장
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.IsValid() && !string.IsNullOrEmpty(currentScene.path))
            {
                EditorPrefs.SetString(PreviousScenePrefKey, currentScene.path);
            }

            // playModeStartScene이 해제되었을 수 있으므로 재설정
            var bootstrapScene = FindBootstrapSceneAsset();
            if (bootstrapScene != null)
            {
                EditorSceneManager.playModeStartScene = bootstrapScene;
            }
        }

        /// <summary>
        /// Play 모드 종료 후: 저장해둔 원래 씬으로 복귀합니다.
        /// </summary>
        private static void HandleEnteredEditMode()
        {
            var previousScenePath = EditorPrefs.GetString(PreviousScenePrefKey, string.Empty);

            if (string.IsNullOrEmpty(previousScenePath)) return;

            var currentScene = EditorSceneManager.GetActiveScene();

            // 이미 원래 씬이면 스킵
            if (currentScene.path == previousScenePath) return;

            // 저장되지 않은 변경사항 확인 후 씬 복귀
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(previousScenePath);
            }

            EditorPrefs.DeleteKey(PreviousScenePrefKey);
        }

        /// <summary>
        /// 프로젝트에서 Bootstrap.unity 씬 에셋을 검색합니다.
        /// _Project/01_Game/Scenes/Bootstrap/ 경로를 우선 검색합니다.
        /// </summary>
        private static SceneAsset FindBootstrapSceneAsset()
        {
            // 우선: Game 레이어의 Bootstrap 씬
            const string primaryPath = "Assets/_Project/01_Game/Scenes/Bootstrap/Bootstrap.unity";
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(primaryPath);
            if (asset != null) return asset;

            // 폴백: 프로젝트 전체에서 Bootstrap 씬 검색
            var guids = AssetDatabase.FindAssets("Bootstrap t:SceneAsset");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (asset != null) return asset;
            }

            return null;
        }
    }
}
