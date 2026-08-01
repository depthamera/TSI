using System;
using System.Collections.Generic;
using UnityEngine;

namespace TSI.Core.Time
{
    public enum ClockType
    {
        Continuous,
        Fixed,
        Physics,
        Input
    }

    [Serializable]
    public class ClockNodeData
    {
        public TimeLayerSO LayerKey;
        public ClockType ClockType;
        public int Depth;
    }

    [Icon("Assets/_Project/02_Editor/Icons/icon_so_config.png")]
    [CreateAssetMenu(fileName = "TimeProfile_New", menuName = "TSI/Time/HierarchyProfile")]
    public class TimeHierarchyProfileSO : ScriptableObject
    {
        public List<ClockNodeData> Nodes = new();
    }
}
