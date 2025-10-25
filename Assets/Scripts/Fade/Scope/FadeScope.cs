using VContainer;
using VContainer.Unity;
using Fade.Manager;
using Fade.Service;
using Fade.State;
using Fade.Starter;
using Fade.View;
using UnityEngine;

namespace Fade.Scope
{
    public class FadeScope : LifetimeScope
    {
        [SerializeField] FadeView _fadeView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_fadeView).As<FadeView>();

            // State
            builder.Register<FadeState>(Lifetime.Scoped);

            // Service
            builder.Register<FadeService>(Lifetime.Scoped);

            // Manager
            builder.Register<FadeManager>(Lifetime.Scoped);

            // Starter
            builder.RegisterEntryPoint<FadeStarter>();
        }
    }
}

