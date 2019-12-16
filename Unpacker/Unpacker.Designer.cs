namespace Unpacker
{
    partial class Unpacker
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
            this.barFiles = new System.Windows.Forms.ProgressBar();
            this.barTotal = new System.Windows.Forms.ProgressBar();
            this.labelFiles = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.buttonZip = new System.Windows.Forms.Button();
            this.buttonFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // barFiles
            // 
            this.barFiles.Location = new System.Drawing.Point(12, 25);
            this.barFiles.Name = "barFiles";
            this.barFiles.Size = new System.Drawing.Size(100, 23);
            this.barFiles.TabIndex = 0;
            // 
            // barTotal
            // 
            this.barTotal.Location = new System.Drawing.Point(12, 67);
            this.barTotal.Name = "barTotal";
            this.barTotal.Size = new System.Drawing.Size(100, 23);
            this.barTotal.TabIndex = 1;
            // 
            // labelFiles
            // 
            this.labelFiles.AutoSize = true;
            this.labelFiles.Location = new System.Drawing.Point(10, 9);
            this.labelFiles.Name = "labelFiles";
            this.labelFiles.Size = new System.Drawing.Size(35, 13);
            this.labelFiles.TabIndex = 2;
            this.labelFiles.Text = "label1";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(42, 51);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(35, 13);
            this.labelTotal.TabIndex = 3;
            this.labelTotal.Text = "label2";
            // 
            // buttonZip
            // 
            this.buttonZip.Location = new System.Drawing.Point(12, 96);
            this.buttonZip.Name = "buttonZip";
            this.buttonZip.Size = new System.Drawing.Size(100, 23);
            this.buttonZip.TabIndex = 4;
            this.buttonZip.Text = "Download Zip";
            this.buttonZip.UseVisualStyleBackColor = true;
            this.buttonZip.Click += new System.EventHandler(this.DownloadZip);
            // 
            // buttonFiles
            // 
            this.buttonFiles.Location = new System.Drawing.Point(12, 125);
            this.buttonFiles.Name = "buttonFiles";
            this.buttonFiles.Size = new System.Drawing.Size(100, 23);
            this.buttonFiles.TabIndex = 5;
            this.buttonFiles.Text = "Download Files";
            this.buttonFiles.UseVisualStyleBackColor = true;
            this.buttonFiles.Click += new System.EventHandler(this.DownloadFiles);
            // 
            // Unpacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(119, 154);
            this.Controls.Add(this.buttonFiles);
            this.Controls.Add(this.buttonZip);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.labelFiles);
            this.Controls.Add(this.barTotal);
            this.Controls.Add(this.barFiles);
            this.Name = "Unpacker";
            this.Text = "Unpacker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar barFiles;
        private System.Windows.Forms.ProgressBar barTotal;
        private System.Windows.Forms.Label labelFiles;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Button buttonZip;
        private System.Windows.Forms.Button buttonFiles;
    }
}