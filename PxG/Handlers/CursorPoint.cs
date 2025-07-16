using System.Runtime.InteropServices;

namespace PxG.Handlers
{
    public class CursorPoint
    {
        #region P/Invoke para funções da API do Windows

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
        
     
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        #endregion

        #region Constantes do Mouse

        // Constantes para os eventos do mouse
        private const uint MouseeventfLeftdown = 0x02;
        private const uint MouseeventfLeftup = 0x04;
        private const uint MouseeventfRightdown = 0x08; 
        private const uint MouseeventfRightup = 0x10;   

        #endregion

        #region Estruturas
        
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        #endregion

        /// <summary>
        /// Retorna as coordenadas globais (relativas à tela inteira).
        /// </summary>
        public System.Drawing.Point? GetCurrentPosition()
        {
            if (GetCursorPos(out Point point))
            {
                return new System.Drawing.Point(point.X, point.Y);
            }
            return null;
        }

        /// <summary>
        /// Move o cursor para as coordenadas globais especificadas.
        /// </summary>
        /// <param name="point">O ponto na tela para onde mover o cursor.</param>
        public void SetCursorPosition(System.Drawing.Point point)
        {
            SetCursorPos(point.X, point.Y);
        }
        /// <summary>
        /// Obtém a posição do cursor do mouse relativa à área de cliente de uma janela específica.
        /// </summary>
        public System.Drawing.Point? GetMousePositionInWindow(IntPtr hWnd)
        {
            if (GetCursorPos(out Point point))
            {
                if (ScreenToClient(hWnd, ref point))
                {
                    return new System.Drawing.Point(point.X, point.Y);
                }
            }
            return null;
        }

        /// <summary>
        /// Executa um clique DENTRO da janela selecionada.
        /// Converte as coordenadas da janela para coordenadas de tela e simula um clique.
        /// </summary>
        /// <param name="hWnd">O handle da janela onde o clique ocorrerá.</param>
        /// <param name="windowPoint">A posição do clique (X, Y) relativa ao canto superior esquerdo da janela.</param>
        public void ClickOnWindowPoint(IntPtr hWnd, System.Drawing.Point windowPoint)
        {
            // 1. Converte o ponto relativo da janela para um ponto absoluto na tela.
            Point clientPoint = new Point { X = windowPoint.X, Y = windowPoint.Y };
            if (!ClientToScreen(hWnd, ref clientPoint))
            {
                
                throw new InvalidOperationException("Não foi possível converter as coordenadas da janela para a tela.");
            }

            // 2. Move o cursor para o ponto exato na tela.
            SetCursorPos(clientPoint.X, clientPoint.Y);

            // Pequena pausa para garantir que o sistema operacional processe o movimento do mouse.
            Thread.Sleep(50); 

            // 3. Simula o clique do mouse (botão esquerdo para baixo e para cima).
            mouse_event(MouseeventfLeftdown, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
            Thread.Sleep(50); // Pausa entre pressionar e soltar.
            mouse_event(MouseeventfLeftup, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Executa um clique com o botão ESQUERDO dentro da janela selecionada.
        /// Método específico para deixar claro que é clique esquerdo.
        /// </summary>
        /// <param name="hWnd">O handle da janela onde o clique ocorrerá.</param>
        /// <param name="windowPoint">A posição do clique (X, Y) relativa ao canto superior esquerdo da janela.</param>
        public void LeftClickOnWindowPoint(IntPtr hWnd, System.Drawing.Point windowPoint)
        {
            // 1. Converte o ponto relativo da janela para um ponto absoluto na tela.
            Point clientPoint = new Point { X = windowPoint.X, Y = windowPoint.Y };
            if (!ClientToScreen(hWnd, ref clientPoint))
            {
                throw new InvalidOperationException("Não foi possível converter as coordenadas da janela para a tela.");
            }

            // 2. Move o cursor para o ponto exato na tela.
            SetCursorPos(clientPoint.X, clientPoint.Y);

            // Pequena pausa para garantir que o sistema operacional processe o movimento do mouse.
            Thread.Sleep(50); 

            // 3. Simula o clique do mouse (botão esquerdo para baixo e para cima).
            mouse_event(MouseeventfLeftdown, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
            Thread.Sleep(50); // Pausa entre pressionar e soltar.
            mouse_event(MouseeventfLeftup, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
        }
        
        /// <summary>
        /// Executa um clique com o botão DIREITO dentro da janela selecionada.
        /// </summary>
        /// <param name="hWnd">O handle da janela onde o clique ocorrerá.</param>
        /// <param name="windowPoint">A posição do clique (X, Y) relativa ao canto superior esquerdo da janela.</param>
        public void RightClickOnWindowPoint(IntPtr hWnd, System.Drawing.Point windowPoint)
        {
            Point clientPoint = new Point { X = windowPoint.X, Y = windowPoint.Y };
            if (!ClientToScreen(hWnd, ref clientPoint))
            {
                throw new InvalidOperationException("Não foi possível converter as coordenadas da janela para a tela.");
            }

            SetCursorPos(clientPoint.X, clientPoint.Y);
            Thread.Sleep(50);

            // Simula o clique do mouse (botão direito para baixo e para cima).
            mouse_event(MouseeventfRightdown, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
            Thread.Sleep(50);
            mouse_event(MouseeventfRightup, (uint)clientPoint.X, (uint)clientPoint.Y, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Move o cursor para uma posição específica dentro de uma janela.
        /// </summary>
        /// <param name="hWnd">O handle da janela</param>
        /// <param name="windowPoint">A posição (X, Y) relativa ao canto superior esquerdo da janela</param>
        public void MoveMouseToWindowPosition(IntPtr hWnd, System.Drawing.Point windowPoint)
        {
            // 1. Converte o ponto relativo da janela para um ponto absoluto na tela.
            Point clientPoint = new Point { X = windowPoint.X, Y = windowPoint.Y };
            if (!ClientToScreen(hWnd, ref clientPoint))
            {
                throw new InvalidOperationException("Não foi possível converter as coordenadas da janela para a tela.");
            }

            // 2. Move o cursor para o ponto exato na tela.
            SetCursorPos(clientPoint.X, clientPoint.Y);
        }

        /// <summary>
        /// Move o mouse para um ponto específico dentro de uma janela e salva a posição anterior para restaurar depois
        /// </summary>
        /// <param name="hWnd">Handle da janela</param>
        /// <param name="windowPoint">Ponto relativo à janela</param>
        /// <returns>A posição anterior do mouse para poder restaurar depois</returns>
        public System.Drawing.Point? MoveMouseToWindowPoint(IntPtr hWnd, System.Drawing.Point windowPoint)
        {
            // Salva a posição atual do mouse
            var currentPosition = GetCurrentPosition();
            
            // Converte o ponto relativo da janela para coordenadas de tela
            Point clientPoint = new Point { X = windowPoint.X, Y = windowPoint.Y };
            if (!ClientToScreen(hWnd, ref clientPoint))
            {
                throw new InvalidOperationException("Não foi possível converter as coordenadas da janela para a tela.");
            }

            // Move o cursor para o ponto na tela
            SetCursorPos(clientPoint.X, clientPoint.Y);
            
            return currentPosition;
        }

    }
}