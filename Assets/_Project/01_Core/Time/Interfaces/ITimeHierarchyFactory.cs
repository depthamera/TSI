using System.Collections.Generic;
using MessagePipe;

namespace TSI.Core.Time
{
    public readonly struct TimeHierarchyResult
    {
        public readonly List<ClockBase> RootClocks;
        public readonly Dictionary<TimeLayer, IClock> Clocks;

        public TimeHierarchyResult(List<ClockBase> rootClocks, Dictionary<TimeLayer, IClock> clocks)
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