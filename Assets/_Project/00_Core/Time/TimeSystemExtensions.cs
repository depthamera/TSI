using MessagePipe;
using TSI.Core;
using VContainer;
using VContainer.Unity;

namespace TSI.Core.Time
{
    public static class TimeSystemExtensions
    {
        /// <summary>
        /// Time 시스템의 전체 DI 등록을 한 번에 수행합니다.
        /// MessageBroker, PhysicsInterpolationManager, SOTimeHierarchyFactory, TimeManager를 등록합니다.
        /// </summary>
        /// <param name="builder">VContainer의 IContainerBuilder</param>
        /// <param name="options">RegisterMessagePipe()의 반환값</param>
        /// <param name="profile">Clock 트리 구성을 정의하는 TimeHierarchyProfileSO</param>
        public static void RegisterTimeSystem(
            this IContainerBuilder builder,
            MessagePipeOptions options,
            TimeHierarchyProfileSO profile)
        {
            // Message Brokers
            builder.RegisterMessageBroker<TimeLayerSO, TickMessage>(options);
            builder.RegisterMessageBroker<TimeLayerSO, TimeStateMessage>(options);

            // Physics Interpolation
            builder.RegisterMessageBroker<PhysicsInterpolationMessage>(options);
            builder.Register<PhysicsInterpolationManager>(Lifetime.Singleton);

            // Clock Hierarchy
            builder.RegisterInstance(profile);
            builder.Register<SOTimeHierarchyFactory>(Lifetime.Singleton)
                   .As<ITimeHierarchyFactory>();

            // Entry Point
            builder.RegisterEntryPoint<TimeManager>().AsSelf();
        }
    }
}
