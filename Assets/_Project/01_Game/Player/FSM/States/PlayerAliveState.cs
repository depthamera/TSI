using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 플레이어의 최상위 Root SuperState.
    /// 모든 살아있는 상태(Grounded, Attack 등)의 부모.
    /// MovementOverride에서 전체 기본값(CanMove, SpeedMultiplier, GravityScale) 설정.
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Alive", menuName = "TSI/FSM/States/Player/Alive State")]
    public class PlayerAliveState : FSMState
    {
    }
}
