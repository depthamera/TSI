using System;
using System.Collections.Generic;
using UnityEngine;

namespace TSI.Game.FSM
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_state.png")]
    public abstract class FSMState : ScriptableObject
    {
        [Header("Hierarchy")]
        [Tooltip("부모 State. null이면 Root State.")]
        [SerializeField] private FSMState parentState;
        [Tooltip("기본 자식 State. null이면 Leaf State. SuperState 진입 시 이 자식으로 재귀 추적.")]
        [SerializeField] private FSMState defaultChildState;
        [Tooltip("Orthogonal Region 목록. 설정 시 defaultChildState 대신 Region별 독립 FSM을 실행.")]
        [SerializeField] private List<FSMRegion> regions;

        [Header("Tags")]
        [Tooltip("이 State 진입 시 Blackboard에 추가되는 태그 목록. 퇴장 시 자동 제거.")]
        [SerializeField] private StateTagSO[] tags;

        [Header("Movement")]
        [SerializeField] private MovementConfigOverride movementOverride;

        public FSMState ParentState => parentState;
        public bool IsLeaf => defaultChildState == null && !HasRegions;
        public MovementConfigOverride MovementOverride => movementOverride;

        /// <summary>
        /// Orthogonal Region을 보유한 SuperState인지 여부.
        /// true이면 defaultChildState는 무시되고 각 Region이 독립적으로 실행됨.
        /// </summary>
        public bool HasRegions => regions != null && regions.Count > 0;
        public IReadOnlyList<FSMRegion> Regions => regions;
        public IReadOnlyList<StateTagSO> Tags => tags != null ? tags : Array.Empty<StateTagSO>();

        /// <summary>
        /// SuperState 진입 시 최종 Leaf State 결정.
        /// defaultChildState를 재귀적으로 추적하여 최하위 Leaf 반환.
        /// Region 소유 SuperState에서는 자신을 반환 (Region 처리는 Runner가 담당).
        /// 필요 시 하위 클래스에서 오버라이드하여 조건부 자식 결정 가능.
        /// </summary>
        public virtual FSMState GetLeafState(AgentBlackboard bb)
        {
            // Region 소유 SuperState에서는 여기서 멈춤
            if (HasRegions)
                return this;

            return defaultChildState == null
                ? this
                : defaultChildState.GetLeafState(bb);
        }

        /// <summary>
        /// Root 부모부터 자신까지의 계층 리스트 반환. [Root, ..., Parent, Self]
        /// </summary>
        public List<FSMState> GetAncestorChain()
        {
            var chain = new List<FSMState>();
            var current = this;
            int safetyCounter = 0;

            while (current != null)
            {
                chain.Add(current);
                current = current.parentState;

                if (++safetyCounter > 32)
                {
                    Debug.LogError($"[FSM] GetAncestorChain: 순환 참조 감지. State '{name}'의 parentState 설정을 확인하세요.", this);
                    break;
                }
            }

            chain.Reverse();
            return chain;
        }

        /// <summary>
        /// 특정 State의 하위 자식인지 확인.
        /// </summary>
        public bool IsChildOf(FSMState ancestor)
        {
            var current = parentState;
            while (current != null)
            {
                if (current == ancestor) return true;
                current = current.parentState;
            }
            return false;
        }

        /// <summary>
        /// State 진입 시 호출. 모듈 캐싱 등 초기화 수행.
        /// </summary>
        public virtual void OnEnter(AgentBlackboard bb) { }

        /// <summary>
        /// 매 프레임 호출. dt는 Custom Time 적용 후의 delta.
        /// </summary>
        public virtual void OnTick(AgentBlackboard bb, float dt) { }

        /// <summary>
        /// State 퇴장 시 호출. 정리 작업 수행.
        /// </summary>
        public virtual void OnExit(AgentBlackboard bb) { }
    }
}
