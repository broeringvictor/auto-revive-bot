using OpenCvSharp;
using OpenCvSharp.Extensions; 

namespace PxG.Handlers
{
    public static class ScreenAnalyzer
    {
        /// <summary>
        /// Procura pelo ícone de um pokemon-desmaiado.png em uma área da tela usando Template Matching.
        /// </summary>
        /// <param name="pokemonSlotPosition">A posição clicada pelo usuário no slot do Pokémon.</param>
        /// <param name="confidenceThreshold">O nível de confiança mínimo para considerar uma correspondência (ex: 0.7 para 70%).</param>
        /// <returns>True se o ícone for encontrado com a confiança necessária.</returns>
        public static bool FindFaintedIcon(System.Drawing.Point pokemonSlotPosition, double confidenceThreshold = 0.6)
        {
            System.Drawing.Rectangle searchArea =
                new System.Drawing.Rectangle(pokemonSlotPosition.X - 20, pokemonSlotPosition.Y - 20, 40, 40);
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "pokemon-desmaiado.png");

            // A variável 'template' é criada e só existe dentro deste bloco 'using'
            using (Mat template = Cv2.ImRead(templatePath, ImreadModes.Unchanged))
            {
                // 1. Verificação de segurança (está no lugar certo)
                if (template.Empty())
                {
                    throw new Exception(
                        @"OpenCV não conseguiu carregar a imagem do template em: {templatePath}. O arquivo pode estar corrompido ou em um formato PNG não suportado. Tente salvá-lo novamente com o MS Paint.");
                }

                // 2. O resto da lógica que USA a variável 'template' deve continuar AQUI DENTRO.
                using (Bitmap screenAreaBitmap = new Bitmap(searchArea.Width, searchArea.Height))
                {
                    using (Graphics g = Graphics.FromImage(screenAreaBitmap))
                    {
                        g.CopyFromScreen(searchArea.Location, System.Drawing.Point.Empty, searchArea.Size);
                    }

                    using (Mat screenMat = BitmapConverter.ToMat(screenAreaBitmap))
                    {
                        using (Mat result = new Mat())
                        {
                            // Esta linha agora funciona, pois 'template' ainda existe neste escopo.
                            Cv2.MatchTemplate(screenMat, template, result, TemplateMatchModes.CCoeffNormed);

                            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);

                            if (maxVal >= confidenceThreshold)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Se a função chegou até aqui, o ícone não foi encontrado.
            return false;
        }
    }
}