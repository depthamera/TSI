using MessagePipe;
using TSI.Core;
using TSI.Core.Scene;
using TSI.Core.Time;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TSI.Game
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private TimeHierarchyProfileSO _timeHierarchy;

        protected override void Configure(IContainerBuilder builder)
        {
            // Scene Manager
            builder.Register<StandardCustomSceneManager>(Lifetime.Singleton).As<ICustomSceneManager>();

            // Time System
            var options = builder.RegisterMessagePipe();
            builder.RegisterTimeSystem(options, _timeHierarchy);
        }
    }
}
