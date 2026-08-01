using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 점프 Action State. Action Region에 속함.
    /// OnEnter에서 상승 impulse를 부여하고, OnExit에서 가변 점프 높이를 위한 jumpCut 적용.
    /// Posture 전이(Grounded→Airborne)는 물리 조건에 의해 자동 발생.
    /// 
    /// 전이:
    /// - None → JumpAction: KeyPressed(Jump) + HasTag(Grounded)
    /// - JumpAction → None: KeyHeld(Jump, false)
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_JumpAction", menuName = "TSI/FSM/States/Player/Jump Action")]
    public class PlayerJumpAction : FSMState
    {
        [Header("Jump")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float jumpCutMultiplier = 0.5f;

        public override void OnEnter(AgentBlackboard bb)
        {
            var vel = bb.Rigidbody.linearVelocity;
            vel.y = jumpForce;
            bb.Rigidbody.linearVelocity = vel;
        }

        public override void OnExit(AgentBlackboard bb)
        {
            // 아직 상승 중에 퇴장 = 조기 릴리즈 → 속도 감쇠
            if (bb.Rigidbody.linearVelocity.y > 0)
            {
                var vel = bb.Rigidbody.linearVelocity;
                vel.y *= jumpCutMultiplier;
                bb.Rigidbody.linearVelocity = vel;
            }
        }
    }
}
