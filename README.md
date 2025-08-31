# TypoZap for Windows 🪟

A Windows utility app that lives in the system tray and lets users fix grammar errors instantly using Gemini API by selecting text and pressing **Ctrl+Shift+O**. The fixed text automatically replaces the selected text with minimal delay.

## ✨ Features

- **System Tray App**: Runs in the background without showing a main window
- **Global Hotkey**: Press Ctrl+Shift+O from anywhere to correct selected text
- **AI-Powered Grammar Correction**: Uses Google's Gemini API for accurate corrections
- **Secure API Key Storage**: Encrypts and stores your API key securely
- **Smart Clipboard Management**: Preserves your original clipboard content
- **Visual Feedback**: Icon changes show processing status and results
- **Settings Window**: Customize notifications, startup behavior, and more

## 🚀 Quick Start

### Prerequisites

- Windows 10/11 (64-bit)
- .NET 8 Runtime
- Gemini API key from [Google AI Studio](https://ai.google.dev/gemini-api)

### Installation

1. **Download the latest release** from the releases page
2. **Run the installer** (TypoZap-Setup.exe)
3. **Launch TypoZap** - it will appear in your system tray
4. **Enter your Gemini API key** when prompted
5. **Start using** with Ctrl+Shift+O hotkey!

### First Run Setup

1. **Launch TypoZap** - icon appears in system tray
2. **Enter Gemini API key** when requested
3. **Grant permissions** if Windows asks for accessibility
4. **Select text** in any application
5. **Press Ctrl+Shift+O** to correct grammar instantly!

## 🔧 How It Works

### Workflow
1. **Select text** in any application (Word, Notepad, Chrome, etc.)
2. **Press Ctrl+Shift+O** (the global hotkey)
3. **App automatically copies selected text** (simulates Ctrl+C)
4. **Text sent to Gemini API** for grammar correction
5. **Corrected text pasted back** using Ctrl+V
6. **Original clipboard content restored** to preserve your data

### Visual Feedback
| Icon State | Meaning |
|------------|---------|
| 🔤 | Ready - App is ready to use |
| ⏳ | Processing - Sending text to Gemini API |
| ✅ | Success - Correction completed successfully |
| ❌ | Error - Something went wrong |

## 🛠️ Building from Source

### Requirements
- Visual Studio 2022 or .NET 8 SDK
- Windows 10/11 development environment

### Build Steps
```bash
# Clone the repository
git clone <repository-url>
cd typo_zap_windows

# Restore dependencies
dotnet restore

# Build in Release mode
dotnet build -c Release

# Run the application
dotnet run -c Release
```

### Project Structure
```
typo_zap_windows/
├── TypoZap.csproj          # Project file
├── App.xaml                 # Main application XAML
├── App.xaml.cs              # Application logic
├── MainWindow.xaml          # Hidden main window
├── MainWindow.xaml.cs       # Main window logic
├── HotkeyManager.cs         # Global hotkey handling
├── ClipboardManager.cs      # Clipboard operations
├── GeminiService.cs         # Gemini API integration
├── ApiKeyWindow.xaml        # API key input dialog
├── SettingsWindow.xaml      # Settings configuration
├── Assets/                  # Icons and resources
└── README.md                # This file
```

## ⚙️ Configuration

### Settings Window
Access settings by right-clicking the system tray icon and selecting "Settings":

- **Hotkey Settings**: View current hotkey (Ctrl+Shift+O)
- **Notification Settings**: Enable/disable various notifications
- **API Settings**: Change your Gemini API key
- **Advanced Settings**: Startup with Windows, minimize to tray

### API Key Management
Your Gemini API key is:
- **Encrypted** using Windows Data Protection API
- **Stored locally** in your user profile
- **Never transmitted** to any third party
- **Automatically loaded** on startup

## 🔒 Security & Privacy

### Data Protection
- **API Keys**: Stored securely using Windows encryption
- **Text Processing**: No local storage, only temporary processing
- **Network Security**: HTTPS communication with Gemini API
- **Permission Scope**: Minimal required permissions only

### Privacy Features
- **No Logging**: Text content is not logged locally
- **Temporary Processing**: Text exists only in memory during correction
- **Secure Storage**: Credentials use Windows security features

## 🐛 Troubleshooting

### Common Issues

#### Hotkey Not Working
- **Check if TypoZap is running** in system tray
- **Verify no other apps** are using Ctrl+Shift+O
- **Restart TypoZap** if needed

#### API Key Issues
- **Verify your API key** is correct
- **Check internet connection** for API calls
- **Ensure API key has proper permissions** in Google AI Studio

#### Text Not Replacing
- **Select text first** before pressing hotkey
- **Check if target application** supports clipboard operations
- **Verify accessibility permissions** if prompted

#### App Not Starting
- **Check .NET 8 Runtime** is installed
- **Run as Administrator** if permission issues
- **Check Windows Event Viewer** for error details

### Debug Mode
```bash
# Run with detailed logging
dotnet run -c Debug
```

## 📋 System Requirements

- **OS**: Windows 10 (version 1903) or later
- **Architecture**: 64-bit (x64)
- **Runtime**: .NET 8 Desktop Runtime
- **Memory**: 50MB RAM minimum
- **Storage**: 10MB disk space
- **Permissions**: System tray access, clipboard access

## 🔄 Updates

TypoZap will check for updates automatically. You can also:
- **Check manually** from the system tray menu
- **Download latest** from the releases page
- **Enable auto-updates** in settings

## 🤝 Contributing

We welcome contributions! Please see our contributing guidelines for:
- **Bug reports** and feature requests
- **Code contributions** and pull requests
- **Documentation** improvements
- **Testing** and feedback

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- **Google Gemini API** for AI-powered grammar correction
- **WPF Community** for Windows UI framework
- **Open Source Contributors** for various libraries and tools

## 📞 Support

- **GitHub Issues**: Report bugs and request features
- **Discussions**: Community support and questions
- **Documentation**: Comprehensive guides and tutorials

---

**Ready to start TypoZapping? Download the latest release and experience the magic! ✨**

*TypoZap - Making perfect grammar effortless on Windows*
