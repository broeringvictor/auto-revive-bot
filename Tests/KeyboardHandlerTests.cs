using PxG.Handlers;


namespace Tests;

[TestFixture]
public class KeyboardHandlerTests
{
    [Test]
    public void TryParseKey_SimpleTeclas_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Teclas de função
            Assert.That(KeyboardHandler.TryParseKey("F1", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.F1));
            
            Assert.That(KeyboardHandler.TryParseKey("F12", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.F12));

            // Letras
            Assert.That(KeyboardHandler.TryParseKey("A", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.A));
            
            Assert.That(KeyboardHandler.TryParseKey("Z", out Keys key4), Is.True);
            Assert.That(key4, Is.EqualTo(Keys.Z));

            // Números
            Assert.That(KeyboardHandler.TryParseKey("1", out Keys key5), Is.True);
            Assert.That(key5, Is.EqualTo(Keys.D1));
            
            Assert.That(KeyboardHandler.TryParseKey("0", out Keys key6), Is.True);
            Assert.That(key6, Is.EqualTo(Keys.D0));

            // Teclas especiais
            Assert.That(KeyboardHandler.TryParseKey("Space", out Keys key7), Is.True);
            Assert.That(key7, Is.EqualTo(Keys.Space));
            
            Assert.That(KeyboardHandler.TryParseKey("Enter", out Keys key8), Is.True);
            Assert.That(key8, Is.EqualTo(Keys.Enter));
        });
    }

    [Test]
    public void TryParseKey_TeclasComModificadores_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Shift + tecla
            Assert.That(KeyboardHandler.TryParseKey("Shift+F1", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.Shift | Keys.F1));
            
            Assert.That(KeyboardHandler.TryParseKey("Shift+A", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.Shift | Keys.A));

            // Ctrl + tecla
            Assert.That(KeyboardHandler.TryParseKey("Ctrl+A", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.Control | Keys.A));
            
            Assert.That(KeyboardHandler.TryParseKey("Ctrl+F1", out Keys key4), Is.True);
            Assert.That(key4, Is.EqualTo(Keys.Control | Keys.F1));

            // Alt + tecla
            Assert.That(KeyboardHandler.TryParseKey("Alt+F4", out Keys key5), Is.True);
            Assert.That(key5, Is.EqualTo(Keys.Alt | Keys.F4));
            
            Assert.That(KeyboardHandler.TryParseKey("Alt+Tab", out Keys key6), Is.True);
            Assert.That(key6, Is.EqualTo(Keys.Alt | Keys.Tab));
        });
    }

    [Test]
    public void TryParseKey_CombinacaoMultiplosModificadores_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Ctrl + Shift + tecla
            Assert.That(KeyboardHandler.TryParseKey("Ctrl+Shift+A", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.Control | Keys.Shift | Keys.A));
            
            // Ctrl + Alt + tecla
            Assert.That(KeyboardHandler.TryParseKey("Ctrl+Alt+Delete", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.Control | Keys.Alt | Keys.Delete));
            
            // Alt + Shift + tecla
            Assert.That(KeyboardHandler.TryParseKey("Alt+Shift+Tab", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.Alt | Keys.Shift | Keys.Tab));
        });
    }

    [Test]
    public void TryParseKey_CaseInsensitive_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Minúsculas
            Assert.That(KeyboardHandler.TryParseKey("f1", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.F1));
            
            Assert.That(KeyboardHandler.TryParseKey("shift+f1", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.Shift | Keys.F1));
            
            // Maiúsculas
            Assert.That(KeyboardHandler.TryParseKey("F1", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.F1));
            
            Assert.That(KeyboardHandler.TryParseKey("SHIFT+F1", out Keys key4), Is.True);
            Assert.That(key4, Is.EqualTo(Keys.Shift | Keys.F1));
            
            // Misto
            Assert.That(KeyboardHandler.TryParseKey("Shift+f1", out Keys key5), Is.True);
            Assert.That(key5, Is.EqualTo(Keys.Shift | Keys.F1));
        });
    }

    [Test]
    public void TryParseKey_TeclasEspeciaisAlternativas_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Variações de Space
            Assert.That(KeyboardHandler.TryParseKey("SPACE", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.Space));
            
            Assert.That(KeyboardHandler.TryParseKey("SPACEBAR", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.Space));
            
            Assert.That(KeyboardHandler.TryParseKey("ESPAÇO", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.Space));

            // Variações de Escape
            Assert.That(KeyboardHandler.TryParseKey("ESC", out Keys key4), Is.True);
            Assert.That(key4, Is.EqualTo(Keys.Escape));
            
            Assert.That(KeyboardHandler.TryParseKey("ESCAPE", out Keys key5), Is.True);
            Assert.That(key5, Is.EqualTo(Keys.Escape));

            // Variações de Control
            Assert.That(KeyboardHandler.TryParseKey("Ctrl+A", out Keys key6), Is.True);
            Assert.That(key6, Is.EqualTo(Keys.Control | Keys.A));
            
            Assert.That(KeyboardHandler.TryParseKey("Control+A", out Keys key7), Is.True);
            Assert.That(key7, Is.EqualTo(Keys.Control | Keys.A));
        });
    }

    [Test]
    public void TryParseKey_EntradasInvalidas_DeveRetornarFalse()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // String vazia ou nula
            Assert.That(KeyboardHandler.TryParseKey("", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey("   ", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey(null!, out Keys _), Is.False);

            // Teclas inexistentes
            Assert.That(KeyboardHandler.TryParseKey("F99", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey("TECLA_INEXISTENTE", out Keys _), Is.False);
            
            // Modificadores inválidos
            Assert.That(KeyboardHandler.TryParseKey("Windows+A", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey("Super+F1", out Keys _), Is.False);
            
            // Formato inválido
            Assert.That(KeyboardHandler.TryParseKey("Shift++F1", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey("+F1", out Keys _), Is.False);
            Assert.That(KeyboardHandler.TryParseKey("F1+", out Keys _), Is.False);
        });
    }

    [Test]
    public void TryParseKey_ModificadoresSozinhos_DeveConverterCorretamente()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(KeyboardHandler.TryParseKey("Shift", out Keys key1), Is.True);
            Assert.That(key1, Is.EqualTo(Keys.Shift));
            
            Assert.That(KeyboardHandler.TryParseKey("Ctrl", out Keys key2), Is.True);
            Assert.That(key2, Is.EqualTo(Keys.Control));
            
            Assert.That(KeyboardHandler.TryParseKey("Alt", out Keys key3), Is.True);
            Assert.That(key3, Is.EqualTo(Keys.Alt));
        });
    }

    [Test]
    public void GetValidKeysExamples_DeveRetornarExemplosValidos()
    {
        // Arrange & Act
        string examples = KeyboardHandler.GetValidKeysExamples();
        
        // Assert
        Assert.That(examples, Is.Not.Null.And.Not.Empty);
        Assert.That(examples, Does.Contain("F1"));
        Assert.That(examples, Does.Contain("Shift+F1"));
        Assert.That(examples, Does.Contain("Ctrl+A"));
    }
}

