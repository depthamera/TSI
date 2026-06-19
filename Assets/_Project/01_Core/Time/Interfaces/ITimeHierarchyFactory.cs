using System.Collections.Generic;
using MessagePipe;

namespace TSI.Core.Time
{
    public readonly struct TimeHierarchyResult
    {
        public readonly List<ClockBase> RootClocks;
        public readonly Dictionary<TimeLayerSO, IClock> Clocks;

        public TimeHierarchyResult(List<ClockBase> rootClocks, Dictionary<TimeLayerSO, IClock> clocks)
        {
            RootClocks = rootClocks;
            Clocks = clocks;
        }
        
    }
    public interface ITimeHierarchyFactory
    {
        TimeHierarchyResult Create();
    }
}