using NUnit.Framework;
using PxG.Handlers; 
using System.Drawing;
using System.Threading;

namespace Tests
{
    [TestFixture]
    public class ScreenAnalyzerTests
    {
        [Test]
        public void FindFaintedIcon_QuandoIconeEstaVisivelNaTela_DeveRetornarTrue()
        {
            // Arrange (Preparação)
            var posicaoParaTestar = new Point(47, 86);
            double confiancaMinima = 0.7; // 70% de confiança

            // --- AVISO PARA O USUÁRIO ---
            // Este teste é manual e precisa que a tela esteja preparada.
            var mensagemDeAviso = "AVISO: Este teste irá iniciar em 5 segundos.\n" +
                                  "Por favor, certifique-se de que a tela do jogo está visível e que o ícone do Pokémon desmaiado\n" +
                                  $"está na posição exata da tela: X={posicaoParaTestar.X}, Y={posicaoParaTestar.Y}";

            // Exibe o aviso nos resultados do teste
            TestContext.Progress.WriteLine(mensagemDeAviso);
            
            // Dá um tempo para o usuário arrumar a tela
            Thread.Sleep(5000);

            // Act (Ação)
            bool foiEncontrado = false;
            try
            {
                foiEncontrado = ScreenAnalyzer.FindFaintedIcon(posicaoParaTestar, confiancaMinima);
            }
            catch (Exception ex)
            {
                // Se o teste falhar com uma exceção, o NUnit vai reportar isso de forma clara.
                Assert.Fail($"O teste falhou com uma exceção inesperada: {ex.Message}");
            }

            // Assert (Verificação)
            Assert.That(foiEncontrado, Is.True, "A função FindFaintedIcon deveria ter encontrado o ícone na tela, mas retornou 'false'.");
        }
    }
}