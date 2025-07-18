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
        private readonly MedicinePokemonHandler _medicineHandler;
        private readonly GlobalMouseHook _mouseHook;
        private readonly GlobalKeyboardHook _keyboardHook;

        // Variáveis de estado
        private Point _revivePosition;
        private Point _medicinePosition;
        
        private bool _isCapturingRevivePosition;
        private bool _isCapturingMedicinePosition;

        private bool _isCapturingPokemonKey;
        private bool _isCapturingReviveKey;
        private bool _isCapturingMedicineKey;

        // Timer para timeout da captura de teclas
        private readonly System.Windows.Forms.Timer _captureTimeout;
        
        // Configurações da aplicação
        private AppSettings _settings;
        
        // Controle do modo automático
        private bool _isAutoModeActive = false;
        private readonly Dictionary<Keys, CancellationTokenSource> _activeTasks = new();

        public Main()
        {
            InitializeComponent();
            
            // Carrega as configurações salvas
            _settings = SettingsManager.LoadSettings();
            
            _windowSelector = new WindowSelector();
            _reviveHandler = new RevivePokemonHandler();
            _medicineHandler = new MedicinePokemonHandler();
            
            // Inicializa o hook e se inscreve no evento de clique
            _mouseHook = new GlobalMouseHook();
            _mouseHook.MouseClicked += OnGlobalMouseClick;
            
            // Inicializa o hook global de teclado
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyDown += OnGlobalKeyDown;
            _keyboardHook.KeyUp += OnGlobalKeyUp;
            
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
            txtMedicineKey.Text = _settings.MedicineKey;
            
            // Carrega a posição salva para o revive
            if (_settings.RevivePositionX > 0 && _settings.RevivePositionY > 0)
            {
                _revivePosition = new Point(_settings.RevivePositionX, _settings.RevivePositionY);
                lblStatus.Text = $"Posição Revive: X={_revivePosition.X}, Y={_revivePosition.Y}";
                lblStatus.ForeColor = Color.Green;
            }
            
            // Carrega a posição salva para a medicina
            if (_settings.MedicinePositionX > 0 && _settings.MedicinePositionY > 0)
            {
                _medicinePosition = new Point(_settings.MedicinePositionX, _settings.MedicinePositionY);
                lblMedicineStatus.Text = $"Posição Medicina: X={_medicinePosition.X}, Y={_medicinePosition.Y}";
                lblMedicineStatus.ForeColor = Color.Green;
            }
            
            // Atualiza a lista de janelas e tenta selecionar a última janela usada
            _windowSelector.RefreshWindowsList();
            cmbWindows.DataSource = _windowSelector.OpenWindows;
            
            if (!string.IsNullOrEmpty(_settings.LastSelectedWindow))
            {
                var lastWindow = _windowSelector.OpenWindows.Where(w => w.Title == _settings.LastSelectedWindow).FirstOrDefault();
                // Verifica se encontrou alguma janela comparando o Handle
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
            _settings.MedicineKey = txtMedicineKey.Text;
            
            _settings.RevivePositionX = _revivePosition.X;
            _settings.RevivePositionY = _revivePosition.Y;
            
            _settings.MedicinePositionX = _medicinePosition.X;
            _settings.MedicinePositionY = _medicinePosition.Y;
            
            if (cmbWindows.SelectedItem is WindowInfo selectedWindow)
            {
                _settings.LastSelectedWindow = selectedWindow.Title;
            }
            
            if (SettingsManager.SaveSettings(_settings))
            {
                // Feedback sutil de que as configura��ões foram salvas (opcional)
                // Console.WriteLine("Configurações salvas com sucesso!");
            }
        }

        private void OnCaptureTimeout(object? sender, EventArgs e)
        {
            _captureTimeout.Stop();
            CancelAllCaptures();
            lblStatus.Text = "Status: Captura de Revive cancelada por timeout";
            lblStatus.ForeColor = Color.Red;
            lblMedicineStatus.Text = "Status: Captura de Medicina cancelada por timeout";
            lblMedicineStatus.ForeColor = Color.Red;
        }

        private void OnFormActivated(object? sender, EventArgs e)
        {
            // Quando o formulário é ativado durante captura, garante que tem foco
            if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingMedicineKey)
            {
                this.Focus();
            }
        }

        private void OnFormLostFocus(object? sender, EventArgs e)
        {
            // Se perdeu o foco durante captura, tenta recuperar depois de um delay
            if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingMedicineKey)
            {
                Task.Delay(100).ContinueWith(_ => {
                    this.Invoke(() => {
                        if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingMedicineKey)
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
            // Inicia o modo de captura para o Revive
            _isCapturingRevivePosition = true;
            _isCapturingMedicinePosition = false; // Garante que o outro modo está desativado
            lblStatus.Text = "Status: Clique para gravar a posição do Revive...";
            lblStatus.ForeColor = Color.Blue;
            
            // Ativa o hook para observar o próximo clique
            _mouseHook.Start();
        }

        private void btnSetMedicinePosition_Click(object sender, EventArgs e)
        {
            // Inicia o modo de captura para a Medicina
            _isCapturingMedicinePosition = true;
            _isCapturingRevivePosition = false; // Garante que o outro modo está desativado
            lblMedicineStatus.Text = "Status: Clique para gravar a posição da Medicina...";
            lblMedicineStatus.ForeColor = Color.Blue;
            
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

        private void btnCaptureMedicineKey_Click(object sender, EventArgs e)
        {
            // Cancela qualquer captura em andamento
            CancelAllCaptures();
            
            _isCapturingMedicineKey = true;
            btnCaptureMedicineKey.Text = "Pressione uma tecla...";
            btnCaptureMedicineKey.BackColor = Color.LightBlue;
            btnCaptureMedicineKey.Enabled = false;
            lblMedicineStatus.Text = "Status: Pressione a tecla para a Medicina (ESC para cancelar)";
            lblMedicineStatus.ForeColor = Color.Blue;
            
            // Inicia o timeout
            _captureTimeout.Start();
            
            // Força o foco no formulário
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

            if (_isCapturingMedicineKey)
            {
                _isCapturingMedicineKey = false;
                btnCaptureMedicineKey.Text = "Capturar Tecla";
                btnCaptureMedicineKey.BackColor = SystemColors.Control;
                btnCaptureMedicineKey.Enabled = true;
            }
        }

        private void OnFormKeyDown(object? sender, KeyEventArgs e)
        {
            // Se ESC for pressionado durante a captura, cancela
            if (e.KeyCode == Keys.Escape && (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingMedicineKey))
            {
                CancelAllCaptures();
                lblStatus.Text = "Status: Captura cancelada";
                lblStatus.ForeColor = Color.Orange;
                lblMedicineStatus.Text = "Status: Captura cancelada";
                lblMedicineStatus.ForeColor = Color.Orange;
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
            else if (_isCapturingMedicineKey)
            {
                _captureTimeout.Stop();
                _isCapturingMedicineKey = false;
                
                string keyCombo = GetKeyComboString(e);
                txtMedicineKey.Text = keyCombo;
                
                btnCaptureMedicineKey.Text = "Capturar Tecla";
                btnCaptureMedicineKey.BackColor = SystemColors.Control;
                btnCaptureMedicineKey.Enabled = true;
                lblMedicineStatus.Text = $"✓ Tecla Medicina capturada: {keyCombo}";
                lblMedicineStatus.ForeColor = Color.Green;
                e.Handled = true;
                
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
            // Verifica qual posição está sendo capturada
            if (_isCapturingRevivePosition)
            {
                _isCapturingRevivePosition = false;
                _mouseHook.Stop();

                _revivePosition = new Point(e.X, e.Y);

                this.Invoke(() => {
                    lblStatus.Text = $"Posição Revive: X={_revivePosition.X}, Y={_revivePosition.Y}";
                    lblStatus.ForeColor = Color.Green;
                    SaveCurrentSettings();
                });
            }
            else if (_isCapturingMedicinePosition)
            {
                _isCapturingMedicinePosition = false;
                _mouseHook.Stop();

                _medicinePosition = new Point(e.X, e.Y);

                this.Invoke(() => {
                    lblMedicineStatus.Text = $"Posição Medicina: X={_medicinePosition.X}, Y={_medicinePosition.Y}";
                    lblMedicineStatus.ForeColor = Color.Green;
                    SaveCurrentSettings();
                });
            }
        }
        
        private void btnExecuteRevive_Click(object sender, EventArgs e)
        {
            // Toggle do modo automático para Revive
            ToggleAutoMode(btnExecuteRevive, txtReviveKey, lblStatus, "Revive");
        }

        private void btnExecuteMedicine_Click(object sender, EventArgs e)
        {
            // Toggle do modo automático para Medicine
            ToggleAutoMode(btnExecuteMedicine, txtMedicineKey, lblMedicineStatus, "Medicine");
        }

        private void ToggleAutoMode(Button button, TextBox keyTextBox, Label statusLabel, string modeName)
        {
            _isAutoModeActive = _activeTasks.Count > 0;

            if (!_isAutoModeActive)
            {
                if (StartAutoMode(keyTextBox, statusLabel, modeName))
                {
                    button.Text = $"🔴 PARAR MODO AUTO ({modeName})";
                    button.BackColor = Color.Red;
                    button.ForeColor = Color.White;
                }
            }
            else
            {
                StopAutoMode(keyTextBox, statusLabel, modeName);
                button.Text = $"▶️ INICIAR MODO AUTO ({modeName})";
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
            }
        }

        private void OnGlobalKeyDown(object? sender, Keys pressedKey)
        {
            if (!_isAutoModeActive || _activeTasks.ContainsKey(pressedKey)) return;

            if (cmbWindows.SelectedItem is not WindowInfo selectedWindow) return;

            CancellationTokenSource cts = new CancellationTokenSource();
            _activeTasks[pressedKey] = cts;

            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (KeyboardHandler.TryParseKey(txtReviveKey.Text, out var reviveKey) && pressedKey == reviveKey)
                    {
                        ExecuteAutoRevive(selectedWindow);
                    }
                    else if (KeyboardHandler.TryParseKey(txtMedicineKey.Text, out var medicineKey) && pressedKey == medicineKey)
                    {
                        ExecuteAutoMedicine(selectedWindow);
                    }
                    await Task.Delay(500, cts.Token); // Delay entre execuções
                }
            }, cts.Token);
        }

        private void OnGlobalKeyUp(object? sender, Keys releasedKey)
        {
            if (_activeTasks.TryGetValue(releasedKey, out var cts))
            {
                cts.Cancel();
                _activeTasks.Remove(releasedKey);
            }
        }

        private bool StartAutoMode(TextBox keyTextBox, Label statusLabel, string modeName)
        {
            if (!KeyboardHandler.TryParseKey(keyTextBox.Text, out var hotkey))
            {
                MessageBox.Show($"A tecla de atalho para '{modeName}' é inválida.", "Erro de Configuração");
                return false;
            }

            // Verifica se uma janela foi selecionada
            if (cmbWindows.SelectedItem is not WindowInfo selectedWindow)
            {
                MessageBox.Show("Por favor, selecione uma janela do jogo antes de ativar o modo automático.", "Janela não selecionada");
                return false;
            }

            // Configura a janela alvo no hook de teclado
            _keyboardHook.SetTargetWindow(selectedWindow.Handle);
            _keyboardHook.AddTargetKey(hotkey);
            _isAutoModeActive = true;
            
            statusLabel.Text = $"🟢 MODO AUTO ATIVO - Pressione {keyTextBox.Text} (apenas na janela do jogo)";
            statusLabel.ForeColor = Color.Green;
            
            return true;
        }

        private void StopAutoMode(TextBox keyTextBox, Label statusLabel, string modeName)
        {
            if (KeyboardHandler.TryParseKey(keyTextBox.Text, out var hotkey))
            {
                _keyboardHook.RemoveTargetKey(hotkey);
                if (_activeTasks.TryGetValue(hotkey, out var cts))
                {
                    cts.Cancel();
                    _activeTasks.Remove(hotkey);
                }
            }

            if (_keyboardHook.TargetKeyCount == 0)
            {
                _isAutoModeActive = false;
                // Remove a restrição de janela quando não há mais teclas ativas
                _keyboardHook.ClearTargetWindow();
            }

            statusLabel.Text = $"Status: Modo automático ({modeName}) desativado";
            statusLabel.ForeColor = Color.Orange;
        }

        // Adicione a palavra-chave 'async' na assinatura do método
        private async void ExecuteAutoRevive(WindowInfo selectedWindow)
        {
            if (!KeyboardHandler.TryParseKey(txtPokemonKey.Text, out var pokemonKey) ||
                !KeyboardHandler.TryParseKey(txtReviveKey.Text, out var reviveKey))
                return;

            var relativePoint = selectedWindow.ScreenToClient(_revivePosition);

            // Adicione a palavra-chave 'await' aqui na chamada
            await _reviveHandler.ExecuteSmartRevive(selectedWindow.Handle, pokemonKey, reviveKey, relativePoint);

            // Agora, este código só será executado DEPOIS que o revive terminar completamente.
            this.Invoke(() => {
                lblStatus.Text = "⚡ Revive executado!";
                lblStatus.ForeColor = Color.Blue;
                Task.Delay(1000).ContinueWith(_ => this.Invoke(() => {
                    if (_isAutoModeActive)
                    {
                        lblStatus.Text = $"🟢 MODO AUTO ATIVO - Pressione {txtReviveKey.Text}";
                        lblStatus.ForeColor = Color.Green;
                    }
                }));
            });
        }

        private void ExecuteAutoMedicine(WindowInfo selectedWindow)
        {
            if (!KeyboardHandler.TryParseKey(txtMedicineKey.Text, out var medicineKey)) return;

            var relativePoint = selectedWindow.ScreenToClient(_medicinePosition);
            _medicineHandler.ExecuteMedicine(selectedWindow.Handle, medicineKey, relativePoint);
            
            this.Invoke(() => {
                lblMedicineStatus.Text = "💊 Medicine usada!";
                lblMedicineStatus.ForeColor = Color.Blue;
                Task.Delay(1000).ContinueWith(_ => this.Invoke(() => {
                    if (_isAutoModeActive)
                    {
                        lblMedicineStatus.Text = $"🟢 MODO AUTO ATIVO - Pressione {txtMedicineKey.Text}";
                        lblMedicineStatus.ForeColor = Color.Green;
                    }
                }));
            });
        }
        
        // Garante que o hook seja desativado e as configurações sejam salvas ao fechar o formulário
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Para todos os modos automáticos
            _keyboardHook.ClearTargetKeys();
            _keyboardHook.ClearTargetWindow();
            foreach (var cts in _activeTasks.Values)
            {
                cts.Cancel();
            }
            _activeTasks.Clear();
            _isAutoModeActive = false;
            
            // Salva as configurações finais
            SaveCurrentSettings();
            
            // Dispose dos recursos
            _mouseHook.Dispose();
            _keyboardHook.Dispose();
        }

        private void lblMedicineStatus_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                // Load saved settings and apply them to the UI
                LoadSettingsToUI();

                // Refresh the list of open windows
                _windowSelector.RefreshWindowsList();
                cmbWindows.DataSource = _windowSelector.OpenWindows;

                // Optionally, set the default selected window if available
                if (!string.IsNullOrEmpty(_settings.LastSelectedWindow))
                {
                    var lastWindow = _windowSelector.OpenWindows
                        .FirstOrDefault(w => w.Title == _settings.LastSelectedWindow);
                    // Check if a valid window was found by comparing Handle
                    if (lastWindow.Handle != IntPtr.Zero)
                    {
                        cmbWindows.SelectedItem = lastWindow;
                    }
                }

                // Update status labels or other UI elements as needed
                lblStatus.Text = "Application loaded successfully.";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                // Handle any errors during initialization
                MessageBox.Show($"An error occurred during initialization: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
