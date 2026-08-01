namespace TSI.Game.Player
{
    /// <summary>
    /// 공격 상태를 추적하는 Blackboard 모듈.
    /// AttackState에서 StartAttack()으로 시작, 매 틱 Tick()으로 타이머 갱신.
    /// </summary>
    public class AttackModule
    {
        /// <summary>
        /// 공격 지속시간 타이머. 0 이하이면 공격 종료.
        /// </summary>
        public float Timer { get; private set; }

        /// <summary>
        /// 히트박스가 활성화되어야 하는 구간인지 여부.
        /// 공격 모션 중 특정 프레임에서만 히트 판정을 하기 위해 사용.
        /// </summary>
        public bool IsHitboxActive { get; private set; }

        /// <summary>
        /// 공격이 완료되었는지. TransitionTable에서 AttackFinishedCondition이 참조.
        /// </summary>
        public bool IsFinished => Timer <= 0f;

        /// <summary>
        /// 이번 공격에서 이미 히트한 대상을 추적하여 중복 히트 방지.
        /// </summary>
        private readonly System.Collections.Generic.HashSet<UnityEngine.Collider2D> _hitTargets = new();

        public void StartAttack(float duration)
        {
            Timer = duration;
            IsHitboxActive = true;
            _hitTargets.Clear();
        }

        public void Tick(float dt)
        {
            if (Timer > 0f)
                Timer -= dt;

            if (Timer <= 0f)
                IsHitboxActive = false;
        }

        /// <summary>
        /// 대상이 이미 이번 공격에서 히트되었는지 확인.
        /// </summary>
        public bool HasAlreadyHit(UnityEngine.Collider2D collider) => _hitTargets.Contains(collider);

        /// <summary>
        /// 히트한 대상을 기록.
        /// </summary>
        public void RecordHit(UnityEngine.Collider2D collider) => _hitTargets.Add(collider);

        public void Reset()
        {
            Timer = 0f;
            IsHitboxActive = false;
            _hitTargets.Clear();
        }
    }
}
