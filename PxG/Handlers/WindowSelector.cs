using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using PxG.Models;

namespace PxG.Handlers
{
    /// <summary>
    /// Gerencia a descoberta e listagem de janelas abertas no sistema operacional.
    /// </summary>
    /// <remarks>
    /// Esta classe utiliza chamadas P/Invoke para a API do Windows (user32.dll) a fim de enumerar
    /// janelas visíveis que possuem um título, sendo ideal para cenários onde o usuário
    /// precisa selecionar a janela de um aplicativo específico (ex: um jogo).
    /// </remarks>
    public class WindowSelector
    {
        #region Construtor

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="WindowSelector"/>.
        /// </summary>
        public WindowSelector()
        {
            OpenWindows = new List<WindowInfo>();
        }

        #endregion

        #region Propriedades Públicas

        /// <summary>
        /// Obtém a lista de informações das janelas abertas e visíveis.
        /// </summary>
        /// <value>
        /// Uma lista de <see cref="WindowInfo"/> que é populada após a chamada do método <see cref="RefreshWindowsList"/>.
        /// </value>
        public List<WindowInfo> OpenWindows { get; private set; }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Atualiza a lista <see cref="OpenWindows"/>, encontrando todas as janelas que são visíveis e têm um título.
        /// </summary>
        /// <remarks>
        /// Este método limpa a lista atual antes de preenchê-la com as janelas recém-encontradas.
        /// </remarks>
        public void RefreshWindowsList()
        {
            OpenWindows.Clear();
            // Inicia o processo de enumeração de janelas, passando a nossa função de callback.
            EnumWindows(EnumTheWindows, IntPtr.Zero);
        }

        #endregion

        #region Métodos Privados e Delegates

        /// <summary>
        /// Representa o método de callback usado pela função <see cref="EnumWindows"/>.
        /// </summary>
        /// <param name="hWnd">Um handle para a janela encontrada.</param>
        /// <param name="lParam">Um valor definido pela aplicação (não utilizado neste caso).</param>
        /// <returns><c>true</c> para continuar a enumeração; <c>false</c> para parar.</returns>
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        /// <summary>
        /// Função de callback chamada pela <c>EnumWindows</c> para cada janela encontrada no sistema.
        /// </summary>
        /// <param name="hWnd">O handle da janela sendo processada.</param>
        /// <param name="lParam">Parâmetro adicional passado por <c>EnumWindows</c> (ignorado).</param>
        /// <returns>Retorna sempre <c>true</c> para garantir que todas as janelas sejam enumeradas.</returns>
        private bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            int size = GetWindowTextLength(hWnd);

            // Filtra apenas janelas que são visíveis e possuem um título.
            if (!IsWindowVisible(hWnd) || size <= 0)
            {
                return true; // Continua para a próxima janela.
            }
            
            var sb = new StringBuilder(size + 1);
            GetWindowText(hWnd, sb, sb.Capacity);

            // Obtém o nome do processo associado à janela.
            GetWindowThreadProcessId(hWnd, out uint processId);
            string processName = "N/A";
            try
            {
                 // Tenta obter o nome do processo. O processo pode não existir mais.
                 processName = Process.GetProcessById((int)processId).ProcessName + ".exe";
            }
            catch (ArgumentException)
            {
                // O processo pode ter terminado entre a chamada da API e o GetProcessById. Ignoramos o erro.
            }
           
            // Adiciona as informações da janela à lista.
            OpenWindows.Add(new WindowInfo
            {
                Handle = hWnd,
                Title = sb.ToString(),
                ProcessName = processName
            });

            return true; // Retorna true para continuar enumerando outras janelas.
        }

        #endregion

        #region P/Invoke para Funções da API Win32

        /// <summary>
        /// Obtém o texto do título da janela especificada.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        /// <summary>
        /// Obtém o tamanho, em caracteres, do texto do título da janela especificada.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// Determina o estado de visibilidade da janela especificada.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// Enumera todas as janelas de nível superior na tela, passando o handle de cada uma para uma função de callback.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// Recupera o identificador da thread que criou a janela e, opcionalmente, o identificador do processo.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        #endregion
    }
}