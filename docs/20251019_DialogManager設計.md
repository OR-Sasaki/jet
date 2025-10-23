# DialogManager 実装プラン

## 概要

VContainerを使用した依存性注入パターンに基づき、親子関係を持つダイアログシステムを実装します。
各シーン固有のダイアログ（CreditsDialog、SettingsDialogなど）を柔軟に登録・管理できる仕組みを提供します。

## 要件

1. **親子関係を持つダイアログシステム**
   - ダイアログは親子関係を持つことができる
   - 子ダイアログを閉じると親ダイアログが再表示される
   - 親ダイアログを閉じるとすべての子ダイアログも閉じる

2. **シーン横断的な管理**
   - DialogManagerはRootScopeで登録し、全シーンで使用可能にする
   - 各シーンで固有のダイアログタイプを登録できる

3. **使いやすいAPI**
   ```csharp
   // 基本的な使用方法
   dialogManager.OpenDialog<CreditsDialog>();
   
   // 親を指定してダイアログを開く
   dialogManager.OpenDialog<ConfirmDialog>(parentDialog);
   
   // ダイアログを閉じる
   dialogManager.CloseDialog(dialog);
   ```

## アーキテクチャ設計

### 1. クラス構成

```
Root/
├── Manager/
│   └── DialogManager.cs          # ダイアログの生成・管理を担当
├── Service/
│   └── DialogService.cs          # ダイアログ関連のビジネスロジック
├── State/
│   └── DialogState.cs            # 現在開いているダイアログのスタック管理
└── View/
    ├── Dialog.cs                 # ダイアログの基底クラス
    └── SceneDialogRegistry.cs    # シーンごとのダイアログ登録用コンポーネント

Root/
└── View/
    └── ConfirmDialog.cs          # 確認ダイアログ（汎用）

Title/
└── View/
    ├── CreditsDialog.cs          # クレジットダイアログ
    └── SettingsDialog.cs         # 設定ダイアログ
```

### 2. DialogManager の責務

**主な機能:**
- ダイアログのファクトリー登録・管理
- ダイアログの生成・表示
- ダイアログの親子関係の管理
- ダイアログのスタック管理
- **ファクトリー登録の解除（メモリリーク対策）**

**実装概要:**
```csharp
namespace Root.Manager
{
    public class DialogManager
    {
        private readonly DialogState _state;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, Func<Dialog>> _factories;
        
        public DialogManager(DialogState state, ILogger logger);
        
        // ダイアログのファクトリーを登録
        public void RegisterDialogFactory<T>(Func<T> factory) where T : Dialog;
        
        // ダイアログのファクトリー登録を解除
        public void UnregisterDialogFactory<T>() where T : Dialog;
        
        // 複数のダイアログファクトリー登録を解除
        public void UnregisterDialogFactories(IEnumerable<Type> dialogTypes);
        
        // ダイアログを開く
        public T OpenDialog<T>(Dialog parent = null) where T : Dialog;
        
        // ダイアログを閉じる
        public void CloseDialog(Dialog dialog);
        
        // すべてのダイアログを閉じる
        public void CloseAllDialogs();
    }
}
```

**実装詳細:**
```csharp
public void RegisterDialogFactory<T>(Func<T> factory) where T : Dialog
{
    var type = typeof(T);
    if (_factories.ContainsKey(type))
    {
        _logger.LogWarning($"Dialog factory for {type.Name} is already registered. Overwriting.");
    }
    _factories[type] = () => factory();
}

public void UnregisterDialogFactory<T>() where T : Dialog
{
    var type = typeof(T);
    if (_factories.ContainsKey(type))
    {
        _factories.Remove(type);
    }
}

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
```

**注意:**
- AGENTS.mdの規約に従い、`Debug.Log`ではなく`ILogger`を使用
- `ILogger`にはPhase 1で`LogWarning`と`LogError`メソッドを追加済み

### 3. DialogState の責務 ✅ **実装済み**

**主な機能:**
- 現在開いているダイアログのスタック管理
- ダイアログの親子関係の追跡

