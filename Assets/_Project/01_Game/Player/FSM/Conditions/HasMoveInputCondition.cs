using TSI.Game.FSM;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 이동 입력이 있는지 판단. Idle → Walk 전이에 사용.
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_HasMoveInput", menuName = "TSI/FSM/Conditions/Player/Has Move Input")]
    public class HasMoveInputCondition : FSMCondition
    {
        [SerializeField] private InputActionKeySO moveKey;
        [SerializeField] private float threshold = 0.01f;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.InputProvider.ReadAxis(moveKey).sqrMagnitude > threshold;
        }
    }
}
