namespace Driving_License_Management.UserControls
{
    partial class ctrlPersonCardWithFilter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlPersonCardWithFilter));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ctrlPersonDetails1 = new Driving_License_Management.UserControls.ctrlPersonCard();
            this.ctrlFilter1 = new Driving_License_Management.UserControls.ctrlFilter();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAdd.BackgroundImage")));
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(460, 24);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(43, 33);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.White;
            this.btnFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFind.BackgroundImage")));
            this.btnFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Location = new System.Drawing.Point(409, 23);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(39, 33);
            this.btnFind.TabIndex = 12;
            this.btnFind.UseVisualStyleBackColor = false;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter";
            // 
            // ctrlPersonDetails1
            // 
            this.ctrlPersonDetails1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ctrlPersonDetails1.Location = new System.Drawing.Point(5, 68);
            this.ctrlPersonDetails1.Name = "ctrlPersonDetails1";
            this.ctrlPersonDetails1.Size = new System.Drawing.Size(755, 322);
            this.ctrlPersonDetails1.TabIndex = 17;
            this.ctrlPersonDetails1.OnLinklableClick += new System.Action<int>(this.ctrlPersonDetails1_OnLinklableClick);
            // 
            // ctrlFilter1
            // 
            this.ctrlFilter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctrlFilter1.CBFilter = false;
            this.ctrlFilter1.Cursor = System.Windows.Forms.Cursors.Default;
            this.ctrlFilter1.Location = new System.Drawing.Point(5, 17);
            this.ctrlFilter1.Name = "ctrlFilter1";
            this.ctrlFilter1.SelectedcbIndex = -1;
            this.ctrlFilter1.Size = new System.Drawing.Size(749, 45);
            this.ctrlFilter1.TabIndex = 15;
            this.ctrlFilter1.txtFilterValue = "";
            this.ctrlFilter1.OncbChosen += new System.Action(this.ctrlFilter1_OncbChosen);
            // 
            // ctrlPersonCardWithFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.ctrlPersonDetails1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ctrlFilter1);
            this.Name = "ctrlPersonCardWithFilter";
            this.Size = new System.Drawing.Size(763, 402);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ctrlPersonCard ctrlPersonDetails1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label label1;
        private ctrlFilter ctrlFilter1;
    }
}
