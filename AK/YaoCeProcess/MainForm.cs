using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YaoCeProcess
{
    public partial class MainForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量

        private int         CHART_ITEM_INDEX = 0;
        private const int   WM_USER = 0x400;
        public const int    WM_YAOCE_SystemStatus_DATA = WM_USER + 102;
        public const int    WM_YAOCE_daoHangKuaiSu_DATA = WM_USER + 103;
        public const int    WM_YAOCE_daoHangManSu_DATA = WM_USER + 104;

        // UDP
        private UdpClient   udpClient;
        // 码流记录
        private DataLogger  dataLogger = new DataLogger();
        // 网络消息处理
        private DataParser  dataParser;

        //-----------------------------------------------------//

        public MainForm()
        {
            InitializeComponent();

            // 窗口居中显示
            this.StartPosition = FormStartPosition.CenterScreen;

            // 传递窗口句柄
            dataParser = new DataParser(Handle);
            
            // 按钮暂时未使用，隐藏(?? 无效果，待确定 )
            BtnStartStop.Visible = false;

            // 传递主窗口指针
            Logger.GetInstance().SetMainForm(this);
        }

         ~MainForm()
        {
            
        }

        // 窗口事件
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            // 捕捉关闭窗体消息(用户点击关闭窗体控制按钮) 
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                // 关闭日志文件
                Logger.GetInstance().closeFile();

                // 关闭码流记录
                dataLogger.Stop();

                // 关闭消息处理
                dataParser.Stop();
            }
            base.WndProc(ref m);
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_YAOCE_SystemStatus_DATA:
                    IntPtr ptr = m.LParam;
                    /*
                    S_OBJECT sObject = Marshal.PtrToStructure<S_OBJECT>(ptr);
                    listSObject.Add(sObject);
                    AddPosition(sObject.X, sObject.Y, sObject.Z);
                    AddSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    CHART_ITEM_INDEX++;
                    CheckPosition(sObject.X, sObject.Y, sObject.Z);
                    CheckSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    AddPointOfFall(Algorithm.CalcIdealPointOfFall());
                    Marshal.FreeHGlobal(ptr);
                    */
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            settingForm.StartPosition = FormStartPosition.CenterScreen;

            if (settingForm.ShowDialog() != DialogResult.OK)
            {
                return ;
            }

            //-----------------------------------------------------//

            // 关闭已经存在的连接
            udpClient?.Close();

            // 关闭码流记录
            dataLogger.Stop();

            //-----------------------------------------------------//

            // 创建新的日志文件
            Logger.GetInstance().NewFile();
            String errMsg;
            if (!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加载配置文件失败," + errMsg);
                XtraMessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功");

            //-----------------------------------------------------//

            // 创建UDP Socket，加入组播，接收网络数据
            try
            {
                udpClient = new UdpClient(Config.GetInstance().port);
                udpClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr));
                
                dataParser.Start();
                dataLogger.Start();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加入组播组成功");
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加入组播组失败，" + ex.Message);
                XtraMessageBox.Show("加入组播组失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                udpClient.Close();
                
                dataParser.Stop();
                dataLogger.Stop();
                return;
            }

            //-----------------------------------------------------//

            // 开启UDP网络数据接收
            udpClient.BeginReceive(EndReceive, null);

            /*
            // 清空所有的曲线
            CHART_ITEM_INDEX = 0;
            foreach (Series series in positionChart.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in speedChart.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in pointChartControl.Series)
            {
                series.Points.Clear();
            }
            // 禁用按钮
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            chartUpateTimer.Start();

            // 绘图时上下限
            positionChart.Titles[0].Text = String.Format("X范围:{0}-{1}, Y范围{2}-{3}, Z范围{4}-{5}", Config.GetInstance().locMinX, Config.GetInstance().locMaxX,
                Config.GetInstance().locMinY, Config.GetInstance().locMaxY, Config.GetInstance().locMinZ, Config.GetInstance().locMaxZ);
            speedChart.Titles[0].Text = String.Format("Vx范围:{0}-{1}, Vy范围{2}-{3}, Vz范围{4}-{5}", Config.GetInstance().speedMinX, Config.GetInstance().speedMaxX,
                Config.GetInstance().speedMinY, Config.GetInstance().speedMaxY, Config.GetInstance().speedMinZ, Config.GetInstance().speedMaxZ);
            */
        }

        private void EndReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] recvBuffer = udpClient?.EndReceive(ar, ref endPoint);
                dataParser.Enqueue(recvBuffer);
                dataLogger.Enqueue(recvBuffer);
                udpClient.BeginReceive(EndReceive, null);
            }
            catch (Exception)
            {
            }
        }

        delegate void LogDelegate(DateTime time, Logger.LOG_LEVEL level, String msg);
        public void Log(DateTime time, Logger.LOG_LEVEL level, String msg)
        {
            if (InvokeRequired)
            {
                Invoke(new LogDelegate(Log), time, level, msg);
            }
            else
            {
                if (LogListView.Items.Count == 100)
                {
                    LogListView.Items.RemoveAt(0);
                }
                ListViewItem item = new ListViewItem
                {
                    Text = time.ToString("G")
                };
                String strLevel;
                switch (level)
                {
                    case Logger.LOG_LEVEL.LOG_INFO:
                        strLevel = "信息";
                        item.BackColor = Color.White;
                        break;
                    case Logger.LOG_LEVEL.LOG_WARN:
                        strLevel = "警告";
                        item.BackColor = Color.Yellow;
                        break;
                    case Logger.LOG_LEVEL.LOG_ERROR:
                        strLevel = "错误";
                        item.BackColor = Color.Red;
                        break;
                    default:
                        return;
                }
                item.SubItems.Add(strLevel);
                item.SubItems.Add(msg);
                LogListView.Items.Add(item);
                LogListView.EnsureVisible(LogListView.Items.Count - 1);
            }
        }
    }
}
