using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

[assembly: AssemblyTitle("MDViewer")]
[assembly: AssemblyDescription("Lightweight Single-Executable Markdown Viewer for Windows 11")]
[assembly: AssemblyProduct("MDViewer")]
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
[assembly: AssemblyInformationalVersion("1.2.0")]

namespace MDViewer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Setup WebView2 native loader folder in %LOCALAPPDATA%\MDViewer\native
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string nativeDir = Path.Combine(localAppData, "MDViewer", "native");
                Directory.CreateDirectory(nativeDir);

                string loaderPath = Path.Combine(nativeDir, "WebView2Loader.dll");
                ExtractResource("MDViewer.Resources.WebView2Loader.dll", loaderPath);

                CoreWebView2Environment.SetLoaderDllFolderPath(nativeDir);

                string initialFile = (args != null && args.Length > 0 && File.Exists(args[0])) ? Path.GetFullPath(args[0]) : null;
                Application.Run(new MainForm(initialFile));
            }
            catch (Exception ex)
            {
                MessageBox.Show("起動エラー: " + ex.Message + "\n\n" + ex.StackTrace, "MDViewer エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ExtractResource(string resourceName, string targetPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                if (File.Exists(targetPath) && new FileInfo(targetPath).Length == stream.Length)
                {
                    return; // Already up to date
                }

                using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }

    public class MainForm : Form
    {
        private WebView2 webView;
        private FileSystemWatcher fileWatcher;
        private string currentFilePath;
        private System.Threading.Timer reloadDebounceTimer;
        private readonly string initialFile;
        private bool initialFileLoaded = false;
        private bool isPinned = false;

        public MainForm(string initialFile)
        {
            this.initialFile = initialFile;

            // Form properties
            this.Text = "MDViewer v1.2.0";
            this.Width = 1100;
            this.Height = 780;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 400);
            this.AllowDrop = true;

            // Create App Icon
            this.Icon = CreateAppIcon();

            // Initialize WebView2
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            // Drag and drop on Form
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;

            // Form events
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private Icon CreateAppIcon()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("MDViewer.Resources.app.ico"))
                {
                    if (stream != null)
                    {
                        return new Icon(stream);
                    }
                }
            }
            catch { }

            // Fallback dynamic icon
            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Brush brush = new SolidBrush(Color.FromArgb(9, 105, 218)))
                {
                    g.FillRectangle(brush, 4, 3, 24, 26);
                }

                using (Pen pen = new Pen(Color.White, 2.2f))
                {
                    g.DrawLines(pen, new Point[] {
                        new Point(8, 20),
                        new Point(8, 11),
                        new Point(12, 16),
                        new Point(16, 11),
                        new Point(16, 20)
                    });
                    g.DrawLines(pen, new Point[] {
                        new Point(20, 11),
                        new Point(20, 19)
                    });
                    g.DrawLines(pen, new Point[] {
                        new Point(18, 17),
                        new Point(20, 20),
                        new Point(22, 17)
                    });
                }
            }
            return Icon.FromHandle(bmp.GetHicon());
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string userDataFolder = Path.Combine(localAppData, "MDViewer", "WebView2Data");
                Directory.CreateDirectory(userDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.NavigationCompleted += WebView_NavigationCompleted;

                // Load embedded viewer.html
                string html = LoadViewerHtml();
                webView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 初期化エラー: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string LoadViewerHtml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("MDViewer.Resources.viewer.html"))
            {
                if (stream == null) throw new FileNotFoundException("リソース viewer.html が見つかりません。");
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess && !initialFileLoaded && !string.IsNullOrEmpty(initialFile))
            {
                initialFileLoaded = true;
                OpenFile(initialFile);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                // Parse simple message types
                if (json.Contains("\"open_file_dialog\""))
                {
                    ShowOpenFileDialog();
                }
                else if (json.Contains("\"toggle_pin\""))
                {
                    isPinned = !isPinned;
                    this.TopMost = isPinned;
                }
                else if (json.Contains("\"reload_file\""))
                {
                    ReloadCurrentFile();
                }
                else if (json.Contains("\"open_external_url\""))
                {
                    var match = Regex.Match(json, "\"url\"\\s*:\\s*\"([^\"]+)\"");
                    if (match.Success)
                    {
                        string url = match.Groups[1].Value.Replace("\\/", "/");
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                    }
                }
                else if (json.Contains("\"ui_ready\""))
                {
                    if (!initialFileLoaded && !string.IsNullOrEmpty(initialFile))
                    {
                        initialFileLoaded = true;
                        OpenFile(initialFile);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Message handle error: " + ex.Message);
            }
        }

        public void OpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            try
            {
                currentFilePath = Path.GetFullPath(filePath);
                string dir = Path.GetDirectoryName(currentFilePath);
                string fileName = Path.GetFileName(currentFilePath);

                // Setup virtual host mapping for local relative images
                try
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        "local.mdviewer",
                        dir,
                        CoreWebView2HostResourceAccessKind.Allow
                    );
                }
                catch { }

                // Read file with retry in case external editor has lock
                string content = ReadFileWithRetry(currentFilePath);
                string lastMod = File.GetLastWriteTime(currentFilePath).ToString("yyyy/MM/dd HH:mm:ss");

                // Process relative image paths: e.g. ![alt](images/pic.png) -> ![alt](https://local.mdviewer/images/pic.png)
                string processedContent = ProcessImagePaths(content);

                // Send to JS
                string script = string.Format(
                    "window.renderMarkdown({0}, {1}, {2});",
                    ToJsonString(processedContent),
                    ToJsonString(currentFilePath),
                    ToJsonString(lastMod)
                );

                this.BeginInvoke(new Action(() => {
                    this.Text = fileName + " - MDViewer v1.2.0";
                    webView.ExecuteScriptAsync(script);
                }));

                // Setup file watcher
                SetupFileWatcher(currentFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ファイルの読み込みに失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string ProcessImagePaths(string markdown)
        {
            // Match markdown image syntax: ![alt](url)
            return Regex.Replace(markdown, @"!\[([^\]]*)\]\(([^)]+)\)", m =>
            {
                string alt = m.Groups[1].Value;
                string url = m.Groups[2].Value.Trim();

                // If already absolute or web URL or data uri, leave as is
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("file://", StringComparison.OrdinalIgnoreCase) ||
                    Regex.IsMatch(url, @"^[a-zA-Z]:\\"))
                {
                    return m.Value;
                }

                // Strip leading ./ if present
                if (url.StartsWith("./")) url = url.Substring(2);

                string virtualUrl = "https://local.mdviewer/" + url.Replace('\\', '/');
                return string.Format("![{0}]({1})", alt, virtualUrl);
            });
        }

        private string ReadFileWithRetry(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                }
            }
            return File.ReadAllText(path, Encoding.UTF8);
        }

        private void SetupFileWatcher(string path)
        {
            try
            {
                if (fileWatcher != null)
                {
                    fileWatcher.EnableRaisingEvents = false;
                    fileWatcher.Dispose();
                    fileWatcher = null;
                }

                string dir = Path.GetDirectoryName(path);
                string filter = Path.GetFileName(path);

                fileWatcher = new FileSystemWatcher(dir, filter);
                fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
                fileWatcher.Changed += (s, e) => DebounceReload();
                fileWatcher.Created += (s, e) => DebounceReload();
                fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FileWatcher error: " + ex.Message);
            }
        }

        private void DebounceReload()
        {
            if (reloadDebounceTimer != null)
            {
                reloadDebounceTimer.Dispose();
            }
            reloadDebounceTimer = new System.Threading.Timer(_ =>
            {
                this.BeginInvoke(new Action(() => ReloadCurrentFile()));
            }, null, 250, Timeout.Infinite);
        }

        private void ReloadCurrentFile()
        {
            if (!string.IsNullOrEmpty(currentFilePath) && File.Exists(currentFilePath))
            {
                OpenFile(currentFilePath);
            }
        }

        private void ShowOpenFileDialog()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Markdown ファイルを開く";
                ofd.Filter = "Markdown Files (*.md;*.markdown;*.mdown;*.mkd)|*.md;*.markdown;*.mdown;*.mkd|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    OpenFile(ofd.FileName);
                }
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    OpenFile(files[0]);
                }
            }
        }

        private string ToJsonString(string text)
        {
            if (text == null) return "null";
            var sb = new StringBuilder("\"");
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < ' ')
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fileWatcher != null)
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }
            if (reloadDebounceTimer != null)
            {
                reloadDebounceTimer.Dispose();
            }
        }
    }
}
