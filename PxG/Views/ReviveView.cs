using PxG.Handlers;
using PxG.Models;
using PxG.Services; 
using System.Diagnostics; // Adicionado para Stopwatch

namespace PxG.Views
{
    public partial class ReviveView : Form
    {
 
        private enum CaptureState { None, Position, PokemonKey, ReviveKey, ExecuteKey }

        private readonly WindowSelector _windowSelector;
        private readonly GlobalMouseHook _captureMouseHook; 
        private readonly AutoReviveService _autoReviveService;

        private AppSettings _settings;
        private CaptureState _currentCaptureState = CaptureState.None;
        private readonly System.Windows.Forms.Timer _captureTimeout;
        
        public ReviveView()
        {
            InitializeComponent();
            _settings = SettingsManager.LoadSettings();
            _windowSelector = new WindowSelector();
            
    
            _autoReviveService = new AutoReviveService(new RevivePokemonHandler());
            _autoReviveService.StatusUpdated += OnServiceStatusUpdated;

            _captureMouseHook = new GlobalMouseHook();
            _captureMouseHook.MouseEvent += OnCaptureMouseEvent;

            _captureTimeout = new System.Windows.Forms.Timer { Interval = 15000 }; // Timeout de 15s
            _captureTimeout.Tick += OnCaptureTimeout;

            this.KeyPreview = true;
            this.KeyDown += OnFormKeyDown;
            LoadSettingsToUi();
        }

        #region UI Loading and Settings
        private void LoadSettingsToUi()
        {
            txtPokemonKey.Text = _settings.PokemonKey;
            txtReviveKey.Text = _settings.ReviveKey;
            txtExecuteKey.Text = _settings.ExecuteKey;
            
            UpdatePositionLabel(new Point(_settings.RevivePositionX, _settings.RevivePositionY));
            RefreshWindowList();

            if (!string.IsNullOrEmpty(_settings.LastSelectedWindow))
            {
                var lastWindow = cmbWindows.Items.OfType<WindowInfo>()
                    .FirstOrDefault(w => w.Title == _settings.LastSelectedWindow);

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
            if (cmbWindows.SelectedItem is WindowInfo selectedWindow)
            {
                _settings.LastSelectedWindow = selectedWindow.Title;
            }
            SettingsManager.SaveSettings(_settings);
        }

        private void UpdatePositionLabel(Point position)
        {
            if (position.IsEmpty)
            {
                lblStatus.Text = "Posição do Revive: Não definida";
                lblStatus.ForeColor = Color.Red;
            }
            else
            {
                _settings.RevivePositionX = position.X;
                _settings.RevivePositionY = position.Y;
                lblStatus.Text = $"Posição Revive: X={position.X}, Y={position.Y}";
                lblStatus.ForeColor = Color.Green;
            }
        }
        #endregion

        #region Capture Logic
        // Método unificado para iniciar a captura
        private void StartCapture(CaptureState stateToCapture)
        {
            CancelCurrentCapture(); // Garante que qualquer captura anterior seja cancelada
            _currentCaptureState = stateToCapture;
            UpdateUiForCaptureState(true);
            _captureTimeout.Start();

            if (stateToCapture == CaptureState.Position || stateToCapture == CaptureState.ExecuteKey)
            {
                _captureMouseHook.Start();
            }

            this.TopMost = true;
            this.Activate();
            this.TopMost = false;
        }

        private void CancelCurrentCapture()
        {
            if (_currentCaptureState == CaptureState.None) return;

            _captureTimeout.Stop();
            _captureMouseHook.Stop();
            UpdateUiForCaptureState(false);
            _currentCaptureState = CaptureState.None;
        }
        
        private void OnCaptureTimeout(object? sender, EventArgs e)
        {
            UpdateStatus("Captura cancelada por tempo esgotado.", Color.Red);
            CancelCurrentCapture();
        }

        private void OnCaptureMouseEvent(object? sender, GlobalMouseEventArgs e)
        {
            // Trata captura de Posição
            if (_currentCaptureState == CaptureState.Position && e.Button == MouseButton.Left && e.Action == MouseAction.Down)
            {
                UpdatePositionLabel(new Point(e.Point.X, e.Point.Y));
                SaveCurrentSettings();
                CancelCurrentCapture();
                return;
            }

            // Trata captura de Gatilho do Mouse
            if (_currentCaptureState == CaptureState.ExecuteKey && e.Action == MouseAction.Down && (e.Button == MouseButton.XButton1 || e.Button == MouseButton.XButton2))
            {
                FinalizeKeyCapture(e.Button.ToString());
            }
        }
        
        private void OnFormKeyDown(object? sender, KeyEventArgs e)
        {
            if (_currentCaptureState == CaptureState.None) return;
            
            e.Handled = true; // Impede que a tecla seja processada por outros controles

            if (e.KeyCode == Keys.Escape)
            {
                UpdateStatus("Captura cancelada pelo usuário.", Color.Orange);
                CancelCurrentCapture();
                return;
            }

            // Ignora teclas modificadoras sozinhas
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu) return;

            FinalizeKeyCapture(GetKeyComboString(e));
        }

        // Finaliza a captura de tecla (seja do teclado ou mouse)
        private void FinalizeKeyCapture(string keyText)
        {
            TextBox? targetTextBox = null;
            switch (_currentCaptureState)
            {
                case CaptureState.PokemonKey: targetTextBox = txtPokemonKey; break;
                case CaptureState.ReviveKey: targetTextBox = txtReviveKey; break;
                case CaptureState.ExecuteKey: targetTextBox = txtExecuteKey; break;
            }
            
            if(targetTextBox != null)
            {
                targetTextBox.Text = keyText;
                UpdateStatus($"✓ Tecla '{_currentCaptureState}' capturada: {keyText}", Color.Green);
                SaveCurrentSettings();
            }
            CancelCurrentCapture();
        }

