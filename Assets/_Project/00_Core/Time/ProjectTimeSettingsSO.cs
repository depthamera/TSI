using UnityEngine;

namespace TSI.Core.Time
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_config.png")]
    [CreateAssetMenu(menuName = "TSI/Time/ProjectTimeSettings")]
    public class ProjectTimeSettingsSO : ScriptableObject
    {
        public TimeLayerSO PhysicsLayer;
    }
}
