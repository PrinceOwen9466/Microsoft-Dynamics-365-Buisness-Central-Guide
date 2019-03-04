//using CefSharp;
//using CefSharp.Wpf.Internals;
using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Guide.Common.Infrastructure.Resources.Misc
{
    /*
    public class ChromiumSilentKeyboardHandler : IKeyboardHandler
    {
        #region Properties

        #region Statics
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYUP = 0x105;
        const int WM_CHAR = 0x102;
        const int WM_SYSCHAR = 0x106;
        const int VK_TAB = 0x9;
        const int VK_LEFT = 0x25;
        const int VK_UP = 0x26;
        const int VK_RIGHT = 0x27;
        const int VK_DOWN = 0x28;
        #endregion

        #endregion

        #region Methods

        #region IKeyboardHandler Implementation
        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            Core.Log.Debug(windowsKeyCode);
            if (windowsKeyCode == VK_LEFT || windowsKeyCode == VK_RIGHT)
            {
                if (!(chromiumWebBrowser is UIElement))
                    return true;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UIElement parent = ((UIElement)chromiumWebBrowser).FindParent<UIElement>();
                    if (parent == null) return;

                    var key = windowsKeyCode == VK_LEFT ? Key.Left : Key.Right;

                    parent.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(parent), 0, key)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    });
                });
                return true;
            }


            isKeyboardShortcut = false;
            return false;
        }
        #endregion

        #endregion
    }
    */
}
