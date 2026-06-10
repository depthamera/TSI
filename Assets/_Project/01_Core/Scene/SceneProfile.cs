using UnityEngine;

namespace TSI.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneProfile", menuName = "TSI/Scene/Scene Profile")]
    public class SceneProfile : ScriptableObject
    {
        [SerializeField] private string scenePath;
        public string ScenePath => scenePath;
    }
}
