# Changelog

All notable changes to TypoZap will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2025-09-15

### 🐛 Fixed

- **Clipboard Copy Issue**: Fixed a bug where the application would fail to copy text if the text was already in the clipboard, which commonly occurs after pasting text.

## [1.2.0] - 2025-01-27

### 🎉 Major Update - Custom Hotkeys & Enhanced UI

TypoZap v1.2.0 introduces the highly requested custom hotkey feature along with significant UI/UX improvements and enhanced customization options.

### ✨ Added

#### Custom Hotkey System
- **Custom hotkey recording** - Set any key combination you prefer (Ctrl+Shift+G, Alt+F12, etc.)
- **Real-time hotkey recording interface** with visual feedback
- **Immediate hotkey application** - no restart required after changing hotkey
- **Hotkey validation** and conflict detection
- **Reset to default** functionality for quick restoration

#### Enhanced Settings Window
- **Streamlined UI** with improved layout and spacing
- **Hotkey management section** with current hotkey display
- **Professional styling** with better visual hierarchy
- **Removed Advanced Settings** section for cleaner interface
- **Enhanced user guidance** with helpful notes and instructions

#### Professional About Dialog
- **Custom About window** replacing basic MessageBox
- **Clickable GitHub link** for easy access to documentation
- **Professional branding** with "TypoZap built by Manish Jangid" message
- **Clean design** without version number clutter
- **Responsive layout** with proper spacing and typography

#### Custom Tone Support
- **Custom tone creation** via `tones.json` modification
- **Override prompt support** using `<>` brackets in custom tones
- **Comprehensive documentation** for tone creation rules
- **Example custom tones** provided in documentation
- **Flexible tone system** supporting any writing style

#### UI/UX Improvements
- **Professional credits** throughout the application
- **Enhanced button layouts** with better spacing
- **Improved error handling** with user-friendly messages
- **Better visual feedback** during operations
- **Consistent styling** across all windows

### 🔧 Changed

#### Settings Window
- **Removed Advanced Settings** section (startup and minimize options)
- **Removed Reset to Defaults** button for cleaner interface
- **Added hotkey recording section** with current hotkey display
- **Improved button layout** with professional credits
- **Enhanced user guidance** with helpful notes

#### API Key Window
- **Removed Paste Key button** for simplified interface
- **Added professional credits** below buttons
- **Improved layout** with better spacing
- **Enhanced visual hierarchy**

#### Hotkey Recording Window
- **Clean interface** without unnecessary elements
- **Real-time key capture** with visual feedback
- **Improved button layout** and spacing
- **Better user guidance** and instructions

#### Application Startup
- **Dynamic hotkey loading** from settings on startup
- **Correct startup notification** showing actual custom hotkey
- **Improved timing** for hotkey registration and notifications

### 🐛 Fixed

#### Hotkey Management
- **Fixed hotkey persistence** - custom hotkeys now save and load correctly
- **Fixed immediate application** - hotkey changes take effect without restart
- **Fixed startup notification** - now shows current hotkey instead of hardcoded default
- **Fixed hotkey registration** - proper cleanup and re-registration

#### UI/UX Issues
- **Fixed button layouts** and spacing issues
- **Fixed text alignment** and visual hierarchy
- **Fixed window positioning** and sizing
- **Fixed error message clarity** and user guidance

### 📚 Documentation

#### Enhanced README
- **Custom hotkey guide** with step-by-step instructions
- **Custom tones documentation** with examples and rules
- **Enhanced troubleshooting** section for hotkey issues
- **Updated project structure** for developers
- **Comprehensive feature overview** with all new capabilities

#### Developer Documentation
- **Updated code comments** for new features
- **Enhanced error handling** documentation
- **Improved code organization** with better separation of concerns

### 🔧 Technical Improvements

#### Hotkey System
- **Dynamic hotkey registration** with proper cleanup
- **Hotkey string parsing** supporting all modifier keys
- **Enhanced error handling** for hotkey conflicts
- **Thread-safe operations** for UI updates

#### Settings Management
- **Improved settings persistence** with better error handling
- **Enhanced settings validation** and user feedback
- **Better configuration management** for custom options

#### Code Organization
- **Modular hotkey recording** system
- **Enhanced service architecture** with better separation
- **Improved error handling** throughout the application
- **Better code documentation** and comments

### 📦 Distribution

#### Release Package
- **Self-contained executable** (147.65 MB)
- **Complete documentation** and setup instructions
- **All assets and dependencies** included
- **Professional release notes** in README.txt

#### Build System
- **Updated version numbers** to 1.2.0.0
- **Enhanced build scripts** for release automation
- **Improved packaging** and distribution process

### 🎯 Breaking Changes

#### Settings Window
- **Removed Advanced Settings** section - startup and minimize options no longer available
- **Removed Reset to Defaults** button - individual reset options available instead

#### Hotkey System
- **Changed default hotkey** from Ctrl+Shift+F to Ctrl+Alt+Q for better compatibility
- **Enhanced hotkey validation** - some previously accepted combinations may now be rejected

### 💡 Known Limitations

#### Current Scope
- **Single hotkey support** - only one custom hotkey at a time
- **Windows-only** - macOS and Linux versions not available
- **English language focus** - optimized for English text correction

#### Technical Constraints
- **Hotkey conflicts** - some combinations may conflict with system shortcuts
- **Application compatibility** - some applications may have special text selection behaviors
- **API dependency** - requires valid Google Gemini API key

### 🚀 Future Roadmap

#### Planned Features
- **Multiple hotkey support** for different correction types
- **Hotkey profiles** for different use cases
- **Advanced tone customization** with GUI editor
- **Plugin architecture** for extensibility

#### Technical Improvements
- **Code signing certificate** to eliminate security warnings
- **Auto-update mechanism** for seamless updates
- **Performance optimizations** and memory usage improvements
- **Enhanced error recovery** and diagnostics

---

### Installation Requirements

- **Operating System**: Windows 10/11 (64-bit)
- **Memory**: 512MB RAM minimum, 1GB recommended
- **Storage**: 200MB free disk space
- **Network**: Internet connection for AI processing
- **API**: Google Gemini API key (free tier available)

### Getting Started

1. Download `TypoZap-v1.2.0.zip` from releases
2. Extract to desired location
3. Run `TypoZap.exe` and configure your Gemini API key
4. Set your custom hotkey in Settings (default: Ctrl+Alt+Q)
5. Use your custom hotkey to correct selected text in any application

### Support & Contributing

- **Issues**: Report bugs and feature requests via GitHub Issues
- **Documentation**: Comprehensive guides included in package
- **Community**: Join discussions and share feedback

---

**Full Changelog**: [v1.0.0...v1.2.0](https://github.com/ManishJangid007/typo_zap_windows/compare/v1.0.0...v1.2.0)

**Download**: TypoZap-v1.2.0.zip (63.06 MB)

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
