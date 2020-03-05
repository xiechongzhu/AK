namespace RadarProcess
{
    partial class TestInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestInfoForm));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.editTestName = new DevExpress.XtraEditors.TextEdit();
            this.editOperator = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.editComment = new DevExpress.XtraEditors.MemoEdit();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.editTestName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editOperator.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editComment.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(120, 24);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "试验名称：";
            // 
            // editTestName
            // 
            this.editTestName.Location = new System.Drawing.Point(148, 6);
            this.editTestName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editTestName.Name = "editTestName";
            this.editTestName.Size = new System.Drawing.Size(200, 38);
            this.editTestName.TabIndex = 1;
            // 
            // editOperator
            // 
            this.editOperator.Location = new System.Drawing.Point(516, 6);
            this.editOperator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editOperator.Name = "editOperator";
            this.editOperator.Size = new System.Drawing.Size(200, 38);
            this.editOperator.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(380, 12);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(120, 24);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "试验人员：";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(12, 64);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(72, 24);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "备注：";
            // 
            // editComment
            // 
            this.editComment.Location = new System.Drawing.Point(148, 76);
            this.editComment.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editComment.Name = "editComment";
            this.editComment.Size = new System.Drawing.Size(566, 288);
            this.editComment.TabIndex = 5;
            // 
            // btnOK
            // 
            this.btnOK.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.ImageOptions.Image")));
            this.btnOK.Location = new System.Drawing.Point(390, 370);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(150, 68);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(564, 370);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 68);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TestInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.editComment);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.editOperator);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.editTestName);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "测试信息";
            ((System.ComponentModel.ISupportInitialize)(this.editTestName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editOperator.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editComment.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit editTestName;
        private DevExpress.XtraEditors.TextEdit editOperator;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.MemoEdit editComment;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}