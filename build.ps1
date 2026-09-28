$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$ilrepack = "packages\ILRepack.2.0.48\tools\ILRepack.exe"

New-Item -ItemType Directory -Force -Path "bin" | Out-Null

$wv2WinForms = "packages\Microsoft.Web.WebView2.1.0.4191.47\lib\net462\Microsoft.Web.WebView2.WinForms.dll"
$wv2Core = "packages\Microsoft.Web.WebView2.1.0.4191.47\lib\net462\Microsoft.Web.WebView2.Core.dll"

Write-Host "Step 1: Compiling C# source..." -ForegroundColor Cyan

$cscArgs = @(
    "/target:winexe",
    "/platform:x64",
    "/optimize+",
    "/out:bin\MDViewer_raw.exe",
    "/reference:System.dll",
    "/reference:System.Drawing.dll",
    "/reference:System.Windows.Forms.dll",
    "/reference:System.Core.dll",
    "/reference:$wv2WinForms",
    "/reference:$wv2Core",
    "/win32icon:src\Resources\app.ico",
    "/resource:src\Resources\viewer.html,MDViewer.Resources.viewer.html",
    "/resource:src\Resources\WebView2Loader.dll,MDViewer.Resources.WebView2Loader.dll",
    "/resource:src\Resources\app.ico,MDViewer.Resources.app.ico",
    "src\Program.cs"
)

& $csc $cscArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "CSC compilation failed with code $LASTEXITCODE"
    exit $LASTEXITCODE
}
Write-Host "Compilation succeeded: bin\MDViewer_raw.exe" -ForegroundColor Green

Write-Host "Step 2: Merging assemblies with ILRepack into single MDViewer.exe..." -ForegroundColor Cyan

$ilrepackArgs = @(
    "/out:MDViewer.exe",
    "/target:winexe",
    "/targetplatform:v4",
    "/wildcards",
    "bin\MDViewer_raw.exe",
    $wv2WinForms,
    $wv2Core
)

& $ilrepack $ilrepackArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "ILRepack failed with code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Single-file executable created successfully!" -ForegroundColor Green
$exe = Get-Item "MDViewer.exe"
Write-Host "Executable: $($exe.FullName)" -ForegroundColor Yellow
Write-Host "Size: $([math]::Round($exe.Length / 1MB, 2)) MB ($($exe.Length) bytes)" -ForegroundColor Yellow
