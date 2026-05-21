using UnityEngine;

namespace TSI.Core.Physics
{
    public class PhysicsBody2D : IPhysicsBody
    {
        private readonly Rigidbody2D _rigidbody;
        
        public PhysicsBody2D(Rigidbody2D rigidbody) => _rigidbody = rigidbody;
        
        public Vector3 Position =>  _rigidbody.position;
        public void MovePosition(Vector3 position) => _rigidbody.MovePosition(position);
    }
}