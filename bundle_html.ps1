$template = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\src\Resources\viewer.template.html", [System.Text.Encoding]::UTF8)
$gfmCss = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\assets\github-markdown.min.css", [System.Text.Encoding]::UTF8)
$darkCss = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\assets\highlight-dark.min.css", [System.Text.Encoding]::UTF8)
$lightCss = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\assets\highlight-light.min.css", [System.Text.Encoding]::UTF8)
$markedJs = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\assets\marked.min.js", [System.Text.Encoding]::UTF8)
$hljsJs = [System.IO.File]::ReadAllText("c:\HisaData\App\MDViewer\assets\highlight.min.js", [System.Text.Encoding]::UTF8)

$html = $template.Replace("/* __GITHUB_MARKDOWN_CSS__ */", $gfmCss)
$html = $html.Replace("/* __HIGHLIGHT_DARK_CSS__ */", $darkCss)
$html = $html.Replace("/* __HIGHLIGHT_LIGHT_CSS__ */", $lightCss)
$html = $html.Replace("/* __MARKED_JS__ */", $markedJs)
$html = $html.Replace("/* __HIGHLIGHT_JS__ */", $hljsJs)

[System.IO.File]::WriteAllText("c:\HisaData\App\MDViewer\src\Resources\viewer.html", $html, [System.Text.Encoding]::UTF8)
Write-Output "viewer.html generated. Size: $((Get-Item 'c:\HisaData\App\MDViewer\src\Resources\viewer.html').Length) bytes"
