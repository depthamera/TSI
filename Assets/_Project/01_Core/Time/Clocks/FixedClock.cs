using MessagePipe;

namespace TSI.Core.Time
{
    public class FixedClock : FixedClockBase
    {
        protected float AccumulatedDeltaTime { get; set; }
        
        public FixedClock(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer) : base(publisher, layer) { }


        internal override void Tick(float deltaTime)
        {
            var currentSteps = 0;
            
            AccumulatedDeltaTime += deltaTime * TimeScale;

            while (AccumulatedDeltaTime >= FixedTimestep && currentSteps < MaxSteps)
            {
                AccumulatedDeltaTime -= FixedTimestep;
                currentSteps++;
                
                Publisher?.Publish(Layer, new TickMessage(FixedTimestep));

                foreach (var child in Children)
                    child.Tick(FixedTimestep);
                
                OnAfterStep(FixedTimestep);
            }
        }
        
        protected virtual void OnAfterStep(float fixedTimestep) {}
    }
}