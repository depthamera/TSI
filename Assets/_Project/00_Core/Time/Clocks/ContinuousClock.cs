using System;
using MessagePipe;
using VContainer.Unity;

namespace TSI.Core.Time
{
    public class ContinuousClock : ClockBase
    {
        public ContinuousClock(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer, IPublisher<TimeLayerSO, TimeStateMessage> statePublisher) : base(publisher, layer, statePublisher) {}
        
        internal override void Tick(float deltaTime)
        {
            if(IsPaused) return;
            
            DeltaTime = deltaTime * TimeScale;
            TickPublisher?.Publish(Layer, new TickMessage(DeltaTime));

            foreach (var child in Children)
            {
                child.Tick(DeltaTime);
            }
        }
    }
}