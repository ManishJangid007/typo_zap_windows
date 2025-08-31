#!/usr/bin/env pwsh

Write-Host "🚀 Building TypoZap MSI Installer..." -ForegroundColor Green
Write-Host ""

# Check if WiX Toolset is available
$wixPath = ""
$possibleWixPaths = @(
    "${env:ProgramFiles(x86)}\WiX Toolset v3.11\bin",
    "${env:ProgramFiles}\WiX Toolset v3.11\bin",
    "${env:PROGRAMFILES}\WiX Toolset v3.11\bin",
    "C:\Program Files (x86)\WiX Toolset v3.11\bin",
    "C:\Program Files\WiX Toolset v3.11\bin"
)

foreach ($path in $possibleWixPaths) {
    if (Test-Path "$path\candle.exe") {
        $wixPath = $path
        break
    }
}

if (-not $wixPath) {
    Write-Host "❌ WiX Toolset not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install WiX Toolset v3.11 from:" -ForegroundColor Yellow
    Write-Host "https://github.com/wixtoolset/wix3/releases" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Alternative: Use NSIS installer instead by running build-nsis.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Found WiX Toolset at: $wixPath" -ForegroundColor Green

# First build the application
Write-Host "🔨 Building application..." -ForegroundColor Yellow
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Application build failed!" -ForegroundColor Red
    exit 1
}

# Create build directory
Write-Host "📁 Preparing installer build..." -ForegroundColor Yellow
if (Test-Path "installer\build") {
    Remove-Item "installer\build" -Recurse -Force
}
New-Item -ItemType Directory -Force -Path "installer\build" | Out-Null

# Copy application files to installer build directory
$sourceDir = "bin\Release\net8.0-windows"
$buildDir = "installer\build"

Copy-Item "$sourceDir\*" "$buildDir\" -Recurse -Force
Copy-Item "Assets" "$buildDir\Assets" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "README.md" "$buildDir\" -ErrorAction SilentlyContinue
Copy-Item "ICON_USAGE.md" "$buildDir\" -ErrorAction SilentlyContinue

# Create license file for installer
@"
MIT License

Copyright (c) 2024 TypoZap

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
"@ | Out-File "$buildDir\License.txt"

# Compile WiX source
Write-Host "🔥 Compiling WiX installer..." -ForegroundColor Yellow
& "$wixPath\candle.exe" -dSourceDir="$buildDir" installer\TypoZap.wxs -out installer\TypoZap.wixobj

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ WiX compilation failed!" -ForegroundColor Red
    exit 1
}

# Link MSI
Write-Host "🔗 Linking MSI package..." -ForegroundColor Yellow
& "$wixPath\light.exe" installer\TypoZap.wixobj -out installer\TypoZap.msi -ext WixUIExtension

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ MSI linking failed!" -ForegroundColor Red
    exit 1
}

# Clean up intermediate files
Remove-Item "installer\TypoZap.wixobj" -ErrorAction SilentlyContinue
Remove-Item "installer\TypoZap.wixpdb" -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "✅ MSI Installer created successfully!" -ForegroundColor Green
Write-Host "📁 Location: installer\TypoZap.msi" -ForegroundColor Cyan
Write-Host "📦 Size: $([math]::Round((Get-Item 'installer\TypoZap.msi').Length / 1MB, 2)) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "🎯 Ready for distribution!" -ForegroundColor Green
Write-Host "Note: MSI may trigger SmartScreen warning on first run - this is normal for unsigned installers." -ForegroundColor Yellow
