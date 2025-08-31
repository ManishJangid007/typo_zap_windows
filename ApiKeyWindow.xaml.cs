using System;
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
        
        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WinForms.Clipboard.ContainsText())
                {
                    var clipboardText = WinForms.Clipboard.GetText().Trim();
                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        ApiKeyTextBox.Text = clipboardText;
                        ShowStatus("API key pasted from clipboard", true);
                    }
                    else
                    {
                        ShowStatus("Clipboard is empty or contains only whitespace", false);
                    }
                }
                else
                {
                    ShowStatus("No text found in clipboard", false);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error pasting from clipboard: {ex.Message}", false);
            }
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
    }
}
