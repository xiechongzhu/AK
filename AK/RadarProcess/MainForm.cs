using DataModels;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LinqToDB;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        private int CHART_ITEM_INDEX = 0;
        private const int WM_USER = 0x400;
        public const int WM_RADAR_DATA = WM_USER + 100;
        private List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
        private List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
        private List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
        private List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
        private List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
        private List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();
        private List<FallPoint> fallPoints = new List<FallPoint>();

        private UdpClient udpClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        private HistoryData historyData = new HistoryData();
        private DateTime positionAlertTime, speedAlertTime, fallPointAlertTime;
        private FallPoint ideaPoint;
        public MainForm()
        {
            dataParser = new DataParser(Handle);
            InitializeComponent();
            btnStop.Enabled = false;
            Logger.GetInstance().SetMainForm(this);
            positionAlertTime = DateTime.Now;
            speedAlertTime = DateTime.Now;
            fallPointAlertTime = DateTime.Now;
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
                XtraMessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                XtraMessageBox.Show("加入组播组失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                udpClient.Close();
                dataParser.Stop();
                dataLogger.Stop();
                return;
            }
            CHART_ITEM_INDEX = 0;
            foreach(Series series in positionChart.Series)
            {
                series.Points.Clear();
            }
            foreach(Series series in speedChart.Series)
            {
                series.Points.Clear();
            }
            foreach(Series series in pointChartControl.Series)
            {
                series.Points.Clear();
            }
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            pointChartControl.Series["理想落点"].Points.Add(new SeriesPoint(ideaPoint.x, ideaPoint.y));
            historyData.IdeaFallPoint = ideaPoint;
            pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(ideaPoint.x - Config.GetInstance().sideLine,
                ideaPoint.y + Config.GetInstance().forwardLine, ideaPoint.y - Config.GetInstance().backwardLine));
            pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(ideaPoint.x + Config.GetInstance().sideLine,
                ideaPoint.y + Config.GetInstance().forwardLine, ideaPoint.y - Config.GetInstance().backwardLine));
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            chartUpateTimer.Start();
            udpClient.BeginReceive(EndReceive, null);
            positionChart.Titles[0].Text = String.Format("X范围:{0}-{1}, Y范围{2}-{3}, Z范围{4}-{5}", Config.GetInstance().locMinX, Config.GetInstance().locMaxX,
                Config.GetInstance().locMinY, Config.GetInstance().locMaxY, Config.GetInstance().locMinZ, Config.GetInstance().locMaxZ);
            speedChart.Titles[0].Text = String.Format("Vx范围:{0}-{1}, Vy范围{2}-{3}, Vz范围{4}-{5}", Config.GetInstance().speedMinX, Config.GetInstance().speedMaxX,
                Config.GetInstance().speedMinY, Config.GetInstance().speedMaxY, Config.GetInstance().speedMinZ, Config.GetInstance().speedMaxZ);
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
            chartUpateTimer.Stop();
            udpClient?.Close();
            dataParser.Stop();
            dataLogger.Stop();
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "退出组播组成功");
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "关闭套接字成功");
            SaveTestInfo();
            try
            {
                using (FileStream fs = new FileStream(TestInfo.GetInstance().strHistoryFile, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, historyData);
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show("保存历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            historyData.Clear();
            btnSetting.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            positionXBuffer.Clear();
            positionYBuffer.Clear();
            positionZBuffer.Clear();
            speedVxBuffer.Clear();
            speedVyBuffer.Clear();
            speedVzBuffer.Clear();
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
                XtraMessageBox.Show("保存试验信息失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    IntPtr ptr = m.LParam;
                    S_OBJECT sObject = Marshal.PtrToStructure<S_OBJECT>(ptr);
                    historyData.AddObject(sObject);
                    AddPosition(sObject.X, sObject.Y, sObject.Z);
                    AddSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    CHART_ITEM_INDEX++;
                    CheckPosition(sObject.X, sObject.Y, sObject.Z);
                    CheckSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    FallPoint fallPoint = Algorithm.CalcPointOfFall();
                    CheckFallPoint(fallPoint);
                    AddPointOfFall(fallPoint);
                    historyData.AddFallPoint(fallPoint);
                    Marshal.FreeHGlobal(ptr);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void AddPosition(double x, double y, double z)
        {
            positionXBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, x));
            positionYBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, y));
            positionZBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, z));
        }

        private void AddSpeed(double vx, double vy, double vz)
        {
            speedVxBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, vx));
            speedVyBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, vy));
            speedVzBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, vz));
        }

        private void AddPointOfFall(FallPoint fallPoint)
        {
            if(fallPoints.Count == 5)
            {
                fallPoints.RemoveAt(fallPoints.Count - 1);
            }
            fallPoints.Add(fallPoint);
            List<FallPoint> temp = new List<FallPoint>(fallPoints.ToArray());
            int i = 0;
            while(temp.Count > 0)
            {
                FallPoint point = temp[0];
                pointChartControl.Series[String.Format("落点{0}", i+1)].Points.Clear();
                pointChartControl.Series[String.Format("落点{0}", i+1)].Points.Add(new SeriesPoint(point.x, point.y));
                i++;
                temp.RemoveAt(0);
            }
        }

        private void CheckPosition(double x, double y, double z)
        {
            if(DateTime.Now < positionAlertTime.AddSeconds(10))
            {
                return;
            }
            if(x > Config.GetInstance().locMaxX || x < Config.GetInstance().locMinX)
            {
                alertControl.Show(this, "提示", "位置X超出范围:\n" + x.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置X超出范围:" + x.ToString());
                positionAlertTime = DateTime.Now;
            }
            if (y > Config.GetInstance().locMaxY || y < Config.GetInstance().locMinY)
            {
                alertControl.Show(this, "提示", "位置Y超出范围:\n" + y.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Y超出范围:" + y.ToString());
                positionAlertTime = DateTime.Now;
            }
            if (z > Config.GetInstance().locMaxZ || z < Config.GetInstance().locMinZ)
            {
                alertControl.Show(this, "提示", "位置X超出范围:\n" + z.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Z超出范围:" + z.ToString());
                positionAlertTime = DateTime.Now;
            }
        }

        private void chartUpateTimer_Tick(object sender, EventArgs e)
        {
            positionChart.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
            positionChart.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
            positionChart.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
            speedChart.Series["速度X"].Points.AddRange(speedVxBuffer.ToArray());
            speedChart.Series["速度Y"].Points.AddRange(speedVyBuffer.ToArray());
            speedChart.Series["速度Z"].Points.AddRange(speedVzBuffer.ToArray());
            positionXBuffer.Clear();
            positionYBuffer.Clear();
            positionZBuffer.Clear();
            speedVxBuffer.Clear();
            speedVyBuffer.Clear();
            speedVzBuffer.Clear();
        }

        private void CheckSpeed(double vx, double vy, double vz)
        {
            if (DateTime.Now < speedAlertTime.AddSeconds(10))
            {
                return;
            }
            if (vx > Config.GetInstance().speedMaxX || vx < Config.GetInstance().speedMinX)
            {
                alertControl.Show(this, "提示", "速度VX超出范围:\n" + vx.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:" + vx.ToString());
                speedAlertTime = DateTime.Now;
            }
            if (vy > Config.GetInstance().speedMaxY || vy < Config.GetInstance().speedMinY)
            {
                alertControl.Show(this, "提示", "速度VY超出范围:\n" + vy.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:" + vy.ToString());
                speedAlertTime = DateTime.Now;
            }
            if (vz > Config.GetInstance().speedMaxZ || vz < Config.GetInstance().speedMinZ)
            {
                alertControl.Show(this, "提示", "速度VZ超出范围:\n" + vz.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:" + vz.ToString());
                speedAlertTime = DateTime.Now;
            }
        }

        private void CheckFallPoint(FallPoint fallPoint)
        {
            if (DateTime.Now < fallPointAlertTime.AddSeconds(10))
            {
                return;
            }
            if(fallPoint.x < ideaPoint.x - Config.GetInstance().sideLine ||
                fallPoint.x > ideaPoint.x + Config.GetInstance().sideLine ||
                fallPoint.y < ideaPoint.y - Config.GetInstance().backwardLine ||
                fallPoint.y > ideaPoint.y + Config.GetInstance().forwardLine)
            {
                alertControl.Show(this, "提示", String.Format("落点超出范围:\nX={0}\nY={1}", fallPoint.x, fallPoint.y));
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, String.Format("落点超出范围:X={0},Y={1}", fallPoint.x, fallPoint.y));
                speedAlertTime = DateTime.Now;
            }
        }
    }
}
