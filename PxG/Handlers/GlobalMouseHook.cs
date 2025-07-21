// GlobalMouseHook.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    public class GlobalMouseHook : IDisposable
    {
        
        public event EventHandler<GlobalMouseEventArgs>? MouseEvent;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        
        private const int WhMouseLl = 14;
        private const int WmLbuttondown = 0x0201;
        private const int WmLbuttonup = 0x0202;
        private const int WmXbuttondown = 0x020B;
        private const int WmXbuttonup = 0x020C;
        
        // Constantes para identificar os botões extras
        private const int Xbutton1 = 0x0001;
        private const int Xbutton2 = 0x0002;

        private IntPtr _hookId = IntPtr.Zero;
        private readonly LowLevelMouseProc _proc;

        public GlobalMouseHook()
        {
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookId != IntPtr.Zero) return;
            _hookId = SetHook(_proc);
        }

        public void Stop()
        {
            if (_hookId == IntPtr.Zero) return;
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(WhMouseLl, proc, GetModuleHandle(curModule.ModuleName!), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                Msllhookstruct hookStruct = (Msllhookstruct)Marshal.PtrToStructure(lParam, typeof(Msllhookstruct))!;
                MouseAction action = MouseAction.None;
                MouseButton button = MouseButton.None;

                int msg = wParam.ToInt32();

                if (msg == WmLbuttondown || msg == WmLbuttonup)
                {
                    action = (msg == WmLbuttondown) ? MouseAction.Down : MouseAction.Up;
                    button = MouseButton.Left;
                }
                else if (msg == WmXbuttondown || msg == WmXbuttonup)
                {
                    action = (msg == WmXbuttondown) ? MouseAction.Down : MouseAction.Up;
                    int xButton = (int)(hookStruct.mouseData >> 16);
                    button = (xButton == Xbutton1) ? MouseButton.XButton1 : MouseButton.XButton2;
                }

                if (action != MouseAction.None)
                {
                    MouseEvent?.Invoke(this, new GlobalMouseEventArgs(button, action, hookStruct.pt));
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        #region P/Invoke e Estruturas
        [StructLayout(LayoutKind.Sequential)]
        public struct Point { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct Msllhookstruct
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }

    // Enums e EventArgs para o novo evento
    public enum MouseButton { None, Left, XButton1, XButton2 }
    public enum MouseAction { None, Down, Up }

    public class GlobalMouseEventArgs(MouseButton button, MouseAction action, GlobalMouseHook.Point point) : EventArgs
    {
        public MouseButton Button { get; } = button;
        public MouseAction Action { get; } = action;
        public GlobalMouseHook.Point Point { get; } = point;
    }
}