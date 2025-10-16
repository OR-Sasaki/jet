using Root.Service;
using VContainer;
using VContainer.Unity;

namespace Root.Scope
{
    public class RootScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<ILogger, EditorLogger>(Lifetime.Singleton);
        }
    }
}
