
using MessagePipe;
using TSI.App.Input;
using TSI.Core.Time;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;

namespace TSI.Demo
{
    public class GameplayPOCLifeitmeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<TimeLayer, TickMessage>(options);

            builder.Register<DefaultTimeHierarchyFactory>(Lifetime.Singleton)
                .As<ITimeHierarchyFactory>();

            builder.RegisterEntryPoint<TimeManager>();
        }
    }
}
