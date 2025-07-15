using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    /// <summary>
    /// Hook global para capturar teclas pressionadas em todo o sistema
    /// </summary>
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private static GlobalKeyboardHook? _instance;
        
        // Eventos para pressionar e soltar teclas
        public event EventHandler<Keys>? KeyDown;
        public event EventHandler<Keys>? KeyUp;
        
        // Teclas que estamos monitorando
        private readonly HashSet<Keys> _targetKeys = new();

        /// <summary>
        /// Obtém o número de teclas que estão sendo monitoradas atualmente
        /// </summary>
        public int TargetKeyCount => _targetKeys.Count;

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
            _instance = this;
            _hookID = SetHook(_proc); // O hook começa a ouvir imediatamente
        }

        /// <summary>
        /// Adiciona uma tecla (com modificadores) para ser monitorada.
        /// </summary>
        public void AddTargetKey(Keys key)
        {
            _targetKeys.Add(key);
        }

        /// <summary>
        /// Remove uma tecla da lista de monitoramento.
        /// </summary>
        public void RemoveTargetKey(Keys key)
        {
            _targetKeys.Remove(key);
        }

        /// <summary>
        /// Limpa todas as teclas monitoradas.
        /// </summary>
        public void ClearTargetKeys()
        {
            _targetKeys.Clear();
        }

        /// <summary>
        /// Inicia o monitoramento (se parado).
        /// </summary>
        public void Start()
        {
            if (_hookID == IntPtr.Zero)
                _hookID = SetHook(_proc);
        }

        /// <summary>
        /// Para o monitoramento
        /// </summary>
        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule?.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _instance != null)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys pressedKey = (Keys)vkCode;

                // Constrói a tecla completa com os modificadores atuais
                bool ctrlPressed = (GetAsyncKeyState(0x11) & 0x8000) != 0 || (GetAsyncKeyState(0xA2) & 0x8000) != 0 || (GetAsyncKeyState(0xA3) & 0x8000) != 0;
                bool altPressed = (GetAsyncKeyState(0x12) & 0x8000) != 0 || (GetAsyncKeyState(0xA4) & 0x8000) != 0 || (GetAsyncKeyState(0xA5) & 0x8000) != 0;
                bool shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0 || (GetAsyncKeyState(0xA0) & 0x8000) != 0 || (GetAsyncKeyState(0xA1) & 0x8000) != 0;

                Keys keyWithModifiers = pressedKey;
                if (ctrlPressed) keyWithModifiers |= Keys.Control;
                if (altPressed) keyWithModifiers |= Keys.Alt;
                if (shiftPressed) keyWithModifiers |= Keys.Shift;

                // Verifica se a tecla pressionada (com modificadores) é uma das que estamos monitorando
                if (_instance._targetKeys.Contains(keyWithModifiers))
                {
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        _instance.KeyDown?.Invoke(_instance, keyWithModifiers);
                    }
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        _instance.KeyUp?.Invoke(_instance, keyWithModifiers);
                    }
                }
            }
            
            // Garante que o hook seja passado para o próximo na cadeia
            return CallNextHookEx(_instance?._hookID ?? IntPtr.Zero, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        ~GlobalKeyboardHook()
        {
            Dispose();
        }

        // Delegate para o callback do hook
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Funções da Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);
        
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}
