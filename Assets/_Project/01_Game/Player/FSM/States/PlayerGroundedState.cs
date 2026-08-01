using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 플레이어 지상 상태 부모 SuperState.
    /// Idle, Walk 등 지상 이동 상태를 그룹핑.
    /// parentState = PlayerAliveState, defaultChildState = PlayerIdleState (인스펙터 설정).
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Grounded", menuName = "TSI/FSM/States/Player/Grounded State")]
    public class PlayerGroundedState : FSMState
    {
    }
}
