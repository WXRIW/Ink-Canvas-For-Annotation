using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Ink_Canvas.Helpers
{
    internal class ProcessToCheckWindowHelper
    {
        public static bool IsProcessMinimized(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                if (!process.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    // 获取主窗口句柄
                    IntPtr mainWindowHandle = process.MainWindowHandle;
                    // 获取窗口状态
                    int windowState = GetWindowState(mainWindowHandle);
                    // 检查窗口状态是否为最小化
                    return (windowState == SW_SHOWMINIMIZED);
                }
            }
            return true;
        }

        private static int GetWindowState(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(windowHandle, ref placement);
            return placement.showCmd;
        }

        const int SW_SHOWMINIMIZED = 2;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }
    }
}
