namespace PxG.Views
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            cmbWindows = new System.Windows.Forms.ComboBox();
            btnRefresh = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtPokemonKey = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtReviveKey = new System.Windows.Forms.TextBox();
            btnSetPosition = new System.Windows.Forms.Button();
            btnExecuteRevive = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();
            btnCapturePokemonKey = new System.Windows.Forms.Button();
            btnCaptureReviveKey = new System.Windows.Forms.Button();
            grpRevive = new System.Windows.Forms.GroupBox();
            grpMedicine = new System.Windows.Forms.GroupBox();
            lblMedicineKey = new System.Windows.Forms.Label();
            btnCaptureMedicineKey = new System.Windows.Forms.Button();
            txtMedicineKey = new System.Windows.Forms.TextBox();
            lblMedicineStatus = new System.Windows.Forms.Label();
            btnExecuteMedicine = new System.Windows.Forms.Button();
            btnSetMedicinePosition = new System.Windows.Forms.Button();
            grpRevive.SuspendLayout();
            grpMedicine.SuspendLayout();
            SuspendLayout();
            // 
            // cmbWindows
            // 
            cmbWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbWindows.FormattingEnabled = true;
            cmbWindows.Location = new System.Drawing.Point(12, 12);
            cmbWindows.Name = "cmbWindows";
            cmbWindows.Size = new System.Drawing.Size(273, 23);
            cmbWindows.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(291, 11);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(75, 23);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Atualizar";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(99, 15);
            label1.TabIndex = 2;
            label1.Text = "Atalho Pokémon:";
            // 
            // txtPokemonKey
            // 
            txtPokemonKey.Location = new System.Drawing.Point(6, 43);
            txtPokemonKey.Name = "txtPokemonKey";
            txtPokemonKey.ReadOnly = true;
            txtPokemonKey.Size = new System.Drawing.Size(100, 23);
            txtPokemonKey.TabIndex = 3;
            txtPokemonKey.Text = "F1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(188, 25);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(82, 15);
            label2.TabIndex = 4;
            label2.Text = "Atalho Revive:";
            // 
            // txtReviveKey
            // 
            txtReviveKey.Location = new System.Drawing.Point(188, 43);
            txtReviveKey.Name = "txtReviveKey";
            txtReviveKey.ReadOnly = true;
            txtReviveKey.Size = new System.Drawing.Size(100, 23);
            txtReviveKey.TabIndex = 5;
            txtReviveKey.Text = "F2";
            // 
            // btnCapturePokemonKey
            // 
            btnCapturePokemonKey.Location = new System.Drawing.Point(6, 72);
            btnCapturePokemonKey.Name = "btnCapturePokemonKey";
            btnCapturePokemonKey.Size = new System.Drawing.Size(100, 23);
            btnCapturePokemonKey.TabIndex = 9;
            btnCapturePokemonKey.Text = "Capturar Tecla";
            btnCapturePokemonKey.UseVisualStyleBackColor = true;
            btnCapturePokemonKey.Click += btnCapturePokemonKey_Click;
            // 
            // btnCaptureReviveKey
            // 
            btnCaptureReviveKey.Location = new System.Drawing.Point(188, 72);
            btnCaptureReviveKey.Name = "btnCaptureReviveKey";
            btnCaptureReviveKey.Size = new System.Drawing.Size(100, 23);
            btnCaptureReviveKey.TabIndex = 10;
            btnCaptureReviveKey.Text = "Capturar Tecla";
            btnCaptureReviveKey.UseVisualStyleBackColor = true;
            btnCaptureReviveKey.Click += btnCaptureReviveKey_Click;
            // 
            // btnSetPosition
            // 
            btnSetPosition.Location = new System.Drawing.Point(6, 112);
            btnSetPosition.Name = "btnSetPosition";
            btnSetPosition.Size = new System.Drawing.Size(143, 23);
            btnSetPosition.TabIndex = 6;
            btnSetPosition.Text = "Selecionar Posição Alvo";
            btnSetPosition.UseVisualStyleBackColor = true;
            btnSetPosition.Click += btnSetPosition_Click;
            // 
            // btnExecuteRevive
            // 
            btnExecuteRevive.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnExecuteRevive.Location = new System.Drawing.Point(6, 152);
            btnExecuteRevive.Name = "btnExecuteRevive";
            btnExecuteRevive.Size = new System.Drawing.Size(342, 33);
            btnExecuteRevive.TabIndex = 7;
            btnExecuteRevive.Text = "▶️ INICIAR MODO AUTO";
            btnExecuteRevive.UseVisualStyleBackColor = true;
            btnExecuteRevive.Click += btnExecuteRevive_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new System.Drawing.Point(155, 112);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(193, 23);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Status: Pronto";
            // 
            // grpRevive
            // 
            grpRevive.Controls.Add(label1);
            grpRevive.Controls.Add(btnCaptureReviveKey);
            grpRevive.Controls.Add(txtPokemonKey);
            grpRevive.Controls.Add(btnCapturePokemonKey);
            grpRevive.Controls.Add(label2);
            grpRevive.Controls.Add(lblStatus);
            grpRevive.Controls.Add(txtReviveKey);
            grpRevive.Controls.Add(btnExecuteRevive);
            grpRevive.Controls.Add(btnSetPosition);
            grpRevive.Location = new System.Drawing.Point(12, 41);
            grpRevive.Name = "grpRevive";
            grpRevive.Size = new System.Drawing.Size(354, 198);
            grpRevive.TabIndex = 11;
            grpRevive.TabStop = false;
            grpRevive.Text = "Reviver Pokémon";
            // 
            // grpMedicine
            // 
            grpMedicine.Controls.Add(lblMedicineStatus);
            grpMedicine.Controls.Add(btnExecuteMedicine);
            grpMedicine.Controls.Add(txtMedicineKey);
            grpMedicine.Controls.Add(btnCaptureMedicineKey);
            grpMedicine.Controls.Add(lblMedicineKey);
            grpMedicine.Controls.Add(btnSetMedicinePosition);
            grpMedicine.Location = new System.Drawing.Point(12, 245);
            grpMedicine.Name = "grpMedicine";
            grpMedicine.Size = new System.Drawing.Size(354, 198);
            grpMedicine.TabIndex = 12;
            grpMedicine.TabStop = false;
            grpMedicine.Text = "Usar Medicina";
            // 
            // lblMedicineKey
            // 
            lblMedicineKey.AutoSize = true;
            lblMedicineKey.Location = new System.Drawing.Point(6, 25);
            lblMedicineKey.Name = "lblMedicineKey";
            lblMedicineKey.Size = new System.Drawing.Size(85, 15);
            lblMedicineKey.TabIndex = 2;
            lblMedicineKey.Text = "Atalho Medicina:";
            // 
            // txtMedicineKey
            // 
            txtMedicineKey.Location = new System.Drawing.Point(6, 43);
            txtMedicineKey.Name = "txtMedicineKey";
            txtMedicineKey.ReadOnly = true;
            txtMedicineKey.Size = new System.Drawing.Size(100, 23);
            txtMedicineKey.TabIndex = 3;
            txtMedicineKey.Text = "F3";
            // 
            // btnCaptureMedicineKey
            // 
            btnCaptureMedicineKey.Location = new System.Drawing.Point(6, 72);
            btnCaptureMedicineKey.Name = "btnCaptureMedicineKey";
            btnCaptureMedicineKey.Size = new System.Drawing.Size(100, 23);
            btnCaptureMedicineKey.TabIndex = 9;
            btnCaptureMedicineKey.Text = "Capturar Tecla";
            btnCaptureMedicineKey.UseVisualStyleBackColor = true;
            btnCaptureMedicineKey.Click += btnCaptureMedicineKey_Click;
            // 
            // btnSetMedicinePosition
            // 
            btnSetMedicinePosition.Location = new System.Drawing.Point(6, 112);
            btnSetMedicinePosition.Name = "btnSetMedicinePosition";
            btnSetMedicinePosition.Size = new System.Drawing.Size(143, 23);
            btnSetMedicinePosition.TabIndex = 6;
            btnSetMedicinePosition.Text = "Selecionar Posição Alvo";
            btnSetMedicinePosition.UseVisualStyleBackColor = true;
            btnSetMedicinePosition.Click += btnSetMedicinePosition_Click;
            // 
            // btnExecuteMedicine
            // 
            btnExecuteMedicine.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnExecuteMedicine.Location = new System.Drawing.Point(6, 152);
            btnExecuteMedicine.Name = "btnExecuteMedicine";
            btnExecuteMedicine.Size = new System.Drawing.Size(342, 33);
            btnExecuteMedicine.TabIndex = 7;
            btnExecuteMedicine.Text = "▶️ INICIAR MODO AUTO";
            btnExecuteMedicine.UseVisualStyleBackColor = true;
            btnExecuteMedicine.Click += btnExecuteMedicine_Click;
            // 
            // lblMedicineStatus
            // 
            lblMedicineStatus.Location = new System.Drawing.Point(155, 112);
            lblMedicineStatus.Name = "lblMedicineStatus";
            lblMedicineStatus.Size = new System.Drawing.Size(193, 23);
            lblMedicineStatus.TabIndex = 8;
            lblMedicineStatus.Text = "Status: Pronto";
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(378, 451);
            Controls.Add(grpMedicine);
            Controls.Add(grpRevive);
            Controls.Add(btnRefresh);
            Controls.Add(cmbWindows);
            Name = "Main";
            Text = "PxG Reviver";
            FormClosing += Main_FormClosing;
            grpRevive.ResumeLayout(false);
            grpRevive.PerformLayout();
            grpMedicine.ResumeLayout(false);
            grpMedicine.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cmbWindows;
        private Button btnRefresh;
        private Label label1;
        private TextBox txtPokemonKey;
        private Label label2;
        private TextBox txtReviveKey;
        private Button btnSetPosition;
        private Button btnExecuteRevive;
        private Label lblStatus;
        private Button btnCapturePokemonKey;
        private Button btnCaptureReviveKey;
        private System.Windows.Forms.GroupBox grpRevive;
        private System.Windows.Forms.GroupBox grpMedicine;
        private System.Windows.Forms.Label lblMedicineKey;
        private System.Windows.Forms.Button btnCaptureMedicineKey;
        private System.Windows.Forms.TextBox txtMedicineKey;
        private System.Windows.Forms.Label lblMedicineStatus;
        private System.Windows.Forms.Button btnExecuteMedicine;
        private System.Windows.Forms.Button btnSetMedicinePosition;
    }
}

