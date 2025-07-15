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
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private static GlobalKeyboardHook? _instance;
        
        // Evento que será disparado quando uma tecla específica for pressionada
        public event EventHandler<Keys>? KeyPressed;
        
        // Teclas que estamos monitorando
        private Keys _targetKey = Keys.None;
        private bool _requireCtrl = false;
        private bool _requireAlt = false;
        private bool _requireShift = false;

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
            _instance = this;
        }

        /// <summary>
        /// Define qual tecla ou combina��ão monitorar
        /// </summary>
        public void SetTargetKey(Keys key)
        {
            // Extrai os modificadores da tecla
            _requireCtrl = (key & Keys.Control) == Keys.Control;
            _requireAlt = (key & Keys.Alt) == Keys.Alt;
            _requireShift = (key & Keys.Shift) == Keys.Shift;
            
            // Remove os modificadores para obter só a tecla principal
            _targetKey = key & ~Keys.Control & ~Keys.Alt & ~Keys.Shift;
        }

        /// <summary>
        /// Inicia o monitoramento
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
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && _instance != null)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys pressedKey = (Keys)vkCode;

                // Verifica se é a tecla que estamos monitorando
                if (pressedKey == _instance._targetKey)
                {
                    // Verifica os modificadores
                    // Usando GetAsyncKeyState para verificar o estado atual dos modificadores
                    bool ctrlPressed = (GetAsyncKeyState(0x11) & 0x8000) != 0 || (GetAsyncKeyState(0xA2) & 0x8000) != 0 || (GetAsyncKeyState(0xA3) & 0x8000) != 0; // VK_CONTROL, VK_LCONTROL, VK_RCONTROL
                    bool altPressed = (GetAsyncKeyState(0x12) & 0x8000) != 0 || (GetAsyncKeyState(0xA4) & 0x8000) != 0 || (GetAsyncKeyState(0xA5) & 0x8000) != 0;   // VK_MENU (Alt), VK_LMENU, VK_RMENU
                    bool shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0 || (GetAsyncKeyState(0xA0) & 0x8000) != 0 || (GetAsyncKeyState(0xA1) & 0x8000) != 0; // VK_SHIFT, VK_LSHIFT, VK_RSHIFT

                    if (ctrlPressed == _instance._requireCtrl &&
                        altPressed == _instance._requireAlt &&
                        shiftPressed == _instance._requireShift)
                    {
                        // Combina a tecla com os modificadores para o evento
                        Keys fullKey = pressedKey;
                        if (ctrlPressed) fullKey |= Keys.Control;
                        if (altPressed) fullKey |= Keys.Alt;
                        if (shiftPressed) fullKey |= Keys.Shift;

                        _instance.KeyPressed?.Invoke(_instance, fullKey);
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
