using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace TypoZap
{
    public partial class HotkeyRecordingWindow : Window
    {
        public event Action<string>? HotkeyRecorded;
        
        private readonly List<Key> _pressedKeys = new List<Key>();
        private bool _isRecording = false;
        
        public HotkeyRecordingWindow()
        {
            InitializeComponent();
            this.KeyDown += HotkeyRecordingWindow_KeyDown;
            this.KeyUp += HotkeyRecordingWindow_KeyUp;
            this.Loaded += (s, e) => StartRecording();
        }
        
        private void StartRecording()
        {
            _isRecording = true;
            HotkeyDisplayTextBlock.Text = "Press keys...";
            this.Focus();
        }
        
        private void HotkeyRecordingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isRecording) return;
            
            // Ignore modifier keys for now, we'll handle them separately
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin)
            {
                return;
            }
            
            // Check if this is a valid key for hotkey
            if (IsValidHotkeyKey(e.Key))
            {
                _pressedKeys.Clear();
                
                // Add modifier keys
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    _pressedKeys.Add(Key.LeftCtrl);
                }
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
                {
                    _pressedKeys.Add(Key.LeftAlt);
                }
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    _pressedKeys.Add(Key.LeftShift);
                }
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows))
                {
                    _pressedKeys.Add(Key.LWin);
                }
                
                // Add the main key
                _pressedKeys.Add(e.Key);
                
                UpdateDisplay();
            }
        }
        
        private void HotkeyRecordingWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Stop recording when user releases keys
            if (_isRecording && _pressedKeys.Count > 0)
            {
                _isRecording = false;
            }
        }
        
        private bool IsValidHotkeyKey(Key key)
        {
            // Allow letters, numbers, and function keys
            return (key >= Key.A && key <= Key.Z) ||
                   (key >= Key.D0 && key <= Key.D9) ||
                   (key >= Key.F1 && key <= Key.F24) ||
                   key == Key.Space ||
                   key == Key.Enter ||
                   key == Key.Tab ||
                   key == Key.Escape ||
                   key == Key.Back ||
                   key == Key.Delete ||
                   key == Key.Insert ||
                   key == Key.Home ||
                   key == Key.End ||
                   key == Key.PageUp ||
                   key == Key.PageDown ||
                   key == Key.Up ||
                   key == Key.Down ||
                   key == Key.Left ||
                   key == Key.Right;
        }
        
        private void UpdateDisplay()
        {
            if (_pressedKeys.Count == 0)
            {
                HotkeyDisplayTextBlock.Text = "Press keys...";
                return;
            }
            
            var hotkeyParts = new List<string>();
            
            foreach (var key in _pressedKeys)
            {
                switch (key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        hotkeyParts.Add("Ctrl");
                        break;
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        hotkeyParts.Add("Alt");
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        hotkeyParts.Add("Shift");
                        break;
                    case Key.LWin:
                    case Key.RWin:
                        hotkeyParts.Add("Win");
                        break;
                    default:
                        hotkeyParts.Add(key.ToString());
                        break;
                }
            }
            
            HotkeyDisplayTextBlock.Text = string.Join("+", hotkeyParts);
        }
        
        private string GetHotkeyString()
        {
            if (_pressedKeys.Count == 0) return "";
            
            var hotkeyParts = new List<string>();
            
            foreach (var key in _pressedKeys)
            {
                switch (key)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        hotkeyParts.Add("Ctrl");
                        break;
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        hotkeyParts.Add("Alt");
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        hotkeyParts.Add("Shift");
                        break;
                    case Key.LWin:
                    case Key.RWin:
                        hotkeyParts.Add("Win");
                        break;
                    default:
                        hotkeyParts.Add(key.ToString());
                        break;
                }
            }
            
            return string.Join("+", hotkeyParts);
        }
        
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _pressedKeys.Clear();
            HotkeyDisplayTextBlock.Text = "Press keys...";
            _isRecording = true;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pressedKeys.Count == 0)
            {
                WinForms.MessageBox.Show("Please record a hotkey first.", "No Hotkey", 
                    WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Warning);
                return;
            }
            
            var hotkey = GetHotkeyString();
            HotkeyRecorded?.Invoke(hotkey);
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
