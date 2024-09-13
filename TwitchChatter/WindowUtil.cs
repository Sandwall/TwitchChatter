using System;
using System.Runtime.InteropServices;

namespace TwitchChatter
{
    class WindowUtil
    {
        //For rounded corners and black titlebar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
        // Copied from dwmapi.h
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        // Copied from dwmapi.h
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        //To use little memory
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetProcessWorkingSetSize(int hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        public static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, (int)attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        public static void SetRoundedWindow(IntPtr hwnd)
        {
            var attribute = (int)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(hwnd, attribute, ref preference, sizeof(uint));
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }
    }

}