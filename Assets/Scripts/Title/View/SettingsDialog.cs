using Root.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Title.View
{
    /// <summary>
    /// 設定ダイアログ
    /// </summary>
    public class SettingsDialog : Dialog
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMP_Text _volumeText;

        protected override void Awake()
        {
            base.Awake();

            if (_titleText != null)
            {
                _titleText.text = "Settings";
            }

            if (_volumeSlider != null)
            {
                _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
                _volumeSlider.value = AudioListener.volume;
                UpdateVolumeText(_volumeSlider.value);
            }
        }

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
            UpdateVolumeText(value);
        }

        private void UpdateVolumeText(float value)
        {
            if (_volumeText != null)
            {
                _volumeText.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
            }
        }
    }
}

