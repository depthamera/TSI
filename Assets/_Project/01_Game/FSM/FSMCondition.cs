using UnityEngine;

namespace TSI.Game.FSM
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_condition.png")]
    public abstract class FSMCondition : ScriptableObject
    {
        /// <summary>
        /// 전이 조건 판단. Blackboard 데이터만 읽고, 부수효과 없이 결과 반환.
        /// </summary>
        public abstract bool Evaluate(AgentBlackboard bb);
    }
}
