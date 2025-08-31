using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TypoZap
{
    public class HotkeyManager : IDisposable
    {
        // Windows API constants
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int VK_O = 0x4F; // Virtual key code for 'O'
        
        // Windows API functions
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        private readonly int _hotkeyId = 1;
        private bool _isRegistered = false;
        
        public event EventHandler? HotkeyPressed;
        
        public bool RegisterHotkey(System.Windows.Window mainWindow)
        {
            try
            {
                // Get the window handle using WindowInteropHelper
                var windowHelper = new System.Windows.Interop.WindowInteropHelper(mainWindow);
                var mainWindowHandle = windowHelper.Handle;
                
                if (mainWindowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("❌ Window handle is zero, window may not be fully initialized");
                    return false;
                }
                
                Console.WriteLine($"🔑 Registering hotkey with window handle: {mainWindowHandle}");
                
                // Register the hotkey: Ctrl+Shift+O
                var modifiers = (uint)(MOD_CONTROL | MOD_SHIFT);
                var result = RegisterHotKey(mainWindowHandle, _hotkeyId, modifiers, (uint)VK_O);
                
                if (result)
                {
                    _isRegistered = true;
                    Console.WriteLine("✅ Global hotkey Ctrl+Shift+O registered successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine("❌ Failed to register global hotkey");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error registering hotkey: {ex.Message}");
                return false;
            }
        }
        
        public bool UnregisterHotkey(System.Windows.Window mainWindow)
        {
            if (!_isRegistered) return true;
            
            try
            {
                var windowHelper = new System.Windows.Interop.WindowInteropHelper(mainWindow);
                var mainWindowHandle = windowHelper.Handle;
                
                if (mainWindowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("❌ Window handle is zero during unregistration");
                    return false;
                }
                
                var result = UnregisterHotKey(mainWindowHandle, _hotkeyId);
                if (result)
                {
                    _isRegistered = false;
                    Console.WriteLine("✅ Global hotkey unregistered successfully");
                }
                else
                {
                    var error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    Console.WriteLine($"❌ Failed to unregister hotkey. Windows error: {error}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error unregistering hotkey: {ex.Message}");
                return false;
            }
        }
        
        public void ProcessHotkeyMessage(int message, IntPtr wParam, IntPtr lParam)
        {
            if (message == WM_HOTKEY && wParam.ToInt32() == _hotkeyId)
            {
                Console.WriteLine("🔥 Hotkey Ctrl+Shift+O pressed!");
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public void Dispose()
        {
            // Note: This will need to be called with the mainWindow parameter
            // The App class should handle this properly
        }
    }
}
