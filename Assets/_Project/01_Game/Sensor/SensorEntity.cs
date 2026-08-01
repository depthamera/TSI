using TSI.Core.Time;
using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Sensor
{
    /// <summary>
    /// 에이전트의 물리 센서들을 구동하는 TimeEntityBase.
    /// Sensor Clock에서 Tick을 받아 센서 결과를 AgentBlackboard에 기록.
    /// Gameplay Clock(FSMRunner)보다 앞선 Clock에 배치하여 순서 보장.
    ///
    /// 초기화 순서:
    ///   1. FSMRunner.Awake() → AgentBlackboard 생성
    ///   2. PlayerController.Start() → runner.Initialize() → sensorEntity.Initialize()
    /// </summary>
    [RequireComponent(typeof(FSMRunner))]
    public class SensorEntity : TimeEntityBase
    {
        [Header("Ground Detection")]
        [SerializeField] private GroundDetector groundDetector;

        private AgentBlackboard _blackboard;

        /// <summary>
        /// FSMRunner 초기화 후 호출. Blackboard를 연결.
        /// </summary>
        public void Initialize(AgentBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        protected override void OnEntityTick()
        {
            if (_blackboard == null) return;

            _blackboard.IsGrounded = groundDetector.Check(_blackboard.Rigidbody);
        }
    }
}
