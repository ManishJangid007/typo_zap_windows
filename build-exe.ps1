#!/usr/bin/env pwsh

Write-Host "🚀 Building TypoZap Self-Contained Executable..." -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "publish") {
    Remove-Item "publish" -Recurse -Force
}

# Create publish directory
New-Item -ItemType Directory -Force -Path "publish" | Out-Null

# Build self-contained executable
Write-Host "🔨 Building self-contained executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o "publish\TypoZap" --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

# Copy assets
Write-Host "📁 Copying assets..." -ForegroundColor Yellow
if (Test-Path "Assets") {
    Copy-Item "Assets" "publish\TypoZap\Assets" -Recurse
}

# Copy documentation
Copy-Item "README.md" "publish\TypoZap\" -ErrorAction SilentlyContinue
Copy-Item "ICON_USAGE.md" "publish\TypoZap\" -ErrorAction SilentlyContinue

# Create version info
$version = "1.0.0"
$buildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

@"
TypoZap for Windows v$version
Build Date: $buildDate

Files in this package:
- TypoZap.exe - Main application
- Assets\ - Application icons and resources
- README.md - Usage instructions

Installation:
1. Extract to any folder (e.g., C:\Program Files\TypoZap)
2. Run TypoZap.exe
3. Enter your Gemini API key when prompted
4. Start using with Ctrl+Alt+Q!

No installation required - this is a portable executable.
"@ | Out-File "publish\TypoZap\README.txt"

Write-Host ""
Write-Host "✅ Self-contained executable created successfully!" -ForegroundColor Green
Write-Host "📁 Location: publish\TypoZap\TypoZap.exe" -ForegroundColor Cyan
Write-Host "📦 Size: $((Get-Item 'publish\TypoZap\TypoZap.exe').Length / 1MB) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "🎯 Ready for distribution - no .NET runtime required!" -ForegroundColor Green
