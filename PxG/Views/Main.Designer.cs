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
            label1.Location = new System.Drawing.Point(12, 53);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(99, 15);
            label1.TabIndex = 2;
            label1.Text = "Atalho Pokémon:";
            // 
            // txtPokemonKey
            // 
            txtPokemonKey.Location = new System.Drawing.Point(12, 71);
            txtPokemonKey.Name = "txtPokemonKey";
            txtPokemonKey.ReadOnly = true;
            txtPokemonKey.Size = new System.Drawing.Size(100, 23);
            txtPokemonKey.TabIndex = 3;
            txtPokemonKey.Text = "F1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(200, 53);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(82, 15);
            label2.TabIndex = 4;
            label2.Text = "Atalho Revive:";
            // 
            // txtReviveKey
            // 
            txtReviveKey.Location = new System.Drawing.Point(200, 71);
            txtReviveKey.Name = "txtReviveKey";
            txtReviveKey.ReadOnly = true;
            txtReviveKey.Size = new System.Drawing.Size(100, 23);
            txtReviveKey.TabIndex = 5;
            txtReviveKey.Text = "F2";
            // 
            // btnCapturePokemonKey
            // 
            btnCapturePokemonKey.Location = new System.Drawing.Point(12, 100);
            btnCapturePokemonKey.Name = "btnCapturePokemonKey";
            btnCapturePokemonKey.Size = new System.Drawing.Size(100, 23);
            btnCapturePokemonKey.TabIndex = 9;
            btnCapturePokemonKey.Text = "Capturar Tecla";
            btnCapturePokemonKey.UseVisualStyleBackColor = true;
            btnCapturePokemonKey.Click += btnCapturePokemonKey_Click;
            // 
            // btnCaptureReviveKey
            // 
            btnCaptureReviveKey.Location = new System.Drawing.Point(200, 100);
            btnCaptureReviveKey.Name = "btnCaptureReviveKey";
            btnCaptureReviveKey.Size = new System.Drawing.Size(100, 23);
            btnCaptureReviveKey.TabIndex = 10;
            btnCaptureReviveKey.Text = "Capturar Tecla";
            btnCaptureReviveKey.UseVisualStyleBackColor = true;
            btnCaptureReviveKey.Click += btnCaptureReviveKey_Click;
            // 
            // btnSetPosition
            // 
            btnSetPosition.Location = new System.Drawing.Point(12, 140);
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
            btnExecuteRevive.Location = new System.Drawing.Point(12, 180);
            btnExecuteRevive.Name = "btnExecuteRevive";
            btnExecuteRevive.Size = new System.Drawing.Size(354, 33);
            btnExecuteRevive.TabIndex = 7;
            btnExecuteRevive.Text = "EXECUTAR REVIVE";
            btnExecuteRevive.UseVisualStyleBackColor = true;
            btnExecuteRevive.Click += btnExecuteRevive_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new System.Drawing.Point(173, 140);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(193, 23);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Status: Pronto";
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(378, 231);
            Controls.Add(btnCaptureReviveKey);
            Controls.Add(btnCapturePokemonKey);
            Controls.Add(lblStatus);
            Controls.Add(btnExecuteRevive);
            Controls.Add(btnSetPosition);
            Controls.Add(txtReviveKey);
            Controls.Add(label2);
            Controls.Add(txtPokemonKey);
            Controls.Add(label1);
            Controls.Add(btnRefresh);
            Controls.Add(cmbWindows);
            Text = "PxG Reviver";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblStatus;

        #endregion

        private ComboBox cmbWindows;
        private Button btnRefresh;
        private Label label1;
        private TextBox txtPokemonKey;
        private Label label2;
        private TextBox txtReviveKey;
        private Button btnSetPosition;
        private Button btnExecuteRevive;
        private Button btnCapturePokemonKey;
        private Button btnCaptureReviveKey;
    }
}

