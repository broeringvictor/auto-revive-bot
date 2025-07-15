using System.Runtime.InteropServices;

namespace PxG.Models;

public struct WindowInfo
{
    public IntPtr Handle { get; set; }
    public string Title { get; set; }
    public string ProcessName { get; set; }

    /// <summary>
    /// Define como o objeto será exibido em controles como ComboBox.
    /// </summary>
    public override string ToString() => $"{Title} ({ProcessName})";
    
    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
        
    /// <summary>
    /// Converte um ponto global da tela para um ponto relativo a esta janela.
    /// </summary>
    /// <param name="screenPoint">O ponto na tela a ser convertido.</param>
    /// <returns>O ponto com coordenadas relativas à janela.</returns>
    public Point ScreenToClient(Point screenPoint)
    {
        // 2. Chama a função da API usando o Handle desta janela
        ScreenToClient(this.Handle, ref screenPoint);
        return screenPoint;
    }
}