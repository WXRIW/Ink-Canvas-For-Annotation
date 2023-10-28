using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Ink_Canvas.Helpers
{
    internal class SimulateMouseClick
    {
        public static void SimulateMouseClickAtTopLeft()
        {
            // 获取指定位置的屏幕坐标
            int x = 100;
            int y = 100;
            System.Drawing.Point originalMousePosition = System.Windows.Forms.Cursor.Position;
            // 创建一个INPUT结构，用于模拟鼠标按下和释放事件
            INPUT[] inputs = new INPUT[2];
            inputs[0] = new INPUT
            {
                Type = InputType.MOUSE,
                Data = new MOUSEINPUT { dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN }
            };
            inputs[1] = new INPUT
            {
                Type = InputType.MOUSE,
                Data = new MOUSEINPUT { dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP }
            };

            // 移动鼠标到指定位置
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);

            // 发送输入事件
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            // 恢复鼠标位置
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                System.Windows.Forms.Cursor.Position = originalMousePosition;
            }));
        }

        // 定义INPUT结构和相关枚举
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public InputType Type;
            public MOUSEINPUT Data;
        }

        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private enum InputType : uint
        {
            MOUSE = 0,
        }

        [Flags]
        private enum MouseEventFlags : uint
        {
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
