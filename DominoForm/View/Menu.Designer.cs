namespace DominoForm.View
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.connect_B = new System.Windows.Forms.Button();
            this.host_B = new System.Windows.Forms.Button();
            this.exit_B = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connect_B
            // 
            this.connect_B.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connect_B.BackColor = System.Drawing.Color.Transparent;
            this.connect_B.BackgroundImage = global::DominoForm.Properties.Resources.domino_tile_11;
            this.connect_B.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.connect_B.FlatAppearance.BorderSize = 0;
            this.connect_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connect_B.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.connect_B.Location = new System.Drawing.Point(287, 27);
            this.connect_B.Name = "connect_B";
            this.connect_B.Size = new System.Drawing.Size(239, 96);
            this.connect_B.TabIndex = 0;
            this.connect_B.Text = "Join Game";
            this.connect_B.UseVisualStyleBackColor = false;
            // 
            // host_B
            // 
            this.host_B.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.host_B.BackColor = System.Drawing.Color.Transparent;
            this.host_B.BackgroundImage = global::DominoForm.Properties.Resources.domino_tile_11;
            this.host_B.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.host_B.FlatAppearance.BorderSize = 0;
            this.host_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.host_B.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.host_B.Location = new System.Drawing.Point(287, 144);
            this.host_B.Name = "host_B";
            this.host_B.Size = new System.Drawing.Size(239, 96);
            this.host_B.TabIndex = 1;
            this.host_B.Text = "Host Game";
            this.host_B.UseVisualStyleBackColor = false;
            // 
            // exit_B
            // 
            this.exit_B.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exit_B.BackColor = System.Drawing.Color.Transparent;
            this.exit_B.BackgroundImage = global::DominoForm.Properties.Resources.domino_tile_11;
            this.exit_B.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.exit_B.FlatAppearance.BorderSize = 0;
            this.exit_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_B.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.exit_B.Location = new System.Drawing.Point(287, 261);
            this.exit_B.Name = "exit_B";
            this.exit_B.Size = new System.Drawing.Size(239, 96);
            this.exit_B.TabIndex = 2;
            this.exit_B.Text = "Exit";
            this.exit_B.UseVisualStyleBackColor = false;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = global::DominoForm.Properties.Resources.Wood;
            this.ClientSize = new System.Drawing.Size(800, 379);
            this.Controls.Add(this.exit_B);
            this.Controls.Add(this.host_B);
            this.Controls.Add(this.connect_B);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Domino";
            this.TransparencyKey = System.Drawing.Color.Silver;
            this.Load += new System.EventHandler(this.Menu_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public Button connect_B;
        public Button host_B;
        public Button exit_B;
    }
}