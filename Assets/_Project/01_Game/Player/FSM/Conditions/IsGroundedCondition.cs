using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 지면 판정 조건. expectedValue로 양방향 커버.
    /// true: 지면에 있는가 (Airborne → Grounded 착지).
    /// false: 공중인가 (Grounded → Falling 절벽 낙하).
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_IsGrounded", menuName = "TSI/FSM/Conditions/Is Grounded")]
    public class IsGroundedCondition : FSMCondition
    {
        [SerializeField] private bool expectedValue = true;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.IsGrounded == expectedValue;
        }
    }
}
