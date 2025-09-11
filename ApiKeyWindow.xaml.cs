using System;
using System.Diagnostics;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class ApiKeyWindow : Window
    {
        public event Action<string>? ApiKeySet;
        
        public ApiKeyWindow()
        {
            InitializeComponent();
            Loaded += ApiKeyWindow_Loaded;
        }
        
        private void ApiKeyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus on the API key text box
            ApiKeyTextBox.Focus();
            ApiKeyTextBox.SelectAll();
        }
        
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var apiKey = ApiKeyTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(apiKey))
            {
                ShowStatus("Please enter an API key", false);
                return;
            }
            
            if (apiKey.Length < 10)
            {
                ShowStatus("API key seems too short. Please check and try again.", false);
                return;
            }
            
            try
            {
                // Validate the API key format (basic check)
                if (!apiKey.StartsWith("AI") && !apiKey.Contains("."))
                {
                    ShowStatus("API key format doesn't look correct. Please verify.", false);
                    return;
                }
                
                // Raise the event with the API key
                ApiKeySet?.Invoke(apiKey);
                
                // Close the window
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowStatus($"Error saving API key: {ex.Message}", false);
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void ApiKeyLinkTextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Open the Google AI Studio API key page in the default browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://aistudio.google.com/apikey",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ShowStatus($"Error opening browser: {ex.Message}", false);
            }
        }
        
        private void ShowStatus(string message, bool isSuccess)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = isSuccess ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            
            // Clear status after 3 seconds
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, args) =>
            {
                StatusTextBlock.Text = "";
                timer.Stop();
            };
            timer.Start();
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
    }
}
