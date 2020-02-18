namespace RadarProcess
{
    partial class AlertForm
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
            this.AlertLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AlertLabel
            // 
            this.AlertLabel.Font = new System.Drawing.Font("宋体", 15F);
            this.AlertLabel.ForeColor = System.Drawing.Color.Red;
            this.AlertLabel.Location = new System.Drawing.Point(12, 9);
            this.AlertLabel.Name = "AlertLabel";
            this.AlertLabel.Size = new System.Drawing.Size(451, 113);
            this.AlertLabel.TabIndex = 0;
            this.AlertLabel.Text = "AlertLabel";
            this.AlertLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(475, 131);
            this.Controls.Add(this.AlertLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AlertForm";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AlertForm";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label AlertLabel;
    }
}