**実装概要:**
```csharp
namespace Root.State
{
    public class DialogState
    {
        // ダイアログ情報を保持する内部クラス
        private class DialogInfo
        {
            public Dialog Instance { get; set; }
            public Dialog Parent { get; set; }
            public List<Dialog> Children { get; set; }
        }
        
        private readonly Dictionary<Dialog, DialogInfo> _dialogInfos;
        private readonly Stack<Dialog> _dialogStack;
        
        // ダイアログをスタックに追加
        public void Push(Dialog dialog, Dialog parent = null);
        
        // ダイアログをスタックから削除
        public void Pop(Dialog dialog);
        
        // 最前面のダイアログを取得
        public Dialog GetTopDialog();
        
        // 子ダイアログを取得
        public IEnumerable<Dialog> GetChildren(Dialog parent);
        
        // 親ダイアログを取得
        public Dialog GetParent(Dialog dialog);
        
        // すべてのダイアログを取得
        public IEnumerable<Dialog> GetAllDialogs();
        
        // すべてクリア
        public void Clear();
    }
}
```

### 4. DialogService の責務

**主な機能:**
- ダイアログの表示/非表示アニメーション制御
- 背景のオーバーレイ管理
- ダイアログの閉じる処理のビジネスロジック

**実装概要:**
```csharp
namespace Root.Service
{
    public class DialogService
    {
        private readonly DialogState _state;
        
        // ダイアログを表示
        public void Show(Dialog dialog);
        
        // ダイアログを非表示
        public void Hide(Dialog dialog);
        
        // 背景オーバーレイを更新
        public void UpdateOverlay();
    }
}
```

### 5. Dialog (基底クラス) の拡張 ✅ **実装済み**

**実装概要:**
```csharp
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
        
        // ダイアログが開かれたときの処理
        public virtual void OnOpen()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }
        
        // ダイアログが閉じられたときの処理
        public virtual void OnClose()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }
        
        // ダイアログを閉じる
        protected void Close()
        {
            if (_dialogManager != null)
            {
                _dialogManager.CloseDialog(this);
            }
        }
    }
}
```

### 6. SceneDialogRegistry の実装 ✅ **実装済み**

各シーンでダイアログを登録するためのコンポーネント

**実装概要:**
```csharp
namespace Root.View
{
    public class SceneDialogRegistry : MonoBehaviour
    {
        [SerializeField] private List<Dialog> _dialogPrefabs;
        
        private DialogManager _dialogManager;
        private IObjectResolver _resolver;
        private ILogger _logger;
        private List<Type> _registeredDialogTypes = new List<Type>();
        
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
                    _resolver.Inject(instance);  // Runtime生成したインスタンスに依存性注入
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
```

**IObjectResolverが必要な理由:**
- Unityの`Instantiate()`だけでは依存性注入が実行されない
- `_resolver.Inject(instance)`を呼ぶことで、生成したダイアログに`DialogManager`などを注入
- これにより、ダイアログ内で`_dialogManager.CloseDialog(this)`などが正常に動作

## 実装手順

### Phase 1: 基礎の実装 ✅ **完了**
1. **DialogState の実装** ✅
   - ダイアログスタックの管理機能
   - 親子関係の追跡機能
   - 実装済み: `Push()`, `Pop()`, `GetTopDialog()`, `GetChildren()`, `GetParent()`, `GetAllDialogs()`, `Clear()`

2. **DialogManager の基本実装** ✅
   - ファクトリー登録機能
   - **ファクトリー登録解除機能**
     - `UnregisterDialogFactory<T>()`：単一型の登録解除
     - `UnregisterDialogFactories(IEnumerable<Type>)`：複数型の一括登録解除
   - ダイアログの生成機能
   - 基本的な開く/閉じる機能
   - `ILogger`を使用したログ出力（AGENTS.md規約準拠）

3. **Dialog 基底クラスの拡張** ✅
   - DialogManagerとの連携
   - OnOpen/OnCloseのライフサイクルメソッド追加
   - abstractクラスに変更
   - CanvasGroupのサポート追加

4. **ILogger の拡張** ✅
   - `LogWarning()`メソッド追加
   - `LogError()`メソッド追加
   - EditorLoggerに実装追加

5. **RootScope への登録** ✅
   - DialogState, DialogManager, DialogServiceを登録済み

### Phase 2: シーン連携の実装 ✅ **完了**
4. **SceneDialogRegistry の実装** ✅
   - シーンにダイアログプレハブを配置
   - DialogManagerへの自動登録
   - **OnDestroy()でファクトリー登録を解除**
     - 登録した型を追跡するリスト実装
     - シーン破棄時の自動クリーンアップ
   - ILoggerを使用したログ出力（AGENTS.md規約準拠）
   - `IObjectResolver.Inject()`による動的生成インスタンスへの依存性注入

5. **具体的なダイアログの実装** ✅
   - **CreditsDialog** - クレジット表示ダイアログ
   - **SettingsDialog** - 設定ダイアログ（音量調整機能付き）
   - **MenuButtonClickService** - DialogManagerと連携してダイアログを開く