[TestFixture]
public class KeyCombinationBehaviorTests
{
    [Test]
    public void SimularCapturaTecla_TeclasSimples_DeveGerarStringCorreta()
    {
        // Simulando o comportamento do GetKeyComboString para teclas simples
        Assert.Multiple(() =>
        {
            // Simula pressionar F1 sem modificadores
            var resultado1 = SimularKeyCombo(Keys.F1, false, false, false);
            Assert.That(resultado1, Is.EqualTo("F1"));
            
            // Simula pressionar A sem modificadores
            var resultado2 = SimularKeyCombo(Keys.A, false, false, false);
            Assert.That(resultado2, Is.EqualTo("A"));
            
            // Simula pressionar Space sem modificadores
            var resultado3 = SimularKeyCombo(Keys.Space, false, false, false);
            Assert.That(resultado3, Is.EqualTo("Space"));
        });
    }

    [Test]
    public void SimularCapturaTecla_TeclasComModificadores_DeveGerarStringCorreta()
    {
        // Simulando o comportamento do GetKeyComboString para teclas com modificadores
        Assert.Multiple(() =>
        {
            // Simula pressionar Shift+F1
            var resultado1 = SimularKeyCombo(Keys.F1, false, false, true);
            Assert.That(resultado1, Is.EqualTo("Shift+F1"));
            
            // Simula pressionar Ctrl+A
            var resultado2 = SimularKeyCombo(Keys.A, true, false, false);
            Assert.That(resultado2, Is.EqualTo("Ctrl+A"));
            
            // Simula pressionar Alt+Tab
            var resultado3 = SimularKeyCombo(Keys.Tab, false, true, false);
            Assert.That(resultado3, Is.EqualTo("Alt+Tab"));
            
            // Simula pressionar Ctrl+Shift+A
            var resultado4 = SimularKeyCombo(Keys.A, true, false, true);
            Assert.That(resultado4, Is.EqualTo("Ctrl+Shift+A"));
        });
    }

