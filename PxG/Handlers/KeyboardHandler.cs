using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    public static class KeyboardHandler
    {
        // Constantes da API do Windows
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint MAPVK_VK_TO_VSC = 0x00; // Mapeia uma tecla virtual para um scan code

        #region P/Invoke para Funções da API

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // NOVA FUNÇÃO: Mapeia o código de uma tecla virtual para um código de varredura (scan code)
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        #endregion

        /// <summary>
        /// Envia um pressionamento de tecla (apertar e soltar) para a janela alvo,
        /// incluindo o Scan Code para maior compatibilidade com jogos.
        /// </summary>
        /// <param name="hWnd">O handle da janela que receberá a tecla.</param>
        /// <param name="key">A tecla a ser pressionada.</param>
        public static void SendKey(IntPtr hWnd, Keys key)
        {
            if (hWnd == IntPtr.Zero) return;

            // 1. Obter o Scan Code da tecla virtual
            uint scanCode = MapVirtualKey((uint)key, MAPVK_VK_TO_VSC);

            // 2. Construir o parâmetro lParam para o evento KEYDOWN
            // Formato: (1 | (scanCode << 16))
            // Bit 0: Repeat count (1 para uma única pressão)
            // Bits 16-23: Scan code
            IntPtr lParamDown = (IntPtr)((scanCode << 16) | 1);
            
            // 3. Construir o parâmetro lParam para o evento KEYUP
            // Formato: (1 | (scanCode << 16) | (1 << 30) | (1 << 31))
            // Bit 30: Previous key state (1 para indicar que a tecla estava pressionada)
            // Bit 31: Transition state (1 para indicar que a tecla está sendo solta)
            IntPtr lParamUp = (IntPtr)((scanCode << 16) | 0xC0000001);

            // 4. Enviar as mensagens com os parâmetros corretos
            PostMessage(hWnd, WM_KEYDOWN, (IntPtr)key, lParamDown);
            Thread.Sleep(120); // Pausa recomendada entre pressionar e soltar
            PostMessage(hWnd, WM_KEYUP, (IntPtr)key, lParamUp);
        }

        /// <summary>
        /// Converte uma string de tecla para o enum Keys de forma mais robusta.
        /// Suporta diferentes formatos de entrada do usuário, incluindo combinações com modificadores.
        /// </summary>
        /// <param name="keyText">Texto da tecla inserido pelo usuário (ex: "Shift+F1", "Ctrl+A")</param>
        /// <param name="key">A tecla convertida (saída)</param>
        /// <returns>True se a conversão foi bem-sucedida</returns>
        public static bool TryParseKey(string keyText, out Keys key)
        {
            key = Keys.None;
            
            if (string.IsNullOrWhiteSpace(keyText))
                return false;

            // Limpa e normaliza a entrada
            keyText = keyText.Trim().ToUpperInvariant();

            // Se contém modificadores (ex: "SHIFT+F1")
            if (keyText.Contains("+"))
            {
                var parts = keyText.Split('+');
                if (parts.Length < 2) return false;

                // A última parte é sempre a tecla principal
                string mainKeyText = parts[^1];
                
                // Converte a tecla principal
                if (!TryParseSimpleKey(mainKeyText, out Keys mainKey))
                    return false;

                // Adiciona os modificadores
                Keys modifiers = Keys.None;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    switch (parts[i])
                    {
                        case "CTRL":
                        case "CONTROL":
                            modifiers |= Keys.Control;
                            break;
                        case "ALT":
                            modifiers |= Keys.Alt;
                            break;
                        case "SHIFT":
                            modifiers |= Keys.Shift;
                            break;
                        default:
                            return false; // Modificador inválido
                    }
                }

                key = mainKey | modifiers;
                return true;
            }
            else
            {
                // Tecla simples sem modificadores
                return TryParseSimpleKey(keyText, out key);
            }
        }

        /// <summary>
        /// Converte uma tecla simples (sem modificadores) para o enum Keys
        /// </summary>
        private static bool TryParseSimpleKey(string keyText, out Keys key)
        {
            key = Keys.None;

            // Mapeamento de entradas comuns que o usuário pode digitar (PRIMEIRO)
            var keyMappings = new Dictionary<string, Keys>
            {
                // Números (mapeamento explícito ANTES da conversão automática)
                {"0", Keys.D0}, {"1", Keys.D1}, {"2", Keys.D2}, {"3", Keys.D3}, {"4", Keys.D4},
                {"5", Keys.D5}, {"6", Keys.D6}, {"7", Keys.D7}, {"8", Keys.D8}, {"9", Keys.D9},
                
                // Teclas de função
                {"F1", Keys.F1}, {"F2", Keys.F2}, {"F3", Keys.F3}, {"F4", Keys.F4},
                {"F5", Keys.F5}, {"F6", Keys.F6}, {"F7", Keys.F7}, {"F8", Keys.F8},
                {"F9", Keys.F9}, {"F10", Keys.F10}, {"F11", Keys.F11}, {"F12", Keys.F12},
                
                // Letras
                {"A", Keys.A}, {"B", Keys.B}, {"C", Keys.C}, {"D", Keys.D}, {"E", Keys.E},
                {"F", Keys.F}, {"G", Keys.G}, {"H", Keys.H}, {"I", Keys.I}, {"J", Keys.J},
                {"K", Keys.K}, {"L", Keys.L}, {"M", Keys.M}, {"N", Keys.N}, {"O", Keys.O},
                {"P", Keys.P}, {"Q", Keys.Q}, {"R", Keys.R}, {"S", Keys.S}, {"T", Keys.T},
                {"U", Keys.U}, {"V", Keys.V}, {"W", Keys.W}, {"X", Keys.X}, {"Y", Keys.Y},
                {"Z", Keys.Z},
                
                // Teclas especiais
                {"SPACE", Keys.Space}, {"SPACEBAR", Keys.Space}, {"ESPAÇO", Keys.Space},
                {"ENTER", Keys.Enter}, {"RETURN", Keys.Return},
                {"ESC", Keys.Escape}, {"ESCAPE", Keys.Escape},
                {"TAB", Keys.Tab},
                {"SHIFT", Keys.Shift}, {"CTRL", Keys.Control}, {"ALT", Keys.Alt},
                {"UP", Keys.Up}, {"DOWN", Keys.Down}, {"LEFT", Keys.Left}, {"RIGHT", Keys.Right},
                {"INSERT", Keys.Insert}, {"DELETE", Keys.Delete}, {"HOME", Keys.Home}, {"END", Keys.End},
                {"PAGEUP", Keys.PageUp}, {"PAGEDOWN", Keys.PageDown},
                
                // Variações em português
                {"CIMA", Keys.Up}, {"BAIXO", Keys.Down}, {"ESQUERDA", Keys.Left}, {"DIREITA", Keys.Right},
                {"INSERIR", Keys.Insert}, {"DELETAR", Keys.Delete}, {"INICIO", Keys.Home}, {"FIM", Keys.End}
            };

            // Primeiro tenta o mapeamento customizado
            if (keyMappings.TryGetValue(keyText, out key))
                return true;

            // Depois tenta a conversão direta (para casos não cobertos pelo mapeamento)
            if (Enum.TryParse(keyText, true, out key))
                return true;

            return false;
        }

        /// <summary>
        /// Obtém uma lista de teclas válidas para exibir ao usuário
        /// </summary>
        /// <returns>Lista de exemplos de teclas válidas</returns>
        public static string GetValidKeysExamples()
        {
            return "Exemplos válidos: F1, F2, A, B, Space, Enter, Shift+F1, Ctrl+A, Alt+Tab, etc.";
        }
    }
}