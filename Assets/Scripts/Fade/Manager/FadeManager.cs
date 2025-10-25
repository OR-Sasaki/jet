using System;
using System.Threading.Tasks;
using Fade.Service;
using Fade.State;
using Root.Service;

namespace Fade.Manager
{
    public class FadeManager
    {
        private readonly FadeState _state;
        private readonly FadeService _service;
        private readonly ILogger _logger;

        public FadeManager(FadeState state, FadeService service, ILogger logger)
        {
            _state = state;
            _service = service;
            _logger = logger;
        }

        // フェードシーケンスを開始
        public async Task StartFadeSequence(string targetSceneName, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
        {
            _logger.Log($"Starting fade sequence to scene: {targetSceneName}");

            try
            {
                // Phase 1: フェードアウト
                _state.SetPhase(FadePhase.FadingOut);
                await _service.FadeOut(fadeOutDuration);

                // Phase 2: シーン切り替え
                _state.SetPhase(FadePhase.Loading);
                await _service.LoadScene(targetSceneName);

                // Phase 3: フェードイン
                _state.SetPhase(FadePhase.FadingIn);
                await _service.FadeIn(fadeInDuration);

                // Phase 4: 完了
                _state.SetPhase(FadePhase.Completed);
                _logger.Log("Fade sequence completed");

                // Fadeシーンをアンロード
                await _service.UnloadFadeScene();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during fade sequence: {ex.Message}");
                _state.SetPhase(FadePhase.Error);
            }
        }

        // 現在のフェード状態を取得
        public FadePhase GetCurrentPhase()
        {
            return _state.GetPhase();
        }
    }
}

