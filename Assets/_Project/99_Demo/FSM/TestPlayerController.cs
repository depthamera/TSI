using TSI.Game.FSM;
using TSI.Game.Player;
using UnityEngine;

namespace TSI.Demo
{
    public class TestPlayerController : MonoBehaviour
    {
        [SerializeField] private FSMRunner _runner;

        private void Start()
        {
            _runner.Initialize(bb => bb.RegisterModule(new AttackModule()));
        }
    }
}
