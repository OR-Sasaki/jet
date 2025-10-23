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

            // DialogManager

            // 使う側は DialogManager.OpenDialog<CreditsDialog>(nullable Dialog parent) で呼ぶ
            // ダイアログには親子関係がある

            // そのためには...
            // RootでDialogManagerを登録したい。ただし、CreditsDialogみたいなものをどうやって渡すかを考える必要がある。

            // SceneDialogコンポーネントをシーン上に配置し、Dialogコンポーネントを登録する
            // SceneDialogコンポーネントの[Inject]Init()で、DialogManagerへ<型, Func>のようなファクトリーを登録する
        }
    }
}
