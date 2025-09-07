using System;
using System.Windows;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _currentSettings;
        private readonly string _settingsPath;
        
        public SettingsWindow()
        {
            InitializeComponent();
            _settingsPath = GetSettingsPath();
            LoadSettings();
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
        
        private void ApplySettingsToUI()
        {
            StartWithWindowsCheckBox.IsChecked = _currentSettings.StartWithWindows;
            MinimizeToTrayCheckBox.IsChecked = _currentSettings.MinimizeToTray;
        }
        
        private void SaveSettings()
        {
            try
            {
                // Update settings from UI
                _currentSettings.StartWithWindows = StartWithWindowsCheckBox.IsChecked ?? false;
                _currentSettings.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked ?? true;
                _currentSettings.LastModified = DateTime.Now;
                
                // Serialize and encrypt settings
                var json = JsonConvert.SerializeObject(_currentSettings, Formatting.Indented);
                var encryptedJson = EncryptSettings(json);
                
                // Save to file
                File.WriteAllText(_settingsPath, encryptedJson);
                
                // Apply startup with Windows setting
                ApplyStartupSetting();
                
                WinForms.MessageBox.Show("Settings saved successfully!", "Success", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
            }
        }
        
        private void ApplyStartupSetting()
        {
            try
            {
                var startupKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                
                if (_currentSettings.StartWithWindows)
                {
                    var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    startupKey?.SetValue("TypoZap", $"\"{exePath}\"");
                }
                else
                {
                    startupKey?.DeleteValue("TypoZap", false);
                }
                
                startupKey?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting startup with Windows: {ex.Message}");
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
        
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = WinForms.MessageBox.Show(
                "Are you sure you want to reset all settings to default values?",
                "Reset Settings",
                WinForms.MessageBoxButtons.YesNo,
                WinForms.MessageBoxIcon.Question);
                
            if (result == WinForms.DialogResult.Yes)
            {
                _currentSettings = new AppSettings();
                ApplySettingsToUI();
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
    
    public class AppSettings
    {
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTray { get; set; } = true;
        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}
