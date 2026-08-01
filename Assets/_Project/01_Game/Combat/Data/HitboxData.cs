using System;
using UnityEngine;

namespace TSI.Game.Combat
{
    /// <summary>
    /// 공격의 히트박스 형상을 정의하는 데이터.
    /// AttackState SO의 인스펙터에서 설정하며, FacingDirection에 따라 Offset.x가 반전됨.
    /// </summary>
    [Serializable]
    public struct HitboxData
    {
        [Tooltip("캐릭터 중심 기준 상대 위치 (오른쪽 기준으로 설정)")]
        public Vector2 Offset;

        [Tooltip("히트박스 크기")]
        public Vector2 Size;

        [Tooltip("히트 판정에 사용할 레이어 마스크")]
        public LayerMask TargetLayers;

        /// <summary>
        /// FacingDirection을 적용한 월드 좌표 기준 히트박스 중심 계산.
        /// </summary>
        public Vector2 GetWorldCenter(Vector2 origin, int facingDirection)
        {
            return origin + new Vector2(Offset.x * facingDirection, Offset.y);
        }
    }
}
