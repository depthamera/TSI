
using UnityEngine;

namespace TSI.Game.FSM
{
    [CreateAssetMenu(fileName = "State_Test_Walk", menuName = "TSI/FSM/Test/State/Walk State")]
    public class WalkState : FSMState
    {
        // 이동은 이 State SO 에셋의 MovementOverride(CanMove, SpeedMultiplier)로 설정.
        // MovementController가 입력 방향 × SpeedMultiplier로 실제 이동을 처리.

        public override void OnEnter(AgentBlackboard bb)
        {
            Debug.Log("WalkState OnEnter");
        }
    }
}
