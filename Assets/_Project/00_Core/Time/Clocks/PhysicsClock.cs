using System;
using MessagePipe;
using UnityEngine;

namespace TSI.Core.Time
{
    public class PhysicsClock : FixedClock
    {
        private readonly IPublisher<PhysicsInterpolationMessage> _interpolationPublisher;
        private readonly Action _restoreInterpolatedPositions;

        public PhysicsClock(IPublisher<TimeLayerSO, TickMessage> tickPublisher, TimeLayerSO layer, 
            IPublisher<PhysicsInterpolationMessage> interpolationPublisher,
            IPublisher<TimeLayerSO, TimeStateMessage> statePublisher,
            Action restoreInterpolatedPositions = null) 
            : base(tickPublisher, layer, statePublisher) 
        { 
            _interpolationPublisher = interpolationPublisher;
            _restoreInterpolatedPositions = restoreInterpolatedPositions;
        }

        internal override void Tick(float deltaTime)
        {
            var currentSteps = 0;
            AccumulatedDeltaTime += deltaTime * TimeScale;

            if (AccumulatedDeltaTime >= FixedTimestep)
            {
                _restoreInterpolatedPositions?.Invoke();
                UnityEngine.Physics.SyncTransforms();
                UnityEngine.Physics2D.SyncTransforms();
            }

            while (AccumulatedDeltaTime >= FixedTimestep && currentSteps < MaxSteps)
            {

                AccumulatedDeltaTime -= FixedTimestep;
                currentSteps++;
                
                TickPublisher?.Publish(Layer, new TickMessage(FixedTimestep));
                foreach (var child in Children)
                    child.Tick(FixedTimestep);           
                
                UnityEngine.Physics.Simulate(FixedTimestep);
                UnityEngine.Physics2D.Simulate(FixedTimestep);
         
                OnAfterStep(FixedTimestep);
            }
            
            var alpha = AccumulatedDeltaTime / FixedTimestep;
            _interpolationPublisher.Publish(new PhysicsInterpolationMessage(alpha, currentSteps));
        }
    }
}