using System;
using System.Collections.Generic;
using System.Linq;
using TSI.Game.Input;
using UnityEngine;

namespace TSI.Game.FSM
{
    public sealed class AgentBlackboard
    {
        // ── Core 필드 (모든 에이전트 공유) ──
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody { get; }
        public IInputProvider InputProvider { get; }
        public AgentStats Stats { get; }

        /// <summary>
        /// 에이전트가 바라보는 방향. 1 = 오른쪽, -1 = 왼쪽.
        /// MovementController에서 입력에 따라 갱신.
        /// </summary>
        public int FacingDirection { get; set; } = 1;

        /// <summary>
        /// 지면에 있는지 여부. SensorEntity가 매 틱 GroundDetector로 갱신.
        /// </summary>
        public bool IsGrounded { get; set; }

        // FSMRunner가 State 전환 시 설정
        public MovementConfig CurrentMovementConfig { get; set; }

        // ── 태그 시스템 ──
        private readonly HashSet<StateTagSO> _activeTags = new();

        /// <summary> 특정 태그가 현재 활성인지 확인. O(1). </summary>
        public bool HasTag(StateTagSO tag) => _activeTags.Contains(tag);
        /// <summary> 태그 추가. FSMRunner가 State Enter 시 호출. 외부 시스템(버프 등)도 사용 가능. </summary>
        public void AddTag(StateTagSO tag) => _activeTags.Add(tag);
        /// <summary> 태그 제거. FSMRunner가 State Exit 시 호출. </summary>
        public void RemoveTag(StateTagSO tag) => _activeTags.Remove(tag);

        // ── 모듈 시스템 ──
        private readonly Dictionary<Type, object> _modules = new();

        public AgentBlackboard(Transform transform, Rigidbody2D rigidbody, IInputProvider inputProvider, AgentStats stats)
        {
            Transform = transform;
            Rigidbody = rigidbody;
            InputProvider = inputProvider;
            Stats = stats;
            CurrentMovementConfig = MovementConfig.Default;
        }

        public void RegisterModule<T>(T module) where T : class
        {
            _modules[typeof(T)] = module;
        }

        public T GetModule<T>() where T : class
        {
            if (_modules.TryGetValue(typeof(T), out var module))
                return (T)module;

            throw new InvalidOperationException(
                $"Module '{typeof(T).Name}' not registered on AgentBlackboard. " +
                $"Registered modules: [{string.Join(", ", _modules.Keys.Select(t => t.Name))}]");
        }

        public bool TryGetModule<T>(out T module) where T : class
        {
            if (_modules.TryGetValue(typeof(T), out var obj))
            {
                module = (T)obj;
                return true;
            }

            module = null;
            return false;
        }
    }
}
