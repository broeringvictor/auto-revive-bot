
namespace PxG.Handlers
{
    /// <summary>
    /// Orquestra a sequência de ações para usar um item de medicina em um Pokémon.
    /// </summary>
    public class MedicinePokemonHandler
    {
        private readonly CursorPoint _cursorHandler = new();

        /// <summary>
        /// Executa a sequência para usar um item de medicina.
        /// </summary>
        /// <param name="targetWindowHandle">O handle da janela do jogo.</param>
        /// <param name="medicineKey">A tecla de atalho para o item de medicina.</param>
        /// <param name="pokemonPosition">A posição (X, Y) onde o Pokémon está para receber o clique.</param>
        public void ExecuteMedicine(IntPtr targetWindowHandle, Keys medicineKey, Point pokemonPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }

            // Salva a posição atual do mouse para restaurar depois
            Point originalMousePosition = _cursorHandler.GetMousePositionInWindow(targetWindowHandle) ?? Point.Empty;

            // Etapa 1: Pressionar a tecla do item de medicina.
            // Objetivo: Ativar o item de cura, que pode mudar o cursor.
            KeyboardHandler.SendKey(targetWindowHandle, medicineKey);
            Thread.Sleep(300); // Atraso para o jogo processar o comando.

            // Etapa 2: Clicar na posição do Pokémon.
            // Objetivo: Usar o item de cura no Pokémon na posição especificada.
            _cursorHandler.LeftClickOnWindowPoint(targetWindowHandle, pokemonPosition);
            Thread.Sleep(100); // Pequeno atraso para garantir que o clique foi processado

            // Etapa 3: Restaurar a posição original do mouse
            if (originalMousePosition != Point.Empty)
            {
                _cursorHandler.MoveMouseToWindowPoint(targetWindowHandle, originalMousePosition);
            }
        }
    }
}
