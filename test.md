# MDViewer 検証用ドキュメント

これは **MDViewer** の動作テスト用 Markdown ファイルです。Windows 11 上で超軽量・単一 EXE として軽快に動作します。

## 機能一覧
- [x] 超軽量 (わずか 1MB の単一 exe)
- [x] インストーラ不要・ポータブル
- [x] GitHub スタイル Markdown (GFM)
- [x] 自動リロード (Auto Reload)
- [x] 目次 (TOC) 自動生成とスクロール連動
- [x] ダークモード / ライトモード切替
- [x] シンタックスハイライト & コードコピー
- [x] 印刷・PDF 保存
- [x] ページ内全文検索 (Ctrl+F)
- [x] 常に最前面ピン留め

## ソースコード表示テスト

### C#
```csharp
using System;

namespace Demo {
    public class MarkdownViewer {
        public static void Main() {
            Console.WriteLine("MDViewer - 超軽量 1.07MB!");
        }
    }
}
```

### TypeScript / JavaScript
```typescript
interface ViewerConfig {
  theme: 'dark' | 'light';
  autoReload: boolean;
  fontSize: number;
}

const config: ViewerConfig = {
  theme: 'dark',
  autoReload: true,
  fontSize: 15
};
```

## テーブル表示テスト

| ショートカット | 機能名 | 詳細 |
| :--- | :--- | :--- |
| `Ctrl + O` | ファイルを開く | ダイアログから .md ファイルを選択 |
| `Ctrl + T` | 目次トグル | サイドバーのアウトラインを表示/非表示 |
| `Ctrl + F` | ページ内検索 | マッチ個所をハイライト＆前後移動 |
| `Ctrl + P` | 印刷 / PDF | 綺麗にレイアウトされた印刷プレビュー |
| `Ctrl + R` / `F5` | 再読み込み | 手動でドキュメントを最新化 |

## 引用
> MDViewer は、Edge Chromium (WebView2) エンジンを活用しながら、
> 余計なランタイム同梱を省くことで 1MB 台という驚異的な軽さを実現しています。

---
2026 MDViewer Project
