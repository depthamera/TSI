using TSI.Game.FSM;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 범용 WasPressed 조건. InputActionKeySO를 인스펙터에서 설정하여
    /// 어떤 키든 WasPressed 체크 가능. AttackPressedCondition을 대체.
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_KeyPressed", menuName = "TSI/FSM/Conditions/Key Pressed")]
    public class KeyPressedCondition : FSMCondition
    {
        [SerializeField] private InputActionKeySO actionKey;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.InputProvider.WasPressed(actionKey);
        }
    }
}
