using TSI.Game.Combat;
using TSI.Game.FSM;
using UnityEngine;

namespace TSI.Game.Player
{
    /// <summary>
    /// 공격 State. SO 에셋의 인스펙터에서 공격 지속시간, 데미지, 히트박스 형상을 설정.
    /// 히트 판정은 Physics2D.OverlapBoxAll 기반이며, FacingDirection에 따라 히트박스가 반전됨.
    /// </summary>
    [CreateAssetMenu(fileName = "State_Player_Attack", menuName = "TSI/FSM/States/Player/Attack")]
    public class PlayerAttackState : FSMState
    {
        [Header("Attack")]
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float hitStopDuration = 0.05f;

        [Header("Hitbox")]
        [SerializeField] private HitboxData hitboxData = new()
        {
            Offset = new Vector2(0.8f, 0f),
            Size = new Vector2(1f, 1f),
        };

        // Gizmo 시각화를 위해 외부에서 읽을 수 있도록 노출
        public HitboxData HitboxData => hitboxData;

        private AttackModule _cachedModule;

        public override void OnEnter(AgentBlackboard bb)
        {
            _cachedModule = bb.GetModule<AttackModule>();
            _cachedModule.StartAttack(duration);
        }

        public override void OnTick(AgentBlackboard bb, float dt)
        {
            _cachedModule.Tick(dt);

            if (_cachedModule.IsHitboxActive)
            {
                PerformHitDetection(bb);
            }
        }

        public override void OnExit(AgentBlackboard bb)
        {
            _cachedModule.Reset();
            _cachedModule = null;
        }

        private void PerformHitDetection(AgentBlackboard bb)
        {
            var center = hitboxData.GetWorldCenter(
                (Vector2)bb.Transform.position, bb.FacingDirection);

            var hits = Physics2D.OverlapBoxAll(
                center, hitboxData.Size, 0f, hitboxData.TargetLayers);

            foreach (var hit in hits)
            {
                // 자기 자신 제외
                if (hit.transform == bb.Transform) continue;

                // 이번 공격에서 이미 히트한 대상 제외
                if (_cachedModule.HasAlreadyHit(hit)) continue;

                if (hit.TryGetComponent<IHittable>(out var hittable))
                {
                    var knockbackDir = new Vector2(bb.FacingDirection, 0f);
                    var hitData = new HitData(damage, knockbackDir, hitStopDuration);
                    hittable.Hit(hitData);
                    _cachedModule.RecordHit(hit);
                }
            }
        }
    }
}
