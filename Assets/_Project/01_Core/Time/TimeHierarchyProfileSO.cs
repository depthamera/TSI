using System;
using System.Collections.Generic;
using UnityEngine;

namespace TSI.Core
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

    [CreateAssetMenu(fileName = "TimeHierarchyProfile", menuName = "TSI/Time/HierarchyProfile")]
    public class TimeHierarchyProfileSO : ScriptableObject
    {
        public List<ClockNodeData> Nodes = new();
    }
}
