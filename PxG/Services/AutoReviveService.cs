using PxG.Handlers;
using PxG.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxG.Services
{
    /// <summary>
    /// Orquestra a lógica de automação do "Revive", gerenciando os hooks globais,
    /// o estado da automação e a execução das tarefas.
    /// </summary>
    /// <remarks>
    /// Esta classe é IDisposable e deve ser corretamente descartada para liberar os hooks do sistema.
    /// </remarks>
    public class AutoReviveService : IDisposable
    {
        private readonly RevivePokemonHandler _reviveHandler;
        private readonly GlobalMouseHook _mouseHook;
        private readonly GlobalKeyboardHook _keyboardHook;
        private DateTime _lastExecutionTime = DateTime.MinValue;
        private IntPtr _targetWindowHandle = IntPtr.Zero;

        /// <summary>
        /// Ocorre quando o status da automação muda (ex: iniciado, parado, executado).
        /// Usado para notificar a interface do usuário.
        /// </summary>
        public event Action<string, Color>? StatusUpdated;

        /// <summary>
        /// Obtém um valor que indica se o serviço de automação está atualmente ativo e ouvindo por gatilhos.
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AutoReviveService"/>.
        /// </summary>
        /// <param name="reviveHandler">A instância do handler que executa a lógica de reviver.</param>
        public AutoReviveService(RevivePokemonHandler reviveHandler)
        {
            _reviveHandler = reviveHandler;
            _mouseHook = new GlobalMouseHook();
            _keyboardHook = new GlobalKeyboardHook();

            _mouseHook.MouseEvent += OnGlobalMouseEvent;
            _keyboardHook.KeyDown += OnGlobalKeyDown;
        }

        /// <summary>
        /// Inicia o serviço de automação, ativando os hooks para a janela alvo.
        /// </summary>
        /// <param name="settings">As configurações da aplicação contendo as teclas de atalho e posições.</param>
        /// <param name="targetWindowHandle">O handle da janela onde a automação deve ser executada.</param>
        public void Start(AppSettings settings, IntPtr targetWindowHandle)
        {
            if (IsRunning) return;

            if (!KeyboardHandler.TryParseKey(settings.ExecuteKey, out var executeKey) && !IsMouseTrigger(settings.ExecuteKey))
            {
                StatusUpdated?.Invoke("Erro: Tecla de execução inválida.", Color.Red);
                return;
            }

            _targetWindowHandle = targetWindowHandle;
            IsRunning = true;
            
            _keyboardHook.AddTargetKey(executeKey);
            _keyboardHook.SetTargetWindow(_targetWindowHandle); 
            _mouseHook.Start();

            StatusUpdated?.Invoke($"Modo Auto ATIVO. Pressione '{settings.ExecuteKey}'.", Color.Green);
        }
        
        /// <summary>
        /// Para o serviço de automação, desativando e limpando os hooks globais.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;

            _keyboardHook.ClearTargetKeys();
            _keyboardHook.ClearTargetWindow();
            _mouseHook.Stop();
            IsRunning = false;
            _targetWindowHandle = IntPtr.Zero; 
            
            StatusUpdated?.Invoke("Modo Auto DESATIVADO.", Color.Orange);
        }

        /// <summary>
        /// Método central que processa um gatilho (tecla ou mouse), respeitando o cooldown,
        /// e dispara a tarefa de automação.
        /// </summary>
        /// <param name="triggerKey">A representação em string do gatilho que foi acionado.</param>
        private void HandleTrigger(string triggerKey)
        {
            var settings = SettingsManager.LoadSettings(); 
            if (!IsRunning || !triggerKey.Equals(settings.ExecuteKey, StringComparison.OrdinalIgnoreCase)) return;
    
            if ((DateTime.UtcNow - _lastExecutionTime).TotalSeconds < 1)
            {
                return;
            }
            _lastExecutionTime = DateTime.UtcNow;
            
            Task.Run(() => ExecuteReviveTask(settings));
        }

        /// <summary>
        /// Executa a tarefa de reviver em uma thread separada para não bloquear a UI.
        /// </summary>
        /// <param name="settings">As configurações atuais da aplicação.</param>
        private async Task ExecuteReviveTask(AppSettings settings)
        {
            var windowHandle = _targetWindowHandle; 
            if (windowHandle == IntPtr.Zero) return;

            if (!KeyboardHandler.TryParseKey(settings.PokemonKey, out var pokemonKey) ||
                !KeyboardHandler.TryParseKey(settings.ReviveKey, out var reviveKey))
            {
                StatusUpdated?.Invoke("Erro: Tecla de Pokémon ou Revive inválida.", Color.Red);
                return;
            }

            var revivePosition = new Point(settings.RevivePositionX, settings.RevivePositionY);
            var windowInfo = new WindowInfo { Handle = windowHandle };
            var relativePoint = windowInfo.ScreenToClient(revivePosition);

            StatusUpdated?.Invoke("⚡ Revive executado!", Color.DodgerBlue);
            await _reviveHandler.ExecuteFastRevive(windowHandle, pokemonKey, reviveKey, relativePoint);
        }
        
        /// <summary>
        /// Manipulador para o evento de tecla pressionada do hook global de teclado.
        /// </summary>
        private void OnGlobalKeyDown(object? sender, Keys pressedKey) => HandleTrigger(pressedKey.ToString());
        
        /// <summary>
        /// Manipulador para os eventos de mouse do hook global.
        /// </summary>
        private void OnGlobalMouseEvent(object? sender, GlobalMouseEventArgs e)
        {
            if (e.Action == MouseAction.Down && IsMouseTrigger(e.Button.ToString()))
            {
                HandleTrigger(e.Button.ToString());
            }
        }

        /// <summary>
        /// Verifica se a tecla informada corresponde a um botão de mouse usado como gatilho.
        /// </summary>
        private bool IsMouseTrigger(string key) => key.Equals("XButton1") || key.Equals("XButton2");

        /// <summary>
        /// Libera os recursos utilizados pelo serviço, garantindo que os hooks sejam desativados.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _mouseHook.Dispose();
            _keyboardHook.Dispose();
        }
    }
}