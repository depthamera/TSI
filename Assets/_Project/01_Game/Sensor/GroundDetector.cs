using System;
using UnityEngine;

namespace TSI.Game.Sensor
{
    /// <summary>
    /// BoxCast 기반 지면 판정.
    /// SensorEntity가 인스펙터에서 보유하며, 매 틱 Check()를 호출하여
    /// AgentBlackboard.IsGrounded를 갱신.
    /// </summary>
    [Serializable]
    public class GroundDetector
    {
        [SerializeField] private Vector2 boxSize = new(0.8f, 0.1f);
        [SerializeField] private float castDistance = 0.1f;
        [SerializeField] private LayerMask groundLayers;

        public bool Check(Rigidbody2D rb)
        {
            var origin = (Vector2)rb.transform.position;
            var hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, castDistance, groundLayers);
            return hit.collider != null;
        }
    }
}
