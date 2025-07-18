using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    /// <summary>
    /// Hook global unificado para capturar teclas e botões do mouse
    /// </summary>
    public class GlobalInputHook : IDisposable
    {
        // Constantes para hooks
        private const int WhKeyboardLl = 13;
        private const int WhMouseLl = 14;
        
        // Constantes para mensagens de teclado
        private const int WmKeydown = 0x0100;
        private const int WmKeyup = 0x0101;
        
        // Constantes para mensagens de mouse
        private const int WmLbuttondown = 0x0201;
        private const int WmLbuttonup = 0x0202;
        private const int WmRbuttondown = 0x0204;
        private const int WmRbuttonup = 0x0205;
        private const int WmMbuttondown = 0x0207;
        private const int WmMbuttonup = 0x0208;
        private const int WmXbuttondown = 0x020B;
        private const int WmXbuttonup = 0x020C;
        
        // Delegates para callbacks
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        private readonly LowLevelKeyboardProc _keyboardProc;
        private readonly LowLevelMouseProc _mouseProc;
        private IntPtr _keyboardHookId = IntPtr.Zero;
        private IntPtr _mouseHookId = IntPtr.Zero;
        private static GlobalInputHook? _instance;
        
        // Eventos para diferentes tipos de entrada
        public event EventHandler<InputType>? InputTriggered;
        
        // Inputs que estamos monitorando
        private readonly HashSet<InputType> _targetInputs = new();
        
        // Janela alvo
        private IntPtr _targetWindow = IntPtr.Zero;
        
        public enum InputType
        {
            // Teclas comuns
            F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
            
            // Botões do mouse
            MouseLeft,
            MouseRight,
            MouseMiddle,
            MouseX1,     // Botão lateral traseiro
            MouseX2,     // Botão lateral frontal
            
            // Teclas com modificadores (exemplos)
            CtrlF1, CtrlF2, CtrlF3,
            ShiftF1, ShiftF2, ShiftF3,
            AltF1, AltF2, AltF3
        }
        
        public GlobalInputHook()
        {
            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;
            _instance = this;
        }
        
        /// <summary>
        /// Define a janela onde os inputs devem funcionar
        /// </summary>
        public void SetTargetWindow(IntPtr windowHandle)
        {
            _targetWindow = windowHandle;
        }
        
        /// <summary>
        /// Adiciona um input para ser monitorado
        /// </summary>
        public void AddTargetInput(InputType input)
        {
            _targetInputs.Add(input);
        }
        
        /// <summary>
        /// Remove um input da lista de monitoramento
        /// </summary>
        public void RemoveTargetInput(InputType input)
        {
            _targetInputs.Remove(input);
        }
        
        /// <summary>
        /// Limpa todos os inputs monitorados
        /// </summary>
        public void ClearTargetInputs()
        {
            _targetInputs.Clear();
        }
        
        /// <summary>
        /// Inicia o monitoramento
        /// </summary>
        public void Start()
        {
            if (_keyboardHookId == IntPtr.Zero)
                _keyboardHookId = SetKeyboardHook(_keyboardProc);
            if (_mouseHookId == IntPtr.Zero)
                _mouseHookId = SetMouseHook(_mouseProc);
        }
        
        /// <summary>
        /// Para o monitoramento
        /// </summary>
        public void Stop()
        {
            if (_keyboardHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
            }
            if (_mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookId);
                _mouseHookId = IntPtr.Zero;
            }
        }
        
        private static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WhKeyboardLl, proc,
                GetModuleHandle(curModule?.ModuleName), 0);
        }
        
        private static IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WhMouseLl, proc,
                GetModuleHandle(curModule?.ModuleName), 0);
        }
        
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WmKeydown)
            {
                if (!IsTargetWindowActive()) 
                    return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
                
                int vkCode = Marshal.ReadInt32(lParam);
                var inputType = ConvertKeyToInputType((Keys)vkCode);
                
                if (inputType.HasValue && _targetInputs.Contains(inputType.Value))
                {
                    InputTriggered?.Invoke(this, inputType.Value);
                }
            }
            
            return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }
        
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (!IsTargetWindowActive()) 
                    return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
                
                var inputType = ConvertMouseMessageToInputType(wParam);
                
                if (inputType.HasValue && _targetInputs.Contains(inputType.Value))
                {
                    // Para botões X, precisamos verificar qual botão específico foi pressionado
                    if (wParam == (IntPtr)WmXbuttondown)
                    {
                        var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
                        var xButton = GetXButtonFromMouseData(hookStruct.mouseData);
                        if (xButton.HasValue && _targetInputs.Contains(xButton.Value))
                        {
                            InputTriggered?.Invoke(this, xButton.Value);
                        }
                    }
                    else
                    {
                        InputTriggered?.Invoke(this, inputType.Value);
                    }
                }
            }
            
            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }
        
        private bool IsTargetWindowActive()
        {
            if (_targetWindow == IntPtr.Zero) return true; // Se não há janela específica, aceita globalmente
            return GetForegroundWindow() == _targetWindow;
        }
        
        private InputType? ConvertKeyToInputType(Keys key)
        {
            // Verifica modificadores
            bool ctrlPressed = (GetAsyncKeyState(0x11) & 0x8000) != 0;
            bool altPressed = (GetAsyncKeyState(0x12) & 0x8000) != 0;
            bool shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0;
            
            return key switch
            {
                Keys.F1 when ctrlPressed => InputType.CtrlF1,
                Keys.F1 when altPressed => InputType.AltF1,
                Keys.F1 when shiftPressed => InputType.ShiftF1,
                Keys.F1 => InputType.F1,
                Keys.F2 when ctrlPressed => InputType.CtrlF2,
                Keys.F2 when altPressed => InputType.AltF2,
                Keys.F2 when shiftPressed => InputType.ShiftF2,
                Keys.F2 => InputType.F2,
                Keys.F3 when ctrlPressed => InputType.CtrlF3,
                Keys.F3 when altPressed => InputType.AltF3,
                Keys.F3 when shiftPressed => InputType.ShiftF3,
                Keys.F3 => InputType.F3,
                Keys.F4 => InputType.F4,
                Keys.F5 => InputType.F5,
                Keys.F6 => InputType.F6,
                Keys.F7 => InputType.F7,
                Keys.F8 => InputType.F8,
                Keys.F9 => InputType.F9,
                Keys.F10 => InputType.F10,
                Keys.F11 => InputType.F11,
                Keys.F12 => InputType.F12,
                _ => null
            };
        }
        
        private InputType? ConvertMouseMessageToInputType(IntPtr wParam)
        {
            return wParam.ToInt32() switch
            {
                WmLbuttondown => InputType.MouseLeft,
                WmRbuttondown => InputType.MouseRight,
                WmMbuttondown => InputType.MouseMiddle,
                WmXbuttondown => null, // Será tratado separadamente
                _ => null
            };
        }
        
        private InputType? GetXButtonFromMouseData(uint mouseData)
        {
            // Os botões X são identificados pelos bits altos do mouseData
            var xButton = (mouseData >> 16) & 0xFFFF;
            return xButton switch
            {
                1 => InputType.MouseX1, // XBUTTON1 (botão "voltar")
                2 => InputType.MouseX2, // XBUTTON2 (botão "avançar")
                _ => null
            };
        }
        
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
        
        ~GlobalInputHook()
        {
            Dispose();
        }
        
        // Estrutura para dados do mouse
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }
        
        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);
        
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
