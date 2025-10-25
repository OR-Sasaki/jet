# Fadeシーン 実装プラン

## 概要

VContainerを使用した依存性注入パターンに基づき、シーン遷移時のフェードイン/フェードアウトシステムを実装します。
SceneLoaderから呼び出され、画面全体をスムーズにフェードさせながらシーン遷移を行います。

## 要件

1. **フェードシーケンス**
   - フェードアウト：画面を暗転させる
   - シーン切り替え：新しいシーンをロード & 古いシーンを破棄
   - フェードイン：画面を明るくする
   - 自動削除：フェードイン完了後、Fadeシーンを自動的にアンロード

2. **SceneLoaderとの統合**
   - SceneLoader経由でシーン遷移を行う
   - Fadeシーンを透過的に挟み込む
   - 既存のシーン遷移コードに影響を与えない

3. **非同期処理**
   - async/awaitを使用した非同期処理
   - フェードアニメーションとシーンロードの適切な同期
   - ロード中の進行状況表示（オプション）

4. **拡張性**
   - フェード時間のカスタマイズ
   - フェードカーブのカスタマイズ（Linear、EaseIn、EaseOut等）
   - ロード画面の追加（将来の拡張用）

## 実装手順

### Phase 1: 基礎の実装
1. **FadeState の実装**
   - フェーズ管理機能
   - ターゲットシーン名の保持
   - フェード時間の保持

2. **FadeView の実装**
   - UI要素の配置（Canvas + CanvasGroup + Image）
   - フェードアニメーション機能
   - AnimationCurveのサポート

3. **FadeService の実装**
   - FadeOut/FadeInメソッド
   - シーンロード/アンロード機能
   - FadeViewの検索機能

4. **FadeManager の実装**
   - フェードシーケンスの制御
   - エラーハンドリング
   - ILoggerによるログ出力（AGENTS.md規約準拠）

### Phase 2: 統合の実装
5. **FadeScope の実装**
   - 依存性注入の設定
   - `Lifetime.Scoped`を使用

6. **FadeStarter の実装**
   - エントリーポイント
   - FadeSceneDataからの設定読み込み

7. **SceneLoader の拡張**
   - Fadeシーンの統合
   - FadeSceneDataの実装
   - 既存の`LoadScene`メソッドを`LoadSceneImmediate`にリネーム
   - 新しい`LoadScene`メソッドを追加（Fade経由）

### Phase 3: Unityエディタでの作業
8. **Fadeシーンの作成**
   - 新しいシーン「Fade」を作成
   - Canvas配置（Screen Space - Overlay、Sort Order: 9999）
   - CanvasGroup + Image（黒色、Stretch to Fill）
   - FadeScopeをアタッチ
   - FadeViewをアタッチ

9. **Build Settingsへの登録**
   - FadeシーンをBuild Settingsに追加

### Phase 4: テストと動作確認
10. **動作確認**
    - Title → Game シーン遷移
    - フェードアウト/インの動作確認
    - ロード時間の確認
    - Fadeシーンの自動削除確認

## アーキテクチャ設計

### 1. クラス構成

```
Fade/
├── Manager/
│   └── FadeManager.cs              # フェード処理の制御
├── Service/
│   └── FadeService.cs              # フェードシーケンスのビジネスロジック
├── State/
│   └── FadeState.cs                # フェード状態の管理
├── View/
│   └── FadeView.cs                 # フェード表示のUI
├── Scope/
│   └── FadeScope.cs                # 依存性注入の設定
└── Starter/
    └── FadeStarter.cs              # エントリーポイント

Root/Service/
└── SceneLoader.cs (拡張)           # Fadeシーンとの統合
```

### 2. FadeManager の責務

**主な機能:**
- フェードシーケンスの開始
- フェード完了の検知
- Fadeシーンのライフサイクル管理

**実装概要:**
```csharp
namespace Fade.Manager
{
    public class FadeManager
    {
        private readonly FadeState _state;
        private readonly FadeService _service;
        private readonly ILogger _logger;

        public FadeManager(FadeState state, FadeService service, ILogger logger);

        // フェードシーケンスを開始
        public async Task StartFadeSequence(string targetSceneName, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f);

        // 現在のフェード状態を取得
        public FadePhase GetCurrentPhase();
    }
}
```

