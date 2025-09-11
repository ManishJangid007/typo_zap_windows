using System;
using System.Diagnostics;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        
        private void GitHubLink_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Open the GitHub repository in the default browser
                Process.Start(new ProcessStartInfo
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
