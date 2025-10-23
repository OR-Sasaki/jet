using System.Collections.Generic;
using System.Linq;
using Root.View;

namespace Root.State
{
    public class DialogState
    {
        // ダイアログ情報を保持する内部クラス
        private class DialogInfo
        {
            public Dialog Instance { get; set; }
            public Dialog Parent { get; set; }
            public List<Dialog> Children { get; set; } = new List<Dialog>();
        }

        private readonly Dictionary<Dialog, DialogInfo> _dialogInfos = new Dictionary<Dialog, DialogInfo>();
        private readonly Stack<Dialog> _dialogStack = new Stack<Dialog>();

        /// <summary>
        /// ダイアログをスタックに追加
        /// </summary>
        public void Push(Dialog dialog, Dialog parent = null)
        {
            if (dialog == null) return;

            var info = new DialogInfo
            {
                Instance = dialog,
                Parent = parent
            };

            _dialogInfos[dialog] = info;
            _dialogStack.Push(dialog);

            // 親が存在する場合、親の子リストに追加
            if (parent != null && _dialogInfos.ContainsKey(parent))
            {
                _dialogInfos[parent].Children.Add(dialog);
            }
        }

        /// <summary>
        /// ダイアログをスタックから削除
        /// </summary>
        public void Pop(Dialog dialog)
        {
            if (dialog == null || !_dialogInfos.ContainsKey(dialog)) return;

            var info = _dialogInfos[dialog];

            // 親の子リストから削除
            if (info.Parent != null && _dialogInfos.ContainsKey(info.Parent))
            {
                _dialogInfos[info.Parent].Children.Remove(dialog);
            }

            // スタックから削除（スタックの途中の要素を削除する場合は再構築）
            if (_dialogStack.Count > 0 && _dialogStack.Peek() == dialog)
            {
                _dialogStack.Pop();
            }
            else
            {
                // スタックの途中の要素を削除する場合
                var tempStack = new Stack<Dialog>();
                while (_dialogStack.Count > 0)
                {
                    var current = _dialogStack.Pop();
                    if (current != dialog)
                    {
                        tempStack.Push(current);
                    }
                }
                while (tempStack.Count > 0)
                {
                    _dialogStack.Push(tempStack.Pop());
                }
            }

            _dialogInfos.Remove(dialog);
        }

        /// <summary>
        /// 最前面のダイアログを取得
        /// </summary>
        public Dialog GetTopDialog()
        {
            return _dialogStack.Count > 0 ? _dialogStack.Peek() : null;
        }

        /// <summary>
        /// 指定したダイアログの子ダイアログを取得
        /// </summary>
        public IEnumerable<Dialog> GetChildren(Dialog parent)
        {
            if (parent == null || !_dialogInfos.ContainsKey(parent))
            {
                return Enumerable.Empty<Dialog>();
            }

            return _dialogInfos[parent].Children;
        }

        /// <summary>
        /// 指定したダイアログの親ダイアログを取得
        /// </summary>
        public Dialog GetParent(Dialog dialog)
        {
            if (dialog == null || !_dialogInfos.ContainsKey(dialog))
            {
                return null;
            }

            return _dialogInfos[dialog].Parent;
        }

        /// <summary>
        /// 現在開いているすべてのダイアログを取得
        /// </summary>
        public IEnumerable<Dialog> GetAllDialogs()
        {
            return _dialogInfos.Keys;
        }

        /// <summary>
        /// すべてのダイアログをクリア
        /// </summary>
        public void Clear()
        {
            _dialogInfos.Clear();
            _dialogStack.Clear();
        }
    }
}
