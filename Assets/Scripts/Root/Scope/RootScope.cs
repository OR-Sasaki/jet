using Root.Manager;
using Root.Service;
using Root.State;
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

            // Dialog System
            builder.Register<DialogState>(Lifetime.Singleton);
            builder.Register<DialogManager>(Lifetime.Singleton);
            builder.Register<DialogService>(Lifetime.Singleton);
        }
    }
}