**実装詳細:**
```csharp
public async Task StartFadeSequence(string targetSceneName, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
{
    _logger.Log($"Starting fade sequence to scene: {targetSceneName}");

    try
    {
        // Phase 1: フェードアウト
        _state.SetPhase(FadePhase.FadingOut);
        await _service.FadeOut(fadeOutDuration);

        // Phase 2: シーン切り替え
        _state.SetPhase(FadePhase.Loading);
        await _service.LoadScene(targetSceneName);

        // Phase 3: フェードイン
        _state.SetPhase(FadePhase.FadingIn);
        await _service.FadeIn(fadeInDuration);

        // Phase 4: 完了
        _state.SetPhase(FadePhase.Completed);
        _logger.Log("Fade sequence completed");

        // Fadeシーンをアンロード
        await _service.UnloadFadeScene();
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error during fade sequence: {ex.Message}");
        _state.SetPhase(FadePhase.Error);
    }
}
```

### 3. FadeState の責務

**主な機能:**
- 現在のフェードフェーズの管理
- ターゲットシーン名の保持
- フェード設定の保持

**実装概要:**
```csharp
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
        public void SetPhase(FadePhase phase);

        // 現在のフェーズを取得
        public FadePhase GetPhase();

        // ターゲットシーン名を設定
        public void SetTargetScene(string sceneName);

        // ターゲットシーン名を取得
        public string GetTargetScene();

        // フェード時間を設定
        public void SetDurations(float fadeOut, float fadeIn);

        // フェード時間を取得
        public (float fadeOut, float fadeIn) GetDurations();

        // 状態をリセット
        public void Reset();
    }
}
```

### 4. FadeService の責務

**主な機能:**
- フェードアウトアニメーション
- フェードインアニメーション
- シーンのロード/アンロード処理
- FadeViewの検索

**実装概要:**
```csharp
namespace Fade.Service
{
    public class FadeService
    {
        private readonly ILogger _logger;
        private FadeView _fadeView;

        public FadeService(ILogger logger);

        // FadeViewを検索して初期化
        private void InitializeFadeView();

        // フェードアウト（画面を暗転）
        public async Task FadeOut(float duration);

        // フェードイン（画面を明るく）
        public async Task FadeIn(float duration);

        // シーンをロード
        public async Task LoadScene(string sceneName);

        // Fadeシーンをアンロード
        public async Task UnloadFadeScene();
    }
}
```

**実装詳細:**
```csharp
private void InitializeFadeView()
{
    if (_fadeView == null)
    {
        _fadeView = Object.FindObjectOfType<FadeView>();
        if (_fadeView == null)
        {
            _logger.LogError("FadeView not found in Fade scene!");
        }
    }
}

public async Task FadeOut(float duration)
{
    InitializeFadeView();
    _logger.Log($"Fading out (duration: {duration}s)");
    await _fadeView.AnimateFade(0f, 1f, duration);
}

public async Task FadeIn(float duration)
{
    InitializeFadeView();
    _logger.Log($"Fading in (duration: {duration}s)");
    await _fadeView.AnimateFade(1f, 0f, duration);
}

public async Task LoadScene(string sceneName)
{
    _logger.Log($"Loading scene: {sceneName}");

    // 古いシーンを特定（Fadeシーン以外の最初のシーン）
    string oldSceneName = null;
    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
        var scene = SceneManager.GetSceneAt(i);
        if (scene.name != "Fade")
        {
            oldSceneName = scene.name;
            break;
        }
    }

    // 新しいシーンをロード（加算モード）
    var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    while (!asyncLoad.isDone)
    {
        await Task.Yield();
    }

    // 新しいシーンをアクティブに設定
    var newScene = SceneManager.GetSceneByName(sceneName);
    SceneManager.SetActiveScene(newScene);

    // 古いシーンをアンロード
    if (!string.IsNullOrEmpty(oldSceneName))
    {
        _logger.Log($"Unloading old scene: {oldSceneName}");
        var asyncUnload = SceneManager.UnloadSceneAsync(oldSceneName);
        while (asyncUnload != null && !asyncUnload.isDone)
        {
            await Task.Yield();
        }
    }
}

public async Task UnloadFadeScene()
{
    _logger.Log("Unloading Fade scene");
    var asyncUnload = SceneManager.UnloadSceneAsync("Fade");
    while (asyncUnload != null && !asyncUnload.isDone)
    {
        await Task.Yield();
    }
}
```

### 5. FadeView の責務

**主な機能:**
- フェード用のUIの制御
- アニメーション処理
- CanvasGroupのアルファ値変更

**実装概要:**
```csharp
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
```

### 6. FadeScope の実装

**実装概要:**
```csharp
using VContainer;
using VContainer.Unity;
using Fade.Manager;
using Fade.Service;
using Fade.State;

namespace Fade.Scope
{
    public class FadeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // State
            builder.Register<FadeState>(Lifetime.Scoped);

            // Service
            builder.Register<FadeService>(Lifetime.Scoped);

            // Manager
            builder.Register<FadeManager>(Lifetime.Scoped);

            // Starter
            builder.RegisterEntryPoint<FadeStarter>();
        }
    }
}
```

