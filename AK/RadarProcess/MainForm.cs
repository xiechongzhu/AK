using DataModels;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Linq;
using LinqToDB;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GMap.NET.MapProviders;
using GMap.NET;
using DevExpress.XtraCharts;

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        private int CHART_ITEM_INDEX = 0;
        private const int WM_USER = 0x400;
        public const int WM_RADAR_DATA = WM_USER + 100;

        private UdpClient udpClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        private List<S_OBJECT> listSObject = new List<S_OBJECT>();
        public MainForm()
        {
            dataParser = new DataParser(Handle);
            InitializeComponent();
            btnStop.Enabled = false;
            Logger.GetInstance().SetMainForm(this);
            InitGMap();
            /*SwiftPlotDiagramAxisX axisX = ((SwiftPlotDiagram)chartControl.Diagram).AxisX;
            axisX.VisualRange.SetMinMaxValues((double)axisX.WholeRange.MaxValue - 50, (double)axisX.WholeRange.MaxValue);*/
        }

        private void InitGMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMapControl.MapProvider = GoogleChinaMapProvider.Instance;
            gMapControl.MinZoom = 1;
            gMapControl.MaxZoom = 13;
            gMapControl.Zoom = 9;
            gMapControl.ShowCenter = false;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            settingForm.ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TestInfoForm testInfoForm = new TestInfoForm();
            if(testInfoForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            TestInfo.GetInstance().New();
            testInfoForm.GetTestInfo(out TestInfo.GetInstance().strTestName, out TestInfo.GetInstance().strOperator, out TestInfo.GetInstance().strComment);
            Logger.GetInstance().NewFile();
            String errMsg;
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加载配置文件失败," + errMsg);
                MessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功");
            try
            { 
                udpClient = new UdpClient(Config.GetInstance().port);
                udpClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr));
                dataParser.Start();
                dataLogger.Start();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加入组播组成功");
            }
            catch(Exception ex)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加入组播组失败，" + ex.Message);
                MessageBox.Show("加入组播组失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                udpClient.Close();
                dataParser.Stop();
                dataLogger.Stop();
                return;
            }
            CHART_ITEM_INDEX = 0;
            foreach(DevExpress.XtraCharts.Series sery in positionChart.Series)
            {
                sery.Points.Clear();
            }
            udpClient.BeginReceive(EndReceive, null);
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
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
            catch(Exception)
            { }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            udpClient?.DropMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr));
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "退出组播组成功");
            udpClient?.Close();
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "关闭套接字成功");
            dataParser.Stop();
            dataLogger.Stop();
            SaveTestInfo();
            try
            {
                using (FileStream fs = new FileStream(TestInfo.GetInstance().strHistoryFile, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, listSObject);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            listSObject.Clear();
            btnSetting.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
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
                if(LogListView.Items.Count == 100)
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStop.Enabled)
            {
                btnStop_Click(sender, e);
            }
        }

        private void SaveTestInfo()
        {
            try
            {
                var db = new DatabaseDB();
                db.Insert<DataModels.TestInfo>(new DataModels.TestInfo
                {

                    TestName = TestInfo.GetInstance().strTestName,
                    Operator = TestInfo.GetInstance().strOperator,
                    Comment = TestInfo.GetInstance().strComment,
                    Time = TestInfo.GetInstance().dateTime
                }) ;
                db.CommitTransaction();
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存试验信息失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm();
            historyForm.ShowDialog();
        }

        protected override void DefWndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_RADAR_DATA:
                    S_OBJECT sObject = (S_OBJECT)m.GetLParam(typeof(S_OBJECT));
                    listSObject.Add(sObject);
                    AddPosition(sObject.X, sObject.Y, sObject.Z);
                    AddSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    CHART_ITEM_INDEX++;
                    break;
            }
            base.DefWndProc(ref m);
        }

        private void AddPosition(double x, double y, double z)
        {
            Random random = new Random();
            x = random.Next(10, 20);
            y = random.Next(10, 20);
            z = random.Next(10, 20);
            positionChart.Series["位置X"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, x));
            positionChart.Series["位置Y"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, y));
            positionChart.Series["位置Z"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, z));
        }

        private void AddSpeed(double vx, double vy, double vz)
        {
            Random random = new Random();
            vx = random.Next(100, 1000);
            vy = random.Next(100, 1000);
            vz = random.Next(100, 1000);
            speedChart.Series["速度X"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, vx));
            speedChart.Series["速度Y"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, vy));
            speedChart.Series["速度Z"].Points.Add(new DevExpress.XtraCharts.SeriesPoint(CHART_ITEM_INDEX, vz));
        }
    }
}
