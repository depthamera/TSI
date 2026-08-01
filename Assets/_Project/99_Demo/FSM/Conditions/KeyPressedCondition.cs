using UnityEngine;

namespace TSI.Game.FSM
{
    [CreateAssetMenu(fileName = "Cond_Test_KeyPressed", menuName = "TSI/FSM/Test/Condition/TestKey")]
    public class KeyPressedCondition : FSMCondition
    {
        
        public override bool Evaluate(AgentBlackboard bb)
        {
            return true;
        }
 
    }
}
