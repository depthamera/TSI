using UnityEngine;

namespace TSI.Game.FSM
{
    /// <summary>
    /// SO 기반 태그 키. InputActionKeySO와 동일한 "빈 SO = 식별자" 패턴.
    /// State, 전투 시스템, 버프 시스템 등에서 에이전트 상태를 태그로 표현.
    /// AgentBlackboard의 HashSet에 추가/제거하여 O(1) 질의 가능.
    /// </summary>
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_config.png")]
    [CreateAssetMenu(fileName = "Tag_New", menuName = "TSI/FSM/State Tag")]
    public class StateTagSO : ScriptableObject { }
}
