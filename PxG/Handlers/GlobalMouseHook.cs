// GlobalMouseHook.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    public class GlobalMouseHook : IDisposable
    {
        // Evento que será disparado quando um clique do mouse for detectado
        public event EventHandler<Point>? MouseClicked;

        // Delegado para o procedimento do hook
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Constantes da API do Windows
        private const int WhMouseLl = 14;
        private const int WmLbuttondown = 0x0201; // Clique com o botão esquerdo

        // Handle para o nosso hook
        private IntPtr _hookId = IntPtr.Zero;
        private readonly LowLevelMouseProc _proc;

        public GlobalMouseHook()
        {
            // Mantém uma referência ao delegado para que o Garbage Collector não o remova
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookId != IntPtr.Zero) return; // O hook já está ativo
            _hookId = SetHook(_proc);
        }

        public void Stop()
        {
            if (_hookId == IntPtr.Zero) return; // O hook já está inativo
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
            if (nCode >= 0 && wParam == WmLbuttondown)
            {
                // Extrai as coordenadas do clique do mouse
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
                MouseClicked?.Invoke(this, hookStruct.pt);
            }
            // Passa o evento para o próximo hook na cadeia. ESSENCIAL!
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        
        // Garante que o hook seja removido ao descartar o objeto
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        #region P/Invoke e Estruturas
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
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
}