**注意:**
- Fadeシーンは一時的なシーンのため、`Lifetime.Scoped`を使用
- Scopedライフタイムは、そのシーン（Scope）内でのみ有効
- FadeViewはFadeServiceが`FindObjectOfType`で検索するため、VContainerでの登録は不要
- もし他のScopeから参照する必要がある場合は、RootScopeで登録する
  ```csharp
  // RootScope.cs での登録例（必要な場合のみ）
  protected override void Configure(IContainerBuilder builder)
  {
      // ... 既存の登録 ...
      builder.Register<FadeManager>(Lifetime.Singleton);
  }
  ```

### 7. FadeStarter の実装

**実装概要:**
```csharp
using VContainer.Unity;
using Fade.Manager;
using Fade.State;
using Root.Service;

namespace Fade.Starter
{
    public class FadeStarter : IStartable
    {
        private readonly FadeManager _fadeManager;
        private readonly FadeState _fadeState;

        public FadeStarter(FadeManager fadeManager, FadeState fadeState)
        {
            _fadeManager = fadeManager;
            _fadeState = fadeState;
        }

        public async void Start()
        {
            // FadeSceneDataから設定を取得
            _fadeState.SetTargetScene(FadeSceneData.TargetSceneName);
            _fadeState.SetDurations(FadeSceneData.FadeOutDuration, FadeSceneData.FadeInDuration);

            // フェードシーケンスを開始
            var targetScene = _fadeState.GetTargetScene();
            var (fadeOut, fadeIn) = _fadeState.GetDurations();
            await _fadeManager.StartFadeSequence(targetScene, fadeOut, fadeIn);
        }
    }
}
```

### 8. SceneLoader の拡張

**実装概要:**
```csharp
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Fade.State;

namespace Root.Service
{
    public class SceneLoader
    {
        private readonly ILogger _logger;

        public SceneLoader(ILogger logger)
        {
            _logger = logger;
        }

        // Fadeシーン経由でシーンをロード
        public async void LoadScene(string sceneName, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
        {
            _logger.Log($"Loading scene with fade: {sceneName}");

            // FadeStateを静的に設定（Fadeシーンで使用）
            FadeSceneData.TargetSceneName = sceneName;
            FadeSceneData.FadeOutDuration = fadeOutDuration;
            FadeSceneData.FadeInDuration = fadeInDuration;

            // Fadeシーンをロード
            SceneManager.LoadScene("Fade", LoadSceneMode.Additive);
        }

        // 即座にシーンをロード（フェードなし）
        public void LoadSceneImmediate(string sceneName)
        {
            _logger.Log($"Loading scene immediately: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }

    // Fadeシーン用の静的データ
    public static class FadeSceneData
    {
        public static string TargetSceneName { get; set; }
        public static float FadeOutDuration { get; set; } = 0.5f;
        public static float FadeInDuration { get; set; } = 0.5f;
    }
}
```

**注意:**
- `FadeSceneData`は静的クラスで、Fadeシーンへパラメータを渡すために使用
- FadeStarterでこのデータを読み取り、FadeStateに設定する

## 使用例

### 例1: Titleシーンから Gameシーンへの遷移（フェード付き）

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
                // フェード付きでシーン遷移
                _sceneLoader.LoadScene("Game", fadeOutDuration: 0.3f, fadeInDuration: 0.5f);
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

### 例2: 即座にシーン遷移（フェードなし）

```csharp
// EntryPoint など、初回ロード時
public class EntryPointManager
{
    private readonly SceneLoader _sceneLoader;

    public void Initialize()
    {
        // 初回ロードはフェードなしで即座に遷移
        _sceneLoader.LoadSceneImmediate("Title");
    }
}
```

### 例3: Fadeシーンの構造（Unity エディタでの作業）

```
Fadeシーン
└── Canvas (Screen Space - Overlay, Sort Order: 9999)
    ├── FadeScope (Script)
    └── FadePanel (GameObject + FadeView)
        ├── CanvasGroup (Component)
        └── Image (Component, Color: Black, Alpha: 1, Anchor: Stretch)
```

**設定手順:**
1. 新しいシーン「Fade」を作成（Assets/Scenes/Fade.unity）
2. Canvas を作成
   - Render Mode: Screen Space - Overlay
   - Sort Order: 9999（最前面に表示）
3. Canvas下に空のGameObject「FadePanel」を作成
4. FadePanelに以下を追加：
   - CanvasGroup コンポーネント
   - Image コンポーネント（Color: Black, Alpha: 1）
   - RectTransform を Stretch に設定（全画面）
   - FadeView スクリプト
5. Canvasに FadeScope スクリプトをアタッチ
6. Build Settings に Fade シーンを追加

## 注意点と考慮事項

