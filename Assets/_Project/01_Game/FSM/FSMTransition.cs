using System;
using UnityEngine;

namespace TSI.Game.FSM
{
    [Serializable]
    public class FSMTransition
    {
        [SerializeField] private FSMState from;
        [SerializeField] private FSMState to;
        [SerializeField] private FSMCondition condition;
        [SerializeField] private int priority; // 높을수록 먼저 평가

        public FSMState From => from;
        public FSMState To => to;
        public FSMCondition Condition => condition;
        public int Priority => priority;
    }
}
