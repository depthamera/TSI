using UnityEngine;

namespace TSI.Core.Physics
{
    public interface IPhysicsBody
    {
        Vector3 Position { get; }
        void MovePosition(Vector3 position);
    }
}