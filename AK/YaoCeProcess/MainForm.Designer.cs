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
            this.TabPage_DaoHangManSu = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LogListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
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
            this.BtnSetting.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtnSetting.ImageOptions.Image")));
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
            this.layoutControl_MainContent.Controls.Add(this.layoutControl1);
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
            this.TabControl_SoftStatus.Size = new System.Drawing.Size(1087, 382);
            this.TabControl_SoftStatus.TabIndex = 4;
            this.TabControl_SoftStatus.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.TabPage_XiTongPanJue,
            this.TabPage_DaoHangKuaiSu,
            this.TabPage_DaoHangManSu});
            // 
            // TabPage_XiTongPanJue
            // 
            this.TabPage_XiTongPanJue.Name = "TabPage_XiTongPanJue";
            this.TabPage_XiTongPanJue.Size = new System.Drawing.Size(1085, 350);
            this.TabPage_XiTongPanJue.Text = "系统判决状态";
            // 
            // TabPage_DaoHangKuaiSu
            // 
            this.TabPage_DaoHangKuaiSu.Name = "TabPage_DaoHangKuaiSu";
            this.TabPage_DaoHangKuaiSu.Size = new System.Drawing.Size(1085, 595);
            this.TabPage_DaoHangKuaiSu.Text = "导航数据（快速）";
            // 
            // TabPage_DaoHangManSu
            // 
            this.TabPage_DaoHangManSu.Name = "TabPage_DaoHangManSu";
            this.TabPage_DaoHangManSu.Size = new System.Drawing.Size(1085, 595);
            this.TabPage_DaoHangManSu.Text = "导航数据（慢速）";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1111, 651);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.TabControl_SoftStatus;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1091, 386);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.LogListView);
            this.layoutControl1.Location = new System.Drawing.Point(12, 398);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup2;
            this.layoutControl1.Size = new System.Drawing.Size(1087, 241);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.layoutControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 386);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1091, 245);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5});
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1087, 241);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // LogListView
            // 
            this.LogListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.LogListView.FullRowSelect = true;
            this.LogListView.HideSelection = false;
            this.LogListView.Location = new System.Drawing.Point(2, 2);
            this.LogListView.Margin = new System.Windows.Forms.Padding(2);
            this.LogListView.Name = "LogListView";
            this.LogListView.Size = new System.Drawing.Size(1083, 237);
            this.LogListView.TabIndex = 6;
            this.LogListView.UseCompatibleStateImageBehavior = false;
            this.LogListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "时间";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "等级";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "内容";
            this.columnHeader3.Width = 400;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.LogListView;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1087, 241);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
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
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private System.Windows.Forms.ListView LogListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}

