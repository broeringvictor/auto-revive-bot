using PxG.Models; 

namespace PxG.Handlers
{
    /// <summary>
    /// Orquestra a sequência de ações para reviver um Pokémon.
    /// </summary>
    public class RevivePokemonHandler
    {
        private readonly CursorPoint _cursorHandler = new();

        /// <summary>
        /// Executa a sequência completa para usar um reviver em um Pokémon.
        /// </summary>
        /// <param name="targetWindowHandle">O handle da janela do jogo.</param>
        /// <param name="pokemonKey">A tecla de atalho para selecionar o Pokémon.</param>
        /// <param name="reviveKey">A tecla de atalho para o item de reviver.</param>
        /// <param name="pokemonBarPosition">A posição (X, Y) na barra onde o Pokémon está para receber o clique direito.</param>
        public void ExecuteRevive(IntPtr targetWindowHandle, Keys pokemonKey, Keys reviveKey, System.Drawing.Point pokemonBarPosition)
        {
            if (targetWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("O handle da janela alvo não pode ser nulo.", nameof(targetWindowHandle));
            }

            // Salva a posição atual do mouse para restaurar depois
            Point originalMousePosition = _cursorHandler.GetMousePositionInWindow(targetWindowHandle) ?? Point.Empty;

            // Etapa 1: Pressionar a tecla do Pokémon.
            // Objetivo: Garantir que o Pokémon esteja na pokébola ou cancelar qualquer ação anterior.
            // Se o Pokémon estiver fora, ele volta. Se já estiver na pokébola, nada acontece.
            KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
            Thread.Sleep(300); // Atraso para o jogo processar o comando.

            // Etapa 2: Pressionar a tecla do item de reviver.
            // Objetivo: Ativar o item "revive", o que geralmente muda o cursor do mouse.
            KeyboardHandler.SendKey(targetWindowHandle, reviveKey);
            Thread.Sleep(500); // Atraso crucial para o jogo registrar a ativação do item e mudar o estado do cursor.

            // Etapa 3: Clicar na posição do Pokémon na barra.
            // Objetivo: Usar o item "revive" no Pokémon que está na posição especificada.
            _cursorHandler.LeftClickOnWindowPoint(targetWindowHandle, pokemonBarPosition);
            Thread.Sleep(200); // Atraso longo para permitir que a animação ou menu de confirmação do jogo apareça.

            // Etapa 4: Pressionar a tecla do Pokémon novamente.
            // Objetivo: Confirmar o uso do revive e/ou trazer o Pokémon de volta à batalha.
            KeyboardHandler.SendKey(targetWindowHandle, pokemonKey);
            Thread.Sleep(200); 

     
            if (originalMousePosition != Point.Empty)
            {
                _cursorHandler.MoveMouseToWindowPoint(targetWindowHandle, originalMousePosition);
            }
        }
    }
}