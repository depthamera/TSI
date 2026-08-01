using System;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.FSM
{
    [Serializable]
    public class MovementController
    {
        [SerializeField] private InputActionKeySO moveActionKey;

        /// <summary>
        /// 현재 State의 MovementConfig에 따라 이동 처리.
        /// FSMRunner가 매 tick 끝에 호출하여 실행 순서를 보장.
        /// moveActionKey로 Blackboard의 InputProvider에서 직접 입력을 읽음.
        /// </summary>
        public void Tick(AgentBlackboard bb, float dt)
        {
            var config = bb.CurrentMovementConfig;
            var inputDir = bb.InputProvider.ReadAxis(moveActionKey);

            // 입력이 있으면 방향 갱신 (이동 불가 상태에서도 방향은 전환 가능)
            if (inputDir.x != 0)
                bb.FacingDirection = inputDir.x > 0 ? 1 : -1;

            if (config.CanMove)
            {
                var velocity = bb.Rigidbody.linearVelocity;
                velocity.x = inputDir.x * bb.Stats.BaseSpeed * config.SpeedMultiplier;
                bb.Rigidbody.linearVelocity = velocity;
            }

            bb.Rigidbody.gravityScale = config.GravityScale;

            // RootMotion 처리는 추후 애니메이션 시스템 연동 시 구현
        }
    }
}
