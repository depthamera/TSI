using System.Collections.Generic;
using TSI.Core.Physics;
using MessagePipe;
using UnityEngine;

namespace TSI.Core.Time
{
    public class PhysicsClock : FixedClock
    {
        private readonly List<RigidbodyEntry> _entries = new();

        private class RigidbodyEntry
        {
            public IPhysicsBody Body;
            public Transform RenderTransform;
            public Vector3 PrevPosition;
            public Vector3 CurrPosition;
        }

        public PhysicsClock(IPublisher<TimeLayer, TickMessage> publisher, TimeLayer layer) 
            : base(publisher, layer) { }

        public void Register(IPhysicsBody body, Transform renderTransform)
        {
            _entries.Add(new RigidbodyEntry
            {
                Body = body,
                RenderTransform = renderTransform,
                PrevPosition = body.Position,
                CurrPosition = body.Position
            });
        }

        public void Unregister(IPhysicsBody body)
        {
            _entries.RemoveAll(e => e.Body == body);
        }

        internal override void Tick(float deltaTime)
        {
            var currentSteps = 0;
            AccumulatedDeltaTime += deltaTime * TimeScale;

            while (AccumulatedDeltaTime >= FixedTimestep && currentSteps < MaxSteps)
            {
                foreach (var entry in _entries)
                    entry.PrevPosition = entry.Body.Position;

                AccumulatedDeltaTime -= FixedTimestep;
                currentSteps++;
                
                Publisher?.Publish(Layer, new TickMessage(FixedTimestep));
                foreach (var child in Children)
                    child.Tick(FixedTimestep);
                
                
                UnityEngine.Physics.Simulate(FixedTimestep);
                
                foreach (var entry in _entries)
                    entry.CurrPosition = entry.Body.Position;

                OnAfterStep(FixedTimestep);
            }
            
            var alpha = AccumulatedDeltaTime / FixedTimestep;
            foreach (var entry in _entries)
                entry.RenderTransform.position = Vector3.Lerp(entry.PrevPosition, entry.CurrPosition, alpha);
        }
    }
}