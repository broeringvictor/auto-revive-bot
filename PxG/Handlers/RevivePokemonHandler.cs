using PxG.Models;
using System.Drawing;
using System.Runtime.InteropServices; // Adicionado para Point
using System.Threading.Tasks; // Adicionado para usar Task e async/await

namespace PxG.Handlers
{
    public class RevivePokemonHandler
    {
        private readonly CursorPoint _cursorHandler = new();

        /// <summary>
        /// Bloqueia a entrada do usuário (teclado e mouse) enquanto o reviver é usado.
        /// <remarks> Se travar ou for interrompido enquanto a entrada do usuário estiver bloqueada, ficará "preso" e não conseguirá usar o computador, sendo forçado a reiniciar (usando Ctrl+Alt+Del, que é uma das poucas coisas que o BlockInput não bloqueia).</remarks>
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        /// <summary>
        /// Ultrapassado: versão antiga do método ExecuteSmartRevive, a diferença é que o botão da hotkey não está configurada para o uso rápido, exigindo alguns metódos extras.
        /// Executa a sequência inteligente para usar um reviver em um Pokémon.
        /// Primeiro, verifica se o Pokémon está desmaiado usando análise de imagem.
        /// </summary>
        public async Task ExecuteSmartRevive(IntPtr targetWindowHandle, Keys pokemonKey, Keys reviveKey,
            Point pokemonBarPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException(@"O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }


            // 1) verificamos se o ícone de "desmaiado" está realmente presente.
            // ScreenAnalyzer com uma confiança de 80%.

            #region Identificar se o pokemon está desmaiado

            // Se estiver desmaiado, não precisamos clicar nele.
            bool isFainted = ScreenAnalyzer.FindFaintedIcon(pokemonBarPosition, 0.7);


            if (!isFainted)
            {
                KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
                await Task.Delay(450);
                Console.WriteLine(@"O Pokémon não está desmaiado.");

            }

            #endregion

            // Pega a posição atual do mouse para restaurar depois
            var originalMousePosition = _cursorHandler.GetCurrentPosition();



            #region Segunda etapa: Usar o item de reviver

            // Pressiona a tecla do item de reviver para ativar o cursor de alvo.
            KeyboardHandler.SendKey(targetWindowHandle, reviveKey);

            await Task.Delay(150);

            // Clica na posição do Pokémon para usar o item nele.
            _cursorHandler.LeftClickOnWindowPoint(targetWindowHandle, pokemonBarPosition);
            await Task.Delay(150);

            // Pressiona a tecla do Pokémon novamente para mandá-lo para a batalha.
            KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
            await Task.Delay(200);

            // Restaura a posição original do mouse
            if (originalMousePosition.HasValue)
            {
                _cursorHandler.SetCursorPosition(originalMousePosition.Value);
            }


            #endregion

        }

        /// <summary>
        /// Executa uma sequência de revive em um Pokémon, com uma lógica condicional baseada em seu estado.
        /// </summary>
        /// <remarks>
        /// O fluxo de execução é o seguinte:
        /// 1. O método primeiro verifica se o Pokémon na posição especificada está desmaiado.
        /// 2. Se o Pokémon NÃO estiver desmaiado, a tecla de atalho dele é pressionada (geralmente para recolhê-lo) e o fluxo continua.
        /// 3. Em seguida, a entrada do usuário (mouse e teclado) é bloqueada para garantir a precisão da automação.
        /// 4. A sequência de revive é executada: o cursor é movido para a posição do Pokémon, a tecla do item de revive é pressionada, e a tecla do Pokémon é pressionada novamente.
        /// 5. Finalmente, a entrada do usuário é desbloqueada, independentemente do resultado.
        /// Este método garante que a automação seja executada em ambos os cenários (Pokémon desmaiado ou não) para ativar o cooldown do item.
        /// </remarks>
        public async Task ExecuteFastRevive(IntPtr targetWindowHandle, Keys pokemonKey, Keys reviveKey,
            Point pokemonBarPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException(@"O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }

            // A automação agora executa o fluxo completo sem verificar se o Pokémon está desmaiado,
            // garantindo que o cooldown seja sempre ativado.
            var inputBlocked = false;
            try
            {
                bool isFainted = ScreenAnalyzer.FindFaintedIcon(pokemonBarPosition, 0.75);

                if (!isFainted)
                {
                    KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
                    //Console.WriteLine(@"O Pokémon não está desmaiado.");
                    await Task.Delay(350);
                    
                }
                
                if (BlockInput(true))
                {
                    inputBlocked = true;
                }
                else
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    Console.WriteLine($@"FALHA AO BLOQUEAR A ENTRADA. Código de erro Win32: {errorCode}");
                }

                var originalMousePosition = _cursorHandler.GetCurrentPosition();

                _cursorHandler.SetCursorPosition(pokemonBarPosition);
                await Task.Delay(80);

                KeyboardHandler.SendKey(targetWindowHandle, reviveKey);
                await Task.Delay(40);

                KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);

                if (originalMousePosition.HasValue)
                {
                    _cursorHandler.SetCursorPosition(originalMousePosition.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"ERRO DURANTE A EXECUÇÃO RÁPIDA: {ex.Message}");
            }
            finally
            {
                if (inputBlocked)
                {
                    BlockInput(false);
                }
            }
        }
    }
}
