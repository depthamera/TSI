using UnityEngine;

namespace TSI.Core.Physics
{
    public class PhysicsBody3D : IPhysicsBody
    {
        private readonly Rigidbody _rigidbody;
        
        public PhysicsBody3D(Rigidbody rigidbody) => _rigidbody =  rigidbody;
        
        public Vector3 Position => _rigidbody.position;

        public void MovePosition(Vector3 position) => _rigidbody.MovePosition(position);
    }
}