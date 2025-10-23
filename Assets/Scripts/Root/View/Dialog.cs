using Root.Manager;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Root.View
{
    public abstract class Dialog : MonoBehaviour
    {
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected CanvasGroup _canvasGroup;

        protected DialogManager _dialogManager;

        [Inject]
        public void Init(DialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        protected virtual void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Close);
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ダイアログが開かれたときの処理
        /// </summary>
        public virtual void OnOpen()
        {
            // CanvasGroupがあればフェードイン処理などを追加可能
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// ダイアログが閉じられたときの処理
        /// </summary>
        public virtual void OnClose()
        {
            // CanvasGroupがあればフェードアウト処理などを追加可能
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        protected void Close()
        {
            if (_dialogManager != null)
            {
                _dialogManager.CloseDialog(this);
            }
        }
    }
}
