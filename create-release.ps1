#!/usr/bin/env pwsh

Write-Host "Creating TypoZap v1.2.0 Release..." -ForegroundColor Green
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "publish") {
    Remove-Item "publish" -Recurse -Force
}
if (Test-Path "TypoZap-v1.2.0-Release") {
    Remove-Item "TypoZap-v1.2.0-Release" -Recurse -Force
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
New-Item -ItemType Directory -Force -Path "TypoZap-v1.2.0-Release" | Out-Null

# Copy main executable
Copy-Item "publish\TypoZap\TypoZap.exe" "TypoZap-v1.2.0-Release\"

# Copy necessary DLLs (these are often part of self-contained, but good to be explicit if needed)
# Based on TypoZap-v1.1.1-Release structure
Copy-Item "publish\TypoZap\D3DCompiler_47_cor3.dll" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue
Copy-Item "publish\TypoZap\PenImc_cor3.dll" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue
Copy-Item "publish\TypoZap\PresentationNative_cor3.dll" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue
Copy-Item "publish\TypoZap\vcruntime140_cor3.dll" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue
Copy-Item "publish\TypoZap\wpfgfx_cor3.dll" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue

# Copy assets folder
Write-Host "Copying assets..." -ForegroundColor Yellow
if (Test-Path "Assets") {
    Copy-Item "Assets" "TypoZap-v1.2.0-Release\Assets" -Recurse
}

# Copy tones.json
Copy-Item "tones.json" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue

# Copy documentation
Copy-Item "README.md" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue
Copy-Item "ICON_USAGE.md" "TypoZap-v1.2.0-Release\" -ErrorAction SilentlyContinue

# Create release notes/README.txt
$version = "1.2.0"
$buildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

@"
TypoZap for Windows v$version
Build Date: $buildDate

🎉 MAJOR UPDATE - Custom Hotkeys & Enhanced UI

Key Changes in v${version}:
✨ NEW FEATURES:
- Custom Hotkey Recording: Set any key combination you prefer (Ctrl+Shift+G, Alt+F12, etc.)
- Enhanced Settings Window: Streamlined interface with hotkey management
- Professional About Dialog: Clean design with clickable GitHub link
- Custom Tone Support: Add your own writing styles via tones.json
- Immediate Hotkey Changes: No restart required for hotkey updates

🔧 IMPROVEMENTS:
- Removed Advanced Settings section for cleaner UI
- Removed "Reset to Defaults" button (replaced with hotkey reset)
- Removed "Paste Key" button from API key dialog
- Updated startup notification to show current hotkey
- Professional "Developed by Manish Jangid" credits
- Enhanced error handling and user feedback

🎨 UI ENHANCEMENTS:
- Cleaner hotkey recording interface
- Better button layouts and spacing
- Improved visual feedback
- Professional styling throughout

🔧 TECHNICAL IMPROVEMENTS:
- Dynamic hotkey registration system
- Enhanced settings persistence
- Better error handling and fallbacks
- Improved code organization

📚 DOCUMENTATION:
- Updated README with custom hotkey guide
- Added custom tones documentation
- Comprehensive troubleshooting section
- Clear setup and usage instructions

🚀 HOW TO USE NEW FEATURES:
1. Right-click system tray icon → Settings
2. Click "Record New Hotkey" to set custom key combination
3. Choose from available writing tones or add custom ones
4. Save settings to apply changes immediately

📁 CUSTOM TONES:
- Edit tones.json to add your own writing styles
- Follow the documented structure for best results
- Restart TypoZap to load new tones

For detailed documentation, see README.md
For support and updates, visit: https://github.com/ManishJangid007/typo_zap_windows

Ready to experience the enhanced TypoZap! 🎉
"@ | Out-File "TypoZap-v1.2.0-Release\README.txt"

Write-Host "Creating release zip..." -ForegroundColor Yellow

# Create zip file
if (Test-Path "TypoZap-v1.2.0.zip") {
    Remove-Item "TypoZap-v1.2.0.zip" -Force
}
Compress-Archive -Path "TypoZap-v1.2.0-Release" -DestinationPath "TypoZap-v1.2.0.zip"

# Get file sizes
$exeSize = (Get-Item "TypoZap-v1.2.0-Release\TypoZap.exe").Length / 1MB
$zipSize = (Get-Item "TypoZap-v1.2.0.zip").Length / 1MB

Write-Host ""
Write-Host "Release v1.2.0 created successfully!" -ForegroundColor Green
Write-Host "Release folder: TypoZap-v1.2.0-Release\" -ForegroundColor Cyan
Write-Host "Zip file: TypoZap-v1.2.0.zip" -ForegroundColor Cyan
Write-Host "Executable size: $([math]::Round($exeSize, 2)) MB" -ForegroundColor Cyan
Write-Host "Zip file size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ready for distribution!" -ForegroundColor Green