    [Test]
    public void IntegracaoCompleta_CapturarEConverter_DeveManterConsistencia()
    {
        // Testa o fluxo completo: capturar -> converter -> usar
        Assert.Multiple(() =>
        {
            // Teste 1: F1 simples
            var capturado1 = SimularKeyCombo(Keys.F1, false, false, false);
            Assert.That(KeyboardHandler.TryParseKey(capturado1, out Keys convertido1), Is.True);
            Assert.That(convertido1, Is.EqualTo(Keys.F1));
            
            // Teste 2: Shift+F1
            var capturado2 = SimularKeyCombo(Keys.F1, false, false, true);
            Assert.That(KeyboardHandler.TryParseKey(capturado2, out Keys convertido2), Is.True);
            Assert.That(convertido2, Is.EqualTo(Keys.Shift | Keys.F1));
            
            // Teste 3: Ctrl+A
            var capturado3 = SimularKeyCombo(Keys.A, true, false, false);
            Assert.That(KeyboardHandler.TryParseKey(capturado3, out Keys convertido3), Is.True);
            Assert.That(convertido3, Is.EqualTo(Keys.Control | Keys.A));
            
            // Teste 4: Ctrl+Shift+Delete
            var capturado4 = SimularKeyCombo(Keys.Delete, true, false, true);
            Assert.That(KeyboardHandler.TryParseKey(capturado4, out Keys convertido4), Is.True);
            Assert.That(convertido4, Is.EqualTo(Keys.Control | Keys.Shift | Keys.Delete));
        });
    }

    /// <summary>
    /// Simula o comportamento do método GetKeyComboString do formulário
    /// </summary>
    private string SimularKeyCombo(Keys keyCode, bool ctrl, bool alt, bool shift)
    {
        var modifiers = new List<string>();
        
        if (ctrl) modifiers.Add("Ctrl");
        if (alt) modifiers.Add("Alt");
        if (shift) modifiers.Add("Shift");
        
        string mainKey = keyCode.ToString();
        
        // Lógica para teclas modificadoras sozinhas
        if ((keyCode == Keys.ControlKey || keyCode == Keys.LControlKey || keyCode == Keys.RControlKey) && ctrl)
            return "Ctrl";
        if ((keyCode == Keys.Menu || keyCode == Keys.LMenu || keyCode == Keys.RMenu) && alt)
            return "Alt";
        if ((keyCode == Keys.ShiftKey || keyCode == Keys.LShiftKey || keyCode == Keys.RShiftKey) && shift)
            return "Shift";
        
        if (modifiers.Count > 0)
            return string.Join("+", modifiers) + "+" + mainKey;
        else
            return mainKey;
    }
}