using UnityEngine;

namespace TSI.Game.FSM
{
    /// <summary>
    /// 범용 태그 체크 Condition. 프레임워크 레벨에 위치.
    /// Blackboard에 특정 태그가 활성/비활성인지 확인.
    /// Cross-Region 조회 (예: Action에서 Posture 태그 확인)에 사용.
    /// </summary>
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_condition.png")]
    [CreateAssetMenu(fileName = "Cond_HasTag", menuName = "TSI/FSM/Conditions/Has Tag")]
    public class HasTagCondition : FSMCondition
    {
        [SerializeField] private StateTagSO tag;
        [SerializeField] private bool expectedValue = true;

        public override bool Evaluate(AgentBlackboard bb)
        {
            return bb.HasTag(tag) == expectedValue;
        }
    }
}
