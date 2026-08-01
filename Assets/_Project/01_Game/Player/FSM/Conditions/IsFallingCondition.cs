using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 하강 판정 조건. velocity.y < 0 이고 지면에 없을 때 true.
    /// Jump → Falling 자연 정점 전이에 사용 (풀홀드 시 정점 도달).
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_IsFalling", menuName = "TSI/FSM/Conditions/Is Falling")]
    public class IsFallingCondition : FSMCondition
    {
        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.Rigidbody.linearVelocity.y < 0f && !bb.IsGrounded;
        }
    }
}
