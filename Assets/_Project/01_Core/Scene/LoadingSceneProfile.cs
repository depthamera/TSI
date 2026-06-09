using UnityEngine;

namespace TSI.Core.Scene
{
    [CreateAssetMenu(fileName = "LoadingSceneProfile", menuName = "TSI/Scene/Loading Scene Profile")]
    public class LoadingSceneProfile : ScriptableObject
    {
        public SceneRef SceneRef;
    }
}
