using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace TSI.Core.Time
{
    public abstract class TimeEntityBase : MonoBehaviour
    {
        [SerializeField] private TimeLayerSO targetLayer;

        // 이 오브젝트만의 고유한 시간 배율 (평소엔 1, 정지 시 0)
        public float LocalTimeScale { get; set; } = 1.0f;

        // 내부 컴포넌트(AI, 애니메이터, 물리 제어 등)들이 구독할 자체 델타타임
        public float DeltaTime { get; private set; }

        [Inject]
        public void Construct(ISubscriber<TimeLayerSO, TickMessage> timeSubscriber)
        {
            // MessagePipe를 통해 지정된 레이어의 시간 신호를 구독
            timeSubscriber.Subscribe(targetLayer, OnTick)
                .RegisterTo(destroyCancellationToken);
        }

        private void OnTick(TickMessage message)
        {
            // 핵심: 글로벌 레이어 시간과 로컬 배율을 결합
            DeltaTime = message.DeltaTime * LocalTimeScale;

            // 이후 자식 클래스나 내부 컴포넌트에서 이 DeltaTime을 기반으로 구동되도록 함
            OnEntityTick();
        }

        protected abstract void OnEntityTick();
    }
}
