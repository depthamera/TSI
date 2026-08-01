using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// Action이 없는 기본 상태. Action Region의 InitialState.
    /// 로직 없음 — 다른 Action이 끝나면 여기로 복귀.
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_ActionNone", menuName = "TSI/FSM/States/Player/Action None")]
    public class PlayerActionNoneState : FSMState
    {
    }
}
