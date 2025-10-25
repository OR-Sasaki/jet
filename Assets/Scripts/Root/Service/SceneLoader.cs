using UnityEngine.SceneManagement;

namespace Root.Service
{
    public class SceneLoader
    {
        private readonly ILogger _logger;

        public SceneLoader(ILogger logger)
        {
            _logger = logger;
        }

        // Fadeシーン経由でシーンをロード
        public void LoadScene(string sceneName, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
        {
            _logger.Log($"Loading scene with fade: {sceneName}");

            // FadeSceneDataを静的に設定（Fadeシーンで使用）
            FadeSceneData.TargetSceneName = sceneName;
            FadeSceneData.FadeOutDuration = fadeOutDuration;
            FadeSceneData.FadeInDuration = fadeInDuration;

            // Fadeシーンをロード
            SceneManager.LoadScene("Fade", LoadSceneMode.Additive);
        }

        // 即座にシーンをロード（フェードなし）
        public void LoadSceneImmediate(string sceneName)
        {
            _logger.Log($"Loading scene immediately: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }

    // Fadeシーン用の静的データ
    public static class FadeSceneData
    {
        public static string TargetSceneName { get; set; }
        public static float FadeOutDuration { get; set; } = 0.5f;
        public static float FadeInDuration { get; set; } = 0.5f;
    }
}
