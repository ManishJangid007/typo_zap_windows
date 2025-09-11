using System;
using System.Windows;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _currentSettings;
        private readonly string _settingsPath;
        private List<Tone> _availableTones = new List<Tone>();
        
        public SettingsWindow()
        {
            InitializeComponent();
            _settingsPath = GetSettingsPath();
            LoadSettings();
            
            // Load tones after UI is initialized
            Loaded += (s, e) => LoadTones();
        }
        
        private string GetSettingsPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var typoZapPath = Path.Combine(appDataPath, "TypoZap");
            Directory.CreateDirectory(typoZapPath);
            return Path.Combine(typoZapPath, "settings.json");
        }
        
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var encryptedJson = File.ReadAllText(_settingsPath);
                    var decryptedJson = DecryptSettings(encryptedJson);
                    _currentSettings = JsonConvert.DeserializeObject<AppSettings>(decryptedJson) ?? new AppSettings();
                }
                else
                {
                    _currentSettings = new AppSettings();
                }
                
                ApplySettingsToUI();
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Error loading settings: {ex.Message}", "Error", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                _currentSettings = new AppSettings();
                ApplySettingsToUI();
            }
        }
        
        private void LoadTones()
        {
            try
            {
                var tonesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tones.json");
                if (File.Exists(tonesPath))
                {
                    var json = File.ReadAllText(tonesPath);
                    var toneData = JsonConvert.DeserializeObject<ToneData>(json);
                    _availableTones = toneData?.Tones ?? new List<Tone>();
                }
                else
                {
                    // Fallback to default tone if file doesn't exist
                    _availableTones = new List<Tone>
                    {
                        new Tone
                        {
                            Title = "default",
                            Description = "Corrects grammar, spelling, and punctuation without changing the tone or meaning of the text.",
                            Prompt = "Please correct the grammar, spelling, and punctuation in the following text. Return only the corrected text without any explanations or additional formatting:\n{text}"
                        }
                    };
                }

                // Populate the ComboBox
                ToneComboBox.ItemsSource = _availableTones;
                
                // Select the current tone or default to first one
                var currentTone = _availableTones.FirstOrDefault(t => t.Title == _currentSettings.SelectedTone) 
                                ?? _availableTones.FirstOrDefault();
                if (currentTone != null)
                {
                    ToneComboBox.SelectedItem = currentTone;
                    ToneDescriptionTextBlock.Text = currentTone.Description;
                }
                else
                {
                    ToneComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading tones: {ex.Message}");
                // Fallback to default tone
                _availableTones = new List<Tone>
                {
                    new Tone
                    {
                        Title = "default",
                        Description = "Corrects grammar, spelling, and punctuation without changing the tone or meaning of the text.",
                        Prompt = "Please correct the grammar, spelling, and punctuation in the following text. Return only the corrected text without any explanations or additional formatting:\n{text}"
                    }
                };
                ToneComboBox.ItemsSource = _availableTones;
                ToneComboBox.SelectedIndex = 0;
            }
        }

        private void ApplySettingsToUI()
        {
            // Update hotkey display
            CurrentHotkeyTextBlock.Text = _currentSettings.CustomHotkey;
        }
        
        private void SaveSettings()
        {
            try
            {
                // Update settings from UI
                _currentSettings.SelectedTone = (ToneComboBox.SelectedItem as Tone)?.Title ?? "default";
                _currentSettings.LastModified = DateTime.Now;
                
                // Serialize and encrypt settings
                var json = JsonConvert.SerializeObject(_currentSettings, Formatting.Indented);
                var encryptedJson = EncryptSettings(json);
                
                // Save to file
                File.WriteAllText(_settingsPath, encryptedJson);
                
                // Re-register hotkey with new settings
                var app = (App)System.Windows.Application.Current;
                app.ReRegisterHotkey();
                
                WinForms.MessageBox.Show("Settings saved successfully!", "Success", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }
        
        
        private string EncryptSettings(string plainText)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedData = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            catch
            {
                // Fallback to plain text if encryption fails
                return plainText;
            }
        }
        
        private string DecryptSettings(string encryptedText)
        {
            try
            {
                var bytes = Convert.FromBase64String(encryptedText);
                var decryptedData = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                // Return empty settings if decryption fails
                return "{}";
            }
        }
        
        private void ToneComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ToneComboBox.SelectedItem is Tone selectedTone)
            {
                ToneDescriptionTextBlock.Text = selectedTone.Description;
            }
        }

        private void ChangeApiKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var apiKeyWindow = new ApiKeyWindow();
            apiKeyWindow.ApiKeySet += (apiKey) =>
            {
                // The main app will handle the API key change
                WinForms.MessageBox.Show("API key changed successfully!", "Success", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            };
            apiKeyWindow.ShowDialog();
        }
        
        private void CreditsLink_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Open the GitHub repository in the default browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/ManishJangid007/typo_zap_windows",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Error opening browser: {ex.Message}", "Error", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
            }
        }
        
        private void RecordHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            var hotkeyWindow = new HotkeyRecordingWindow();
            hotkeyWindow.HotkeyRecorded += (hotkey) =>
            {
                _currentSettings.CustomHotkey = hotkey;
                CurrentHotkeyTextBlock.Text = hotkey;
                
                // Immediately apply the new hotkey
                var app = (App)System.Windows.Application.Current;
                app.ReRegisterHotkey();
                
                WinForms.MessageBox.Show($"Hotkey recorded: {hotkey}\n\nSave settings to use the new hotkey.", 
                    "Hotkey Recorded", WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            };
            hotkeyWindow.ShowDialog();
        }
        
        private void ResetHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            var result = WinForms.MessageBox.Show(
                "Reset hotkey to default (Ctrl+Alt+Q)?",
                "Reset Hotkey",
                WinForms.MessageBoxButtons.YesNo,
                WinForms.MessageBoxIcon.Question);
                
            if (result == WinForms.DialogResult.Yes)
            {
                _currentSettings.CustomHotkey = "Ctrl+Alt+Q";
                CurrentHotkeyTextBlock.Text = "Ctrl+Alt+Q";
                
                // Immediately apply the default hotkey
                var app = (App)System.Windows.Application.Current;
                app.ReRegisterHotkey();
            }
        }
        
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
