namespace YaoCeProcess
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.layoutControl_ToolBar = new DevExpress.XtraLayout.LayoutControl();
            this.BtnStartStop = new DevExpress.XtraEditors.SimpleButton();
            this.BtnSetting = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControl_MainContent = new DevExpress.XtraLayout.LayoutControl();
            this.TabControl_SoftStatus = new DevExpress.XtraTab.XtraTabControl();
            this.TabPage_XiTongPanJue = new DevExpress.XtraTab.XtraTabPage();
            this.TabPage_DaoHangKuaiSu = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.TabPage_DaoHangManSu = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_ToolBar)).BeginInit();
            this.layoutControl_ToolBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_MainContent)).BeginInit();
            this.layoutControl_MainContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TabControl_SoftStatus)).BeginInit();
            this.TabControl_SoftStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl_ToolBar
            // 
            this.layoutControl_ToolBar.AutoScroll = false;
            this.layoutControl_ToolBar.Controls.Add(this.BtnStartStop);
            this.layoutControl_ToolBar.Controls.Add(this.BtnSetting);
            this.layoutControl_ToolBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl_ToolBar.Location = new System.Drawing.Point(0, 0);
            this.layoutControl_ToolBar.Name = "layoutControl_ToolBar";
            this.layoutControl_ToolBar.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1108, 0, 812, 500);
            this.layoutControl_ToolBar.Root = this.Root;
            this.layoutControl_ToolBar.Size = new System.Drawing.Size(1111, 62);
            this.layoutControl_ToolBar.TabIndex = 0;
            this.layoutControl_ToolBar.Text = "layoutControl1";
            // 
            // BtnStartStop
            // 
            this.BtnStartStop.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtnStartStop.ImageOptions.Image")));
            this.BtnStartStop.Location = new System.Drawing.Point(54, 12);
            this.BtnStartStop.Name = "BtnStartStop";
            this.BtnStartStop.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.BtnStartStop.Size = new System.Drawing.Size(38, 36);
            this.BtnStartStop.StyleController = this.layoutControl_ToolBar;
            this.BtnStartStop.TabIndex = 5;
            // 
            // BtnSetting
            // 
            this.BtnSetting.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtnSet.ImageOptions.Image")));
            this.BtnSetting.Location = new System.Drawing.Point(12, 12);
            this.BtnSetting.Name = "BtnSetting";
            this.BtnSetting.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.BtnSetting.Size = new System.Drawing.Size(38, 36);
            this.BtnSetting.StyleController = this.layoutControl_ToolBar;
            this.BtnSetting.TabIndex = 4;
            this.BtnSetting.Click += new System.EventHandler(this.BtnSetting_Click);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.emptySpaceItem2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1111, 62);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.BtnSetting;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(42, 40);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 40);
            this.emptySpaceItem1.MaxSize = new System.Drawing.Size(84, 10);
            this.emptySpaceItem1.MinSize = new System.Drawing.Size(84, 1);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(84, 2);
            this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.BtnStartStop;
            this.layoutControlItem2.Location = new System.Drawing.Point(42, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(42, 40);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(84, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(1007, 42);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControl_MainContent
            // 
            this.layoutControl_MainContent.Controls.Add(this.TabControl_SoftStatus);
            this.layoutControl_MainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl_MainContent.Location = new System.Drawing.Point(0, 62);
            this.layoutControl_MainContent.Name = "layoutControl_MainContent";
            this.layoutControl_MainContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(687, 146, 812, 500);
            this.layoutControl_MainContent.Root = this.layoutControlGroup1;
            this.layoutControl_MainContent.Size = new System.Drawing.Size(1111, 651);
            this.layoutControl_MainContent.TabIndex = 1;
            this.layoutControl_MainContent.Text = "layoutControl2";
            // 
            // TabControl_SoftStatus
            // 
            this.TabControl_SoftStatus.Location = new System.Drawing.Point(12, 12);
            this.TabControl_SoftStatus.Name = "TabControl_SoftStatus";
            this.TabControl_SoftStatus.SelectedTabPage = this.TabPage_XiTongPanJue;
            this.TabControl_SoftStatus.Size = new System.Drawing.Size(1087, 627);
            this.TabControl_SoftStatus.TabIndex = 4;
            this.TabControl_SoftStatus.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.TabPage_XiTongPanJue,
            this.TabPage_DaoHangKuaiSu,
            this.TabPage_DaoHangManSu});
            // 
            // TabPage_XiTongPanJue
            // 
            this.TabPage_XiTongPanJue.Name = "TabPage_XiTongPanJue";
            this.TabPage_XiTongPanJue.Size = new System.Drawing.Size(1085, 595);
            this.TabPage_XiTongPanJue.Text = "系统判决状态";
            // 
            // TabPage_DaoHangKuaiSu
            // 
            this.TabPage_DaoHangKuaiSu.Name = "TabPage_DaoHangKuaiSu";
            this.TabPage_DaoHangKuaiSu.Size = new System.Drawing.Size(1085, 595);
            this.TabPage_DaoHangKuaiSu.Text = "导航数据（快速）";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1111, 651);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.TabControl_SoftStatus;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1091, 631);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // TabPage_DaoHangManSu
            // 
            this.TabPage_DaoHangManSu.Name = "TabPage_DaoHangManSu";
            this.TabPage_DaoHangManSu.Size = new System.Drawing.Size(1085, 595);
            this.TabPage_DaoHangManSu.Text = "导航数据（慢速）";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1111, 713);
            this.Controls.Add(this.layoutControl_MainContent);
            this.Controls.Add(this.layoutControl_ToolBar);
            this.Name = "MainForm";
            this.Text = "遥测数据显示";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_ToolBar)).EndInit();
            this.layoutControl_ToolBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_MainContent)).EndInit();
            this.layoutControl_MainContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TabControl_SoftStatus)).EndInit();
            this.TabControl_SoftStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl_ToolBar;
        private DevExpress.XtraEditors.SimpleButton BtnStartStop;
        private DevExpress.XtraEditors.SimpleButton BtnSetting;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl_MainContent;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraTab.XtraTabControl TabControl_SoftStatus;
        private DevExpress.XtraTab.XtraTabPage TabPage_XiTongPanJue;
        private DevExpress.XtraTab.XtraTabPage TabPage_DaoHangKuaiSu;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraTab.XtraTabPage TabPage_DaoHangManSu;
    }
}

