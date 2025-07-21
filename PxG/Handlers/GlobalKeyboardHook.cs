using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace PxG.Handlers
{
    /// <summary>
    /// Instala um hook global para capturar eventos de teclado em todo o sistema.
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza a API do Windows (P/Invoke) para monitorar teclas pressionadas,
    /// mesmo quando a aplicação não está em foco. É fundamental que a instância desta classe
    /// seja corretamente descartada através do método <see cref="Dispose"/> para liberar o hook
    /// e evitar vazamentos de recursos. A classe é implementada com um padrão singleton-like
    /// interno para gerenciar a única instância do hook.
    /// </remarks>
    public sealed class GlobalKeyboardHook : IDisposable
    {
        #region Constantes e Campos Privados

        private const int WhKeyboardLl = 13;    // ID do hook para teclado de baixo nível
        private const int WmKeydown = 0x0100;   // Mensagem de tecla pressionada
        private const int WmKeyup = 0x0101;     // Mensagem de tecla liberada

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;
        private static GlobalKeyboardHook? _instance;

        private readonly HashSet<Keys> _targetKeys = new();
        private IntPtr _targetWindow = IntPtr.Zero;

        #endregion

        #region Eventos Públicos

        /// <summary>
        /// Ocorre quando uma das teclas monitoradas (<see cref="AddTargetKey"/>) é pressionada.
        /// </summary>
        /// <remarks>
        /// O evento só é disparado se a janela alvo (<see cref="SetTargetWindow"/>) estiver em foco,
        /// ou se nenhuma janela alvo for definida.
        /// </remarks>
        public event EventHandler<Keys>? KeyDown;

        /// <summary>
        /// Ocorre quando uma das teclas monitoradas (<see cref="AddTargetKey"/>) é liberada.
        /// </summary>
        /// <remarks>
        /// O evento só é disparado se a janela alvo (<see cref="SetTargetWindow"/>) estiver em foco,
        /// ou se nenhuma janela alvo for definida.
        /// </remarks>
        public event EventHandler<Keys>? KeyUp;

        #endregion

        #region Propriedades Públicas

        /// <summary>
        /// Obtém o número de combinações de teclas que estão sendo monitoradas atualmente.
        /// </summary>
        /// <value>O total de teclas na lista de monitoramento.</value>
        public int TargetKeyCount => _targetKeys.Count;

        #endregion

        #region Construtor e Padrão Dispose

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GlobalKeyboardHook"/> e instala o hook de teclado.
        /// </summary>
        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
            _instance = this;
            _hookId = SetHook(_proc); // O hook começa a ouvir imediatamente
        }

        /// <summary>
        /// Libera os recursos (remove o hook do Windows) utilizados pela classe.
        /// </summary>
        public void Dispose()
        {
            Stop(); // Garante que o hook seja liberado
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizador da classe, garante a liberação do hook caso Dispose não seja chamado.
        /// </summary>
        ~GlobalKeyboardHook()
        {
            Stop();
        }

        #endregion

        #region Gerenciamento do Hook e Teclas

        /// <summary>
        /// Define uma janela específica que deve estar em foco para que os eventos de teclado sejam disparados.
        /// </summary>
        /// <param name="windowHandle">O handle (ponteiro) da janela alvo.</param>
        public void SetTargetWindow(IntPtr windowHandle)
        {
            _targetWindow = windowHandle;
        }

        /// <summary>
        /// Remove a restrição de janela, permitindo que os eventos de teclado sejam disparados globalmente.
        /// </summary>
        public void ClearTargetWindow()
        {
            _targetWindow = IntPtr.Zero;
        }

        /// <summary>
        /// Adiciona uma combinação de teclas para ser monitorada pelo hook.
        /// </summary>
        /// <param name="key">A tecla a ser monitorada. Para modificadores, use o operador OR bit a bit (ex: <c>Keys.Control | Keys.C</c>).</param>
        public void AddTargetKey(Keys key)
        {
            _targetKeys.Add(key);
        }

        /// <summary>
        /// Remove uma combinação de teclas da lista de monitoramento.
        /// </summary>
        /// <param name="key">A tecla a ser removida.</param>
        public void RemoveTargetKey(Keys key)
        {
            _targetKeys.Remove(key);
        }

        /// <summary>
        /// Limpa todas as teclas da lista de monitoramento.
        /// </summary>
        public void ClearTargetKeys()
        {
            _targetKeys.Clear();
        }

        /// <summary>
        /// Inicia o monitoramento de teclado, instalando o hook se ele não estiver ativo.
        /// </summary>
        public void Start()
        {
            if (_hookId == IntPtr.Zero)
            {
                _hookId = SetHook(_proc);
            }
        }

        /// <summary>
        /// Para o monitoramento de teclado, desinstalando o hook.
        /// </summary>
        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        #endregion

        #region Lógica Interna do Hook

        /// <summary>
        /// Instala o procedimento de hook na cadeia de hooks do Windows.
        /// </summary>
        /// <param name="proc">O delegate para o método de callback do hook.</param>
        /// <returns>Um handle para o hook se bem-sucedido; caso contrário, <see cref="IntPtr.Zero"/>.</returns>
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            if (curModule?.ModuleName == null) throw new Win32Exception("Não foi possível obter o módulo principal do processo.");
            
            return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        /// <summary>
        /// Método de callback invocado pelo Windows sempre que um evento de teclado ocorre.
        /// </summary>
        /// <param name="nCode">Um código que o hook usa para determinar a ação a ser executada.</param>
        /// <param name="wParam">O identificador da mensagem do teclado (<c>WM_KEYDOWN</c> ou <c>WM_KEYUP</c>).</param>
        /// <param name="lParam">Um ponteiro para uma estrutura <c>KBDLLHOOKSTRUCT</c> contendo detalhes sobre o evento.</param>
        /// <returns>Um handle para o próximo hook na cadeia.</returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _instance != null)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys pressedKey = (Keys)vkCode;

                // Verifica o estado atual das teclas modificadoras
                bool isCtrlPressed = (GetAsyncKeyState((int)Keys.ControlKey) & 0x8000) != 0;
                bool isAltPressed = (GetAsyncKeyState((int)Keys.Menu) & 0x8000) != 0;
                bool isShiftPressed = (GetAsyncKeyState((int)Keys.ShiftKey) & 0x8000) != 0;

                Keys keyWithModifiers = pressedKey;
                if (isCtrlPressed) keyWithModifiers |= Keys.Control;
                if (isAltPressed) keyWithModifiers |= Keys.Alt;
                if (isShiftPressed) keyWithModifiers |= Keys.Shift;
                
                // Dispara o evento apenas se a tecla for monitorada e a janela estiver correta
                if (_instance._targetKeys.Contains(keyWithModifiers))
                {
                    bool isTargetWindowActive = _instance._targetWindow == IntPtr.Zero || GetForegroundWindow() == _instance._targetWindow;
                    if (isTargetWindowActive)
                    {
                        if (wParam == (IntPtr)WmKeydown)
                        {
                            _instance.KeyDown?.Invoke(_instance, keyWithModifiers);
                        }
                        else if (wParam == (IntPtr)WmKeyup)
                        {
                            _instance.KeyUp?.Invoke(_instance, keyWithModifiers);
                        }
                    }
                }
            }
            
            // Garante que o evento seja passado para o próximo hook na cadeia
            return CallNextHookEx(_instance?._hookId ?? IntPtr.Zero, nCode, wParam, lParam);
        }

        #endregion

        #region P/Invoke para Funções da API Win32

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        #endregion
    }
}