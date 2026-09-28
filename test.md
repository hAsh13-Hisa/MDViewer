# MDViewer 検証用ドキュメント (v1.1.0)

これは **MDViewer** の動作テスト用 Markdown ファイルです。Windows 11 上で超軽量・単一 EXE として軽快に動作します。

## v1.1.0 視認性向上アップデートの確認
- [x] **ダークモードのコードコントラスト大幅強化**:
  - コードブロック（`pre`）の背景と枠線を背景からしっかり浮き立たせ、コード文字色・コメント・キーワードの明度を向上。
  - インラインコード（`variableName`、`int count = 10;`、`GET /api/v1/users`）も専用の背景・枠線・アクセントカラーでくっきり表示。
- [x] **ライトモードの表（Table）視認性大幅強化**:
  - 全セルをくっきりとしたグレー枠線（`border: 1.5px`）で引き締め、ヘッダーにグレー背景と強調下線を追加。
  - 偶数行の背景色と、マウスホバー時の行ハイライト効果により、行追跡が極めて容易に。
- [x] **バージョン管理の統合**:
  - UI 上部（ヘッダーバッジ）、ウィンドウタイトル、ステータスバー、ヘルプダイアログに `v1.1.0` を明記。
  - アセンブリ情報にもセマンティックバージョンを登録。

---

## テーブル表示テスト（ライトモードでの視認性を確認）

以下の表は、ライトモードでも罫線・ヘッダー背景・ホバー行がはっきりと見えるようにコントラスト調整されています。

| 機能名 | ショートカット | 説明 | 状態 |
| :--- | :---: | :--- | :---: |
| ファイルを開く | `Ctrl + O` | エクスプローラーダイアログからファイルを選択 | 実装済 |
| 目次トグル | `Ctrl + T` | アウトラインサイドバーの表示 / 非表示 | 実装済 |
| ページ内検索 | `Ctrl + F` | 高速テキスト検索＆ハイライト＆ジャンプ | 実装済 |
| 印刷 / PDF 出力 | `Ctrl + P` | ドキュメント本体のみを綺麗にレイアウト印刷 | 実装済 |
| 手動再読み込み | `Ctrl + R` / `F5` | 外部で変更されたファイルを即時リロード | 実装済 |
| テーマ切り替え | ツールバー | ダークモード / ライトモードを瞬時にトグル | 実装済 |
| 最前面ピン留め | ツールバー | 常に手前に表示してエディタと並行作業 | 実装済 |

---

## コード表示テスト（ダークモードでの視認性を確認）

インラインコードの例: `var total = calculateSum(items);` や `Console.WriteLine("Done");`

### C# コードブロック
```csharp
using System;
using System.IO;

namespace MDViewer {
    /// <summary>
    /// 超軽量 Markdown ビューアのメインエントリ
    /// </summary>
    public static class Program {
        public const string Version = "1.1.0";

        [STAThread]
        public static void Main(string[] args) {
            Console.WriteLine("MDViewer " + Version + " is running smoothly!");
            // 暗い背景でもコメントやキーワードが鮮明に見えます
        }
    }
}
```

### TypeScript / JavaScript コードブロック
```typescript
interface DocumentMeta {
  title: string;
  version: string;
  charCount: number;
  isModified: boolean;
}

// 高コントラストな構文ハイライト
function renderMarkdownDocument(content: string, meta: DocumentMeta): void {
  const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
  console.log(`Rendering [${meta.title}] v${meta.version} (Theme: ${isDark ? 'Dark' : 'Light'})`);
}
```

### Python コードブロック
```python
def check_contrast_ratio(bg_color: str, fg_color: str) -> float:
    # WCAG 2.1 AAA 準拠のコントラスト比を計算
    print(f"Checking contrast between {bg_color} and {fg_color}")
    return 7.5  # 高コントラスト合格
```

---

## 引用とアラート
> **Note**: MDViewer は Edge Chromium (WebView2) エンジンを活用しながら、
> 余計なランタイム同梱を省くことで 1MB 台という驚異的な軽さを実現しています。

---
MDViewer v1.1.0 (2026)
