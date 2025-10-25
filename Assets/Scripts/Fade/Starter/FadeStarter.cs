using VContainer.Unity;
using Fade.Manager;
using Fade.State;
using Root.Service;

namespace Fade.Starter
{
    public class FadeStarter : IStartable
    {
        private readonly FadeManager _fadeManager;
        private readonly FadeState _fadeState;

        public FadeStarter(FadeManager fadeManager, FadeState fadeState)
        {
            _fadeManager = fadeManager;
            _fadeState = fadeState;
        }

        public async void Start()
        {
            // FadeSceneDataから設定を取得
            _fadeState.SetTargetScene(FadeSceneData.TargetSceneName);
            _fadeState.SetDurations(FadeSceneData.FadeOutDuration, FadeSceneData.FadeInDuration);

            // フェードシーケンスを開始
            var targetScene = _fadeState.GetTargetScene();
            var (fadeOut, fadeIn) = _fadeState.GetDurations();
            await _fadeManager.StartFadeSequence(targetScene, fadeOut, fadeIn);
        }
    }
}

