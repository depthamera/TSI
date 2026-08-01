using MessagePipe;

namespace TSI.Core.Time
{
    public abstract class FixedClockBase : ClockBase, IFixedClock
    {
        public float FixedTimestep { get; set; } = 0.016f;
        public int MaxSteps { get; set; } = 3;

        protected FixedClockBase(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer, IPublisher<TimeLayerSO, TimeStateMessage> statePublisher)
            : base(publisher, layer, statePublisher) { }
    }
}