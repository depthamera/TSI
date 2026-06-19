using MessagePipe;
using R3;
using System;
using System.Collections.Generic;
using TSI.Core.Physics;
using UnityEngine;

namespace TSI.Core { 
    public class PhysicsInterpolationManager : IDisposable
    {
        private readonly List<RigidbodyEntry> _entries = new();
        private readonly R3.DisposableBag _disposables;

        private class RigidbodyEntry
        {
            public IPhysicsBody Body;
            public Transform RenderTransform;
            public Vector3 PrevPosition;
            public Vector3 CurrentPosition;
        }

        public PhysicsInterpolationManager(ISubscriber<PhysicsInterpolationMessage> subscriber)
        {
            subscriber
                .Subscribe(OnInterpoationTick)
                .AddTo(ref _disposables);
        }

        public void Register(IPhysicsBody body, Transform renderTransform)
        {
            _entries.Add(new RigidbodyEntry
            {
                Body = body,
                RenderTransform = renderTransform,
                PrevPosition = body.Position,
                CurrentPosition = body.Position,
            });
        }

        public void Unregister(IPhysicsBody body)
        {
            _entries.RemoveAll(e => e.Body == body);
        }

        private void OnInterpoationTick(PhysicsInterpolationMessage message)
        {
            foreach (var entry in _entries)
            {
                if(message.StepCount > 0)
                {
                    entry.PrevPosition = entry.CurrentPosition;
                    entry.CurrentPosition = entry.Body.Position;
                }

                entry.RenderTransform.position = Vector3.Lerp(entry.PrevPosition, entry.CurrentPosition, message.Alpha);          
            }               
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
