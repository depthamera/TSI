using MessagePipe;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TSI.Core
{
    public class PhysicsInterpolationManager : IDisposable
    {
        private readonly List<InterpolationEntry> _entries = new();
        private readonly R3.DisposableBag _disposables;

        private class InterpolationEntry
        {
            public Transform Transform;
            public Func<Vector3> GetPhysicsPosition;
            public Vector3 PrevPosition;
            public Vector3 CurrentPosition;
        }

        public PhysicsInterpolationManager(ISubscriber<PhysicsInterpolationMessage> subscriber)
        {
            subscriber
                .Subscribe(OnInterpolationTick)
                .AddTo(ref _disposables);
        }

        public void Register(Transform transform, Func<Vector3> getPhysicsPosition)
        {
            var position = getPhysicsPosition();
            _entries.Add(new InterpolationEntry
            {
                Transform = transform,
                GetPhysicsPosition = getPhysicsPosition,
                PrevPosition = position,
                CurrentPosition = position,
            });
        }

        public void Unregister(Transform transform)
        {
            _entries.RemoveAll(e => e.Transform == transform);
        }

        private void OnInterpolationTick(PhysicsInterpolationMessage message)
        {
            foreach (var entry in _entries)
            {
                if (message.StepCount > 0)
                {
                    entry.PrevPosition = entry.CurrentPosition;
                    entry.CurrentPosition = entry.GetPhysicsPosition();
                }

                entry.Transform.position = Vector3.Lerp(
                    entry.PrevPosition, entry.CurrentPosition, message.Alpha);
            }
        }

        public void RestorePositions()
        {
            foreach (var entry in _entries)
            {
                entry.Transform.position = entry.CurrentPosition;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
