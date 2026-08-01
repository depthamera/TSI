using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 하강 상태. 점프 후 정점을 지나거나 절벽에서 떨어질 때 진입.
    /// 이동 행동은 MovementOverride로 선언적 설정 (코드 로직 없음).
    /// parentState = PlayerAirborneState (인스펙터 설정).
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Falling", menuName = "TSI/FSM/States/Player/Falling State")]
    public class PlayerFallingState : FSMState
    {
    }
}
