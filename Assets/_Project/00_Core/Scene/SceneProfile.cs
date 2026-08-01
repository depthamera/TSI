using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
#endif

namespace TSI.Core.Scene
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_config.png")]
    [CreateAssetMenu(fileName = "SceneProfile_New", menuName = "TSI/Scene/Scene Profile")]
    public class SceneProfile : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
#endif


        [SerializeField] private string scenePath;
        public string ScenePath => scenePath;

#if UNITY_EDITOR
        private void OnValidate()
        {
            SyncAddressablePath();
        }

        /// <summary>
        /// Addressables 설정에서 현재 sceneAsset의 주소를 다시 읽어와 scenePath를 갱신합니다.
        /// 값이 실제로 변경된 경우에만 에셋을 dirty 마킹합니다.
        /// </summary>
        public void RefreshAddressablePath()
        {
            if (SyncAddressablePath())
            {
                EditorUtility.SetDirty(this);
            }
        }

        /// <returns>scenePath가 변경되었으면 true</returns>
        private bool SyncAddressablePath()
        {
            string newPath = string.Empty;

            if (sceneAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                if(AddressableAssetSettingsDefaultObject.SettingsExists)
                {
                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    var entry = settings.FindAssetEntry(guid);
                    newPath = entry?.address ?? string.Empty;
                }        
            }

            if (scenePath == newPath) return false;

            scenePath = newPath;
            return true;
        }
#endif
    }
}
