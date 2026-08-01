using System.Collections.Generic;
using UnityEngine;

namespace TSI.Game.FSM
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_table.png")]
    [CreateAssetMenu(fileName = "TT_New", menuName = "TSI/FSM/Transition Table")]
    public class FSMTransitionTable : ScriptableObject
    {
        [SerializeField] private FSMState initialState;
        [SerializeField] private List<FSMTransition> transitions = new();

        public FSMState InitialState => initialState;

        /// <summary>
        /// 런타임 초기화 시 1회 호출. from State별로 그룹핑 + priority 내림차순 정렬.
        /// HFSM에서는 Any State(from=null) 없이, 모든 전이가 명시적 from State를 가짐.
        /// </summary>
        public Dictionary<FSMState, List<FSMTransition>> BuildLookup()
        {
            var lookup = new Dictionary<FSMState, List<FSMTransition>>();

            foreach (var t in transitions)
            {
                if (t.From == null)
                {
                    Debug.LogWarning(
                        $"[FSM] Transition with null 'From' detected in '{name}'. Skipping. " +
                        "HFSM에서는 Any State 대신 Root SuperState에 전이를 등록하세요.", this);
                    continue;
                }

                if (!lookup.TryGetValue(t.From, out var list))
                {
                    list = new List<FSMTransition>();
                    lookup[t.From] = list;
                }

                list.Add(t);
            }

            foreach (var list in lookup.Values)
                list.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            return lookup;
        }
    }
}
