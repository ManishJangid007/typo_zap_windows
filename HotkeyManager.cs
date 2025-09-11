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
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;
        private const int VK_Q = 0x51; // Virtual key code for 'Q'
        
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
        private string _currentHotkey = "Ctrl+Alt+Q";
        private uint _currentModifiers = MOD_CONTROL | MOD_ALT;
        private uint _currentVk = VK_Q;
        
        public event EventHandler? HotkeyPressed;
        
        public bool RegisterHotkey(System.Windows.Window mainWindow)
        {
            return RegisterHotkey(mainWindow, _currentHotkey);
        }
        
        public bool RegisterHotkey(System.Windows.Window mainWindow, string hotkeyString)
        {
            try
            {
                // Unregister existing hotkey first
                if (_isRegistered)
                {
                    UnregisterHotkey(mainWindow);
                }
                
                // Parse the hotkey string
                if (!ParseHotkeyString(hotkeyString, out uint modifiers, out uint vk))
                {
                    Console.WriteLine($"❌ Failed to parse hotkey: {hotkeyString}");
                    return false;
                }
                
                // Get the window handle using WindowInteropHelper
                var windowHelper = new System.Windows.Interop.WindowInteropHelper(mainWindow);
                var mainWindowHandle = windowHelper.Handle;
                
                if (mainWindowHandle == IntPtr.Zero)
                {
                    Console.WriteLine("❌ Window handle is zero, window may not be fully initialized");
                    return false;
                }
                
                Console.WriteLine($"🔑 Registering hotkey '{hotkeyString}' with window handle: {mainWindowHandle}");
                
                // Register the hotkey
                var result = RegisterHotKey(mainWindowHandle, _hotkeyId, modifiers, vk);
                
                if (result)
                {
                    _isRegistered = true;
                    _currentHotkey = hotkeyString;
                    _currentModifiers = modifiers;
                    _currentVk = vk;
                    Console.WriteLine($"✅ Global hotkey '{hotkeyString}' registered successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Failed to register global hotkey '{hotkeyString}'");
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
                Console.WriteLine("🔥 Hotkey Ctrl+Alt+Q pressed!");
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private bool ParseHotkeyString(string hotkeyString, out uint modifiers, out uint vk)
        {
            modifiers = 0;
            vk = 0;
            
            try
            {
                var parts = hotkeyString.Split('+');
                if (parts.Length < 2)
                {
                    return false;
                }
                
                // Parse modifiers
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var part = parts[i].Trim().ToLower();
                    switch (part)
                    {
                        case "ctrl":
                            modifiers |= MOD_CONTROL;
                            break;
                        case "alt":
                            modifiers |= MOD_ALT;
                            break;
                        case "shift":
                            modifiers |= MOD_SHIFT;
                            break;
                        case "win":
                            modifiers |= MOD_WIN;
                            break;
                        default:
                            return false;
                    }
                }
                
                // Parse main key
                var mainKey = parts[parts.Length - 1].Trim();
                vk = GetVirtualKeyCode(mainKey);
                
                return vk != 0;
            }
            catch
            {
                return false;
            }
        }
        
        private uint GetVirtualKeyCode(string keyName)
        {
            // Handle special keys
            switch (keyName.ToUpper())
            {
                case "SPACE": return 0x20;
                case "ENTER": return 0x0D;
                case "TAB": return 0x09;
                case "ESC": return 0x1B;
                case "ESCAPE": return 0x1B;
                case "BACK": return 0x08;
                case "DELETE": return 0x2E;
                case "INSERT": return 0x2D;
                case "HOME": return 0x24;
                case "END": return 0x23;
                case "PAGEUP": return 0x21;
                case "PAGEDOWN": return 0x22;
                case "UP": return 0x26;
                case "DOWN": return 0x28;
                case "LEFT": return 0x25;
                case "RIGHT": return 0x27;
                case "F1": return 0x70;
                case "F2": return 0x71;
                case "F3": return 0x72;
                case "F4": return 0x73;
                case "F5": return 0x74;
                case "F6": return 0x75;
                case "F7": return 0x76;
                case "F8": return 0x77;
                case "F9": return 0x78;
                case "F10": return 0x79;
                case "F11": return 0x7A;
                case "F12": return 0x7B;
                default:
                    // Handle single letters and numbers
                    if (keyName.Length == 1)
                    {
                        char c = char.ToUpper(keyName[0]);
                        if (c >= 'A' && c <= 'Z')
                            return (uint)c;
                        if (c >= '0' && c <= '9')
                            return (uint)c;
                    }
                    return 0;
            }
        }
        
        public string GetCurrentHotkey()
        {
            return _currentHotkey;
        }
        
        public void Dispose()
        {
            // Note: This will need to be called with the mainWindow parameter
            // The App class should handle this properly
        }
    }
}