        // Atualiza a UI para refletir o estado de captura
        private void UpdateUiForCaptureState(bool isCapturing)
        {
            var buttonMap = new Dictionary<CaptureState, (Button btn, string text)>
            {
                { CaptureState.PokemonKey, (btnCapturePokemonKey, "Tecla Pokémon") },
                { CaptureState.ReviveKey, (btnCaptureReviveKey, "Tecla Revive") },
                { CaptureState.ExecuteKey, (btnCaptureExecuteKey, "Tecla Execução") },
                { CaptureState.Position, (btnSetPosition, "Posição Revive") }
            };

            if (buttonMap.TryGetValue(_currentCaptureState, out var map))
            {
                map.btn.Text = isCapturing ? "Capturando..." : $"Capturar {map.text}";
                map.btn.BackColor = isCapturing ? Color.LightBlue : SystemColors.Control;
                map.btn.Enabled = !isCapturing;
                if(isCapturing) UpdateStatus($"Aguardando captura para: {map.text}", Color.Blue);
            }
        }
        #endregion

        #region Event Handlers for UI Controls
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshWindowList();
        private void btnSetPosition_Click(object sender, EventArgs e) => StartCapture(CaptureState.Position);
        private void btnCapturePokemonKey_Click(object sender, EventArgs e) => StartCapture(CaptureState.PokemonKey);
        private void btnCaptureReviveKey_Click(object sender, EventArgs e) => StartCapture(CaptureState.ReviveKey);
        private void btnCaptureExecuteKey_Click(object? sender, EventArgs e) => StartCapture(CaptureState.ExecuteKey);

        private void btnToggleAutoMode_Click(object sender, EventArgs e)
        {
            btnToggleAutoMode.Enabled = false;

            if (_autoReviveService.IsRunning)
            {
                _autoReviveService.Stop();
            }
            else
            {
                if (cmbWindows.SelectedItem is WindowInfo selectedWindow && selectedWindow.Handle != IntPtr.Zero)
                {
                    SaveCurrentSettings();
                    _autoReviveService.Start(_settings, selectedWindow.Handle);
                }
                else
                {
                    MessageBox.Show("Por favor, selecione uma janela válida antes de iniciar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnToggleAutoMode.Enabled = true; // Reabilita se a janela for inválida
                }
            }
        }

        private void OnServiceStatusUpdated(string message, Color color)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            this.Invoke(() => {
                UpdateStatus(message, color);

                btnToggleAutoMode.Text = _autoReviveService.IsRunning ? "🔴 PARAR MODO AUTO" : "▶️ INICIAR MODO AUTO";
                btnToggleAutoMode.BackColor = _autoReviveService.IsRunning ? Color.Tomato : SystemColors.Control;
                btnToggleAutoMode.Enabled = true; // Reabilita o botão após a conclusão da operação
            });
        }
        #endregion

        #region Utility Methods
        private void RefreshWindowList()
        {
            // 1. Salva o handle da janela que está selecionada no momento.
            IntPtr? selectedHandle = null;
            if (cmbWindows.SelectedItem is WindowInfo currentSelection)
            {
                selectedHandle = currentSelection.Handle;
            }

            // 2. Atualiza a lista de janelas a partir do sistema operacional.
            _windowSelector.RefreshWindowsList();

            // 3. Redefine a fonte de dados do ComboBox para a lista atualizada.
            cmbWindows.DataSource = new BindingSource { DataSource = _windowSelector.OpenWindows };
            cmbWindows.DisplayMember = "Title";

            // 4. Se havia uma janela selecionada, tenta encontrá-la na nova lista e selecioná-la novamente.
            if (selectedHandle.HasValue)
            {
                var selectionToRestore = _windowSelector.OpenWindows.FirstOrDefault(w => w.Handle == selectedHandle.Value);
        
                // Como WindowInfo é uma struct, FirstOrDefault retorna uma struct padrão se não encontrar.
                // Verificamos se o handle não é zero para confirmar que encontramos uma janela válida.
                if (selectionToRestore.Handle != IntPtr.Zero)
                {
                    cmbWindows.SelectedItem = selectionToRestore;
                }
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            lblStatus.Text = $"Status: {message}";
            lblStatus.ForeColor = color;
        }

        private string GetKeyComboString(KeyEventArgs e)
        {
            // (Método mantido como estava, pois já era bom)
            var modifiers = new List<string>();
            if (e.Control) modifiers.Add("Ctrl");
            if (e.Alt) modifiers.Add("Alt");
            if (e.Shift) modifiers.Add("Shift");
            Keys mainKey = e.KeyCode;
            return modifiers.Count > 0 ? $"{string.Join("+", modifiers)}+{mainKey}" : mainKey.ToString();
        }

        private void ReviveView_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentSettings();
            _autoReviveService.Dispose();
            _captureMouseHook.Dispose();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the pokemonBarPosition from settings
            Point pokemonBarPosition = new Point(_settings.RevivePositionX, _settings.RevivePositionY);

            // Start stopwatch
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Check if the pokemon is fainted
            bool isFainted = ScreenAnalyzer.FindFaintedIcon(pokemonBarPosition, 0.75);

            // Stop stopwatch
            stopwatch.Stop();

            // Log the result and elapsed time
            Console.WriteLine($"Pokémon está desmaiado: {isFainted}. Tempo de verificação: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
    
}