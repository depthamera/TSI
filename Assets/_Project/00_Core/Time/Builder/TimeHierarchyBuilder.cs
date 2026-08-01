using System.Collections.Generic;

namespace TSI.Core.Time
{
    public class TimeHierarchyBuilder
    {
        private readonly List<ClockBase> _rootClocks = new();
        private readonly Dictionary<TimeLayerSO, IClock> _clocks = new();

        public void RegisterRoot(ClockBase clock)
        {
            _rootClocks.Add(clock);
            _clocks.Add(clock.Layer, clock);
        }

        public void RegisterChild(ClockBase parent, ClockBase child)
        {
            parent.AddChild(child);
            _clocks.Add(child.Layer, child);
        }

        public TimeHierarchyResult Build()
        {
            return new TimeHierarchyResult(_rootClocks, _clocks);
        }
    }
}