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
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMP_Text _volumeText;
        [SerializeField] private Button _resetButton;

        protected override void Awake()
        {
            base.Awake();

            if (_volumeSlider != null)
            {
                _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
                _volumeSlider.value = AudioListener.volume;
                UpdateVolumeText(_volumeSlider.value);
            }

            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(OnResetButtonClick);
            }
        }

        private void OnResetButtonClick()
        {
            // 確認ダイアログを子として開く
            var confirmDialog = _dialogManager.OpenDialog<ConfirmDialog>(parent: this);
            if (confirmDialog != null)
            {
                confirmDialog.SetTitle("Reset Settings");
                confirmDialog.SetMessage("Are you sure you want to reset all settings to default?");
                confirmDialog.OnConfirmed += ResetSettings;
            }
        }

        private void ResetSettings()
        {
            // デフォルト値にリセット
            if (_volumeSlider != null)
            {
                _volumeSlider.value = 1.0f;
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

