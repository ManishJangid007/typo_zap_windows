using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? _notifyIcon;
        private MainWindow? _mainWindow;
        private HotkeyManager? _hotkeyManager;
        private GeminiService? _geminiService;
        private ClipboardManager? _clipboardManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize services
                _geminiService = new GeminiService();
                _clipboardManager = new ClipboardManager();

                // Create hidden main window
                _mainWindow = new MainWindow();
                _mainWindow.Hide(); // Keep hidden

                // Setup system tray
                SetupSystemTray();

                // Setup global hotkey
                _hotkeyManager = new HotkeyManager();
                _hotkeyManager.HotkeyPressed += OnHotkeyPressed;
                _hotkeyManager.RegisterHotkey();

                // Check for API key on first launch
                if (!_geminiService.HasValidApiKey())
                {
                    ShowApiKeyDialog();
                }

                // Show startup notification
                ShowNotification("TypoZap Started", "Press Ctrl+Shift+O to correct text", WinForms.ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Failed to start TypoZap: {ex.Message}", "Error",
    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
                Shutdown();
            }
        }

        private void SetupSystemTray()
        {
            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = LoadIcon("typozap.ico"),
                Text = "TypoZap - Grammar Correction Tool",
                Visible = true
            };

            // Create context menu
            var contextMenu = new WinForms.ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, OnSettingsClicked);
            contextMenu.Items.Add("Set API Key", null, OnSetApiKeyClicked);
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("About", null, OnAboutClicked);
            contextMenu.Items.Add("Exit", null, OnExitClicked);

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += OnTrayIconDoubleClick;
        }

        private Icon LoadIcon(string iconName)
        {
            try
            {
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", iconName);
                if (File.Exists(iconPath))
                {
                    return new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load icon {iconName}: {ex.Message}");
            }

            // Fallback to main icon if specific icon not found
            if (iconName != "typozap.ico")
            {
                try
                {
                    var mainIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "typozap.ico");
                    if (File.Exists(mainIconPath))
                    {
                        return new Icon(mainIconPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load main icon: {ex.Message}");
                }
            }

            // Final fallback to system icon
            return SystemIcons.Application;
        }

        private void OnHotkeyPressed(object? sender, EventArgs e)
        {
            try
            {
                // Change icon to loading state
                ChangeTrayIcon("loader.ico");

                // Process the hotkey action
                ProcessHotkeyAction();
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"Failed to process hotkey: {ex.Message}", ToolTipIcon.Error);
                ChangeTrayIcon("typozap.ico"); // Restore normal icon
            }
        }

        private async void ProcessHotkeyAction()
        {
            try
            {
                // Simulate Ctrl+C to copy selected text
                _clipboardManager?.SimulateCopy();

                // Wait a bit for clipboard to update
                await System.Threading.Tasks.Task.Delay(100);

                // Get selected text from clipboard
                var selectedText = _clipboardManager?.GetClipboardText();

                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No Text Selected", "Please select some text to correct.", WinForms.ToolTipIcon.Warning);
                    ChangeTrayIcon("typozap.ico");
                    return;
                }

                // Send to Gemini for correction
                var correctedText = await _geminiService?.CorrectGrammarAsync(selectedText);

                if (!string.IsNullOrEmpty(correctedText))
                {
                    // Copy corrected text to clipboard
                    _clipboardManager?.SetClipboardText(correctedText);

                    // Wait a bit then simulate Ctrl+V
                    await System.Threading.Tasks.Task.Delay(150);
                    _clipboardManager?.SimulatePaste();

                    // Show success feedback
                    ShowNotification("Text Corrected", "Grammar correction applied successfully!", ToolTipIcon.Info);
                    ChangeTrayIcon("completed.ico");

                    // Restore normal icon after delay
                    await System.Threading.Tasks.Task.Delay(1000);
                    ChangeTrayIcon("typozap.ico");
                }
                else
                {
                    ShowNotification("Correction Failed", "Failed to get corrected text from Gemini API.", WinForms.ToolTipIcon.Error);
                    ChangeTrayIcon("typozap.ico");
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Error", $"Failed to correct text: {ex.Message}", WinForms.ToolTipIcon.Error);
                ChangeTrayIcon("typozap.ico");
            }
        }

        private void ChangeTrayIcon(string iconName)
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Icon = LoadIcon(iconName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to change tray icon: {ex.Message}");
            }
        }

        private void ShowNotification(string title, string message, WinForms.ToolTipIcon icon)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.ShowBalloonTip(3000, title, message, icon);
            }
        }

        private void ShowApiKeyDialog()
        {
            var apiKeyWindow = new ApiKeyWindow();
            apiKeyWindow.ApiKeySet += (apiKey) =>
            {
                _geminiService?.SetApiKey(apiKey);
                ShowNotification("API Key Saved", "Your Gemini API key has been saved successfully.", WinForms.ToolTipIcon.Info);
            };
            apiKeyWindow.ShowDialog();
        }

        private void OnSettingsClicked(object? sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void OnSetApiKeyClicked(object? sender, EventArgs e)
        {
            ShowApiKeyDialog();
        }

        private void OnAboutClicked(object? sender, EventArgs e)
        {
            WinForms.MessageBox.Show(
"TypoZap v1.0.0\n\nA Windows utility app that fixes grammar errors instantly using Gemini API.\n\nPress Ctrl+Shift+O to correct selected text.",
"About TypoZap",
WinForms.MessageBoxButtons.OK,
WinForms.MessageBoxIcon.Information);
        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            Shutdown();
        }

        private void OnTrayIconDoubleClick(object? sender, EventArgs e)
        {
            OnSettingsClicked(sender, e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotkeyManager?.Dispose();
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
