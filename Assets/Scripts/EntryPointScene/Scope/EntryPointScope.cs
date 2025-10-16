using VContainer.Unity;
using VContainer;
using EntryPointScene.Manager;
using EntryPointScene.Starter;

namespace EntryPointScene.Scope
{
    public class EntryPointScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EntryPointManager>(Lifetime.Scoped);
            builder.RegisterEntryPoint<EntryPointStarter>();
        }
    }
}
