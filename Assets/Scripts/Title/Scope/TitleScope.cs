using Title.Manager;
using Title.Service;
using VContainer;
using VContainer.Unity;

namespace Title.Scope
{
    public class TitleScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MenuButtonClickService>(Lifetime.Scoped);
        }
    }
}
