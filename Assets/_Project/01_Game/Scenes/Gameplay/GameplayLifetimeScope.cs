using VContainer;
using VContainer.Unity;

namespace TSI.Game
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameSessionManager>(Lifetime.Singleton);
        }

    }
}
