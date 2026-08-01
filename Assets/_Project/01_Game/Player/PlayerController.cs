using TSI.Game.FSM;
using TSI.Game.Player;
using TSI.Game.Sensor;
using UnityEngine;

namespace TSI.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private FSMRunner _runner;
        [SerializeField] private SensorEntity _sensorEntity;

        private void Start()
        {
            _runner.Initialize(bb => bb.RegisterModule(new AttackModule()));
            _sensorEntity.Initialize(_runner.Blackboard);
        }
    }
}
