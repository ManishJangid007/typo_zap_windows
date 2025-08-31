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
                Console.WriteLine("🚀 TypoZap starting up...");
                
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
                RegisterHotkeyAfterWindowReady();

                // Check for API key on first launch
                if (!_geminiService.HasValidApiKey())
                {
                    Console.WriteLine("⚠️ No API key found, showing API key dialog");
                    ShowApiKeyDialog();
                }
                else
                {
                    Console.WriteLine("✅ API key found and loaded");
                }

                // Show startup notification
                ShowNotification("TypoZap Started", "Press Ctrl+Shift+O to correct text", WinForms.ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Startup error: {ex.Message}");
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
                Console.WriteLine("🔄 Starting hotkey action processing...");
                
                // Change icon to loading state
                ChangeTrayIcon("loader.ico");

                // Simulate Ctrl+C to copy selected text
                Console.WriteLine("📋 Simulating Ctrl+C to copy selected text...");
                _clipboardManager?.SimulateCopy();

                // Wait longer for clipboard to update and stabilize
                Console.WriteLine("⏳ Waiting for clipboard to update...");
                await System.Threading.Tasks.Task.Delay(300);

                // Get selected text from clipboard
                var selectedText = _clipboardManager?.GetClipboardText();
                Console.WriteLine($"📋 Retrieved text from clipboard: '{selectedText}'");

                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    ShowNotification("No Text Selected", "Please select some text to correct.", WinForms.ToolTipIcon.Warning);
                    ChangeTrayIcon("typozap.ico");
                    return;
                }

                // Check if we have a valid API key
                if (!_geminiService?.HasValidApiKey() == true)
                {
                    ShowNotification("API Key Required", "Please set your Gemini API key first.", WinForms.ToolTipIcon.Warning);
                    ChangeTrayIcon("typozap.ico");
                    return;
                }

                // Send to Gemini for correction
                Console.WriteLine("🤖 Sending text to Gemini for correction...");
                var correctedText = await _geminiService?.CorrectGrammarAsync(selectedText);

                Console.WriteLine($"🤖 Gemini response: '{correctedText}'");

                if (!string.IsNullOrEmpty(correctedText))
                {
                    // Copy corrected text to clipboard
                    Console.WriteLine("📋 Copying corrected text to clipboard...");
                    _clipboardManager?.SetClipboardText(correctedText);

                    // Wait longer for clipboard to update
                    Console.WriteLine("⏳ Waiting for clipboard to update with corrected text...");
                    await System.Threading.Tasks.Task.Delay(300);
                    
                    // Verify the corrected text is in clipboard
                    var clipboardText = _clipboardManager?.GetClipboardText();
                    Console.WriteLine($"📋 Clipboard now contains: '{clipboardText}'");
                    
                    if (clipboardText == correctedText)
                    {
                        // Simulate Ctrl+V to paste corrected text
                        Console.WriteLine("📋 Simulating Ctrl+V to paste corrected text...");
                        _clipboardManager?.SimulatePaste();

                        // Show success feedback
                        ShowNotification("Text Corrected", "Grammar correction applied successfully!", WinForms.ToolTipIcon.Info);
                        ChangeTrayIcon("completed.ico");

                        // Restore normal icon after delay
                        await System.Threading.Tasks.Task.Delay(1000);
                        ChangeTrayIcon("typozap.ico");
                    }
                    else
                    {
                        Console.WriteLine("❌ Clipboard content mismatch - corrected text not properly set");
                        ShowNotification("Clipboard Error", "Failed to set corrected text in clipboard.", WinForms.ToolTipIcon.Error);
                        ChangeTrayIcon("typozap.ico");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Gemini returned empty or null corrected text");
                    ShowNotification("Correction Failed", "Failed to get corrected text from Gemini API.", WinForms.ToolTipIcon.Error);
                    ChangeTrayIcon("typozap.ico");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in ProcessHotkeyAction: {ex.Message}");
                ShowNotification("Error", $"Failed to process hotkey: {ex.Message}", WinForms.ToolTipIcon.Error);
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
        
        private void RegisterHotkeyAfterWindowReady()
        {
            // Use a timer to wait for the window to be fully initialized
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                
                if (_mainWindow != null && _hotkeyManager != null)
                {
                    Console.WriteLine("🔑 Attempting to register hotkey...");
                    var success = _hotkeyManager.RegisterHotkey(_mainWindow);
                    if (success)
                    {
                        Console.WriteLine("✅ Hotkey registration completed successfully");
                    }
                    else
                    {
                        Console.WriteLine("❌ Hotkey registration failed");
                    }
                }
            };
            
            timer.Start();
        }

        public void ProcessHotkeyMessage(int message, IntPtr wParam, IntPtr lParam)
        {
            _hotkeyManager?.ProcessHotkeyMessage(message, wParam, lParam);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotkeyManager?.Dispose();
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