### Phase 3: 機能拡張 ✅ **完了**
6. **具体的なダイアログの実装** ✅
   - CreditsDialog - クレジット表示ダイアログ
   - SettingsDialog - 設定ダイアログ（音量調整、リセット機能付き）
   - **ConfirmDialog** - 確認ダイアログ（親子関係のテスト用）
     - SetMessage()とSetTitle()で内容をカスタマイズ
     - OnConfirmed/OnCancelledイベントでコールバック
     - SettingsDialogからリセット確認として使用

7. **DialogService の実装**
   - 現在は基本構造のみ（将来の拡張用に確保）
   - アニメーション機能は各Dialogで実装可能

### Phase 4: 統合とテスト
8. **Titleシーンでの統合**
   - SceneDialogRegistryをTitleシーンに配置
   - MenuButtonClickServiceからDialogManagerを呼び出す

9. **テストケースの作成**
   - 単一ダイアログの開閉
   - 親子ダイアログの動作
   - 複数ダイアログのスタック管理

## 使用例

### 例1: Titleシーンでクレジットダイアログを開く ✅ **実装済み**

```csharp
// MenuButtonClickService.cs
public class MenuButtonClickService
{
    readonly SceneLoader _sceneLoader;
    readonly DialogManager _dialogManager;
    
    public MenuButtonClickService(SceneLoader sceneLoader, DialogManager dialogManager)
    {
        _sceneLoader = sceneLoader;
        _dialogManager = dialogManager;
    }
    
    public void Click(MenuButtonType menuButtonType)
    {
        switch (menuButtonType)
        {
            case MenuButtonType.Singleplayer:
                _sceneLoader.LoadScene("Game");
                break;
            case MenuButtonType.Settings:
                _dialogManager.OpenDialog<SettingsDialog>();
                break;
            case MenuButtonType.Credits:
                _dialogManager.OpenDialog<CreditsDialog>();
                break;
        }
    }
}
```

### 例2: ダイアログ内から子ダイアログを開く ✅ **実装済み**

```csharp
// SettingsDialog.cs
public class SettingsDialog : Dialog
{
    [SerializeField] private Button _resetButton;
    
    protected override void Awake()
    {
        base.Awake();
        
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
}
```

**親子ダイアログの動作:**
1. SettingsDialogが開いている状態でResetボタンをクリック
2. ConfirmDialogが子として開く（SettingsDialogは非表示になる）
3. ConfirmDialogで「Confirm」をクリックすると設定がリセットされる
4. ConfirmDialogが閉じると親のSettingsDialogが再表示される

### 例3: SceneDialogRegistryの配置（Unity エディタでの作業）

```
Titleシーン
├── Canvas
│   ├── TitleUI
│   │   └── MenuButtons
│   └── DialogContainer (SceneDialogRegistry)
│       ├── CreditsDialogPrefab (Inspector で設定) ✅
│       ├── SettingsDialogPrefab (Inspector で設定) ✅
│       └── ConfirmDialogPrefab (Inspector で設定) ✅
```

**設定手順:**
1. Titleシーンを開く
2. Canvas下に空のGameObject「DialogContainer」を作成
3. DialogContainerにSceneDialogRegistryコンポーネントをアタッチ
4. 各ダイアログのプレハブを作成（UI要素を配置）
5. SceneDialogRegistryのDialog Prefabsリストに3つのプレハブを登録

## 注意点と考慮事項

### 1. メモリ管理
- ダイアログのプレハブはシーンロード時に登録される
- 使用済みのダイアログインスタンスは適切に破棄する
- **シーン破棄時のクリーンアップ処理**
  - `SceneDialogRegistry.OnDestroy()`でファクトリー登録を自動解除
  - 登録した型を`_registeredDialogTypes`リストで追跡
  - DialogManagerの`UnregisterDialogFactories()`で一括解除
  - これによりメモリリークを防止し、シーン遷移時の安全性を確保

### 2. 入力制御
- ダイアログが開いているときは背景の操作を無効化
- 最前面のダイアログのみが入力を受け付ける
- Escキーで最前面のダイアログを閉じる機能の検討

### 3. アニメーション
- フェードイン/フェードアウトの実装
- ダイアログ表示時のスケールアニメーション
- 親ダイアログと子ダイアログの切り替えアニメーション

### 4. エラーハンドリング
- 未登録のダイアログタイプを開こうとした場合のエラーメッセージ
- ファクトリー登録時の型チェック
- nullチェックと適切な例外処理

