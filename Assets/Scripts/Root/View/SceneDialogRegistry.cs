using System;
using System.Collections.Generic;
using Root.Manager;
using UnityEngine;
using VContainer;
using ILogger = Root.Service.ILogger;

namespace Root.View
{
    /// <summary>
    /// シーンごとにダイアログを登録するコンポーネント
    /// </summary>
    public class SceneDialogRegistry : MonoBehaviour
    {
        [SerializeField] private List<Dialog> _dialogPrefabs;

        private DialogManager _dialogManager;
        private IObjectResolver _resolver;
        private ILogger _logger;
        private readonly List<Type> _registeredDialogTypes = new();

        [Inject]
        public void Init(DialogManager dialogManager, IObjectResolver resolver, ILogger logger)
        {
            _dialogManager = dialogManager;
            _resolver = resolver;
            _logger = logger;

            RegisterDialogs();
        }

        private void RegisterDialogs()
        {
            foreach (var dialogPrefab in _dialogPrefabs)
            {
                if (dialogPrefab == null)
                {
                    _logger.LogWarning("Dialog prefab is null in SceneDialogRegistry.");
                    continue;
                }

                var dialogType = dialogPrefab.GetType();

                // ファクトリー関数を登録
                _dialogManager.RegisterDialogFactory(dialogType, () =>
                {
                    var instance = Instantiate(dialogPrefab, transform);
                    _resolver.Inject(instance);
                    return instance;
                });

                // 登録した型を記録
                _registeredDialogTypes.Add(dialogType);
            }
        }

        private void OnDestroy()
        {
            // シーン破棄時にファクトリー登録を解除
            if (_dialogManager != null && _registeredDialogTypes.Count > 0)
            {
                _dialogManager.UnregisterDialogFactories(_registeredDialogTypes);
            }
        }
    }
}

