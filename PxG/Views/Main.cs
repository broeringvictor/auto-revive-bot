using PxG.Handlers;
using PxG.Models;
using System;
using System.Drawing; // Use System.Drawing.Point
using System.Windows.Forms;

namespace PxG.Views
{
    public partial class Main : Form
    {
        // Handlers de lógica
        private readonly WindowSelector _windowSelector;
        private readonly RevivePokemonHandler _reviveHandler;
        private readonly GlobalMouseHook _mouseHook;
        private readonly GlobalKeyboardHook _keyboardHook;

        // Variáveis de estado
        private Point _pokemonBarPosition;
        private bool _isCapturingPosition;
        private bool _isCapturingPokemonKey;
        private bool _isCapturingReviveKey;
        private bool _isCapturingHotkeyKey;
        
        // Timer para timeout da captura de teclas
        private readonly System.Windows.Forms.Timer _captureTimeout;
        
        // Configurações da aplicação
        private AppSettings _settings;
        
        // Variáveis para o modo automático
        private bool _isAutoModeActive = false;
        private Keys _hotkeyToTrigger = Keys.None;

        public Main()
        {
            InitializeComponent();
            
            // Carrega as configurações salvas
            _settings = SettingsManager.LoadSettings();
            
            _windowSelector = new WindowSelector();
            _reviveHandler = new RevivePokemonHandler();
            
            // Inicializa o hook e se inscreve no evento de clique
            _mouseHook = new GlobalMouseHook();
            _mouseHook.MouseClicked += OnGlobalMouseClick;
            
            // Inicializa o hook global de teclado
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyPressed += OnGlobalKeyPressed;
            
            // Configura timer para timeout da captura (30 segundos)
            _captureTimeout = new System.Windows.Forms.Timer();
            _captureTimeout.Interval = 30000; // 30 segundos
            _captureTimeout.Tick += OnCaptureTimeout;
            
            // Habilita captura de teclas no formulário
            this.KeyPreview = true;
            this.KeyDown += OnFormKeyDown;
            
            // Eventos adicionais para melhor captura
            this.Activated += OnFormActivated;
            this.LostFocus += OnFormLostFocus;
            
            // Aplica as configurações carregadas
            LoadSettingsToUI();
        }

        /// <summary>
        /// Carrega as configurações salvas para a interface
        /// </summary>
        private void LoadSettingsToUI()
        {
            // Carrega as teclas salvas
            txtPokemonKey.Text = _settings.PokemonKey;
            txtReviveKey.Text = _settings.ReviveKey;
            
            // Carrega a posição salva
            if (_settings.HasValidPosition)
            {
                _pokemonBarPosition = new Point(_settings.PokemonBarPositionX, _settings.PokemonBarPositionY);
                lblStatus.Text = $"Posição carregada: X={_pokemonBarPosition.X}, Y={_pokemonBarPosition.Y}";
                lblStatus.ForeColor = Color.Green;
            }
            
            // Atualiza a lista de janelas e tenta selecionar a última janela usada
            _windowSelector.RefreshWindowsList();
            cmbWindows.DataSource = _windowSelector.OpenWindows;
            
            if (!string.IsNullOrEmpty(_settings.LastSelectedWindow))
            {
                var lastWindow = _windowSelector.OpenWindows.Where(w => w.Title == _settings.LastSelectedWindow).FirstOrDefault();
                // Verifica se encontrou alguma janela comparando com o padrão (Handle = IntPtr.Zero)
                if (lastWindow.Handle != IntPtr.Zero)
                {
                    cmbWindows.SelectedItem = lastWindow;
                }
            }
        }

        /// <summary>
        /// Salva as configurações atuais
        /// </summary>
        private void SaveCurrentSettings()
        {
            _settings.PokemonKey = txtPokemonKey.Text;
            _settings.ReviveKey = txtReviveKey.Text;
            _settings.PokemonBarPositionX = _pokemonBarPosition.X;
            _settings.PokemonBarPositionY = _pokemonBarPosition.Y;
            _settings.HasValidPosition = _pokemonBarPosition != Point.Empty;
            
            if (cmbWindows.SelectedItem is WindowInfo selectedWindow)
            {
                _settings.LastSelectedWindow = selectedWindow.Title;
            }
            
            if (SettingsManager.SaveSettings(_settings))
            {
                // Feedback sutil de que as configurações foram salvas (opcional)
                // Console.WriteLine("Configurações salvas com sucesso!");
            }
        }

