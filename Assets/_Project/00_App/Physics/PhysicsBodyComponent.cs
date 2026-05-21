using System;
using TSI.Core.Physics;
using TSI.Core.Time;
using UnityEngine;
using VContainer;

namespace TSI.App.Physics
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Physics/Custom Interpolation")]
    public class PhysicsBodyComponent : MonoBehaviour
    {
        [SerializeField] private Transform renderTransform;

        private IPhysicsBody _body;
        
        [Inject]
        public void Initialize(TimeManager timeManager)
        {
            if (TryGetComponent<Rigidbody>(out var rb3d))
                _body = new PhysicsBody3D(rb3d);
            else if(TryGetComponent<Rigidbody2D>(out var rb2d))
                _body = new PhysicsBody2D(rb2d);

            if (!renderTransform)
            {
                renderTransform = transform.GetChild(0).GetComponent<Transform>();
            }
            
            var physicsClock = timeManager.GetClock<PhysicsClock>(TimeLayer.Physics);
            physicsClock.Register(_body, renderTransform);
        }
    }
}