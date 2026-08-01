using TSI.Game.FSM;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 범용 IsHeld 조건. expectedValue로 "눌려 있을 때" / "안 눌려 있을 때" 양방향 커버.
    /// expectedValue=false: 버튼 릴리즈 감지 (Jump → Falling 가변 점프 높이에 사용).
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_KeyHeld", menuName = "TSI/FSM/Conditions/Key Held")]
    public class KeyHeldCondition : FSMCondition
    {
        [SerializeField] private InputActionKeySO actionKey;
        [SerializeField] private bool expectedValue = true;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.InputProvider.IsHeld(actionKey) == expectedValue;
        }
    }
}
