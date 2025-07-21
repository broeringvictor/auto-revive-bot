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
        /// Executa uma sequência rápida para reviver um Pokémon, bloqueando a entrada do usuário durante a operação.
        /// </summary>
        /// <remarks>
        /// IMPORTANTE: Este método assume que a tecla de atalho do item de reviver ('reviveKey') 
        /// está configurada para uso rápido no jogo. A entrada do usuário (mouse e teclado) será
        /// desativada temporariamente para garantir a precisão da automação.
        /// </remarks>
        public async Task ExecuteFastRevive(IntPtr targetWindowHandle, Keys pokemonKey, Keys reviveKey,
            Point pokemonBarPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException(@"O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }

            try
            {
                bool success = BlockInput(true);
                if (!success)
                {
                    // Se falhar, obtém o código do erro da API do Windows
                    int errorCode = Marshal.GetLastWin32Error();
                    // Escreve no console para sabermos o que aconteceu
                    Console.WriteLine($@"FALHA AO BLOQUEAR A ENTRADA. Código de erro Win32: {errorCode}");
                    // Código 5 significa "Acesso Negado", o que reforça a necessidade de rodar como Admin.
                }


                // 2) Verificamos se o ícone de "desmaiado" está realmente presente.
                bool isFainted = ScreenAnalyzer.FindFaintedIcon(pokemonBarPosition, 0.75);

                if (!isFainted)
                {
                    KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
                    Console.WriteLine(@"O Pokémon não está desmaiado.");
                    await Task.Delay(450);
                    
                }

                // Pega a posição atual do mouse para restaurar depois
                var originalMousePosition = _cursorHandler.GetCurrentPosition();

                // 3) Segunda etapa: Usar o item de reviver
     

                _cursorHandler.SetCursorPosition(pokemonBarPosition);
                await Task.Delay(150);

                KeyboardHandler.SendKey(targetWindowHandle, reviveKey);
                await Task.Delay(150);

                KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);

                // Restaura a posição original do mouse
                if (originalMousePosition.HasValue)
                {
                    _cursorHandler.SetCursorPosition(originalMousePosition.Value);
                }
                // 4) REATIVA a entrada do usuário, aconteça o que acontecer.
                // Este bloco é executado mesmo que ocorra um erro no 'try'.
                
            }
            finally
            {
                BlockInput(false);
            }
        }
    }
}
