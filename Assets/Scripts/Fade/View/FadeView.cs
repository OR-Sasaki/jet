using UnityEngine;
using System.Threading.Tasks;

namespace Fade.View
{
    public class FadeView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            // 初期状態は完全に暗い
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        // フェードアニメーション（fromAlpha → toAlpha）
        public async Task AnimateFade(float fromAlpha, float toAlpha, float duration)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curveValue = _fadeCurve.Evaluate(t);
                _canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, curveValue);
                await Task.Yield();
            }

            _canvasGroup.alpha = toAlpha;
        }
    }
}

