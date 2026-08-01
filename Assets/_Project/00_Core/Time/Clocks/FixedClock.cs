using MessagePipe;

namespace TSI.Core.Time
{
    public class FixedClock : FixedClockBase
    {
        protected float AccumulatedDeltaTime { get; set; }
        
        public FixedClock(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer, IPublisher<TimeLayerSO, TimeStateMessage> statePublisher) : base(publisher, layer, statePublisher) { }


        internal override void Tick(float deltaTime)
        {
            var currentSteps = 0;
            
            AccumulatedDeltaTime += deltaTime * TimeScale;

            while (AccumulatedDeltaTime >= FixedTimestep && currentSteps < MaxSteps)
            {
                AccumulatedDeltaTime -= FixedTimestep;
                currentSteps++;
                
                TickPublisher?.Publish(Layer, new TickMessage(FixedTimestep));

                foreach (var child in Children)
                    child.Tick(FixedTimestep);
                
                OnAfterStep(FixedTimestep);
            }
        }
        
        protected virtual void OnAfterStep(float fixedTimestep) {}
    }
}