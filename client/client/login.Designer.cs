namespace Go
{
    partial class login
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.start = new System.Windows.Forms.Label();
            this.white = new System.Windows.Forms.Button();
            this.black = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // start
            // 
            this.start.AutoSize = true;
            this.start.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start.Location = new System.Drawing.Point(171, 111);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(273, 110);
            this.start.TabIndex = 7;
            this.start.Text = "Che colore\r\nvuoi usare?";
            this.start.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // white
            // 
            this.white.AutoSize = true;
            this.white.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.white.Location = new System.Drawing.Point(171, 312);
            this.white.Name = "white";
            this.white.Size = new System.Drawing.Size(75, 28);
            this.white.TabIndex = 8;
            this.white.Text = "Bianco";
            this.white.UseVisualStyleBackColor = true;
            this.white.Click += new System.EventHandler(this.white_Click);
            // 
            // black
            // 
            this.black.AutoSize = true;
            this.black.BackColor = System.Drawing.Color.Black;
            this.black.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.black.ForeColor = System.Drawing.Color.White;
            this.black.Location = new System.Drawing.Point(369, 312);
            this.black.Name = "black";
            this.black.Size = new System.Drawing.Size(75, 28);
            this.black.TabIndex = 9;
            this.black.Text = "Nero";
            this.black.UseVisualStyleBackColor = false;
            this.black.Click += new System.EventHandler(this.black_Click);
            // 
            // login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::client.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(614, 458);
            this.Controls.Add(this.black);
            this.Controls.Add(this.white);
            this.Controls.Add(this.start);
            this.MaximumSize = new System.Drawing.Size(630, 497);
            this.MinimumSize = new System.Drawing.Size(630, 497);
            this.Name = "login";
            this.Text = "Log in";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label start;
        private System.Windows.Forms.Button white;
        private System.Windows.Forms.Button black;
    }
}

