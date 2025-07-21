using PxG.Handlers;
using PxG.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PxG.Views
{
    public partial class ReviveView : Form
    {
        private readonly WindowSelector _windowSelector;
        private readonly RevivePokemonHandler _reviveHandler;
        private readonly GlobalMouseHook _mouseHook;
        private readonly GlobalKeyboardHook _keyboardHook;

        private Point _revivePosition;
        private bool _isCapturingRevivePosition;
        private bool _isCapturingPokemonKey;
        private bool _isCapturingReviveKey;
        private bool _isCapturingExecuteKey;

        private readonly System.Windows.Forms.Timer _captureTimeout;
        private AppSettings _settings;
        private bool _isAutoModeActive = false;
        private readonly Dictionary<Keys, CancellationTokenSource> _activeTasks = new();
        private readonly Dictionary<MouseButton, CancellationTokenSource> _activeMouseTasks = new();

        public ReviveView()
        {
            InitializeComponent();
            _settings = SettingsManager.LoadSettings();
            _windowSelector = new WindowSelector();
            _reviveHandler = new RevivePokemonHandler();
            _mouseHook = new GlobalMouseHook();
            _mouseHook.MouseEvent += OnGlobalMouseEvent; 
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyDown += OnGlobalKeyDown;
            _keyboardHook.KeyUp += OnGlobalKeyUp;
            _captureTimeout = new System.Windows.Forms.Timer();
            _captureTimeout.Interval = 30000;
            _captureTimeout.Tick += OnCaptureTimeout;
            this.KeyPreview = true;
            this.KeyDown += OnFormKeyDown;
            this.Activated += OnFormActivated;
            this.LostFocus += OnFormLostFocus;
            LoadSettingsToUI();
        }

        private void LoadSettingsToUI()
        {
            txtPokemonKey.Text = _settings.PokemonKey;
            txtReviveKey.Text = _settings.ReviveKey;
            txtExecuteKey.Text = _settings.ExecuteKey;
            if (_settings.RevivePositionX > 0 && _settings.RevivePositionY > 0)
            {
                _revivePosition = new Point(_settings.RevivePositionX, _settings.RevivePositionY);
                lblStatus.Text = $"Posição Revive: X={_revivePosition.X}, Y={_revivePosition.Y}";
                lblStatus.ForeColor = Color.Green;
            }
            _windowSelector.RefreshWindowsList();
            cmbWindows.DataSource = _windowSelector.OpenWindows;
            if (!string.IsNullOrEmpty(_settings.LastSelectedWindow))
            {
                var lastWindow = _windowSelector.OpenWindows.Where(w => w.Title == _settings.LastSelectedWindow).FirstOrDefault();
                if (lastWindow.Handle != IntPtr.Zero)
                {
                    cmbWindows.SelectedItem = lastWindow;
                }
            }
        }

        private void SaveCurrentSettings()
        {
            _settings.PokemonKey = txtPokemonKey.Text;
            _settings.ReviveKey = txtReviveKey.Text;
            _settings.ExecuteKey = txtExecuteKey.Text;
            _settings.RevivePositionX = _revivePosition.X;
            _settings.RevivePositionY = _revivePosition.Y;
            if (cmbWindows.SelectedItem is WindowInfo selectedWindow)
            {
                _settings.LastSelectedWindow = selectedWindow.Title;
            }
            SettingsManager.SaveSettings(_settings);
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
            if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingExecuteKey)
            {
                this.Focus();
            }
        }

        private void OnFormLostFocus(object? sender, EventArgs e)
        {
            if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingExecuteKey)
            {
                Task.Delay(100).ContinueWith(_ => {
                    this.Invoke(() => {
                        if (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingExecuteKey)
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
            _isCapturingRevivePosition = true;
            lblStatus.Text = "Status: Clique para gravar a posição do Pokémon...";
            lblStatus.ForeColor = Color.Blue;
            _mouseHook.Start(); // Inicia o hook para capturar a posição
        }

        private void btnCapturePokemonKey_Click(object sender, EventArgs e)
        {
            CancelAllCaptures();
            _isCapturingPokemonKey = true;
            btnCapturePokemonKey.Text = "Pressione uma tecla...";
            btnCapturePokemonKey.BackColor = Color.LightBlue;
            btnCapturePokemonKey.Enabled = false;
            lblStatus.Text = "Status: Pressione a tecla para o Pokémon (ESC para cancelar)";
            lblStatus.ForeColor = Color.Blue;
            _captureTimeout.Start();
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        private void btnCaptureReviveKey_Click(object sender, EventArgs e)
        {
            CancelAllCaptures();
            _isCapturingReviveKey = true;
            btnCaptureReviveKey.Text = "Pressione uma tecla...";
            btnCaptureReviveKey.BackColor = Color.LightBlue;
            btnCaptureReviveKey.Enabled = false;
            lblStatus.Text = "Status: Pressione a tecla para o Revive (ESC para cancelar)";
            lblStatus.ForeColor = Color.Blue;
            _captureTimeout.Start();
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        private void btnCaptureExecuteKey_Click(object? sender, EventArgs e)
        {
            CancelAllCaptures();
            _isCapturingExecuteKey = true;
    

            _mouseHook.Start(); 

            btnCaptureExecuteKey.Text = "Pressione uma tecla...";
            btnCaptureExecuteKey.BackColor = Color.LightBlue;
            btnCaptureExecuteKey.Enabled = false;
            lblStatus.Text = "Status: Pressione a tecla de execução (ESC para cancelar)";
            lblStatus.ForeColor = Color.Blue;
            _captureTimeout.Start();
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        private void CancelAllCaptures()
        {
            _captureTimeout.Stop();
            if (_isCapturingPokemonKey)
            {
                _isCapturingPokemonKey = false;
                btnCapturePokemonKey.Text = "Capturar Tecla Pokémon";
                btnCapturePokemonKey.BackColor = SystemColors.Control;
                btnCapturePokemonKey.Enabled = true;
            }
            if (_isCapturingReviveKey)
            {
                _isCapturingReviveKey = false;
                btnCaptureReviveKey.Text = "Capturar Tecla Revive";
                btnCaptureReviveKey.BackColor = SystemColors.Control;
                btnCaptureReviveKey.Enabled = true;
            }
            if (_isCapturingExecuteKey)
            {
                _isCapturingExecuteKey = false;
                btnCaptureExecuteKey.Text = "Capturar Tecla Execução";
                btnCaptureExecuteKey.BackColor = SystemColors.Control;
                btnCaptureExecuteKey.Enabled = true;
            }
        }

        private void OnGlobalMouseEvent(object? sender, GlobalMouseEventArgs e)
        {
            // Lógica para capturar a posição (mantida)
            if (_isCapturingRevivePosition && e.Button == MouseButton.Left && e.Action == MouseAction.Down)
            {
                _isCapturingRevivePosition = false;
                _mouseHook.Stop(); // Pare o hook após capturar a posição
                _revivePosition = new Point(e.Point.X, e.Point.Y);
                this.Invoke(() => {
                    lblStatus.Text = $"Posição do Pokémon: X={_revivePosition.X}, Y={_revivePosition.Y}";
                    lblStatus.ForeColor = Color.Green;
                    SaveCurrentSettings();
                });
                return; // Impede que o clique de captura de posição acione outras lógicas
            }

            // Lógica para capturar a tecla de gatilho
            if (_isCapturingExecuteKey && e.Action == MouseAction.Down && (e.Button == MouseButton.XButton1 || e.Button == MouseButton.XButton2))
            {
                HandleTriggerCapture(e.Button.ToString());
                return;
            }
            
            // Lógica para INICIAR/PARAR a ação com o mouse
            if (_isAutoModeActive)
            {
                if (e.Action == MouseAction.Down)
                    OnGlobalMouseDown(e.Button);
                else if (e.Action == MouseAction.Up)
                    OnGlobalMouseUp(e.Button);
            }
        }

        // MÉTODO NOVO: Lógica para quando um botão do mouse é pressionado
        private void OnGlobalMouseDown(MouseButton pressedButton)
        {
            if (cmbWindows.SelectedItem is not WindowInfo selectedWindow || _activeMouseTasks.ContainsKey(pressedButton)) return;

            // Verifique se o botão pressionado é o gatilho configurado
            if (txtExecuteKey.Text.Equals(pressedButton.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                _activeMouseTasks[pressedButton] = cts;

                Task.Run(async () =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        // Aqui você pode diferenciar as ações se necessário,
                        // mas para o "Revive", a lógica é a mesma.
                        await ExecuteAutoRevive(selectedWindow);
                        await Task.Delay(500, cts.Token); // Pequeno delay
                    }
                }, cts.Token);
            }
        }

        // MÉTODO NOVO: Lógica para quando um botão do mouse é solto
        private void OnGlobalMouseUp(MouseButton releasedButton)
        {
            if (_activeMouseTasks.TryGetValue(releasedButton, out var cts))
            {
                cts.Cancel();
                _activeMouseTasks.Remove(releasedButton);
            }
        }

        // MÉTODO NOVO: Refatorado para lidar com captura de mouse e teclado
        private void HandleTriggerCapture(string keyName)
        {
            _captureTimeout.Stop();
            if (_isCapturingExecuteKey)
            {
                _isCapturingExecuteKey = false;
                txtExecuteKey.Text = keyName;
                btnCaptureExecuteKey.Text = @"Capturar Tecla Execução";
                btnCaptureExecuteKey.BackColor = SystemColors.Control;
                btnCaptureExecuteKey.Enabled = true;
                lblStatus.Text = @"✓ Tecla Execução capturada: {keyName}";
                lblStatus.ForeColor = Color.Green;
                SaveCurrentSettings();
            }
            // Adicione lógica similar para os outros botões de captura se eles também
            // precisarem aceitar teclas do mouse.
        }

        private void OnFormKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && (_isCapturingPokemonKey || _isCapturingReviveKey || _isCapturingExecuteKey))
            {
                CancelAllCaptures();
                lblStatus.Text = @"Status: Captura cancelada";
                lblStatus.ForeColor = Color.Orange;
                e.Handled = true;
                return;
            }
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
                string keyCombo = GetKeyComboString(e);
                txtPokemonKey.Text = keyCombo;
                btnCapturePokemonKey.Text = @"Capturar Tecla Pokémon";
                btnCapturePokemonKey.BackColor = SystemColors.Control;
                btnCapturePokemonKey.Enabled = true;
                lblStatus.Text = @"✓ Tecla Pokémon capturada: {keyCombo}";
                lblStatus.ForeColor = Color.Green;
                e.Handled = true;
                SaveCurrentSettings();
            }
            else if (_isCapturingReviveKey)
            {
                _captureTimeout.Stop();
                _isCapturingReviveKey = false;
                string keyCombo = GetKeyComboString(e);
                txtReviveKey.Text = keyCombo;
                btnCaptureReviveKey.Text = @"Capturar Tecla Revive";
                btnCaptureReviveKey.BackColor = SystemColors.Control;
                btnCaptureReviveKey.Enabled = true;
                lblStatus.Text = @"✓ Tecla Revive capturada: {keyCombo}";
                lblStatus.ForeColor = Color.Green;
                e.Handled = true;
                SaveCurrentSettings();
            }
            else if (_isCapturingExecuteKey)
            {
                HandleTriggerCapture(GetKeyComboString(e));
                e.Handled = true;
            }
        }

        private string GetKeyComboString(KeyEventArgs e)
        {
            var modifiers = new List<string>();
            if (e.Control)
                modifiers.Add("Ctrl");
            if (e.Alt)
                modifiers.Add("Alt");
            if (e.Shift)
                modifiers.Add("Shift");
            Keys mainKey = e.KeyCode;
            if (modifiers.Count > 0)
                return string.Join("+", modifiers) + "+" + mainKey;
            else
                return mainKey.ToString();
        }

        private void btnExecuteRevive_Click(object sender, EventArgs e)
        {
            ToggleAutoMode(btnExecuteRevive, txtReviveKey, lblStatus, "Revive");
        }

        private void ToggleAutoMode(Button button, TextBox keyTextBox, Label statusLabel, string modeName)
        {
            _isAutoModeActive = _activeTasks.Count > 0;
            if (!_isAutoModeActive)
            {
                if (StartAutoMode(keyTextBox, statusLabel, modeName))
                {
                    button.Text = @"🔴 PARAR MODO AUTO ({modeName})";
                    button.BackColor = Color.Red;
                    button.ForeColor = Color.White;
                }
            }
            else
            {
                StopAutoMode(keyTextBox, statusLabel, modeName);
                button.Text = @"▶️ INICIAR MODO AUTO ({modeName})";
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
            }
        }

        private void OnGlobalKeyDown(object? sender, Keys pressedKey)
        {
            if (!_isAutoModeActive || _activeTasks.ContainsKey(pressedKey)) return;
            if (cmbWindows.SelectedItem is not WindowInfo selectedWindow) return;

            // CORREÇÃO: Verificar se a tecla pressionada é a tecla de EXECUÇÃO
            if (txtExecuteKey.Text.Equals(pressedKey.ToString(), StringComparison.OrdinalIgnoreCase) ||
                (KeyboardHandler.TryParseKey(txtExecuteKey.Text, out var executeKey) && pressedKey == executeKey))
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                _activeTasks[pressedKey] = cts;

                Task.Run(async () =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        await ExecuteAutoRevive(selectedWindow);
                        await Task.Delay(500, cts.Token); // Pequeno delay para evitar sobrecarga
                    }
                }, cts.Token);
            }
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
                MessageBox.Show(@"A tecla de atalho para '{modeName}' é inválida.", @"Erro de Configuração");
                return false;
            }
            if (cmbWindows.SelectedItem is not WindowInfo selectedWindow)
            {
                MessageBox.Show(@"Por favor, selecione uma janela do jogo antes de ativar o modo automático.", @"Janela não selecionada");
                return false;
            }
            _keyboardHook.SetTargetWindow(selectedWindow.Handle);
            _keyboardHook.AddTargetKey(hotkey);
            _mouseHook.Start(); // ATIVA O HOOK DO MOUSE
            _isAutoModeActive = true;
            statusLabel.Text = @"🟢 MODO AUTO ATIVO - Pressione {keyTextBox.Text} (apenas na janela do jogo)";
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
            
            // Cancela tarefas ativas do mouse
            foreach (var cts in _activeMouseTasks.Values)
            {
                cts.Cancel();
            }
            _activeMouseTasks.Clear();

            // Para o hook do mouse se não houver mais nenhuma automação ativa
            if (_keyboardHook.TargetKeyCount == 0)
            {
                _isAutoModeActive = false;
                _keyboardHook.ClearTargetWindow();
                _mouseHook.Stop(); // DESATIVA O HOOK DO MOUSE
            }
            statusLabel.Text = @"Status: Modo automático ({modeName}) desativado";
            statusLabel.ForeColor = Color.Orange;
        }

        private async Task ExecuteAutoRevive(WindowInfo selectedWindow)
        {
            if (!KeyboardHandler.TryParseKey(txtPokemonKey.Text, out var pokemonKey) ||
                !KeyboardHandler.TryParseKey(txtReviveKey.Text, out var reviveKey))
                return;

            var relativePoint = selectedWindow.ScreenToClient(_revivePosition);

            await _reviveHandler.ExecuteFastRevive(selectedWindow.Handle, pokemonKey, reviveKey, relativePoint);

            if (IsDisposed || !IsHandleCreated) return;
            Invoke((Action)(async () =>
            {
                lblStatus.Text = @"⚡ Revive executado!";
                lblStatus.ForeColor = Color.Blue;
                await Task.Delay(1000);
                if (_isAutoModeActive)
                {
                    lblStatus.Text = @"🟢 MODO AUTO ATIVO";
                    lblStatus.ForeColor = Color.Green;
                }
            }));
        }

        private void ReviveView_FormClosing(object sender, FormClosingEventArgs e)
        {
            _keyboardHook.ClearTargetKeys();
            _keyboardHook.ClearTargetWindow();
            foreach (var cts in _activeTasks.Values)
            {
                cts.Cancel();
            }
            _activeTasks.Clear();
            foreach (var cts in _activeMouseTasks.Values)
            {
                cts.Cancel();
            }
            _activeMouseTasks.Clear();
            _isAutoModeActive = false;
            SaveCurrentSettings();
            _mouseHook.Dispose(); // Garante a liberação correta
            _keyboardHook.Dispose();
        }
    }
}