        private void OnCaptureTimeout(object? sender, EventArgs e)
        {
            _captureTimeout.Stop();
            CancelAllCaptures();
            lblStatus.Text = "Status: Captura cancelada por timeout";
            lblStatus.ForeColor = Color.Red;
        }

        private void OnFormActivated(object? sender, EventArgs e)
        {
            // Quando o formulário é ativado durante captura, garante que tem foco
            if (_isCapturingPokemonKey || _isCapturingReviveKey)
            {
                this.Focus();
            }
        }

        private void OnFormLostFocus(object? sender, EventArgs e)
        {
            // Se perdeu o foco durante captura, tenta recuperar depois de um delay
            if (_isCapturingPokemonKey || _isCapturingReviveKey)
            {
                Task.Delay(100).ContinueWith(_ => {
                    this.Invoke(() => {
                        if (_isCapturingPokemonKey || _isCapturingReviveKey)
                        {
                            this.BringToFront();
                            this.Focus();
                        }
                    });
                });
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _windowSelector.RefreshWindowsList();
            cmbWindows.DataSource = null;
            cmbWindows.DataSource = _windowSelector.OpenWindows;
        }

        private void btnSetPosition_Click(object sender, EventArgs e)
        {
            // Inicia o modo de captura
            _isCapturingPosition = true;
            lblStatus.Text = "Status: Clique para gravar a posição do alvo...";
            lblStatus.ForeColor = Color.Blue;
            
            // Ativa o hook para observar o próximo clique
            _mouseHook.Start();
        }

        private void btnCapturePokemonKey_Click(object sender, EventArgs e)
        {
            // Cancela qualquer captura em andamento
            CancelAllCaptures();
            
            _isCapturingPokemonKey = true;
            btnCapturePokemonKey.Text = "Pressione uma tecla...";
            btnCapturePokemonKey.BackColor = Color.LightBlue;
            btnCapturePokemonKey.Enabled = false;
            lblStatus.Text = "Status: Pressione a tecla para o Pokémon (ESC para cancelar)";
            lblStatus.ForeColor = Color.Blue;
            
            // Inicia o timeout
            _captureTimeout.Start();
            
            // Força o foco no formulário com método mais agressivo
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        private void btnCaptureReviveKey_Click(object sender, EventArgs e)
        {
            // Cancela qualquer captura em andamento
            CancelAllCaptures();
            
            _isCapturingReviveKey = true;
            btnCaptureReviveKey.Text = "Pressione uma tecla...";
            btnCaptureReviveKey.BackColor = Color.LightBlue;
            btnCaptureReviveKey.Enabled = false;
            lblStatus.Text = "Status: Pressione a tecla para o Revive (ESC para cancelar)";
            lblStatus.ForeColor = Color.Blue;
            
            // Inicia o timeout
            _captureTimeout.Start();
            
            // Força o foco no formulário com método mais agressivo
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        /// <summary>
        /// Cancela todas as capturas em andamento e restaura o estado dos botões
        /// </summary>
        private void CancelAllCaptures()
        {
            _captureTimeout.Stop();
            
            if (_isCapturingPokemonKey)
            {
                _isCapturingPokemonKey = false;
                btnCapturePokemonKey.Text = "Capturar Tecla";
                btnCapturePokemonKey.BackColor = SystemColors.Control;
                btnCapturePokemonKey.Enabled = true;
            }
            
            if (_isCapturingReviveKey)
            {
                _isCapturingReviveKey = false;
                btnCaptureReviveKey.Text = "Capturar Tecla";
                btnCaptureReviveKey.BackColor = SystemColors.Control;
                btnCaptureReviveKey.Enabled = true;
            }
        }

        private void OnFormKeyDown(object? sender, KeyEventArgs e)
        {
            // Se ESC for pressionado durante a captura, cancela
            if (e.KeyCode == Keys.Escape && (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingHotkeyKey))
            {
                CancelAllCaptures();
                lblStatus.Text = "Status: Captura cancelada";
                lblStatus.ForeColor = Color.Orange;
                e.Handled = true;
                return;
            }
            
            // Ignora se for apenas uma tecla modificadora (Shift, Ctrl, Alt)
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu ||
                e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey ||
                e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey ||
                e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
            {
                return;
            }

            if (_isCapturingPokemonKey)
            {
                _captureTimeout.Stop();
                _isCapturingPokemonKey = false;
                
                // Captura a combinação completa de teclas incluindo modificadores
                string keyCombo = GetKeyComboString(e);
                txtPokemonKey.Text = keyCombo;
                
                btnCapturePokemonKey.Text = "Capturar Tecla";
                btnCapturePokemonKey.BackColor = SystemColors.Control;
                btnCapturePokemonKey.Enabled = true;
                lblStatus.Text = $"✓ Tecla Pokémon capturada: {keyCombo}";
                lblStatus.ForeColor = Color.Green;
                e.Handled = true;
                
                // Salva automaticamente a nova configuração
                SaveCurrentSettings();
            }
            else if (_isCapturingReviveKey)
            {
                _captureTimeout.Stop();
                _isCapturingReviveKey = false;
                
                // Captura a combinação completa de teclas incluindo modificadores
                string keyCombo = GetKeyComboString(e);
                txtReviveKey.Text = keyCombo;
                
                btnCaptureReviveKey.Text = "Capturar Tecla";
                btnCaptureReviveKey.BackColor = SystemColors.Control;
                btnCaptureReviveKey.Enabled = true;
                lblStatus.Text = $"✓ Tecla Revive capturada: {keyCombo}";
                lblStatus.ForeColor = Color.Green;
                e.Handled = true;
                
                // Salva automaticamente a nova configuração
                SaveCurrentSettings();
            }
        }

        /// <summary>
        /// Constrói uma string representando a combinação de teclas pressionadas
        /// </summary>
        /// <param name="e">Argumentos do evento de tecla</param>
        /// <returns>String como "Shift+F1", "Ctrl+A", etc.</returns>
        private string GetKeyComboString(KeyEventArgs e)
        {
            var modifiers = new List<string>();
            
            // Adiciona os modificadores na ordem padrão
            if (e.Control)
                modifiers.Add("Ctrl");
            if (e.Alt)
                modifiers.Add("Alt");
            if (e.Shift)
                modifiers.Add("Shift");
            
            // Obtém a tecla principal (sem modificadores)
            Keys mainKey = e.KeyCode;
            
            // Constrói a string final
            if (modifiers.Count > 0)
                return string.Join("+", modifiers) + "+" + mainKey;
            else
                return mainKey.ToString();
        }

        // Este método será chamado pelo hook quando um clique ocorrer EM QUALQUER LUGAR
        private void OnGlobalMouseClick(object? sender, Handlers.GlobalMouseHook.Point e)
        {
            // Só faz algo se estivermos no modo de captura
            if (_isCapturingPosition)
            {
                // Imediatamente desativa o modo de captura e o hook para pegar apenas UM clique
                _isCapturingPosition = false;
                _mouseHook.Stop();

                // Grava a posição do clique
                _pokemonBarPosition = new Point(e.X, e.Y);

                // IMPORTANTE: Como o hook roda em outra thread, precisamos usar Invoke
                // para atualizar a interface gráfica de forma segura.
                this.Invoke(() => {
                    lblStatus.Text = $"Posição capturada: X={_pokemonBarPosition.X}, Y={_pokemonBarPosition.Y}";
                    lblStatus.ForeColor = Color.Green;
                    
                    // Salva automaticamente a nova posição
                    SaveCurrentSettings();
                });
            }
        }
        
        private void btnExecuteRevive_Click(object sender, EventArgs e)
        {
            // Toggle do modo automático
            if (!_isAutoModeActive)
            {
                // Ativar modo automático
                if (!StartAutoMode())
                    return;
            }
            else
            {
                // Desativar modo automático
                StopAutoMode();
            }
        }

        /// <summary>
        /// Método chamado quando uma tecla global é pressionada
        /// </summary>
        private void OnGlobalKeyPressed(object? sender, Keys pressedKey)
        {
            // Verifica se é a hotkey configurada e se o modo automático está ativo
            if (_isAutoModeActive && pressedKey == _hotkeyToTrigger)
            {
                // Executa o revive em uma task separada para não bloquear o hook
                Task.Run(() => {
                    try 
                    {
                        this.Invoke(() => ExecuteAutoRevive());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao executar revive automático: {ex.Message}");
                    }
                });
            }
        }

        /// <summary>
        /// Inicia o modo automático de revive
        /// </summary>
        private bool StartAutoMode()
        {
            try
            {
                if (cmbWindows.SelectedItem is not WindowInfo selectedWindow)
                {
                    MessageBox.Show("Por favor, selecione uma janela.");
                    return false;
                }

                // Valida as configurações
                if (!KeyboardHandler.TryParseKey(txtPokemonKey.Text, out var pokemonKey) ||
                    !KeyboardHandler.TryParseKey(txtReviveKey.Text, out var reviveKey))
                {
                    MessageBox.Show($"Tecla inválida. {KeyboardHandler.GetValidKeysExamples()}");
                    return false;
                }

                if (_pokemonBarPosition == Point.Empty)
                {
                    MessageBox.Show("Por favor, defina a posição do Pokémon primeiro.");
                    return false;
                }

                // Usa InputBox simples para pedir a hotkey
                using var inputForm = new Form();
                inputForm.Text = "Configurar Hotkey";
                inputForm.Size = new Size(400, 200);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                
                var label = new Label();
                label.Text = "Digite a tecla que irá disparar o revive automaticamente:\nExemplos: F5, Ctrl+R, Shift+F1, etc.";
                label.Location = new Point(20, 20);
                label.Size = new Size(350, 40);
                
                var textBox = new TextBox();
                textBox.Text = "F5";
                textBox.Location = new Point(20, 70);
                textBox.Size = new Size(350, 20);
                
                var okButton = new Button();
                okButton.Text = "OK";
                okButton.Location = new Point(200, 110);
                okButton.DialogResult = DialogResult.OK;
                
                var cancelButton = new Button();
                cancelButton.Text = "Cancelar";
                cancelButton.Location = new Point(280, 110);
                cancelButton.DialogResult = DialogResult.Cancel;
                
                inputForm.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(textBox.Text))
                    return false;

                string hotkeyInput = textBox.Text;

                if (!KeyboardHandler.TryParseKey(hotkeyInput, out _hotkeyToTrigger))
                {
                    MessageBox.Show($"Tecla inválida: {hotkeyInput}\n{KeyboardHandler.GetValidKeysExamples()}");
                    return false;
                }

                // Configura e inicia o hook para a tecla específica
                _keyboardHook.SetTargetKey(_hotkeyToTrigger);
                _keyboardHook.Start();

                // Ativa o modo automático
                _isAutoModeActive = true;
                
                // Atualiza interface
                btnExecuteRevive.Text = "🔴 PARAR MODO AUTO";
                btnExecuteRevive.BackColor = Color.Red;
                btnExecuteRevive.ForeColor = Color.White;
                lblStatus.Text = $"🟢 MODO AUTO ATIVO - Pressione {hotkeyInput} para usar revive";
                lblStatus.ForeColor = Color.Green;

                // Salva configurações
                SaveCurrentSettings();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar modo automático: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Para o modo automático de revive
        /// </summary>
        private void StopAutoMode()
        {
            _isAutoModeActive = false;
            _hotkeyToTrigger = Keys.None;
            
            // Para o hook global
            _keyboardHook.Stop();

            // Restaura interface
            btnExecuteRevive.Text = "▶️ INICIAR MODO AUTO";
            btnExecuteRevive.BackColor = SystemColors.Control;
            btnExecuteRevive.ForeColor = SystemColors.ControlText;
            lblStatus.Text = "Status: Modo automático desativado";
            lblStatus.ForeColor = Color.Orange;
        }

        /// <summary>
        /// Executa o revive quando a hotkey é pressionada
        /// </summary>
        private void ExecuteAutoRevive()
        {
            try
            {
                if (!_isAutoModeActive)
                    return;

                if (cmbWindows.SelectedItem is not WindowInfo selectedWindow)
                    return;

                // Converte a posição para relativa à janela
                var relativePoint = selectedWindow.ScreenToClient(_pokemonBarPosition);

                // Converte as teclas
                if (!KeyboardHandler.TryParseKey(txtPokemonKey.Text, out var pokemonKey) ||
                    !KeyboardHandler.TryParseKey(txtReviveKey.Text, out var reviveKey))
                    return;

                // Executa o revive
                _reviveHandler.ExecuteRevive(
                    selectedWindow.Handle,
                    pokemonKey,
                    reviveKey,
                    relativePoint
                );

                // Atualiza status temporariamente
                var originalText = lblStatus.Text;
                var originalColor = lblStatus.ForeColor;
                
                lblStatus.Text = "⚡ Revive executado!";
                lblStatus.ForeColor = Color.Blue;

                // Restaura o status original após 2 segundos
                Task.Delay(2000).ContinueWith(_ => {
                    this.Invoke(() => {
                        if (_isAutoModeActive)
                        {
                            lblStatus.Text = originalText;
                            lblStatus.ForeColor = originalColor;
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar revive automático: {ex.Message}");
            }
        }
        
        // Garante que o hook seja desativado e as configurações sejam salvas ao fechar o formulário
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Para o modo automático se estiver ativo
            if (_isAutoModeActive)
                StopAutoMode();
            
            // Salva as configurações finais
            SaveCurrentSettings();
            
            // Dispose dos recursos
            _mouseHook.Dispose();
            _keyboardHook.Dispose();
        }
    }
}
