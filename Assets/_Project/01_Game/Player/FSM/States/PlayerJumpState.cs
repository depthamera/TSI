using TSI.Game.FSM;
using UnityEngine;
namespace TSI.Game.Player
{
    /// <summary>
    /// 점프 상태. OnEnter에서 수직 속도를 부여하고, OnExit에서 조기 릴리즈 감쇠 처리.
    /// 가변 점프 높이는 FSM 전이로 표현:
    /// - 버튼 릴리즈 → KeyHeldCondition(false) → FallingState 전이 → OnExit에서 velocity 감쇠
    /// - 풀홀드 → IsFallingCondition → FallingState 전이 → velocity.y ≤ 0이므로 감쇠 미적용
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Jump", menuName = "TSI/FSM/States/Player/Jump State")]
    public class PlayerJumpState : FSMState
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
