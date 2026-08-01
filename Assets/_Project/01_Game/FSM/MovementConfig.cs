using System;
using UnityEngine;

namespace TSI.Game.FSM
{
    /// <summary>
    /// 최종 해소된 이동 설정 데이터.
    /// MovementConfigOverride의 누적 적용 결과를 담는 타입.
    /// </summary>
    [Serializable]
    public struct MovementConfig
    {
        public bool CanMove;
        [Range(0f, 2f)]
        public float SpeedMultiplier;
        [Range(0f, 2f)]
        public float GravityScale;

        public static MovementConfig Default => new()
        {
            CanMove = true,
            SpeedMultiplier = 1f,
            GravityScale = 1f,
        };
    }
}
