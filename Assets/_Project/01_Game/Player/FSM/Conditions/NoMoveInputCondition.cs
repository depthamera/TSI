using TSI.Game.FSM;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 이동 입력이 없는지 판단. Walk → Idle 전이에 사용.
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_NoMoveInput", menuName = "TSI/FSM/Conditions/Player/No Move Input")]
    public class NoMoveInputCondition : FSMCondition
    {
        [SerializeField] private InputActionKeySO moveKey;
        [SerializeField] private float threshold = 0.01f;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.InputProvider.ReadAxis(moveKey).sqrMagnitude <= threshold;
        }
    }
}
