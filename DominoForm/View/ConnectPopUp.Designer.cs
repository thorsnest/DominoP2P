namespace DominoForm.View
{
    partial class ConnectPopUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.label1 = new System.Windows.Forms.Label();
            this.ip_TB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.port_TB = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // ip_TB
            // 
            this.ip_TB.Location = new System.Drawing.Point(38, 6);
            this.ip_TB.Name = "ip_TB";
            this.ip_TB.Size = new System.Drawing.Size(212, 23);
            this.ip_TB.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            // 
            // port_TB
            // 
            this.port_TB.Location = new System.Drawing.Point(291, 6);
            this.port_TB.Name = "port_TB";
            this.port_TB.Size = new System.Drawing.Size(50, 23);
            this.port_TB.TabIndex = 2;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(349, 6);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(88, 23);
            this.SubmitButton.TabIndex = 3;
            this.SubmitButton.Text = "Connect";
            this.SubmitButton.UseVisualStyleBackColor = true;
            // 
            // ConnectPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 34);
            this.Controls.Add(this.port_TB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.ip_TB);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ConnectPopUp";
            this.Text = "Connection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        public TextBox ip_TB;
        public Button SubmitButton;
        public TextBox port_TB;
    }
}