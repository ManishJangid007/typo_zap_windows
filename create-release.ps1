#!/usr/bin/env pwsh

Write-Host "Creating TypoZap v1.1.3 Release..." -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "publish") {
    Remove-Item "publish" -Recurse -Force
}
if (Test-Path "TypoZap-v1.1.3-Release") {
    Remove-Item "TypoZap-v1.1.3-Release" -Recurse -Force
}

# Build the application
Write-Host "Building TypoZap..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "publish\TypoZap" --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create release directory
Write-Host "Creating release directory..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "TypoZap-v1.1.3-Release" | Out-Null

# Copy main executable
Copy-Item "publish\TypoZap\TypoZap.exe" "TypoZap-v1.1.3-Release\"

# Copy required DLLs
$requiredDlls = @(
    "D3DCompiler_47_cor3.dll",
    "PenImc_cor3.dll", 
    "PresentationNative_cor3.dll",
    "vcruntime140_cor3.dll",
    "wpfgfx_cor3.dll"
)

foreach ($dll in $requiredDlls) {
    if (Test-Path "publish\TypoZap\$dll") {
        Copy-Item "publish\TypoZap\$dll" "TypoZap-v1.1.3-Release\"
    }
}

# Copy assets
if (Test-Path "Assets") {
    Copy-Item "Assets" "TypoZap-v1.1.3-Release\Assets" -Recurse
}

# Copy tones.json
if (Test-Path "tones.json") {
    Copy-Item "tones.json" "TypoZap-v1.1.3-Release\"
}

# Copy documentation
Copy-Item "README.md" "TypoZap-v1.1.3-Release\" -ErrorAction SilentlyContinue
Copy-Item "ICON_USAGE.md" "TypoZap-v1.1.3-Release\" -ErrorAction SilentlyContinue

# Create release README
$version = "1.1.3"
$buildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

@"
TypoZap for Windows v$version
Build Date: $buildDate

Files in this package:
- TypoZap.exe - Main application
- Assets\ - Application icons and resources
- tones.json - Tone configuration
- README.md - Usage instructions

Installation:
1. Extract to any folder (e.g., C:\Program Files\TypoZap)
2. Run TypoZap.exe
3. Enter your Gemini API key when prompted
4. Start using with Ctrl+Alt+Q!

No installation required - this is a portable executable.

Changes in v1.1.3:
- Updated About dialog with professional design
- Removed version number from About dialog
- Added clickable GitHub link in About dialog
- Streamlined Settings window (removed Advanced Settings)
- Removed "Reset to Defaults" button from Settings
- Added professional "Developed by Manish Jangid" credits
- Removed "Paste Key" button from API key dialog
- Improved overall UI consistency and professionalism
"@ | Out-File "TypoZap-v1.1.3-Release\README.txt"

Write-Host "Creating release zip..." -ForegroundColor Yellow

# Create zip file
if (Test-Path "TypoZap-v1.1.3.zip") {
    Remove-Item "TypoZap-v1.1.3.zip" -Force
}

Compress-Archive -Path "TypoZap-v1.1.3-Release\*" -DestinationPath "TypoZap-v1.1.3.zip"

# Get file sizes
$exeSize = (Get-Item "TypoZap-v1.1.3-Release\TypoZap.exe").Length / 1MB
$zipSize = (Get-Item "TypoZap-v1.1.3.zip").Length / 1MB

Write-Host ""
Write-Host "Release v1.1.3 created successfully!" -ForegroundColor Green
Write-Host "Release folder: TypoZap-v1.1.3-Release\" -ForegroundColor Cyan
Write-Host "Zip file: TypoZap-v1.1.3.zip" -ForegroundColor Cyan
Write-Host "Executable size: $([math]::Round($exeSize, 2)) MB" -ForegroundColor Cyan
Write-Host "Zip file size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ready for distribution!" -ForegroundColor Green
