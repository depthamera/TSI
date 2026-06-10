using TSI.Core.Scene;
using VContainer;
using VContainer.Unity;

namespace TSI.Demo
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<StandardCustomSceneManager>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<GlobalSystemNotifier>(Lifetime.Singleton).AsSelf();
        }
    }
}
