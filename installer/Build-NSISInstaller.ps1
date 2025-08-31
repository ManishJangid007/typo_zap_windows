#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Builds the TypoZap NSIS installer
.DESCRIPTION
    This script builds the TypoZap NSIS installer by:
    1. Checking for NSIS installation
    2. Building the self-contained executable
    3. Preparing build directory with all required files
    4. Compiling the NSIS installer
.PARAMETER BuildConfiguration
    The build configuration to use (Debug or Release). Default is Release.
.PARAMETER OutputDirectory
    The output directory for the installer. Default is current directory.
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$BuildConfiguration = "Release",
    [string]$OutputDirectory = ".",
    [switch]$SkipBuild
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "TypoZap NSIS Installer Builder" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Check for NSIS installation
Write-Host "`nChecking for NSIS installation..." -ForegroundColor Yellow
$nsisPath = $null
$possiblePaths = @(
    "${env:ProgramFiles}\NSIS\makensis.exe",
    "${env:ProgramFiles(x86)}\NSIS\makensis.exe",
    "C:\Program Files\NSIS\makensis.exe",
    "C:\Program Files (x86)\NSIS\makensis.exe"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $nsisPath = $path
        break
    }
}

if (-not $nsisPath) {
    Write-Host "❌ NSIS not found!" -ForegroundColor Red
    Write-Host "Please install NSIS from: https://nsis.sourceforge.io/Download" -ForegroundColor Red
    Write-Host "Alternatively, you can install it via Chocolatey: choco install nsis" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Found NSIS at: $nsisPath" -ForegroundColor Green

# Get project root directory
$projectRoot = Split-Path -Parent $PSScriptRoot
$installerDir = $PSScriptRoot
$buildDir = Join-Path $installerDir "build"

Write-Host "`nProject root: $projectRoot" -ForegroundColor Cyan
Write-Host "Installer directory: $installerDir" -ForegroundColor Cyan
Write-Host "Build directory: $buildDir" -ForegroundColor Cyan

if (-not $SkipBuild) {
    # Build the self-contained executable first
    Write-Host "`nBuilding self-contained executable..." -ForegroundColor Yellow
    try {
        Push-Location $projectRoot
        
        # Clean previous builds
        if (Test-Path "bin") {
            Remove-Item -Path "bin" -Recurse -Force
        }
        if (Test-Path "obj") {
            Remove-Item -Path "obj" -Recurse -Force
        }
        
        # Build self-contained executable
        $publishArgs = @(
            "publish"
            "-c", $BuildConfiguration
            "-r", "win-x64"
            "--self-contained", "true"
            "-p:PublishSingleFile=true"
            "-p:PublishTrimmed=false"
            "-p:IncludeNativeLibrariesForSelfExtract=true"
            "--output", $buildDir
        )
        
        Write-Host "Running: dotnet $($publishArgs -join ' ')" -ForegroundColor Gray
        & dotnet @publishArgs
        
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed with exit code $LASTEXITCODE"
        }
        
        Write-Host "✅ Build completed successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Build failed: $_" -ForegroundColor Red
        exit 1
    }
    finally {
        Pop-Location
    }
}

# Prepare build directory
Write-Host "`nPreparing build directory..." -ForegroundColor Yellow

# Ensure build directory exists
if (-not (Test-Path $buildDir)) {
    New-Item -Path $buildDir -ItemType Directory -Force | Out-Null
}

# Copy additional files to build directory
$filesToCopy = @{
    "README.md" = Join-Path $projectRoot "README.md"
    "ICON_USAGE.md" = Join-Path $projectRoot "ICON_USAGE.md" 
    "License.txt" = Join-Path $projectRoot "LICENSE"
    "Assets" = Join-Path $projectRoot "Assets"
}

foreach ($target in $filesToCopy.Keys) {
    $source = $filesToCopy[$target]
    $destination = Join-Path $buildDir $target
    
    if (Test-Path $source) {
        if (Test-Path $source -PathType Container) {
            # Copy directory
            if (Test-Path $destination) {
                Remove-Item -Path $destination -Recurse -Force
            }
            Copy-Item -Path $source -Destination $destination -Recurse -Force
            Write-Host "  ✅ Copied directory: $target" -ForegroundColor Green
        } else {
            # Copy file
            Copy-Item -Path $source -Destination $destination -Force
            Write-Host "  ✅ Copied file: $target" -ForegroundColor Green
        }
    } else {
        Write-Host "  ⚠️  Source not found: $source" -ForegroundColor Yellow
    }
}

# Verify main executable exists
$mainExe = Join-Path $buildDir "TypoZap.exe"
if (-not (Test-Path $mainExe)) {
    Write-Host "❌ Main executable not found: $mainExe" -ForegroundColor Red
    Write-Host "Please ensure the build completed successfully" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build directory prepared" -ForegroundColor Green

# Build the NSIS installer
Write-Host "`nBuilding NSIS installer..." -ForegroundColor Yellow

$nsiScript = Join-Path $installerDir "TypoZap.nsi"
$outputInstaller = Join-Path $OutputDirectory "TypoZap-Setup.exe"

try {
    Push-Location $installerDir
    
    # Run makensis
    Write-Host "Running: $nsisPath `"$nsiScript`"" -ForegroundColor Gray
    & $nsisPath $nsiScript
    
    if ($LASTEXITCODE -ne 0) {
        throw "NSIS compilation failed with exit code $LASTEXITCODE"
    }
    
    # Move installer to desired output location if different
    $installerOutput = Join-Path $installerDir "TypoZap-Setup.exe"
    if ((Resolve-Path $OutputDirectory) -ne (Resolve-Path $installerDir)) {
        if (Test-Path $installerOutput) {
            Move-Item -Path $installerOutput -Destination $outputInstaller -Force
            Write-Host "✅ Installer moved to: $outputInstaller" -ForegroundColor Green
        }
    } else {
        Write-Host "✅ Installer created: $installerOutput" -ForegroundColor Green
    }
}
catch {
    Write-Host "❌ NSIS compilation failed: $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}

Write-Host "`n🎉 NSIS installer build completed successfully!" -ForegroundColor Green
Write-Host "`nInstaller details:" -ForegroundColor Cyan
Write-Host "  • Location: $outputInstaller" -ForegroundColor White
Write-Host "  • Size: $([math]::Round((Get-Item $outputInstaller).Length / 1MB, 2)) MB" -ForegroundColor White
Write-Host "  • Type: NSIS Installer (unsigned)" -ForegroundColor White

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  • Test the installer on a clean system" -ForegroundColor White
Write-Host "  • Consider code signing to avoid SmartScreen warnings" -ForegroundColor White
Write-Host "  • Update version numbers in TypoZap.nsi for future releases" -ForegroundColor White

Write-Host "`nInstaller features:" -ForegroundColor Cyan
Write-Host "  • Professional installer UI" -ForegroundColor White
Write-Host "  • Start menu shortcuts" -ForegroundColor White
Write-Host "  • Desktop shortcut" -ForegroundColor White
Write-Host "  • Autostart option" -ForegroundColor White
Write-Host "  • Proper uninstaller" -ForegroundColor White
Write-Host "  • Registry entries for Add/Remove Programs" -ForegroundColor White
