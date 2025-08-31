using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypoZap
{
    public class ClipboardManager
    {
        // Windows API constants and functions for simulating keyboard input
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
        
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
        
        // Virtual key codes
        private const byte VK_CONTROL = 0x11;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        
        // Key event flags
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        
        private string? _originalClipboardContent;
        
        public string GetClipboardText()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    return Clipboard.GetText();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting clipboard text: {ex.Message}");
                return string.Empty;
            }
        }
        
        public void SetClipboardText(string text)
        {
            try
            {
                // Store original clipboard content if not already stored
                if (_originalClipboardContent == null)
                {
                    _originalClipboardContent = GetClipboardText();
                }
                
                Clipboard.SetText(text);
                Console.WriteLine($"📋 Text copied to clipboard: {text.Substring(0, Math.Min(50, text.Length))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error setting clipboard text: {ex.Message}");
            }
        }
        
        public void SimulateCopy()
        {
            try
            {
                Console.WriteLine("📋 Simulating Ctrl+C...");
                
                // Press and hold Ctrl
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                
                // Press C
                keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                
                // Release C
                keybd_event(VK_C, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                
                // Release Ctrl
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                
                Console.WriteLine("✅ Ctrl+C simulated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error simulating Ctrl+C: {ex.Message}");
            }
        }
        
        public void SimulatePaste()
        {
            try
            {
                Console.WriteLine("📋 Simulating Ctrl+V...");
                
                // Press and hold Ctrl
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                
                // Press V
                keybd_event(VK_V, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                
                // Release V
                keybd_event(VK_V, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                
                // Release Ctrl
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                
                Console.WriteLine("✅ Ctrl+V simulated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error simulating Ctrl+V: {ex.Message}");
            }
        }
        
        public void RestoreOriginalClipboard()
        {
            try
            {
                if (!string.IsNullOrEmpty(_originalClipboardContent))
                {
                    Clipboard.SetText(_originalClipboardContent);
                    Console.WriteLine("📋 Original clipboard content restored");
                    _originalClipboardContent = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error restoring original clipboard: {ex.Message}");
            }
        }
        
        public void ClearClipboard()
        {
            try
            {
                Clipboard.Clear();
                Console.WriteLine("📋 Clipboard cleared");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error clearing clipboard: {ex.Message}");
            }
        }
        
        public bool HasTextInClipboard()
        {
            try
            {
                return Clipboard.ContainsText() && !string.IsNullOrWhiteSpace(Clipboard.GetText());
            }
            catch
            {
                return false;
            }
        }
    }
}
