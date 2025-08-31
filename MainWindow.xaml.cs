using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace TypoZap
{
    public partial class MainWindow : Window
    {
        // Windows API constants
        private const int WM_HOTKEY = 0x0312;
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure window is properly initialized
            Console.WriteLine("MainWindow loaded");
        }
        
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // This event fires when the window handle is available
            var handle = new WindowInteropHelper(this).Handle;
            Console.WriteLine($"MainWindow handle obtained: {handle}");
            
            // Hook up message processing
            var hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource != null)
            {
                hwndSource.AddHook(WndProc);
                Console.WriteLine("Message hook added successfully");
            }
            else
            {
                Console.WriteLine("Failed to get HwndSource");
            }
        }
        
        // Handle Windows messages including hotkeys
        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                Console.WriteLine($"Hotkey message received: wParam={wParam}, lParam={lParam}");
                // Forward hotkey message to the HotkeyManager
                var app = Application.Current as App;
                app?.ProcessHotkeyMessage(msg, wParam, lParam);
                handled = true; // Message handled
                return nint.Zero;
            }
            
            handled = false; // Let default processing continue
            return nint.Zero;
        }
    }
}
