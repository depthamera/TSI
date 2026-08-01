using System;
using System.Collections.Generic;
using TSI.Core.Time;
using TSI.Game.Input;
using TSI.Game.Player;
using UnityEngine;

namespace TSI.Game.FSM
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FSMRunner : TimeEntityBase
    {
        [Header("FSM")]
        [SerializeField] private FSMTransitionTable transitionTable;
        [SerializeField] private MovementController movementController = new();

        [Header("Agent")]
        [SerializeField] private AgentStatsSO agentStats;

        [Header("Debug")]
        [SerializeField] private bool debugLog;

        // ── 런타임 데이터 ──

        private AgentBlackboard _blackboard;

        // 공유 체인: Root → Region 소유 SuperState까지 (Region이 없으면 Root→Leaf 전체)
        private List<FSMState> _sharedChain = new();
        private Dictionary<FSMState, List<FSMTransition>> _sharedTransitionLookup;

        // Region 런타임 데이터 (null이면 Region 미사용 = 기존 모드)
        private RuntimeRegion[] _runtimeRegions;

        // 기존 호환용 (Region 미사용 시)
        private FSMState _currentState;

        /// <summary>
        /// 현재 최하위 Leaf State.
        /// Region 모드에서는 마지막 Region의 현재 Leaf를 반환.
        /// </summary>
        public FSMState CurrentState => _runtimeRegions != null
            ? _runtimeRegions[^1].CurrentLeaf
            : _currentState;

        public AgentBlackboard Blackboard => _blackboard;

        /// <summary>
        /// 현재 활성화된 공유 계층 체인. 디버그/에디터용.
        /// </summary>
        public IReadOnlyList<FSMState> ActiveStateChain => _sharedChain;

        // ── 초기화 ──

        private void Awake()
        {
            var inputProvider = GetComponentInChildren<IInputProvider>();
            _blackboard = new AgentBlackboard(
                transform,
                GetComponent<Rigidbody2D>(),
                inputProvider,
                agentStats.CreateRuntime());
        }

        /// <summary>
        /// FSM 시작. 모듈 등록이 필요한 경우 콜백에서 수행.
        /// </summary>
        /// <example>
        /// runner.Initialize(bb => bb.RegisterModule(new AttackModule()));
        /// </example>
        public void Initialize(Action<AgentBlackboard> configureModules = null)
        {
            configureModules?.Invoke(_blackboard);
            _sharedTransitionLookup = transitionTable.BuildLookup();

            var initialState = transitionTable.InitialState;

            // Region 소유 SuperState까지 추적
            var regionOwner = FindRegionOwner(initialState);

            if (regionOwner != null)
            {
                // ── Region 모드 ──
                InitializeWithRegions(initialState, regionOwner);
            }
            else
            {
                // ── 기존 단일 체인 모드 ──
                InitializeWithoutRegions(initialState);
            }

            ResolveMovementConfig();

            if (debugLog)
                Debug.Log($"[FSM] {name}: Initialized → {FormatDebugState()}", this);
        }

        /// <summary>
        /// initialState에서 시작하여 Region을 보유한 SuperState를 찾음.
        /// GetLeafState가 Region 소유 SuperState에서 멈추므로, 결과가 HasRegions이면 반환.
        /// 없으면 null (기존 모드).
        /// </summary>
        private FSMState FindRegionOwner(FSMState state)
        {
            var leaf = state.GetLeafState(_blackboard);
            return leaf.HasRegions ? leaf : null;
        }

        private void InitializeWithoutRegions(FSMState initialState)
        {
            _runtimeRegions = null;

            var initialLeaf = initialState.GetLeafState(_blackboard);
            _sharedChain = initialLeaf.GetAncestorChain();
            _currentState = initialLeaf;

            foreach (var state in _sharedChain)
            {
                state.OnEnter(_blackboard);
                AddTagsForState(state);
            }
        }

        private void InitializeWithRegions(FSMState initialState, FSMState regionOwner)
        {
            // 1. 공유 체인 구축: Root → regionOwner
            _sharedChain = regionOwner.GetAncestorChain();

            // 2. 공유 체인 OnEnter + 태그
            foreach (var state in _sharedChain)
            {
                state.OnEnter(_blackboard);
                AddTagsForState(state);
            }

            // 3. Region별 초기화
            var regions = regionOwner.Regions;
            _runtimeRegions = new RuntimeRegion[regions.Count];

            for (int i = 0; i < regions.Count; i++)
            {
                var regionDef = regions[i];
                var rt = new RuntimeRegion
                {
                    Definition = regionDef,
                    TransitionLookup = regionDef.TransitionTable.BuildLookup(),
                };

                var regionInitial = regionDef.TransitionTable.InitialState;
                var regionLeaf = regionInitial.GetLeafState(_blackboard);
                rt.CurrentLeaf = regionLeaf;
                rt.ActiveChain = regionLeaf.GetAncestorChain();

                // Region 체인 OnEnter + 태그
                foreach (var state in rt.ActiveChain)
                {
                    state.OnEnter(_blackboard);
                    AddTagsForState(state);
                }

                _runtimeRegions[i] = rt;
            }
        }

        // ── Tick ──

        protected override void OnEntityTick()
        {
            if (_runtimeRegions == null && _currentState == null) return;

            if (_runtimeRegions != null)
            {
                TickWithRegions();
            }
            else
            {
                TickWithoutRegions();
            }
        }

        private void TickWithoutRegions()
        {
            // 기존 로직 그대로
            EvaluateTransitionsForChain(_sharedChain, _sharedTransitionLookup, isSharedChain: true);

            foreach (var state in _sharedChain)
                state.OnTick(_blackboard, DeltaTime);

            movementController.Tick(_blackboard, DeltaTime);
        }

        private void TickWithRegions()
        {
            // 1. 공유 체인 전이 평가 (Alive→Dead 등)
            if (EvaluateTransitionsForChain(_sharedChain, _sharedTransitionLookup, isSharedChain: true))
            {
                // 공유 전이 발생 시 전체 리셋 (Region 포함)
                // 현재는 공유 전이 테이블이 비어있으므로 이 경로는 미사용
                return;
            }

            // 2. Region별 전이 평가 (Posture → Locomotion → Action 순서)
            for (int i = 0; i < _runtimeRegions.Length; i++)
            {
                EvaluateRegionTransitions(_runtimeRegions[i]);
            }

            // 3. 공유 체인 OnTick
            foreach (var state in _sharedChain)
                state.OnTick(_blackboard, DeltaTime);

            // 4. Region별 OnTick (Posture → Locomotion → Action 순서)
            for (int i = 0; i < _runtimeRegions.Length; i++)
            {
                foreach (var state in _runtimeRegions[i].ActiveChain)
                    state.OnTick(_blackboard, DeltaTime);
            }

            // 5. MovementConfig 해소 + 이동 처리
            ResolveMovementConfig();
            movementController.Tick(_blackboard, DeltaTime);
        }

        // ── 전이 평가 ──

        /// <summary>
        /// 체인 내 Top-Down 전이 평가. 전이 발생 시 true 반환.
        /// </summary>
        private bool EvaluateTransitionsForChain(
            List<FSMState> chain,
            Dictionary<FSMState, List<FSMTransition>> lookup,
            bool isSharedChain)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                var state = chain[i];
                if (!lookup.TryGetValue(state, out var transitions))
                    continue;

                foreach (var transition in transitions)
                {
                    if (transition.Condition.Evaluate(_blackboard))
                    {
                        if (debugLog)
                            Debug.Log($"[FSM] {name}: {state.name} → {transition.To.name} ({transition.Condition.name})", this);

                        if (isSharedChain)
                            SharedTransitionTo(transition.To);
                        else
                            Debug.LogError("[FSM] Non-region chain should not use this path", this);

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Region 내 전이 평가. 기존 LCA 기반 전이 로직을 Region 범위로 한정.
        /// </summary>
        private void EvaluateRegionTransitions(RuntimeRegion region)
        {
            for (int i = 0; i < region.ActiveChain.Count; i++)
            {
                var state = region.ActiveChain[i];
                if (!region.TransitionLookup.TryGetValue(state, out var transitions))
                    continue;

                foreach (var transition in transitions)
                {
                    if (transition.Condition.Evaluate(_blackboard))
                    {
                        if (debugLog)
                            Debug.Log($"[FSM] {name} [{region.Definition.Name}]: " +
                                      $"{state.name} → {transition.To.name} ({transition.Condition.name})", this);

                        RegionTransitionTo(region, transition.To);
                        return; // 한 프레임에 Region당 하나의 전이만
                    }
                }
            }
        }

        // ── 전이 실행 ──

        /// <summary>
        /// 공유 체인의 전이 (Alive→Dead 등).
        /// 모든 Region을 Exit한 후 공유 체인 LCA 전이를 수행.
        /// </summary>
        private void SharedTransitionTo(FSMState target)
        {
            // 1. 모든 Region Exit (역순)
            if (_runtimeRegions != null)
            {
                for (int r = _runtimeRegions.Length - 1; r >= 0; r--)
                {
                    var region = _runtimeRegions[r];
                    for (int i = region.ActiveChain.Count - 1; i >= 0; i--)
                    {
                        RemoveTagsForState(region.ActiveChain[i]);
                        region.ActiveChain[i].OnExit(_blackboard);
                    }
                }
                _runtimeRegions = null;
            }

            // 2. 공유 체인 LCA 전이
            var targetLeaf = target.GetLeafState(_blackboard);
            var targetChain = targetLeaf.GetAncestorChain();

            int lcaIndex = FindLCAIndex(_sharedChain, targetChain);

            // Exit (Leaf → LCA+1)
            for (int i = _sharedChain.Count - 1; i > lcaIndex; i--)
            {
                RemoveTagsForState(_sharedChain[i]);
                _sharedChain[i].OnExit(_blackboard);
            }

            // Enter (LCA+1 → Leaf)
            for (int i = lcaIndex + 1; i < targetChain.Count; i++)
            {
                targetChain[i].OnEnter(_blackboard);
                AddTagsForState(targetChain[i]);
            }

            _sharedChain = targetChain;

            // 새 target이 Region 소유자인 경우 Region 재초기화
            var regionOwner = FindRegionOwner(target);
            if (regionOwner != null)
            {
                InitializeRegionsOnly(regionOwner);
            }
            else
            {
                _currentState = targetLeaf;
            }

            ResolveMovementConfig();
        }

        /// <summary>
        /// Region 내 LCA 기반 전이.
        /// </summary>
        private void RegionTransitionTo(RuntimeRegion region, FSMState target)
        {
            var targetLeaf = target.GetLeafState(_blackboard);
            var targetChain = targetLeaf.GetAncestorChain();

            int lcaIndex = FindLCAIndex(region.ActiveChain, targetChain);

            // Exit (Leaf → LCA+1)
            for (int i = region.ActiveChain.Count - 1; i > lcaIndex; i--)
            {
                RemoveTagsForState(region.ActiveChain[i]);
                region.ActiveChain[i].OnExit(_blackboard);
            }

            // Enter (LCA+1 → Leaf)
            for (int i = lcaIndex + 1; i < targetChain.Count; i++)
            {
                targetChain[i].OnEnter(_blackboard);
                AddTagsForState(targetChain[i]);
            }

            region.ActiveChain = targetChain;
            region.CurrentLeaf = targetLeaf;

            // Region 전이 시에도 MovementConfig 갱신
            ResolveMovementConfig();
        }

        /// <summary>
        /// State 내부에서 직접 전이가 필요한 경우.
        /// Region 모드에서는 대상 State가 속한 Region을 자동 탐색.
        /// </summary>
        public void ForceTransition(FSMState nextState)
        {
            if (_runtimeRegions != null)
            {
                // 대상 State가 속한 Region 탐색
                for (int i = 0; i < _runtimeRegions.Length; i++)
                {
                    var region = _runtimeRegions[i];
                    // 대상의 Root를 추적하여 Region의 InitialState 계열인지 확인
                    if (IsStateInRegion(nextState, region))
                    {
                        RegionTransitionTo(region, nextState);
                        return;
                    }
                }

                // Region에서 못 찾으면 공유 전이
                SharedTransitionTo(nextState);
            }
            else
            {
                // 기존 단일 체인 모드
                LegacyTransitionTo(nextState);
            }
        }

        // ── 태그 관리 ──

        private void AddTagsForState(FSMState state)
        {
            foreach (var tag in state.Tags)
                _blackboard.AddTag(tag);
        }

        private void RemoveTagsForState(FSMState state)
        {
            foreach (var tag in state.Tags)
                _blackboard.RemoveTag(tag);
        }

        // ── MovementConfig 해소 ──

        /// <summary>
        /// 활성 계층의 MovementConfigOverride를 누적 적용하여 최종 MovementConfig를 해소.
        /// Region 모드: 공유 체인 → Region[0] → Region[1] → Region[2] 순서.
        /// </summary>
        private void ResolveMovementConfig()
        {
            var resolved = MovementConfig.Default;

            // 공유 체인
            foreach (var state in _sharedChain)
                resolved = state.MovementOverride.Apply(resolved);

            // Region 체인 (순서대로)
            if (_runtimeRegions != null)
            {
                foreach (var region in _runtimeRegions)
                    foreach (var state in region.ActiveChain)
                        resolved = state.MovementOverride.Apply(resolved);
            }

            _blackboard.CurrentMovementConfig = resolved;
        }

        // ── 유틸리티 ──

        /// <summary>
        /// 두 체인 간 LCA (최저 공통 조상) 인덱스 계산.
        /// 앞에서부터 비교하여 마지막으로 일치하는 인덱스 반환.
        /// </summary>
        private static int FindLCAIndex(List<FSMState> chainA, List<FSMState> chainB)
        {
            int lcaIndex = -1;
            int minLen = Mathf.Min(chainA.Count, chainB.Count);
            for (int i = 0; i < minLen; i++)
            {
                if (chainA[i] == chainB[i])
                    lcaIndex = i;
                else
                    break;
            }
            return lcaIndex;
        }

        /// <summary>
        /// 주어진 State가 해당 Region에 속하는지 확인.
        /// State의 Root 조상이 Region의 InitialState 계열인지 추적.
        /// </summary>
        private bool IsStateInRegion(FSMState state, RuntimeRegion region)
        {
            // Region의 TransitionTable에 등록된 모든 from State와 비교
            // 간단히: state의 Root가 Region 체인의 Root와 같은지 확인
            var stateRoot = GetRootState(state);
            var regionRoot = GetRootState(region.Definition.TransitionTable.InitialState);
            return stateRoot == regionRoot;
        }

        private static FSMState GetRootState(FSMState state)
        {
            var current = state;
            int safety = 0;
            while (current.ParentState != null && ++safety < 32)
                current = current.ParentState;
            return current;
        }

        /// <summary>
        /// Region 소유자의 Region만 초기화 (공유 체인은 유지).
        /// SharedTransitionTo에서 새 target이 Region 소유자일 때 사용.
        /// </summary>
        private void InitializeRegionsOnly(FSMState regionOwner)
        {
            var regions = regionOwner.Regions;
            _runtimeRegions = new RuntimeRegion[regions.Count];

            for (int i = 0; i < regions.Count; i++)
            {
                var regionDef = regions[i];
                var rt = new RuntimeRegion
                {
                    Definition = regionDef,
                    TransitionLookup = regionDef.TransitionTable.BuildLookup(),
                };

                var regionLeaf = regionDef.TransitionTable.InitialState.GetLeafState(_blackboard);
                rt.CurrentLeaf = regionLeaf;
                rt.ActiveChain = regionLeaf.GetAncestorChain();

                foreach (var state in rt.ActiveChain)
                {
                    state.OnEnter(_blackboard);
                    AddTagsForState(state);
                }

                _runtimeRegions[i] = rt;
            }
        }

        /// <summary>
        /// 기존 단일 체인 모드의 LCA 전이. (Region 미사용 시)
        /// </summary>
        private void LegacyTransitionTo(FSMState target)
        {
            var targetLeaf = target.GetLeafState(_blackboard);
            var targetChain = targetLeaf.GetAncestorChain();

            int lcaIndex = FindLCAIndex(_sharedChain, targetChain);

            for (int i = _sharedChain.Count - 1; i > lcaIndex; i--)
            {
                RemoveTagsForState(_sharedChain[i]);
                _sharedChain[i].OnExit(_blackboard);
            }

            for (int i = lcaIndex + 1; i < targetChain.Count; i++)
            {
                targetChain[i].OnEnter(_blackboard);
                AddTagsForState(targetChain[i]);
            }

            _sharedChain = targetChain;
            _currentState = targetLeaf;

            ResolveMovementConfig();
        }

        // ── 디버그 ──

        private string FormatDebugState()
        {
            if (_runtimeRegions == null)
                return FormatChain(_sharedChain);

            var parts = new List<string> { FormatChain(_sharedChain) };
            foreach (var region in _runtimeRegions)
                parts.Add($"{region.Definition.Name}: {FormatChain(region.ActiveChain)}");
            return string.Join(" | ", parts);
        }

        private static string FormatChain(List<FSMState> chain)
        {
            if (chain == null || chain.Count == 0) return "(empty)";
            var names = new string[chain.Count];
            for (int i = 0; i < chain.Count; i++)
                names[i] = chain[i] != null ? chain[i].name : "(null)";
            return string.Join(" > ", names);
        }

        // ── 내부 타입 ──

        /// <summary>
        /// Region 하나의 런타임 상태.
        /// </summary>
        private class RuntimeRegion
        {
            public FSMRegion Definition;
            public FSMState CurrentLeaf;
            public List<FSMState> ActiveChain;
            public Dictionary<FSMState, List<FSMTransition>> TransitionLookup;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            FSMState attackState = null;

            // Region 모드에서 Attack State 탐색
            if (_runtimeRegions != null)
            {
                foreach (var region in _runtimeRegions)
                {
                    if (region.CurrentLeaf is PlayerAttackState)
                    {
                        attackState = region.CurrentLeaf;
                        break;
                    }
                }
            }
            else if (_currentState is PlayerAttackState)
            {
                attackState = _currentState;
            }

            if (attackState is not PlayerAttackState atk) return;

            var hitboxData = atk.HitboxData;
            int facing = _blackboard?.FacingDirection ?? 1;
            var center = hitboxData.GetWorldCenter((Vector2)transform.position, facing);

            Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
            Gizmos.DrawCube(center, hitboxData.Size);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, hitboxData.Size);
        }
#endif
    }
}
