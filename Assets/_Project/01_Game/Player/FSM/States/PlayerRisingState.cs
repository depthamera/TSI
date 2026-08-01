using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 상승 상태. Airborne SuperState 하위.
    /// 로직 없음 — 물리 파라미터는 MovementOverride로 선언적 설정.
    /// parentState = PlayerAirborneState (인스펙터 설정).
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Rising", menuName = "TSI/FSM/States/Player/Rising State")]
    public class PlayerRisingState : FSMState
    {
    }
}
