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
using DevExpress.XtraEditors;

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
        private DateTime positionAlertTime, speedAlertTime;
        public MainForm()
        {
            dataParser = new DataParser(Handle);
            InitializeComponent();
            btnStop.Enabled = false;
            Logger.GetInstance().SetMainForm(this);
            InitGMap();
            positionAlertTime = DateTime.Now;
            speedAlertTime = DateTime.Now;
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
            foreach(Series sery in positionChart.Series)
            {
                sery.Points.Clear();
            }
            foreach(Series sery in speedChart.Series)
            {
                sery.Points.Clear();
            }
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            udpClient.BeginReceive(EndReceive, null);
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
                    formatter.Serialize(fs, listSObject);
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show("保存历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    try
                    {
                        S_OBJECT sObject = (S_OBJECT)m.GetLParam(typeof(S_OBJECT));
                        listSObject.Add(sObject);
                        AddPosition(sObject.X, sObject.Y, sObject.Z);
                        AddSpeed(sObject.VX, sObject.VY, sObject.VZ);
                        CHART_ITEM_INDEX++;
                        CheckPosition(sObject.X, sObject.Y, sObject.Z);
                        CheckSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    }
                    catch (Exception)
                    { }
                    break;
            }
            base.DefWndProc(ref m);
        }

        private void AddPosition(double x, double y, double z)
        {
            positionChart.Series["位置X"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, x));
            positionChart.Series["位置Y"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, y));
            positionChart.Series["位置Z"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, z));
        }

        private void AddSpeed(double vx, double vy, double vz)
        {
            speedChart.Series["速度X"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, vx));
            speedChart.Series["速度Y"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, vy));
            speedChart.Series["速度Z"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, vz));
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
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置X超出范围:\n" + x.ToString());
            }
            if (y > Config.GetInstance().locMaxY || y < Config.GetInstance().locMinY)
            {
                alertControl.Show(this, "提示", "位置Y超出范围:\n" + y.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Y超出范围:\n" + y.ToString());
            }
            if (z > Config.GetInstance().locMaxZ || z < Config.GetInstance().locMinZ)
            {
                alertControl.Show(this, "提示", "位置X超出范围:\n" + z.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Z超出范围:\n" + z.ToString());
            }
            positionAlertTime = DateTime.Now;
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
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:\n" + vx.ToString());
            }
            if (vy > Config.GetInstance().speedMaxY || vy < Config.GetInstance().speedMinY)
            {
                alertControl.Show(this, "提示", "速度VY超出范围:\n" + vy.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:\n" + vy.ToString());
            }
            if (vz > Config.GetInstance().speedMaxZ || vz < Config.GetInstance().speedMinZ)
            {
                alertControl.Show(this, "提示", "速度VZ超出范围:\n" + vz.ToString());
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:\n" + vz.ToString());
            }
            speedAlertTime = DateTime.Now;
        }
    }
}
