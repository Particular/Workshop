using System;
using System.Runtime.InteropServices;

namespace Divergent.Customers.API
{
    static class WindowsNativeMethods
    {
        public const int SW_SHOWMINIMIZED = 2;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
