using PxG.Models;
using System.Drawing; // Adicionado para Point
using System.Threading.Tasks; // Adicionado para usar Task e async/await

namespace PxG.Handlers
{
    public class RevivePokemonHandler
    {
        private readonly CursorPoint _cursorHandler = new();

        /// <summary>
        /// Executa a sequência inteligente para usar um reviver em um Pokémon.
        /// Primeiro, verifica se o Pokémon está desmaiado usando análise de imagem.
        /// </summary>
        public async Task ExecuteSmartRevive(IntPtr targetWindowHandle, Keys pokemonKey, Keys reviveKey, Point pokemonBarPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }
            

            // 1) verificamos se o ícone de "desmaiado" está realmente presente.
            // ScreenAnalyzer com uma confiança de 80%.
            #region Identificar se o pokemon está desmaiado
            
            // Se estiver desmaiado, não precisamos clicar nele.
            bool isFainted = ScreenAnalyzer.FindFaintedIcon(pokemonBarPosition, 0.8);

           
            if (!isFainted)
            {
                KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
                await Task.Delay(500);

            }

            #endregion
            
            // Pega a posição atual do mouse para restaurar depois
            var originalMousePosition = _cursorHandler.GetCurrentPosition();



            #region Segunda etapa: Usar o item de reviver

            // Pressiona a tecla do item de reviver para ativar o cursor de alvo.
            KeyboardHandler.SendKey(targetWindowHandle, reviveKey);
            
            await Task.Delay(200);

            // Clica na posição do Pokémon para usar o item nele.
            _cursorHandler.LeftClickOnWindowPoint(targetWindowHandle, pokemonBarPosition);
            await Task.Delay(200);

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
    }
}