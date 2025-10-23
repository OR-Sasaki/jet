using System;
using System.Collections.Generic;
using System.Linq;
using Root.State;
using Root.View;
using ILogger = Root.Service.ILogger;

namespace Root.Manager
{
    public class DialogManager
    {
        private readonly DialogState _state;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, Func<Dialog>> _factories = new();

        public DialogManager(DialogState state, ILogger logger)
        {
            _state = state;
            _logger = logger;
        }

        /// <summary>
        /// ダイアログのファクトリーを登録
        /// </summary>
        public void RegisterDialogFactory<T>(Type type, Func<T> factory) where T : Dialog
        {
            if (_factories.ContainsKey(type))
            {
                _logger.LogWarning($"Dialog factory for {type.Name} is already registered. Overwriting.");
            }
            _factories[type] = () => factory();
        }

        /// <summary>
        /// ダイアログのファクトリー登録を解除
        /// </summary>
        public void UnregisterDialogFactory<T>() where T : Dialog
        {
            var type = typeof(T);
            if (_factories.ContainsKey(type))
            {
                _factories.Remove(type);
            }
        }

        /// <summary>
        /// 複数のダイアログファクトリー登録を解除
        /// </summary>
        public void UnregisterDialogFactories(IEnumerable<Type> dialogTypes)
        {
            foreach (var type in dialogTypes)
            {
                if (_factories.ContainsKey(type))
                {
                    _factories.Remove(type);
                }
            }
        }

        /// <summary>
        /// ダイアログを開く
        /// </summary>
        public T OpenDialog<T>(Dialog parent = null) where T : Dialog
        {
            var type = typeof(T);

            if (!_factories.ContainsKey(type))
            {
                _logger.LogError($"Dialog factory for {type.Name} is not registered.");
                return null;
            }

            // ファクトリーからダイアログを生成
            var dialog = _factories[type]() as T;
            if (dialog == null)
            {
                _logger.LogError($"Failed to create dialog of type {type.Name}.");
                return null;
            }

            // ダイアログをステートに登録
            _state.Push(dialog, parent);

            // ダイアログを表示
            dialog.gameObject.SetActive(true);
            dialog.OnOpen();

            return dialog;
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        public void CloseDialog(Dialog dialog)
        {
            if (dialog == null) return;

            // 子ダイアログがある場合は先に閉じる
            var children = _state.GetChildren(dialog);
            foreach (var child in children.ToArray()) // ToArrayでコピーを作成してから反復
            {
                CloseDialog(child);
            }

            // ダイアログを非表示にして破棄
            dialog.OnClose();
            dialog.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(dialog.gameObject);

            // ステートから削除
            _state.Pop(dialog);

            // 親ダイアログがある場合は再表示
            var parent = _state.GetParent(dialog);
            if (parent != null && !parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// すべてのダイアログを閉じる
        /// </summary>
        public void CloseAllDialogs()
        {
            var allDialogs = _state.GetAllDialogs().ToArray();
            foreach (var dialog in allDialogs)
            {
                CloseDialog(dialog);
            }
        }
    }
}
