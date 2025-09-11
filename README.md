# TypoZap for Windows 🪟

A Windows utility app that lives in the system tray and lets users fix grammar errors instantly using Gemini API by selecting text and pressing **Ctrl+Alt+Q**. The fixed text automatically replaces the selected text with minimal delay.

## ✨ Features

- **System Tray App**: Runs in the background without showing a main window
- **Customizable Global Hotkey**: Set your own hotkey combination (default: Ctrl+Alt+Q)
- **AI-Powered Grammar Correction**: Uses Google's Gemini API for accurate corrections
- **Multiple Writing Tones**: Choose from General, Polite, Aggressive, Sarcastic, or Funny tones
- **Custom Tone Support**: Add your own custom writing styles and prompts
- **Secure API Key Storage**: Encrypts and stores your API key securely
- **Smart Clipboard Management**: Preserves your original clipboard content
- **Visual Feedback**: Icon changes show processing status and results
- **Settings Window**: Customize hotkeys, tones, and more

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
5. **Start using** with Ctrl+Alt+Q hotkey!

### First Run Setup

1. **Launch TypoZap** - icon appears in system tray
2. **Enter Gemini API key** when requested
3. **Grant permissions** if Windows asks for accessibility
4. **Select text** in any application
5. **Press Ctrl+Alt+Q** to correct grammar instantly!

## 🔧 How It Works

### Workflow
1. **Select text** in any application (Word, Notepad, Chrome, etc.)
2. **Press your custom hotkey** (default: Ctrl+Alt+Q)
3. **App automatically copies selected text** (simulates Ctrl+C)
4. **Text sent to Gemini API** for grammar correction with selected tone
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
├── TypoZap.csproj              # Project file
├── App.xaml                     # Main application XAML
├── App.xaml.cs                  # Application logic
├── MainWindow.xaml              # Hidden main window
├── MainWindow.xaml.cs           # Main window logic
├── HotkeyManager.cs             # Global hotkey handling
├── ClipboardManager.cs          # Clipboard operations
├── GeminiService.cs             # Gemini API integration
├── ApiKeyWindow.xaml            # API key input dialog
├── SettingsWindow.xaml          # Settings configuration
├── HotkeyRecordingWindow.xaml   # Custom hotkey recording dialog
├── AboutWindow.xaml             # About dialog
├── ToneModels.cs                # Tone data models
├── tones.json                   # Writing tone configurations
├── Assets/                      # Icons and resources
└── README.md                    # This file
```

## ⚙️ Configuration

### Settings Window
Access settings by right-clicking the system tray icon and selecting "Settings":

- **Hotkey Settings**: Record and customize your global hotkey combination
- **Tone Settings**: Choose from available writing tones (General, Polite, Aggressive, Sarcastic, Funny)
- **API Settings**: Change your Gemini API key

### Custom Hotkeys
TypoZap allows you to set any hotkey combination you prefer:

1. **Open Settings** → Right-click system tray icon → Settings
2. **Click "Record New Hotkey"** in the Hotkey Settings section
3. **Press your desired key combination** (e.g., Ctrl+Shift+G, Alt+F12, etc.)
4. **Click "Save"** to apply the new hotkey immediately
5. **Use your custom hotkey** from anywhere in Windows

**Supported Keys:**
- **Modifiers**: Ctrl, Alt, Shift, Win (any combination)
- **Main Keys**: A-Z, 0-9, F1-F12, Space, Enter, Arrow keys, etc.
- **Examples**: Ctrl+Shift+G, Alt+F12, Win+Q, Ctrl+Alt+Space

### Custom Tones
You can add your own custom writing styles by editing the `tones.json` file:

1. **Locate the tones.json file** in the TypoZap installation directory
2. **Open it in a text editor** (Notepad, VS Code, etc.)
3. **Add your custom tone** following the existing structure
4. **Restart TypoZap** to load the new tone

#### Adding a Custom Tone
```json
{
    "title": "Your Tone Name",
    "description": "Brief description of what this tone does",
    "prompt": "Your custom prompt here. Use {text} as placeholder for the input text. If the text contains <> brackets, treat the content inside as an override prompt and ignore this prompt. Otherwise, [your instructions here]. Return only the processed text without explanations. Here's the input text:\n{text}"
}
```

#### Tone Structure Rules
- **title**: Display name in the settings dropdown
- **description**: Brief explanation of the tone's purpose
- **prompt**: The AI prompt template (must include `{text}` placeholder)
- **Override Support**: Use `<>` brackets for inline prompt overrides
- **Output Format**: Always return only the processed text, no explanations

#### Example Custom Tones
```json
{
    "title": "Academic",
    "description": "Formal academic writing style with proper citations",
    "prompt": "If the text contains <> brackets, treat the content inside as an override prompt and ignore this prompt. Otherwise, rewrite the following text in a formal academic style, ensuring proper grammar, formal language, and academic tone. Return only the revised text without explanations. Here's the input text:\n{text}"
},
{
    "title": "Casual",
    "description": "Relaxed, conversational tone for informal communication",
    "prompt": "If the text contains <> brackets, treat the content inside as an override prompt and ignore this prompt. Otherwise, rewrite the following text in a casual, conversational tone while maintaining proper grammar. Make it sound natural and friendly. Return only the revised text without explanations. Here's the input text:\n{text}"
}
```

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
- **Verify no other apps** are using your custom hotkey
- **Check hotkey settings** in the Settings window
- **Try a different key combination** if current one conflicts
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
