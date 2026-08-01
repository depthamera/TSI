using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using TSI.Core.Scene;

namespace TSI.Editor
{
    /// <summary>
    /// Addressables 설정이 변경될 때 모든 SceneProfile SO의 scenePath를 자동 갱신합니다.
    /// EntryModified(주소 변경), EntryAdded, EntryRemoved, EntryMoved 등의 이벤트에 반응합니다.
    /// </summary>
    [InitializeOnLoad]
    internal static class SceneProfileAddressableSync
    {
        static SceneProfileAddressableSync()
        {
            AddressableAssetSettings.OnModificationGlobal += OnAddressableModification;
        }

        private static void OnAddressableModification(
            AddressableAssetSettings settings,
            AddressableAssetSettings.ModificationEvent evt,
            object data)
        {
            // 주소 관련 변경 이벤트만 필터링
            switch (evt)
            {
                case AddressableAssetSettings.ModificationEvent.EntryModified:
                case AddressableAssetSettings.ModificationEvent.EntryAdded:
                case AddressableAssetSettings.ModificationEvent.EntryRemoved:
                case AddressableAssetSettings.ModificationEvent.EntryMoved:
                case AddressableAssetSettings.ModificationEvent.EntryCreated:
                    RefreshAllSceneProfiles();
                    break;
            }
        }

        private static void RefreshAllSceneProfiles()
        {
            var guids = AssetDatabase.FindAssets("t:SceneProfile");

            if (guids.Length == 0) return;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var profile = AssetDatabase.LoadAssetAtPath<SceneProfile>(path);

                if (profile != null)
                {
                    // scenePath가 변경된 경우에만 dirty 마킹 (RefreshAddressablePath 내부에서 처리)
                    profile.RefreshAddressablePath();
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}

