using UnityEngine;

namespace TSI.Core
{
    [CreateAssetMenu(menuName = "TSI/Time/ProjectTimeSettings")]
    public class ProjectTimeSettingsSO : ScriptableObject
    {
        public TimeLayerSO PhysicsLayer;
    }
}
