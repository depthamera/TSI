using System;
using MessagePipe;
using VContainer.Unity;

namespace TSI.Core.Time
{
    public class ContinuousClock : ClockBase
    {
        public ContinuousClock(IPublisher<TimeLayer, TickMessage> publisher, TimeLayer layer) : base(publisher, layer) {}
        
        internal override void Tick(float deltaTime)
        {
            if(IsPaused) return;
            
            DeltaTime = deltaTime * TimeScale;
            Publisher?.Publish(Layer, new TickMessage(DeltaTime));

            foreach (var child in Children)
            {
                child.Tick(DeltaTime);
            }
        }
    }
}