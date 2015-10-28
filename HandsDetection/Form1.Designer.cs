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
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOrig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxSkin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProc)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBoxOrig
            // 
            this.ImageBoxOrig.Location = new System.Drawing.Point(12, 12);
            this.ImageBoxOrig.Name = "ImageBoxOrig";
            this.ImageBoxOrig.Size = new System.Drawing.Size(421, 358);
            this.ImageBoxOrig.TabIndex = 2;
            this.ImageBoxOrig.TabStop = false;
            // 
            // ImageBoxSkin
            // 
            this.ImageBoxSkin.Location = new System.Drawing.Point(451, 12);
            this.ImageBoxSkin.Name = "ImageBoxSkin";
            this.ImageBoxSkin.Size = new System.Drawing.Size(421, 358);
            this.ImageBoxSkin.TabIndex = 3;
            this.ImageBoxSkin.TabStop = false;
            // 
            // ImageBoxProc
            // 
            this.ImageBoxProc.Location = new System.Drawing.Point(12, 388);
            this.ImageBoxProc.Name = "ImageBoxProc";
            this.ImageBoxProc.Size = new System.Drawing.Size(421, 358);
            this.ImageBoxProc.TabIndex = 4;
            this.ImageBoxProc.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 769);
            this.Controls.Add(this.ImageBoxProc);
            this.Controls.Add(this.ImageBoxSkin);
            this.Controls.Add(this.ImageBoxOrig);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOrig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxSkin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProc)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBoxOrig;
        private Emgu.CV.UI.ImageBox ImageBoxSkin;
        private Emgu.CV.UI.ImageBox ImageBoxProc;
    }
}

