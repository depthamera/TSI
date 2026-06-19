using System.Collections.Generic;
using TSI.Core.Physics;
using MessagePipe;
using UnityEngine;

namespace TSI.Core.Time
{
    public class PhysicsClock : FixedClock
    {
        private readonly IPublisher<PhysicsInterpolationMessage> _interpolationPublisher;

        public PhysicsClock(IPublisher<TimeLayerSO, TickMessage> tickPublisher, TimeLayerSO layer, 
            IPublisher<PhysicsInterpolationMessage> interpolationPublisher) 
            : base(tickPublisher, layer) 
        { 
            _interpolationPublisher = interpolationPublisher;
        }

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
                
                UnityEngine.Physics.Simulate(FixedTimestep);
         
                OnAfterStep(FixedTimestep);
            }
            
            var alpha = AccumulatedDeltaTime / FixedTimestep;
            _interpolationPublisher.Publish(new PhysicsInterpolationMessage(alpha, currentSteps));
        }
    }
}