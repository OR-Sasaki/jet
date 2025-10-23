using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root.View
{
    /// <summary>
    /// 確認ダイアログ
    /// 親子関係のテスト用
    /// </summary>
    public class ConfirmDialog : Dialog
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        public event Action OnConfirmed;
        public event Action OnCancelled;

        protected override void Awake()
        {
            base.Awake();

            if (_titleText != null)
            {
                _titleText.text = "Confirm";
            }

            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(OnConfirmButtonClick);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelButtonClick);
            }
        }

        /// <summary>
        /// メッセージを設定
        /// </summary>
        public void SetMessage(string message)
        {
            if (_messageText != null)
            {
                _messageText.text = message;
            }
        }

        /// <summary>
        /// タイトルを設定
        /// </summary>
        public void SetTitle(string title)
        {
            if (_titleText != null)
            {
                _titleText.text = title;
            }
        }

        private void OnConfirmButtonClick()
        {
            OnConfirmed?.Invoke();
            Close();
        }

        private void OnCancelButtonClick()
        {
            OnCancelled?.Invoke();
            Close();
        }
    }
}