### 5. 拡張性
- カスタムアニメーションの差し替え可能性
- ダイアログの優先度設定（モーダル/モードレス）
- 複数のダイアログコンテナのサポート

## 今後の拡張案

1. **ダイアログのプール管理**
   - 頻繁に使用されるダイアログをプーリングして再利用

2. **履歴機能**
   - ダイアログの表示履歴を保存
   - 「戻る」ボタンで前のダイアログに戻る

3. **ダイアログのシリアライズ**
   - ダイアログの状態を保存・復元
   - シーン遷移時にダイアログ状態を維持

4. **非同期対応**
   - async/awaitを使用したダイアログの結果待機
   ```csharp
   var result = await dialogManager.OpenDialogAsync<ConfirmDialog>();
   if (result == DialogResult.OK) { ... }
   ```

5. **デバッグ機能**
   - 現在のダイアログスタックを可視化するツール
   - ダイアログのライフサイクルをログ出力

## 実装状況

### ✅ Phase 1: 基礎の実装（完了）
- **DialogState** - スタック管理と親子関係の追跡
  - `Push()`, `Pop()`, `GetTopDialog()`, `GetChildren()`, `GetParent()`, `GetAllDialogs()`, `Clear()`
- **DialogManager** - ファクトリー登録/解除、ダイアログ開閉機能
  - `RegisterDialogFactory<T>()`, `UnregisterDialogFactory<T>()`, `UnregisterDialogFactories()`
  - `OpenDialog<T>()`, `CloseDialog()`, `CloseAllDialogs()`
  - ILoggerによるログ出力（AGENTS.md規約準拠）
- **Dialog基底クラス** - 抽象クラス化、ライフサイクルメソッド、CanvasGroupサポート
  - `OnOpen()`, `OnClose()`, `Close()`
- **ILogger拡張** - LogWarning/LogErrorメソッド追加
- **RootScopeへの登録** - DialogState/DialogManager/DialogServiceの依存性注入設定

### ✅ Phase 2: シーン連携の実装（完了）
- **SceneDialogRegistry** - シーンごとのダイアログ登録コンポーネント
  - DialogManagerへの自動登録
  - OnDestroy()でのファクトリー登録解除（メモリリーク対策）
  - IObjectResolver.Inject()による動的生成インスタンスへの依存性注入
- **具体的なダイアログ実装**
  - **CreditsDialog** - クレジット表示ダイアログ
  - **SettingsDialog** - 設定ダイアログ（音量調整機能付き）
- **MenuButtonClickService** - DialogManagerと連携してダイアログを開く

### ✅ Phase 3: 機能拡張（完了）
- **ConfirmDialog** - 確認ダイアログ（親子関係のテスト用）
  - カスタマイズ可能なメッセージとタイトル
  - OnConfirmed/OnCancelledイベント
  - SettingsDialogのリセット機能で親子関係を実装
- **親子ダイアログの実装例**
  - SettingsDialog → ConfirmDialogの親子関係
  - リセットボタンで確認ダイアログを子として開く

### 🔄 次のステップ（Phase 4: Unity エディタでの作業）
- **Titleシーンへの統合**
  - DialogContainerゲームオブジェクトを配置
  - SceneDialogRegistryコンポーネントをアタッチ
- **プレハブの作成**
  - CreditsDialog UIプレハブ
  - SettingsDialog UIプレハブ（Slider、Resetボタン付き）
  - ConfirmDialog UIプレハブ（Confirm/Cancelボタン付き）
- **SceneDialogRegistryの設定**
  - 各ダイアログプレハブをInspectorで登録
- **動作確認**
  - メニューボタンからダイアログ表示
  - 親子ダイアログの動作確認
  - シーン遷移時のメモリリーク確認

## まとめ

このDialogManager設計は以下の特徴を持ちます：

- ✅ VContainerの依存性注入パターンに完全に準拠
- ✅ シーンごとにダイアログを柔軟に登録可能
- ✅ 親子関係を持つダイアログシステム
- ✅ **シーン破棄時の自動クリーンアップによるメモリリーク対策**
  - `OnDestroy()`での自動登録解除
  - ファクトリー関数への参照を適切に管理
- ✅ **AGENTS.md規約準拠**
  - Debug.Logの代わりにILoggerを使用
  - Manager/Service/State/View構造に準拠
- ✅ 拡張性とメンテナンス性の高いアーキテクチャ
- ✅ 既存のコードベース（Manager/Service/State/View構造）との一貫性

実装は段階的に進め、各フェーズで動作確認とテストを行うことで、安定したダイアログシステムを構築できます。