### 1. シーン管理
- **加算モード（Additive）を使用**
  - Fadeシーンは加算モードでロード
  - 新しいシーンも加算モードでロード
  - 古いシーンを明示的にアンロード
  - これにより、スムーズな切り替えが可能

### 2. Canvas Sort Order
- Fadeシーンのキャンバスは最前面に表示する必要がある
- Sort Order: 9999 を推奨
- これにより、すべてのUIの前面にフェードが表示される

### 3. 非同期処理
- async/awaitを使用してフェードとロードを制御
- `Task.Yield()`で毎フレーム制御を返す
- シーンロード完了を正確に検知

### 4. メモリ管理
- Fadeシーンは一時的なシーンとして使用
- フェード完了後は必ず自動削除
- 古いシーンも適切にアンロード

### 5. エラーハンドリング
- シーンロード失敗時のエラーハンドリング
- Fadeシーンが削除できない場合の対処
- ログ出力による問題の追跡（ILogger使用）

### 6. パフォーマンス
- フェードアニメーションは軽量に保つ
- AnimationCurveで自然な動きを実現
- 不要なUpdate呼び出しを避ける

### 7. デバッグ
- ILoggerで各フェーズをログ出力
- フェード時間の調整が容易
- Build Settingsへのシーン登録を忘れずに

### 8. VContainer ライフタイム管理
- **FadeScopeでは`Lifetime.Scoped`を使用**
  - Fadeシーンは一時的なシーンのため、シーン固有のライフタイムが適切
  - Scopedライフタイムは、そのシーン（Scope）が破棄されると自動的に解放される
  - メモリリークの防止と適切なリソース管理が可能
- **FadeViewの検索**
  - FadeViewはVContainerで登録せず、`FindObjectOfType`で検索
  - Unityシーン内に配置されたGameObjectとして存在
  - FadeServiceが初回アクセス時に自動検索
- **他のScopeから参照する場合**
  - FadeManager等を他のシーンから直接参照する必要がある場合は、RootScopeで`Lifetime.Singleton`として登録
  - 基本的には、FadeSceneDataによる静的データ受け渡しで十分なため、RootScopeへの登録は不要
  - SceneLoaderはRootScopeに登録されているため、すべてのシーンから利用可能

## 今後の拡張案

1. **ロード画面の追加**
   - ロード進行状況の表示
   - ロード中のTips表示
   - プログレスバーの追加

2. **フェードバリエーション**
   - ワイプエフェクト
   - スライドトランジション
   - カスタムシェーダーによるエフェクト

3. **プリロード機能**
   - 次のシーンを事前にロード
   - アセットバンドルのプリロード

4. **フェードカスタマイズ**
   - シーンごとに異なるフェード設定
   - フェード色のカスタマイズ（黒、白など）
   - フェードパターンのプリセット

5. **デバッグツール**
   - フェードシーケンスの可視化
   - フェード時間のリアルタイム調整
   - シーン遷移履歴の記録

## 実装状況

### ⏳ Phase 1: 基礎の実装（準備中）
- **フォルダ構造** - 作成済み（Fade/Manager, Service, State, View, Scope, Starter）
- **FadeState** - 未実装
- **FadeView** - 未実装
- **FadeService** - 未実装
- **FadeManager** - 未実装

### ⏳ Phase 2: 統合の実装（準備中）
- **FadeScope** - 未実装
- **FadeStarter** - 未実装
- **SceneLoader拡張** - 未実装
- **FadeSceneData** - 未実装

### ⏳ Phase 3: Unityエディタでの作業（準備中）
- **Fadeシーン作成** - 未実装
- **Canvas/UI配置** - 未実装
- **Build Settings登録** - 未実装

### ⏳ Phase 4: テストと動作確認（準備中）
- **シーン遷移テスト** - 未実装
- **フェード動作確認** - 未実装

## まとめ

このFadeシステム設計は以下の特徴を持ちます：

- ✅ VContainerの依存性注入パターンに完全に準拠
  - **`Lifetime.Scoped`を使用**した適切なライフタイム管理
  - 一時的なシーンに最適な設計
  - 必要に応じてRootScopeでの登録も可能
- ✅ SceneLoaderとシームレスに統合
- ✅ async/awaitによる非同期処理
- ✅ **AGENTS.md規約準拠**
  - Debug.Logの代わりにILoggerを使用
  - Manager/Service/State/View/Scope/Starter構造に準拠
  - 変数初期化の省略形を使用
- ✅ 拡張性とメンテナンス性の高いアーキテクチャ
- ✅ 既存のコードベースとの一貫性
- ✅ メモリリークの防止と適切なリソース管理

実装は段階的に進め、各フェーズで動作確認とテストを行うことで、安定したフェードシステムを構築できます。
