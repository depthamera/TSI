using TSI.App.Input;
using TSI.App.UI;
using TSI.Core.Time;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TSI.Demo.Time
{
    public class TimePhysicsTestLifetimeScope : LifetimeScope
    {
        [SerializeField] private ManualEventSystem eventSystem;
    
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<TimeLayer, TickMessage>(options);
        
            builder.RegisterComponent(eventSystem)
                .AsSelf();
        
            builder.Register<UnityInputUpdater>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<DefaultTimeHierarchyFactory>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        
            builder.RegisterEntryPoint<TimeManager>()
                .AsSelf();
        }
    }
}
