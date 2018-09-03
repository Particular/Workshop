using System.Runtime.InteropServices;

namespace Divergent.Customers.API
{
    static class ConsoleEx
    {
        public static void TryMinimize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsNativeMethods.ShowWindow(WindowsNativeMethods.GetConsoleWindow(), WindowsNativeMethods.SW_SHOWMINIMIZED);
            }
        }
    }
}
