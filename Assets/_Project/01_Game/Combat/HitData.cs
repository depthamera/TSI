using UnityEngine;

namespace TSI.Game.Combat
{
    public readonly struct HitData
    {
        public readonly float Damage;
        public readonly Vector2 KnockbackDirection;
        public readonly float HitStopDuration;

        public HitData(float damage, Vector2 knockbackDirection, float hitStopDuration = 0f)
        {
            Damage = damage;
            KnockbackDirection = knockbackDirection;
            HitStopDuration = hitStopDuration;
        }
    }
}
