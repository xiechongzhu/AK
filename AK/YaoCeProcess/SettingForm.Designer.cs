namespace YaoCeProcess
{
    partial class SettingForm
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
            DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.editMultiCastIp = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.editPort = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editMultiCastIp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AutoScroll = false;
            this.layoutControl1.Controls.Add(this.tableLayoutPanel1);
            this.layoutControl1.Controls.Add(this.editMultiCastIp);
            this.layoutControl1.Controls.Add(this.editPort);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(870, 192, 812, 500);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(462, 153);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.Root.Name = "Root";
            columnDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
            columnDefinition2.Width = 442D;
            this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            columnDefinition2});
            rowDefinition4.Height = 38D;
            rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
            rowDefinition5.Height = 38D;
            rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
            rowDefinition6.Height = 57D;
            rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
            this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowDefinition4,
            rowDefinition5,
            rowDefinition6});
            this.Root.Size = new System.Drawing.Size(462, 153);
            this.Root.TextVisible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 88);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 53);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tableLayoutPanel1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 76);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 2;
            this.layoutControlItem1.Size = new System.Drawing.Size(442, 57);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // editMultiCastIp
            // 
            this.editMultiCastIp.Location = new System.Drawing.Point(99, 12);
            this.editMultiCastIp.Name = "editMultiCastIp";
            this.editMultiCastIp.Properties.Mask.EditMask = "((([1-9]?|1\\d)\\d|2([0-4]\\d|5[0-5]))\\.){3}(([1-9]?|1\\d)\\d|2([0-4]\\d|5[0-5]))";
            this.editMultiCastIp.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.editMultiCastIp.Size = new System.Drawing.Size(351, 24);
            this.editMultiCastIp.StyleController = this.layoutControl1;
            this.editMultiCastIp.TabIndex = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.editMultiCastIp;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(442, 38);
            this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
            this.layoutControlItem2.Text = "组播地址：";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(75, 18);
            // 
            // editPort
            // 
            this.editPort.Location = new System.Drawing.Point(99, 50);
            this.editPort.Name = "editPort";
            this.editPort.Size = new System.Drawing.Size(351, 24);
            this.editPort.StyleController = this.layoutControl1;
            this.editPort.TabIndex = 6;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.editPort;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 38);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
            this.layoutControlItem3.Size = new System.Drawing.Size(442, 38);
            this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
            this.layoutControlItem3.Text = "端口号：";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(75, 18);
            // 
            // btnOK
            // 
            this.btnOK.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.ImageOptions.Image")));
            this.btnOK.Location = new System.Drawing.Point(222, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(103, 47);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(331, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 47);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 153);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 200);
            this.Name = "SettingForm";
            this.Text = "参数设置";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editMultiCastIp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.TextEdit editMultiCastIp;
        private DevExpress.XtraEditors.TextEdit editPort;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}