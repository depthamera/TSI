using System;
using UnityEngine;

namespace TSI.Game.FSM
{
    /// <summary>
    /// Orthogonal Region 정의.
    /// SuperState가 보유하는 독립적인 상태 머신 영역.
    /// FSMTransitionTable을 참조하여 InitialState와 전이 목록을 가져옴.
    /// </summary>
    [Serializable]
    public class FSMRegion
    {
        [SerializeField] private string name;
        [SerializeField] private FSMTransitionTable transitionTable;

        public string Name => name;
        public FSMTransitionTable TransitionTable => transitionTable;
    }
}
