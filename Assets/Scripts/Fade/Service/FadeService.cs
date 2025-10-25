using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fade.View;
using ILogger = Root.Service.ILogger;

namespace Fade.Service
{
    public class FadeService
    {
        private readonly ILogger _logger;
        private readonly FadeView _fadeView;

        public FadeService(ILogger logger, FadeView fadeView)
        {
            _logger = logger;
            _fadeView = fadeView;
        }

        // フェードアウト（画面を暗転）
        public async Task FadeOut(float duration)
        {
            _logger.Log($"Fading out (duration: {duration}s)");
            if (_fadeView != null)
            {
                await _fadeView.AnimateFade(0f, 1f, duration);
            }
        }

        // フェードイン（画面を明るく）
        public async Task FadeIn(float duration)
        {
            _logger.Log($"Fading in (duration: {duration}s)");
            if (_fadeView != null)
            {
                await _fadeView.AnimateFade(1f, 0f, duration);
            }
        }

        // シーンをロード
        public async Task LoadScene(string sceneName)
        {
            _logger.Log($"Loading scene: {sceneName}");

            // 古いシーンを特定（Fadeシーン以外の最初のシーン）
            string oldSceneName = null;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Fade")
                {
                    oldSceneName = scene.name;
                    break;
                }
            }

            // 新しいシーンをロード（加算モード）
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }

            // 新しいシーンをアクティブに設定
            var newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);

            // 古いシーンをアンロード
            if (!string.IsNullOrEmpty(oldSceneName))
            {
                _logger.Log($"Unloading old scene: {oldSceneName}");
                var asyncUnload = SceneManager.UnloadSceneAsync(oldSceneName);
                while (asyncUnload != null && !asyncUnload.isDone)
                {
                    await Task.Yield();
                }
            }
        }

        // Fadeシーンをアンロード
        public async Task UnloadFadeScene()
        {
            _logger.Log("Unloading Fade scene");
            var asyncUnload = SceneManager.UnloadSceneAsync("Fade");
            while (asyncUnload != null && !asyncUnload.isDone)
            {
                await Task.Yield();
            }
        }
    }
}

