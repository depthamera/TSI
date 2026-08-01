using UnityEngine;

namespace TSI.Game.FSM
{
    [CreateAssetMenu(fileName = "State_Test_Idle", menuName = "TSI/FSM/Test/State/Idle State")]
    public class IdleState : FSMState
    {
        public override void OnEnter(AgentBlackboard bb)
        {
            Debug.Log("IdleState OnEnter");
        }
    }
}
