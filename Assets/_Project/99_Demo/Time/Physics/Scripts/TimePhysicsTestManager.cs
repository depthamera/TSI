using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TSI.Core.Time;
using MessagePipe;
using R3;
using UnityEngine;

using VContainer;
using VContainer.Unity;
using TSI.Core;

namespace TSI.Demo.Time
{
    public class TimePhysicsTestManager : MonoBehaviour
    {
        public Rigidbody[] rigidbodies;
        public Transform sphereTransform;

        public Vector3 startPos;
        public Vector3 endPos;

        public float moveSpeed;

        public SerializableReactiveProperty<float> timeStep = new(0.02f);

        public TimeLayerSO gamePlayLayer;

        private readonly Dictionary<Rigidbody, bool> _rigidReturnMap = new();
        private bool _isTransformReturning = false;
        private IFixedClock _fixedClock;

        [Inject]
        public void Construct(ISubscriber<TimeLayerSO, TickMessage> subscriber, TimeManager manager, ProjectTimeSettingsSO projectTimeSettings)
        {
            subscriber
                .Subscribe(projectTimeSettings.PhysicsLayer, MoveRigidbodies)
                .AddTo(destroyCancellationToken);
        
            subscriber
                .Subscribe(gamePlayLayer, MoveTransform)
                .AddTo(destroyCancellationToken);

            _fixedClock = manager.GetFixedClock(projectTimeSettings.PhysicsLayer);  
        }

        private void Awake()
        {
            foreach (var rigid in rigidbodies)
            {
                _rigidReturnMap.Add(rigid, false);
            }

            timeStep
                .Subscribe(v => _fixedClock.FixedTimestep = v)
                .AddTo(destroyCancellationToken);
        }
    
        private void MoveRigidbodies(TickMessage message)
        {
            foreach (var r in rigidbodies)
            {
                if (!_rigidReturnMap.TryGetValue(r, out var isReturning)) continue;
            
                var target = isReturning ? startPos : endPos;
                target.y = r.position.y;

                var nextPosition = Vector3.MoveTowards(r.position, target, moveSpeed * message.DeltaTime);
                r.MovePosition(nextPosition);

                if (nextPosition == target)
                    _rigidReturnMap[r] = !isReturning;

            }
        }

        private void MoveTransform(TickMessage message)
        {
            var target = _isTransformReturning ? startPos : endPos;
            target.y = sphereTransform.position.y;

            var nextPosition = Vector3.MoveTowards(sphereTransform.position, target, moveSpeed * message.DeltaTime);
            sphereTransform.position = nextPosition;

            if (nextPosition == target)
                _isTransformReturning = !_isTransformReturning;
        }
    }
}
