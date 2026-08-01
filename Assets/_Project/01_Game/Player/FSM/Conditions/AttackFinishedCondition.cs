using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 공격이 완료되었는지 판단. Attack → Idle 전이에 사용.
    /// AttackModule.IsFinished를 확인 (타이머 기반).
    /// </summary>
    [CreateAssetMenu(fileName = "Cond_AttackFinished", menuName = "TSI/FSM/Conditions/Player/Attack Finished")]
    public class AttackFinishedCondition : FSMCondition
    {
        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.GetModule<AttackModule>().IsFinished;
        }
    }
}
