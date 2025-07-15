using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using PxG.Models;

namespace PxG.Handlers
{
    /// <summary>
    /// Classe responsável por encontrar e listar janelas abertas no sistema operacional.
    /// </summary>
    public class WindowSelector
    {

        #region Construtores e Propriedades


        
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public WindowSelector()
        {
            OpenWindows = new List<WindowInfo>();
        }

        #endregion

        #region Inicialização e Propriedades
        

        // Lista pública de janelas abertas e visíveis encontradas pelo método RefreshWindowsList.
        public List<WindowInfo> OpenWindows { get; set; }
        
        // Delegado para o ponteiro da função de callback exigida pela EnumWindows.
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        

        #endregion


        #region P/Invoke para funções da API do Windows

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        

        
        
        #endregion

        /// <summary>
        /// Atualiza a lista de janelas (OpenWindows), encontrando todas as que são visíveis e têm um título.
        /// </summary>
        public void RefreshWindowsList()
        {
            OpenWindows.Clear();
            // Inicia o processo de enumeração de janelas, passando a nossa função de callback.
            EnumWindows(new EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
        }

        /// <summary>
        /// Função de callback chamada pela EnumWindows para cada janela encontrada.
        /// </summary>
        private bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            int size = GetWindowTextLength(hWnd);

            // Filtra apenas janelas que são visíveis e possuem um título.
            if (IsWindowVisible(hWnd) && size > 0)
            {
                var sb = new StringBuilder(size + 1);
                GetWindowText(hWnd, sb, sb.Capacity);

                // Obtém o nome do processo associado à janela.
                GetWindowThreadProcessId(hWnd, out uint processId);
                string processName = "N/A";
                try
                {
                     processName = Process.GetProcessById((int)processId).ProcessName + ".exe";
                }
                catch (ArgumentException)
                {
                    // O processo pode ter terminado, então ignoramos.
                }
               
                // Adiciona as informações da janela à nossa lista.
                OpenWindows.Add(new WindowInfo
                {
                    Handle = hWnd,
                    Title = sb.ToString(),
                    ProcessName = processName
                });
            }

            return true; // Retorna true para continuar enumerando outras janelas.
        }
    }
}