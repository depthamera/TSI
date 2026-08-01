using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 공중 상태 SuperState. Jump와 Falling의 부모.
    /// MovementOverride로 공중 이동 파라미터(속도 감쇠, 중력 등) 설정.
    /// parentState = PlayerAliveState, defaultChildState = PlayerJumpState (인스펙터 설정).
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Airborne", menuName = "TSI/FSM/States/Player/Airborne State")]
    public class PlayerAirborneState : FSMState
    {
    }
}
