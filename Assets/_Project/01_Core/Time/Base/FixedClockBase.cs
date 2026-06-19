using MessagePipe;

namespace TSI.Core.Time
{
    public abstract class FixedClockBase : ClockBase, IFixedClock
    {
        public float FixedTimestep { get; set; } = 0.016f;
        public int MaxSteps { get; set; } = 3;

        protected FixedClockBase(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer)
            : base(publisher, layer) { }
    }
}