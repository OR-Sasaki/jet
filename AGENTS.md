# AGENTS.md

このドキュメントは、Unityプロジェクト「Jet」の主要なフォルダとその用途について説明します。

## プロジェクト概要

Unityで開発された物理ベースのゲームで、プレイヤーがジェットパックを使用してステージを進むゲームです。VContainerを使用した依存性注入パターンを採用しています。

## 主要フォルダ構成

### Assets/Scripts/
ゲームの主要なスクリプトが格納されています。

#### アーキテクチャパターン
各シーンごとに以下の構造で整理されています：
- **Manager/**: ビジネスロジックを担当
- **Scope/**: VContainerの依存性注入設定
- **Service/**: サービス層の実装
- **Starter/**: エントリーポイント
- **State/**: 状態管理
- **View/**: UI・ビューの実装

#### 主要なシーン別フォルダ

**EntryPoint/**
- ゲームの初期化処理を担当
- EntryPointManager: ゲーム全体の初期化管理
- EntryPointScope: 依存性注入の設定
- EntryPointStarter: エントリーポイント

**Title/**
- タイトル画面の実装
- TitleManager: タイトル画面の管理
- MenuButtonClickService: メニューボタンのクリック処理
- MenuButton: メニューボタンのUI実装

**Root/**
- ゲーム全体で共有される機能
- DialogService: ダイアログ表示機能
- SceneLoader: シーン遷移機能
- ILogger/EditorLogger: ログ出力機能

**Player/**
- このフォルダは今後削除され、適切なフォルダへ格納される予定
- 新しくファイルを追加しないこと
- プレイヤーキャラクターの実装
- Player.cs: メインのプレイヤー制御（ジェット、爆弾、チェックポイント移動）
- PlayerTrigger.cs: プレイヤーの衝突判定

**Stage/**
- このフォルダは今後削除され、適切なフォルダへ格納される予定
- 新しくファイルを追加しないこと
- ステージ要素の実装
- CheckPoint: チェックポイント機能
- DeadArea: 死亡エリア
- ForceArea: 力場エリア
- PushArea: プッシュエリア

**UI/**
- UI関連のスクリプト
- JetFuelGauge: ジェット燃料ゲージ

**TemplateScene/**
- 新しいシーンを作成する際のテンプレート

### Assets/Scenes/
ゲームのシーンファイルが格納されています。

- **EntryPoint.unity**: ゲームの初期化シーン
- **Title.unity**: タイトル画面
- **Game.unity**: メインゲームシーン
- **Mock/**: テスト・デモ用のシーン
  - Demo.unity: デモシーン
  - GimicSample.unity: ギミックサンプル
  - test_stage.unity: テストステージ

### Assets/Prefabs/
ゲームオブジェクトのプレハブが格納されています。

### Assets/Resources/
ゲーム設定ファイルなどが格納されています。
- GameSettings.asset: ゲームの各種設定

### Assets/ColorMaterials/
色別のマテリアルファイルが格納されています。

### Assets/PhysicsMaterial/
物理マテリアルが格納されています。
- Ice Physics Material: 氷の物理特性
- Player Physics Material: プレイヤーの物理特性

### Assets/ImportedGraphics/
外部からインポートされたグラフィックアセットが格納されています。

## 主要なクラス

### GameManager
- シングルトンパターンで実装
- チェックポイントの管理
- ゲーム全体の状態管理

### Player
- プレイヤーのメイン制御
- ジェットパック機能
- 爆弾機能
- チェックポイント間の移動
- 各種エリアとの相互作用

### GameSettings
- ゲームの各種パラメータ設定
- ジェット燃料、力の設定など

## 技術スタック

- **Unity**: ゲームエンジン
- **VContainer**: 依存性注入フレームワーク
- **C#**: プログラミング言語

## 開発時の注意点

1. 新しいシーンを作成する際は、TemplateSceneフォルダの構造を参考にしてください
2. 依存性注入を使用するため、新しいサービスは適切なScopeで登録してください
3. プレイヤーの物理挙動は複雑なため、変更時は十分なテストを行ってください
4. 開発フローをまとめたドキュメントを作成する際は、docs/yyyymmdd_XXXXX.mdのように作成して
5. 空白行に余計なスペースがある場合は常に削除して
6. Debug.LogやDebug.LogErrorは使用せず、常にRootScopeで解決されるILoggerを使用するようにして
7. 変数の初期化時に省略できるものは省略する
  例)
  ❌ private readonly Dictionary<Type, Func<Dialog>> _factories = new Dictionary<Type, Func<Dialog>>();
  ⭕️ private readonly Dictionary<Type, Func<Dialog>> _factories = new();
8. テキストの表示にはTextMeshProUGUIではなく、TMP_Textを使用する
