# Changelog

All notable changes to TypoZap will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-08-31

### 🎉 Initial Release

TypoZap v1.0.0 is the first stable release of the AI-powered grammar and style correction tool for Windows.

### ✨ Added

#### Core Features
- **AI-powered text correction** using Google Gemini API
- **System-wide hotkey support** (default: Ctrl+Shift+F)
- **Universal text selection and replacement** works in any Windows application
- **System tray integration** with minimal UI footprint
- **Secure API key management** via Windows Credential Manager
- **Customizable hotkey configuration** in settings

#### Smart Copy Detection
- **Intelligent clipboard monitoring** with fallback mechanisms  
- **Microsoft Teams compatibility** - handles paste+select scenarios correctly
- **Multi-tier copy detection** system:
  - Standard clipboard change detection
  - Fallback copy with delay for slow applications
  - Special handling for same-content scenarios (Teams)
- **Robust error handling** with user-friendly messages

#### User Interface
- **Clean system tray interface** with context menu
- **Settings window** for API key and hotkey configuration
- **API key validation** with secure credential storage
- **Visual feedback** during text processing
- **Professional application icons** and branding

#### Application Architecture  
- **WPF-based Windows application** (.NET 8.0)
- **Global hotkey management** using Windows API
- **Asynchronous AI processing** with proper error handling
- **Modular service architecture** (GeminiService, ClipboardManager, HotkeyManager)
- **Thread-safe operations** for UI updates

### 📦 Distribution & Installation

#### Self-Contained Executable
- **Single-file deployment** - no .NET runtime installation required
- **Windows 10/11 64-bit support**
- **155MB self-contained executable** with all dependencies

#### Multiple Installation Options
- **Manual installation** - extract and run
- **Batch installer** (`install.bat`) with admin privileges
  - Automated installation to Program Files
  - Desktop and Start Menu shortcuts
  - Optional Windows startup integration
  - Complete uninstaller support
- **PowerShell installer** (`Install-TypoZap.ps1`) with advanced features
  - Colored output and progress feedback
  - Parameter support for uninstall and autostart options
  - Enhanced error handling and validation

#### NSIS Installer Support
- **Professional installer scripts** for future MSI/EXE installers
- **WiX Toolset configuration** for MSI packages
- **Build automation scripts** for packaging

### 🔧 Technical Improvements

#### Text Processing
- **Context-aware grammar correction** prompts for Gemini AI
- **Intelligent text selection handling** across different applications
- **Clipboard state management** with original content preservation
- **Retry mechanisms** for failed operations

#### Error Handling & Reliability
- **Comprehensive exception handling** throughout the application
- **User-friendly error messages** with actionable guidance
- **Graceful degradation** when services are unavailable
- **Logging and diagnostics** support

#### Performance & Optimization
- **Optimized clipboard operations** with minimal system impact
- **Efficient hotkey registration** and cleanup
- **Lazy loading** of services and resources
- **Memory-efficient string operations**

### 📚 Documentation

#### User Documentation
- **Comprehensive README.md** with setup and usage instructions
- **Detailed installation guide** (INSTALL.md) with multiple installation methods
- **Icon usage documentation** (ICON_USAGE.md) with attribution
- **Troubleshooting guides** for common issues

#### Developer Documentation
- **Clean, well-commented codebase**
- **Modular architecture** with separation of concerns
- **Build scripts** and automation tools
- **Installation and packaging documentation**

### 🔐 Security & Privacy

#### Data Protection
- **Secure API key storage** using Windows Credential Manager
- **No local data persistence** - text is processed in memory only
- **HTTPS-only communication** with Google Gemini API
- **Minimal data transmission** - only selected text is sent for processing

#### Application Security
- **No elevated privileges required** for normal operation
- **Secure hotkey registration** with proper cleanup
- **Input validation** for all user inputs
- **Safe clipboard operations** with error boundaries

### 🏗️ Build & Development

#### Development Environment
- **.NET 8.0 Windows target framework**
- **WPF application framework**
- **JSON configuration management**
- **NuGet package dependencies** for HTTP client and JSON handling

#### Build System
- **Self-contained publishing** with single-file output
- **Build automation scripts** (PowerShell)
- **Multi-format installer generation** (Batch, PowerShell, NSIS)
- **Automated packaging** and distribution preparation

### 💡 Known Limitations

#### Current Scope
- **Windows-only** - macOS and Linux versions not available
- **Requires internet connection** for AI processing
- **Google Gemini API dependency** - requires valid API key
- **English language focus** - optimized for English text correction

#### Technical Constraints  
- **Unsigned executable** - may trigger Windows SmartScreen warnings
- **Single hotkey support** - only one system-wide hotkey currently supported
- **Text length limits** - governed by Gemini API limitations
- **Some applications** may have special text selection behaviors

### 🎯 Future Roadmap

#### Planned Features
- **Multiple hotkey support** for different correction types
- **Offline mode** with local language models
- **Multi-language support** beyond English
- **Custom correction prompts** and templates
- **Usage statistics** and analytics dashboard

#### Technical Improvements
- **Code signing certificate** to eliminate security warnings
- **Auto-update mechanism** for seamless updates
- **Plugin architecture** for extensibility
- **Performance optimizations** and memory usage improvements

---

### Installation Requirements

- **Operating System**: Windows 10/11 (64-bit)
- **Memory**: 512MB RAM minimum, 1GB recommended
- **Storage**: 200MB free disk space
- **Network**: Internet connection for AI processing
- **API**: Google Gemini API key (free tier available)

### Getting Started

1. Download `TypoZap-v1.0.0-Windows.zip` from releases
2. Extract to desired location
3. Run installer (`install.bat` as administrator) or extract manually
4. Launch TypoZap and configure your Gemini API key
5. Use Ctrl+Shift+F to correct selected text in any application

### Support & Contributing

- **Issues**: Report bugs and feature requests via GitHub Issues
- **Documentation**: Comprehensive guides included in package
- **Community**: Join discussions and share feedback

---

**Full Changelog**: Initial release - all features are new in v1.0.0

**Download**: TypoZap-v1.0.0-Windows.zip (63.07 MB)
