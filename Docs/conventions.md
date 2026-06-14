# 開発ルール・命名規則

このプロジェクトでコードを追加・変更する際のルール。
アーキテクチャ全体は [architecture.md](./architecture.md)、自然言語変換処理のI/Oは [nlp-interface.md](./nlp-interface.md) を参照。

## 名前空間・フォルダ構成

- `Assets/Scripts/<領域>/` ごとに名前空間を分ける（例: `AIRunner.Player`, `AIRunner.Companion`, `AIRunner.Commands`, `AIRunner.UI`, `AIRunner.CameraControl`）。
- Editor専用スクリプトは `Assets/Editor/` 以下、名前空間は `AIRunner.EditorTools`。
- 新しい領域を追加する場合は、`Assets/Scripts/<新領域>/` フォルダと対応する名前空間を作る。

## 自然言語変換処理関連のルール

- `ICommandInterpreter` の実装を追加・変更する場合は、必ず [nlp-interface.md](./nlp-interface.md) の入出力定義（`CompanionContext` / `CompanionResponse`）に準拠する。
- `CommandType` / `EmotionType` に値を追加した場合は、
  1. `RuleBasedCommandInterpreter` に対応するハンドリングを追加する
  2. `nlp-interface.md` の表/列挙定義を更新する
  3. 既存の`Interpret`呼び出し側（`AICompanionBrain`）の`switch`に分岐漏れがないか確認する
- `AICompanionBrain` は `ICommandInterpreter` の実装詳細（ルールベースかLLMか）に依存するコードを書かない。差し替えは `SetInterpreter` 経由で行う。
- LLM呼び出しを含む実装は例外を呼び出し側に漏らさない。失敗時は`Unknown`系のフォールバック`CompanionResponse`を返す。

## キャラクター制御のルール

- 移動ロジックは `CharacterController.Move()` に統一する（`Rigidbody`の物理移動と混在させない）。
- AI仲間の行動状態（`CompanionState`）を追加する場合は、`AICompanionController.Update()`の`switch`に分岐を追加し、対応する公開メソッド（`Follow()`のような）を用意する。状態を外部から直接`enum`で設定可能にしない。

## UIのルール

- ゲームロジック（`AICompanionBrain`, `AICompanionController`等）はUIコンポーネント（`UnityEngine.UI`）に直接依存しない。UI側がロジック側のイベント（例: `AICompanionBrain.OnResponse`）を購読する一方向の依存とする。
- 新しいUI要素を `DemoSceneBuilder` で自動生成する場合、`SerializedObject`経由で`[SerializeField] private`フィールドに配線し、Inspectorからの差し替えも可能な状態を保つ。

## コーディングスタイル

- C#標準の命名規則（クラス/メソッド/プロパティ: PascalCase、ローカル変数/引数: camelCase、private フィールド: camelCase）。
- public APIには日本語コメントで「なぜそうしているか」が分かりにくい箇所のみ最小限のコメントを付ける。処理内容をなぞるコメントは書かない。
- Inspectorで調整するパラメータは`[SerializeField] private`+適切な初期値を設定する。

## ドキュメント更新ルール

- アーキテクチャに影響する変更（新コンポーネント追加、データフロー変更、`ICommandInterpreter`関連の変更）を行った場合は、`Docs/`配下の該当ドキュメントを同じPR/作業内で更新する。
- 大きな設計変更を行う前に、関連ドキュメントを読んで既存方針との矛盾がないか確認する。
