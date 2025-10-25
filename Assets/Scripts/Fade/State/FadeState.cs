namespace Fade.State
{
    public enum FadePhase
    {
        None,
        FadingOut,
        Loading,
        FadingIn,
        Completed,
        Error
    }

    public class FadeState
    {
        private FadePhase _currentPhase = FadePhase.None;
        private string _targetSceneName;
        private float _fadeOutDuration = 0.5f;
        private float _fadeInDuration = 0.5f;

        // フェーズを設定
        public void SetPhase(FadePhase phase)
        {
            _currentPhase = phase;
        }

        // 現在のフェーズを取得
        public FadePhase GetPhase()
        {
            return _currentPhase;
        }

        // ターゲットシーン名を設定
        public void SetTargetScene(string sceneName)
        {
            _targetSceneName = sceneName;
        }

        // ターゲットシーン名を取得
        public string GetTargetScene()
        {
            return _targetSceneName;
        }

        // フェード時間を設定
        public void SetDurations(float fadeOut, float fadeIn)
        {
            _fadeOutDuration = fadeOut;
            _fadeInDuration = fadeIn;
        }

        // フェード時間を取得
        public (float fadeOut, float fadeIn) GetDurations()
        {
            return (_fadeOutDuration, _fadeInDuration);
        }

        // 状態をリセット
        public void Reset()
        {
            _currentPhase = FadePhase.None;
            _targetSceneName = null;
            _fadeOutDuration = 0.5f;
            _fadeInDuration = 0.5f;
        }
    }
}

