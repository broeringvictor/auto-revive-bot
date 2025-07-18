// --- INÍCIO DO CÓDIGO CORRIGIDO ---

using System;
using System.Drawing; // Adicionado para usar Bitmap, Graphics e Rectangle
using System.IO;      // Adicionado para usar a classe Path
using OpenCvSharp;
using OpenCvSharp.Extensions; // Adicionado para usar BitmapConverter

namespace PxG.Handlers
{
    public static class ScreenAnalyzer
    {
        /// <summary>
        /// Procura pelo ícone cinza de "desmaiado" em uma área da tela usando Template Matching.
        /// </summary>
        /// <param name="pokemonSlotPosition">A posição clicada pelo usuário no slot do Pokémon.</param>
        /// <param name="confidenceThreshold">O nível de confiança mínimo para considerar uma correspondência (ex: 0.8 para 80%).</param>
        /// <returns>True se o ícone for encontrado com a confiança necessária.</returns>
        public static bool FindFaintedIcon(System.Drawing.Point pokemonSlotPosition, double confidenceThreshold = 0.8)
        {
            System.Drawing.Rectangle searchArea = new System.Drawing.Rectangle(pokemonSlotPosition.X - 20, pokemonSlotPosition.Y - 20, 40, 40);

            // Carrega a imagem do template que salvamos (o ícone cinza).
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "pokebola.png");
            if (!File.Exists(templatePath))
            {
                // Se o arquivo não for encontrado, podemos lançar um erro ou apenas retornar falso.
                Console.WriteLine("Erro: Arquivo de template não encontrado em " + templatePath);
                return false;
            }

            using (Mat template = Cv2.ImRead(templatePath, ImreadModes.Color))
            using (Bitmap screenAreaBitmap = new Bitmap(searchArea.Width, searchArea.Height))
            {
                // Captura a área da tela onde vamos procurar.
                using (Graphics g = Graphics.FromImage(screenAreaBitmap))
                {
                    // CORREÇÃO: Usando System.Drawing.Point.Empty para resolver o erro 'Empty' não encontrado.
                    g.CopyFromScreen(searchArea.Location, System.Drawing.Point.Empty, searchArea.Size);
                }

                // Converte a captura de tela e o template para o formato do OpenCV.
                using (Mat screenMat = BitmapConverter.ToMat(screenAreaBitmap))
                {
                    // Realiza a correspondência de template.
                    using (Mat result = new Mat())
                    {
                        Cv2.MatchTemplate(screenMat, template, result, TemplateMatchModes.CCoeffNormed);

                        // Encontra a melhor correspondência na área.
                        // CORREÇÃO: Usando descarts (_) para as variáveis que não são usadas, removendo os avisos.
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);

                        // Compara a confiança da melhor correspondência com o nosso limiar.
                        // maxVal nos dá a confiança da correspondência (de 0.0 a 1.0).
                        if (maxVal >= confidenceThreshold)
                        {
                            // Encontramos o ícone com uma boa confiança!
                            return true;
                        }
                    }
                }
            }

            // Se chegamos até aqui, o ícone não foi encontrado com a confiança necessária.
            return false;
        }
    }
}

// --- FIM DO CÓDIGO CORRIGIDO ---