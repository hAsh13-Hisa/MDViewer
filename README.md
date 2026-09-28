# MDViewer - 超軽量 Markdown Viewer for Windows 11

Windows 11 に最適化された、わずか **約 1MB の単一 EXE ファイル (`MDViewer.exe`) のみ** で動作する超高速・高機能 Markdown ビューアです。

インストーラや外部 DLL、重いランタイムは一切不要。USB メモリに入れて持ち歩くことも可能です。

---

## 🌟 主な特長

1. **超軽量＆ポータブル（単一 EXE / 約 1.07 MB）**
   - 外部依存 DLL なし。`MDViewer.exe` 1 つだけでどこでも即座に動作します。
2. **高速・美麗なレンダリング（Edge WebView2 & GFM 完全対応）**
   - Windows 11 標準の WebView2 を利用し、GitHub と同等の美しい Markdown 描画（表組み・タスクリスト・コードブロックなど）を実現。
3. **完全オフライン動作**
   - Markdown パーサー（marked.js）、ハイライト（highlight.js）、スタイルシート（github-markdown-css）を EXE 内に完全内包。ネット未接続でも高速起動・表示。
4. **リアルタイム自動同期（Live Auto-Reload）**
   - メモ帳や VSCode などの外部エディタでファイルを上書き保存すると、自動検知して瞬時に再描画。閲覧中のスクロール位置も正確に維持されます。
5. **目次（TOC / Outline）サイドバー**
   - 見出し（H1〜H6）から目次を自動生成。クリックで該当箇所にスムーズスクロールし、閲覧位置に合わせて現在地をハイライト。
6. **ダークモード / ライトモード対応**
   - ワンクリックでテーマを切り替え可能。シンタックスハイライトも各テーマに合わせて変化します。
7. **コードブロックのワンクリックコピー**
   - コードブロック右上の「Copy」ボタンをクリックするだけでクリップボードにコピー。
8. **ページ内高速検索（Ctrl + F）**
   - キーワードをハイライトし、Enter / Shift+Enter で前後のマッチ箇所へジャンプ。
9. **印刷 & PDF エクスポート（Ctrl + P）**
   - ナビゲーションバーや目次を隠し、文書本体だけを綺麗に印刷・PDF 保存できます。
10. **ウィンドウ最前面固定（Pin on Top）**
    - 他のウィンドウの上に常駐表示させながら作業できます。

---

## 🚀 使い方

### 1. 起動と閲覧
- **ドラッグ＆ドロップ**: `MDViewer.exe` のウィンドウに `.md` ファイルをドラッグ＆ドロップするだけで即表示。
- **ファイルの関連付け**: `.md` ファイルを右クリック ➔「プログラムから開く」➔「別のアプリを選択」で `MDViewer.exe` を指定すると、ダブルクリックで常に開けるようになります。
- **コマンドライン起動**:
  ```powershell
  .\MDViewer.exe "path\to\document.md"
  ```
- **ダイアログから開く**: 上部ツールバーの 📂 アイコン、または `Ctrl + O` を押してファイルを選択。

---

## ⌨️ ショートカットキー一覧

| キー | 機能 |
| :--- | :--- |
| `Ctrl + O` | ファイルを開く（ダイアログ表示） |
| `Ctrl + T` | 目次（TOC）サイドバーの開閉 |
| `Ctrl + F` | ページ内テキスト検索バーの表示 |
| `Ctrl + P` | 印刷 / PDF 保存ダイアログ表示 |
| `Ctrl + R` / `F5` | 現在のファイルを再読み込み |
| `Ctrl + [+]` / `[-]` | 拡大 / 縮小 |
| `Ctrl + 0` | ズーム倍率リセット (100%) |
| `?` / `F1` | ショートカットキー・ヘルプの表示 |
| `Esc` | 検索バー、画像拡大モーダル、ヘルプを閉じる |

---

## 🛠️ プロジェクト構成

```
MDViewer/
├── MDViewer.exe          # ★ 完成品（単一実行可能ファイル 約 1.07MB）
├── build.ps1             # C# コンパイル & ILRepack パッケージングスクリプト
├── bundle_html.ps1       # アセットをインライン化した viewer.html の生成スクリプト
├── test.md               # 動作確認用サンプルドキュメント
├── src/
│   ├── Program.cs        # C# メインプログラム（WinForms + WebView2 + FileWatcher）
│   └── Resources/
│       ├── viewer.template.html  # UI/CSS/JS テンプレート
│       ├── viewer.html           # 全アセット結合済み HTML（EXE内に埋め込み）
│       └── WebView2Loader.dll   # ネイティブローダー（EXE内に埋め込み）
└── assets/               # marked.js, highlight.js, github-markdown.css
```

---

## 📦 再ビルド方法（カスタマイズ時）

PowerShell で以下を実行するだけで、いつでも最新の `MDViewer.exe` を再ビルドできます。

```powershell
pwsh -File .\bundle_html.ps1
pwsh -File .\build.ps1
```
