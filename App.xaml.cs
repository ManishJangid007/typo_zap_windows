using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
                ShowNotification("TypoZap Started", "1. Select text anywhere\n2. Press Ctrl+Shift+O\n3. Text automatically corrected!", WinForms.ToolTipIcon.Info);
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
            string? originalClipboard = null;
            try
            {
                Console.WriteLine("🔄 Starting hotkey action processing...");
                
                // Change icon to loading state
                ChangeTrayIcon("loader.ico");

                // Step 1: Store original clipboard content to restore later
                Console.WriteLine("📋 Storing original clipboard content...");
                originalClipboard = _clipboardManager?.GetClipboardText();
                Console.WriteLine($"📋 Original clipboard: '{originalClipboard?.Substring(0, Math.Min(50, originalClipboard?.Length ?? 0))}...'");
                
                // Step 2: Automatically copy selected text (simulate Ctrl+C)
                Console.WriteLine("📋 Automatically copying selected text...");
                _clipboardManager?.SimulateCopy();
                
                // Step 3: Wait for the copy operation to complete
                Console.WriteLine("⏳ Waiting for copy operation to complete...");
                await System.Threading.Tasks.Task.Delay(500);
                
                // Step 4: Get the selected text from clipboard
                Console.WriteLine("📋 Getting selected text from clipboard...");
                var selectedText = _clipboardManager?.GetClipboardText();
                Console.WriteLine($"📋 Selected text: '{selectedText}'");

                // Step 5: Check if we got new text - if not, try fallback method for Teams/Electron apps
                if (string.IsNullOrWhiteSpace(selectedText) || selectedText == originalClipboard)
                {
                    Console.WriteLine("🔄 Standard copy failed, trying fallback method for Teams/Electron apps...");
                    
                    // Try alternative copy method with delays (works better with Teams)
                    _clipboardManager?.SimulateCopyWithDelay();
                    
                    // Wait a bit longer for stubborn apps
                    await System.Threading.Tasks.Task.Delay(700);
                    
                    // Try getting text again
                    selectedText = _clipboardManager?.GetClipboardText();
                    Console.WriteLine($"📋 Fallback attempt - Selected text: '{selectedText}'");
                    
                    // If still no luck, give up
                    if (string.IsNullOrWhiteSpace(selectedText) || selectedText == originalClipboard)
                    {
                        // Restore original clipboard if no new text was selected
                        if (!string.IsNullOrEmpty(originalClipboard))
                        {
                            _clipboardManager?.SetClipboardText(originalClipboard);
                        }
                        
                        var message = string.IsNullOrWhiteSpace(selectedText) ? 
                            "Could not copy text. This might be a protected field or the text selection failed.\n\nTry:\n1. Re-selecting the text\n2. Using a different application\n3. Manually copying first (Ctrl+C), then using the hotkey." :
                            "No new text selected. Please select different text and try again.";
                        
                        ShowNotification("Copy Failed", "Could not copy text. Try re-selecting or use different app.", WinForms.ToolTipIcon.Warning);
                        ChangeTrayIcon("typozap.ico");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("✅ Fallback copy method succeeded!");
                    }
                }

                // Step 2: Check if we have a valid API key
                if (!_geminiService?.HasValidApiKey() == true)
                {
                    var result = WinForms.MessageBox.Show(
                        "No Gemini API key found.\n\nPlease set your API key first.",
                        "API Key Required",
                        WinForms.MessageBoxButtons.OK,
                        WinForms.MessageBoxIcon.Warning);
                    ChangeTrayIcon("typozap.ico");
                    return;
                }

                // Step 3: Send to Gemini for correction
                Console.WriteLine("🤖 Sending text to Gemini for correction...");
                Console.WriteLine($"🤖 Text being sent: '{selectedText}'");
                
                var correctedText = await _geminiService?.CorrectGrammarAsync(selectedText);
                Console.WriteLine($"🤖 Gemini response: '{correctedText}'");

                if (string.IsNullOrEmpty(correctedText))
                {
                    var result = WinForms.MessageBox.Show(
                        "Gemini API returned empty response.\n\nPlease check your API key and try again.",
                        "API Response Error",
                        WinForms.MessageBoxButtons.OK,
                        WinForms.MessageBoxIcon.Error);
                    ChangeTrayIcon("typozap.ico");
                    return;
                }

                // Step 4: Automatically apply the correction
                Console.WriteLine("📋 Applying correction automatically...");
                
                // Step 5: Copy corrected text to clipboard
                Console.WriteLine("📋 Copying corrected text to clipboard...");
                _clipboardManager?.SetClipboardText(correctedText);

                // Step 6: Wait a moment and verify clipboard
                await System.Threading.Tasks.Task.Delay(200);
                var clipboardText = _clipboardManager?.GetClipboardText();
                Console.WriteLine($"📋 Clipboard now contains: '{clipboardText}'");

                if (clipboardText == correctedText)
                {
                    // Step 7: Simulate Ctrl+V to paste corrected text
                    Console.WriteLine("📋 Simulating Ctrl+V to paste corrected text...");
                    _clipboardManager?.SimulatePaste();

                    // Show success feedback immediately
                    // ShowNotification("Text Corrected", "Grammar correction applied successfully!", WinForms.ToolTipIcon.Info);
                    ChangeTrayIcon("completed.ico");

                    // Step 8: Wait longer for paste to complete, then restore original clipboard in background
                    _ = RestoreClipboardAfterDelay(originalClipboard);

                    // Restore normal icon after delay
                    await System.Threading.Tasks.Task.Delay(1000);
                    ChangeTrayIcon("typozap.ico");
                }
                else
                {
                    var errorResult = WinForms.MessageBox.Show(
                        $"Clipboard verification failed.\n\nExpected: {correctedText}\nActual: {clipboardText}\n\nThis indicates a clipboard issue.",
                        "Clipboard Error",
                        WinForms.MessageBoxButtons.OK,
                        WinForms.MessageBoxIcon.Error);
                    ChangeTrayIcon("typozap.ico");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in ProcessHotkeyAction: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                
                var errorResult = WinForms.MessageBox.Show(
                    $"An error occurred during text correction:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "Error",
                    WinForms.MessageBoxButtons.OK,
                    WinForms.MessageBoxIcon.Error);
                
                ChangeTrayIcon("typozap.ico");
            }
        }

        private async Task RestoreClipboardAfterDelay(string? originalClipboard)
        {
            try
            {
                // Wait longer for the paste operation to complete
                await System.Threading.Tasks.Task.Delay(1500);
                
                if (!string.IsNullOrEmpty(originalClipboard))
                {
                    Console.WriteLine("📋 Restoring original clipboard content after delay...");
                    _clipboardManager?.SetClipboardText(originalClipboard);
                    Console.WriteLine("✅ Original clipboard content restored successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error restoring clipboard: {ex.Message}");
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
