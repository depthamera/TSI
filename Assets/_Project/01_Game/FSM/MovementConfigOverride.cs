using System;
using UnityEngine;

namespace TSI.Game.FSM
{
    /// <summary>
    /// 부모 State의 MovementConfig를 기본값으로 사용하되,
    /// 자식 State가 필요한 필드만 선택적으로 오버라이드하기 위한 구조체.
    /// FSMRunner가 Root→Leaf 순으로 누적 적용하여 최종 MovementConfig를 해소.
    /// </summary>
    [Serializable]
    public struct MovementConfigOverride
    {
        [SerializeField] private bool overrideCanMove;
        [SerializeField] private bool canMove;

        [SerializeField] private bool overrideSpeedMultiplier;
        [SerializeField, Range(0f, 2f)] private float speedMultiplier;

        [SerializeField] private bool overrideGravityScale;
        [SerializeField, Range(0f, 2f)] private float gravityScale;

        /// <summary>
        /// 부모의 config에 자신의 오버라이드를 적용하여 최종 config 반환.
        /// </summary>
        public MovementConfig Apply(MovementConfig parent) => new()
        {
            CanMove         = overrideCanMove         ? canMove         : parent.CanMove,
            SpeedMultiplier = overrideSpeedMultiplier ? speedMultiplier : parent.SpeedMultiplier,
            GravityScale    = overrideGravityScale    ? gravityScale    : parent.GravityScale,
        };
    }
}
