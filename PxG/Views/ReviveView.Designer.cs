namespace PxG.Views
{
    partial class ReviveView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReviveView));
            // Renomeado para maior clareza e consistência com a lógica refatorada.
            btnToggleAutoMode = new System.Windows.Forms.Button(); 
            groupBoxWindow = new System.Windows.Forms.GroupBox();
            cmbWindows = new System.Windows.Forms.ComboBox();
            btnRefresh = new System.Windows.Forms.Button();
            groupBoxKeys = new System.Windows.Forms.GroupBox();
            lblPokemonKey = new System.Windows.Forms.Label();
            txtPokemonKey = new System.Windows.Forms.TextBox();
            btnCapturePokemonKey = new System.Windows.Forms.Button();
            lblReviveKey = new System.Windows.Forms.Label();
            txtReviveKey = new System.Windows.Forms.TextBox();
            btnCaptureReviveKey = new System.Windows.Forms.Button();
            lblExecuteKey = new System.Windows.Forms.Label();
            txtExecuteKey = new System.Windows.Forms.TextBox();
            btnCaptureExecuteKey = new System.Windows.Forms.Button();
            groupBoxPosition = new System.Windows.Forms.GroupBox();
            btnSetPosition = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();
            groupBoxWindow.SuspendLayout();
            groupBoxKeys.SuspendLayout();
            groupBoxPosition.SuspendLayout();
            SuspendLayout();
            // 
            // btnToggleAutoMode
            // 
            btnToggleAutoMode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)0));
            btnToggleAutoMode.Location = new System.Drawing.Point(14, 427);
            btnToggleAutoMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnToggleAutoMode.Name = "btnToggleAutoMode";
            btnToggleAutoMode.Size = new System.Drawing.Size(398, 46);
            btnToggleAutoMode.TabIndex = 8;
            btnToggleAutoMode.Text = "▶️ INICIAR MODO AUTO"; // O texto será atualizado pela lógica
            btnToggleAutoMode.UseVisualStyleBackColor = true;
            btnToggleAutoMode.Click += btnToggleAutoMode_Click; // Evento agora corresponde ao nome do botão
            // 
            // groupBoxWindow
            // 
            groupBoxWindow.Controls.Add(cmbWindows);
            groupBoxWindow.Controls.Add(btnRefresh);
            groupBoxWindow.Location = new System.Drawing.Point(14, 14);
            groupBoxWindow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxWindow.Name = "groupBoxWindow";
            groupBoxWindow.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxWindow.Size = new System.Drawing.Size(398, 63);
            groupBoxWindow.TabIndex = 9;
            groupBoxWindow.TabStop = false;
            groupBoxWindow.Text = "1. Selecione a Janela do Jogo";
            // 
            // cmbWindows
            // 
            cmbWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbWindows.FormattingEnabled = true;
            cmbWindows.Location = new System.Drawing.Point(7, 25);
            cmbWindows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbWindows.Name = "cmbWindows";
            cmbWindows.Size = new System.Drawing.Size(284, 23);
            cmbWindows.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(299, 24);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(88, 27);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Atualizar";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // groupBoxKeys
            // 
            groupBoxKeys.Controls.Add(lblPokemonKey);
            groupBoxKeys.Controls.Add(txtPokemonKey);
            groupBoxKeys.Controls.Add(btnCapturePokemonKey);
            groupBoxKeys.Controls.Add(lblReviveKey);
            groupBoxKeys.Controls.Add(txtReviveKey);
            groupBoxKeys.Controls.Add(btnCaptureReviveKey);
            groupBoxKeys.Controls.Add(lblExecuteKey);
            groupBoxKeys.Controls.Add(txtExecuteKey);
            groupBoxKeys.Controls.Add(btnCaptureExecuteKey);
            groupBoxKeys.Location = new System.Drawing.Point(14, 84);
            groupBoxKeys.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxKeys.Name = "groupBoxKeys";
            groupBoxKeys.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxKeys.Size = new System.Drawing.Size(398, 196);
            groupBoxKeys.TabIndex = 10;
            groupBoxKeys.TabStop = false;
            groupBoxKeys.Text = "2. Configure as Teclas";
            // 
            // lblPokemonKey
            // 
            lblPokemonKey.AutoSize = true;
            lblPokemonKey.Location = new System.Drawing.Point(7, 29);
            lblPokemonKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPokemonKey.Name = "lblPokemonKey";
            lblPokemonKey.Size = new System.Drawing.Size(108, 15);
            lblPokemonKey.TabIndex = 0;
            lblPokemonKey.Text = "Tecla do Pokémon:";
            // 
            // txtPokemonKey
            // 
            txtPokemonKey.Location = new System.Drawing.Point(10, 47);
            txtPokemonKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtPokemonKey.Name = "txtPokemonKey";
            txtPokemonKey.ReadOnly = true;
            txtPokemonKey.Size = new System.Drawing.Size(174, 23);
            txtPokemonKey.TabIndex = 4;
            // 
            // btnCapturePokemonKey
            // 
            btnCapturePokemonKey.Location = new System.Drawing.Point(192, 45);
            btnCapturePokemonKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCapturePokemonKey.Name = "btnCapturePokemonKey";
            btnCapturePokemonKey.Size = new System.Drawing.Size(194, 27);
            btnCapturePokemonKey.TabIndex = 5;
            btnCapturePokemonKey.Text = "Capturar Tecla Pokémon";
            btnCapturePokemonKey.UseVisualStyleBackColor = true;
            btnCapturePokemonKey.Click += btnCapturePokemonKey_Click;
            // 
            // lblReviveKey
            // 
            lblReviveKey.AutoSize = true;
            lblReviveKey.Location = new System.Drawing.Point(7, 87);
            lblReviveKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblReviveKey.Name = "lblReviveKey";
            lblReviveKey.Size = new System.Drawing.Size(91, 15);
            lblReviveKey.TabIndex = 1;
            lblReviveKey.Text = "Tecla do Revive:";
            // 
            // txtReviveKey
            // 
            txtReviveKey.Location = new System.Drawing.Point(10, 105);
            txtReviveKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtReviveKey.Name = "txtReviveKey";
            txtReviveKey.ReadOnly = true;
            txtReviveKey.Size = new System.Drawing.Size(174, 23);
            txtReviveKey.TabIndex = 6;
            // 
            // btnCaptureReviveKey
            // 
            btnCaptureReviveKey.Location = new System.Drawing.Point(192, 103);
            btnCaptureReviveKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCaptureReviveKey.Name = "btnCaptureReviveKey";
            btnCaptureReviveKey.Size = new System.Drawing.Size(194, 27);
            btnCaptureReviveKey.TabIndex = 7;
            btnCaptureReviveKey.Text = "Capturar Tecla Revive";
            btnCaptureReviveKey.UseVisualStyleBackColor = true;
            btnCaptureReviveKey.Click += btnCaptureReviveKey_Click;
            // 
            // lblExecuteKey
            // 
            lblExecuteKey.AutoSize = true;
            lblExecuteKey.Location = new System.Drawing.Point(7, 144);
            lblExecuteKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblExecuteKey.Name = "lblExecuteKey";
            lblExecuteKey.Size = new System.Drawing.Size(105, 15);
            lblExecuteKey.TabIndex = 2;
            lblExecuteKey.Text = "Tecla de Execução:";
            // 
            // txtExecuteKey
            // 
            txtExecuteKey.Location = new System.Drawing.Point(10, 163);
            txtExecuteKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtExecuteKey.Name = "txtExecuteKey";
            txtExecuteKey.ReadOnly = true;
            txtExecuteKey.Size = new System.Drawing.Size(174, 23);
            txtExecuteKey.TabIndex = 8;
            // 
            // btnCaptureExecuteKey
            // 
            btnCaptureExecuteKey.Location = new System.Drawing.Point(192, 160);
            btnCaptureExecuteKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCaptureExecuteKey.Name = "btnCaptureExecuteKey";
            btnCaptureExecuteKey.Size = new System.Drawing.Size(194, 27);
            btnCaptureExecuteKey.TabIndex = 9;
            btnCaptureExecuteKey.Text = "Capturar Tecla Execução";
            btnCaptureExecuteKey.UseVisualStyleBackColor = true;
            btnCaptureExecuteKey.Click += btnCaptureExecuteKey_Click;
            // 
            // groupBoxPosition
            // 
            groupBoxPosition.Controls.Add(btnSetPosition);
            groupBoxPosition.Location = new System.Drawing.Point(14, 287);
            groupBoxPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxPosition.Name = "groupBoxPosition";
            groupBoxPosition.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxPosition.Size = new System.Drawing.Size(398, 81);
            groupBoxPosition.TabIndex = 11;
            groupBoxPosition.TabStop = false;
            groupBoxPosition.Text = "3. Defina a Posição do Pokémon";
            // 
            // btnSetPosition
            // 
            btnSetPosition.Location = new System.Drawing.Point(10, 29);
            btnSetPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetPosition.Name = "btnSetPosition";
            btnSetPosition.Size = new System.Drawing.Size(376, 35);
            btnSetPosition.TabIndex = 2;
            btnSetPosition.Text = "Definir Posição na Tela (Clique)";
            btnSetPosition.UseVisualStyleBackColor = true;
            btnSetPosition.Click += btnSetPosition_Click;
            // 
            // lblStatus
            // 
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(14, 380);
            lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(398, 35);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status: Ocioso";
            lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ReviveView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(426, 489);
            Controls.Add(groupBoxPosition);
            Controls.Add(groupBoxKeys);
            Controls.Add(groupBoxWindow);
            Controls.Add(btnToggleAutoMode); // Renomeado aqui também
            Controls.Add(lblStatus);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Text = "PxG - Revive Automático";
            FormClosing += ReviveView_FormClosing;
            groupBoxWindow.ResumeLayout(false);
            groupBoxKeys.ResumeLayout(false);
            groupBoxKeys.PerformLayout();
            groupBoxPosition.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // As declarações dos controles
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnToggleAutoMode; // Renomeado
        private System.Windows.Forms.GroupBox groupBoxWindow;
        private System.Windows.Forms.ComboBox cmbWindows;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox groupBoxKeys;
        private System.Windows.Forms.Label lblPokemonKey;
        private System.Windows.Forms.TextBox txtPokemonKey;
        private System.Windows.Forms.Button btnCapturePokemonKey;
        private System.Windows.Forms.Label lblReviveKey;
        private System.Windows.Forms.TextBox txtReviveKey;
        private System.Windows.Forms.Button btnCaptureReviveKey;
        private System.Windows.Forms.Label lblExecuteKey;
        private System.Windows.Forms.TextBox txtExecuteKey;
        private System.Windows.Forms.Button btnCaptureExecuteKey;
        private System.Windows.Forms.GroupBox groupBoxPosition;
        private System.Windows.Forms.Button btnSetPosition;
    }
}