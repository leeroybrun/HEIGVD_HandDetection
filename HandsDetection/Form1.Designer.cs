namespace HandsDetection
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ImageBoxOrig = new Emgu.CV.UI.ImageBox();
            this.ImageBoxSkin = new Emgu.CV.UI.ImageBox();
            this.ImageBoxProc = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NbFingersLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOrig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxSkin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProc)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBoxOrig
            // 
            this.ImageBoxOrig.Location = new System.Drawing.Point(24, 23);
            this.ImageBoxOrig.Margin = new System.Windows.Forms.Padding(6);
            this.ImageBoxOrig.Name = "ImageBoxOrig";
            this.ImageBoxOrig.Size = new System.Drawing.Size(842, 688);
            this.ImageBoxOrig.TabIndex = 2;
            this.ImageBoxOrig.TabStop = false;
            // 
            // ImageBoxSkin
            // 
            this.ImageBoxSkin.Location = new System.Drawing.Point(902, 23);
            this.ImageBoxSkin.Margin = new System.Windows.Forms.Padding(6);
            this.ImageBoxSkin.Name = "ImageBoxSkin";
            this.ImageBoxSkin.Size = new System.Drawing.Size(842, 688);
            this.ImageBoxSkin.TabIndex = 3;
            this.ImageBoxSkin.TabStop = false;
            // 
            // ImageBoxProc
            // 
            this.ImageBoxProc.Location = new System.Drawing.Point(24, 746);
            this.ImageBoxProc.Margin = new System.Windows.Forms.Padding(6);
            this.ImageBoxProc.Name = "ImageBoxProc";
            this.ImageBoxProc.Size = new System.Drawing.Size(842, 688);
            this.ImageBoxProc.TabIndex = 4;
            this.ImageBoxProc.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1097, 950);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(484, 42);
            this.label1.TabIndex = 5;
            this.label1.Text = "Nombre de doigts détectés :";
            // 
            // NbFingersLabel
            // 
            this.NbFingersLabel.AutoSize = true;
            this.NbFingersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NbFingersLabel.Location = new System.Drawing.Point(1292, 1038);
            this.NbFingersLabel.Name = "NbFingersLabel";
            this.NbFingersLabel.Size = new System.Drawing.Size(68, 73);
            this.NbFingersLabel.TabIndex = 6;
            this.NbFingersLabel.Text = "5";
            this.NbFingersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1788, 1479);
            this.Controls.Add(this.NbFingersLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ImageBoxProc);
            this.Controls.Add(this.ImageBoxSkin);
            this.Controls.Add(this.ImageBoxOrig);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOrig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxSkin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBoxOrig;
        private Emgu.CV.UI.ImageBox ImageBoxSkin;
        private Emgu.CV.UI.ImageBox ImageBoxProc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label NbFingersLabel;
    }
}

