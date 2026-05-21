using System;
using System.Collections.Generic;
using UnityEngine;

namespace TSI.Demo.Time
{
    public class NoScriptPhysicsTest : MonoBehaviour
    {
        public Rigidbody[] rigidbodies;

        public Vector3 startPos;
        public Vector3 endPos;

        public float moveSpeed;

        private readonly Dictionary<Rigidbody, bool> _rigidReturnMap = new();

        private void FixedUpdate()
        {
            MoveRigidbodies();
        }

        private void Awake()
        {
            // var loop = PlayerLoop.GetCurrentPlayerLoop();
            // PrintLoop(loop, 0);
        
            foreach (var rigid in rigidbodies)
            {
                _rigidReturnMap.Add(rigid, false);
            }

        }
        private void MoveRigidbodies()
        {
            foreach (var r in rigidbodies)
            {
                if (!_rigidReturnMap.TryGetValue(r, out var isReturning)) continue;
            
                var target = isReturning ? startPos : endPos;
                target.y = r.position.y;

                var nextPosition = Vector3.MoveTowards(r.position, target, moveSpeed * UnityEngine.Time.fixedDeltaTime);
                r.MovePosition(nextPosition);

                if (nextPosition == target)
                    _rigidReturnMap[r] = !isReturning;

            }
        }


    }
}
