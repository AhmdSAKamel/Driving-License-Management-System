namespace Driving_License_Management.General_Forms
{
    partial class frmTakePicture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTakePicture));
            this.pbPersonImage = new System.Windows.Forms.PictureBox();
            this.btnTakePicture = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPersonImage
            // 
            this.pbPersonImage.BackColor = System.Drawing.SystemColors.Desktop;
            this.pbPersonImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbPersonImage.Location = new System.Drawing.Point(0, 0);
            this.pbPersonImage.Name = "pbPersonImage";
            this.pbPersonImage.Size = new System.Drawing.Size(800, 387);
            this.pbPersonImage.TabIndex = 0;
            this.pbPersonImage.TabStop = false;
            // 
            // btnTakePicture
            // 
            this.btnTakePicture.BackColor = System.Drawing.Color.White;
            this.btnTakePicture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTakePicture.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnTakePicture.Image = ((System.Drawing.Image)(resources.GetObject("btnTakePicture.Image")));
            this.btnTakePicture.Location = new System.Drawing.Point(345, 398);
            this.btnTakePicture.Name = "btnTakePicture";
            this.btnTakePicture.Size = new System.Drawing.Size(57, 49);
            this.btnTakePicture.TabIndex = 1;
            this.btnTakePicture.UseVisualStyleBackColor = false;
            this.btnTakePicture.Click += new System.EventHandler(this.btnTakePicture_Click);
            // 
            // frmTakePicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTakePicture);
            this.Controls.Add(this.pbPersonImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmTakePicture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Take Picture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTakePicture_FormClosing);
            this.Load += new System.EventHandler(this.frmTakePicture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbPersonImage;
        private System.Windows.Forms.Button btnTakePicture;
    }
}