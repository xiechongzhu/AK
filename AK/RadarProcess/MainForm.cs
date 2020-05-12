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
using static RadarProcess.DataParser;
using DevExpress.Utils.Extensions;

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        public enum FlashType : uint
        {
            FLASHW_STOP = 0, //停止闪烁
            FALSHW_CAPTION = 1, //只闪烁标题
            FLASHW_TRAY = 2, //只闪烁任务栏
            FLASHW_ALL = 3, //标题和任务栏同时闪烁
            FLASHW_PARAM1 = 4,
            FLASHW_PARAM2 = 12,
            FLASHW_TIMER = FLASHW_TRAY | FLASHW_PARAM1, //无条件闪烁任务栏直到发送停止标志或者窗口被激活，如果未激活，停止时高亮
            FLASHW_TIMERNOFG = FLASHW_TRAY | FLASHW_PARAM2 //未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮
        }

        public struct FLASHWINFO
        {
            /// <summary>
            /// 结构大小
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// 要闪烁或停止的窗口句柄
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// 闪烁的类型
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// 闪烁窗口的次数
            /// </summary>
            public uint uCount;
            /// <summary>
            /// 窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            /// </summary>
            public uint dwTimeout;
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private System.Windows.Media.MediaPlayer player = new System.Windows.Media.MediaPlayer();
        private bool isAlertPlaying = false;
        private const int WM_USER = 0x400;
        public const int WM_RADAR_DATA = WM_USER + 100;
        public const int WM_RADAR_DATA_COMMING = WM_USER + 101;
        public const int WM_TELEMETRY_DATA_COMMING = WM_USER + 102;
        public const int WM_T0 = WM_USER + 103;
        public const int WM_YC_I = WM_USER + 104;
        public const int WM_YC_II = WM_USER + 105;

        private UdpClient udpRadarClient;
        private UdpClient udpTelemetryClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        private HistoryData historyData = new HistoryData();
        private DateTime positionAlertTime, speedAlertTime, fallPointAlertTime;
        private FallPoint ideaPoint;
        Algorithm algorithm = new Algorithm();
        public ConstLaunch constLaunchFsx;
        private AlertForm alertForm = new AlertForm();
        List<ListViewItem> logItemList = new List<ListViewItem>();
        private int MAX_CHART_POINTS = 1000;

        private Image grayLedImage;
        private Image greenLedImage;
        private Image redLedImage;
        private DateTime recvRaderNetworkDataTime;
        private DateTime recvTelemetryNetworkDataTime;

        private MyChartControl myChartControl1 = new MyChartControl();
        private MyChartControl myChartControl2 = new MyChartControl();

        public MainForm()
        {
            dataParser = new DataParser(this);
            InitializeComponent();
            myChartControl1.Dock = myChartControl2.Dock = DockStyle.Fill;
            xtraTabPage1.Controls.Add(myChartControl1);
            xtraTabPage2.Controls.Add(myChartControl2);
            btnStop.Enabled = false;
            Logger.GetInstance().SetMainForm(this);
            positionAlertTime = DateTime.MinValue;
            speedAlertTime = DateTime.MinValue;
            fallPointAlertTime = DateTime.MinValue;
            player.MediaEnded += Player_MediaEnded;
            player.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\resource\alert.mp3"));
            if (Config.GetInstance().LoadConfigFile(out _))
            {
                InitChartPoints();
                InitPositionSpeedMaxMin();
                editT0.Text = Config.GetInstance().delayT0.ToString();
            }

            grayLedImage = Image.FromFile(@"resource/LED_gray.png");
            greenLedImage = Image.FromFile(@"resource/LED_green.png");
            redLedImage = Image.FromFile(@"resource/LED_red.png");

            picRadarNetwork.Image = grayLedImage;
            picTelemetryNetwork.Image = grayLedImage;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            if(settingForm.ShowDialog() == DialogResult.OK)
            {
                if (Config.GetInstance().LoadConfigFile(out _))
                {
                    ClearAllChart();
                    InitChartPoints();
                    InitPositionSpeedMaxMin();
                }
            }
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
            MAX_CHART_POINTS = Config.GetInstance().maxPointCount;
            myChartControl1.SetMaxChartPoint(MAX_CHART_POINTS);
            myChartControl2.SetMaxChartPoint(MAX_CHART_POINTS);
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功");
            try
            { 
                udpRadarClient = new UdpClient(Config.GetInstance().radarPort);
                udpRadarClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 1024 * 1024 * 200);
                udpRadarClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strRadarMultiCastIpAddr));
                udpTelemetryClient = new UdpClient(Config.GetInstance().telemetryPort);
                udpTelemetryClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 1024 * 1024 * 200);
                udpTelemetryClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strTelemetryMultiCastIpAddr));
                dataParser.Start();
                dataLogger.Start();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加入组播组成功");
            }
            catch(Exception ex)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加入组播组失败，" + ex.Message);
                XtraMessageBox.Show("加入组播组失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                udpRadarClient?.Close();
                udpTelemetryClient?.Close();
                dataParser.Stop();
                dataLogger.Stop();
                return;
            }
            
            ClearAllChart();
            InitChartPoints();
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            if (Config.GetInstance().source == 0)
            {
                editT0.Enabled = true;
                btnStartT0.Enabled = true;
            }
            constLaunchFsx = algorithm.calc_const_launch_fsx(
                Config.GetInstance().latitudeInit,
                Config.GetInstance().longitudeInit,
                Config.GetInstance().heightInit,
                Config.GetInstance().azimuthInit);
            myChartControl1.StartTimer();
            myChartControl2.StartTimer();
            udpRadarClient.BeginReceive(EndRadarUdpReceive, null);
            udpTelemetryClient.BeginReceive(EndTelemetryUdpReceive, null);
            netWorkTimer.Start();
        }

        private void EndRadarUdpReceive(IAsyncResult ar)
        {
            if (udpRadarClient != null)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    byte[] recvBuffer = udpRadarClient.EndReceive(ar, ref endPoint);
                    dataParser.Enqueue(DataSourceType.DATA_RADER, recvBuffer);
                    dataLogger.Enqueue(DataLogger.DataSourceType.DATA_RADER, recvBuffer);
                    udpRadarClient.BeginReceive(EndRadarUdpReceive, null);
                }
                catch (Exception)
                { }
            }
        }

        private void EndTelemetryUdpReceive(IAsyncResult ar)
        {
            if (udpTelemetryClient != null)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    byte[] recvBuffer = udpTelemetryClient.EndReceive(ar, ref endPoint);
                    dataParser.Enqueue(DataSourceType.DATA_TELEMETRY, recvBuffer);
                    dataLogger.Enqueue(DataLogger.DataSourceType.DATA_TELEMETRY, recvBuffer);
                    udpTelemetryClient.BeginReceive(EndTelemetryUdpReceive, null);
                }
                catch (Exception)
                { }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            myChartControl1.StopTimer();
            myChartControl2.StopTimer();
            udpRadarClient?.Close();
            udpTelemetryClient?.Close();
            dataParser.Stop();
            dataLogger.Stop();
            Logger.GetInstance().Close();
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "退出组播组成功");
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "关闭套接字成功");
            SaveTestInfo();
            btnSetting.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            btnStartT0.Enabled = false;
            editT0.Enabled = false;
            myChartControl1.ClearData();
            myChartControl2.ClearData();
            alertForm.Hide();
            netWorkTimer.Stop();
            picRadarNetwork.Image = grayLedImage;
            picTelemetryNetwork.Image = grayLedImage;
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
                    case Logger.LOG_LEVEL.LOG_SELF_DESTRUCT:
                        strLevel = "自毁";
                        item.BackColor = Color.Red;
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
                logItemList.Add(item);
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

            historyData.LongitudeInit = Config.GetInstance().longitudeInit;
            historyData.LatitudeInit = Config.GetInstance().latitudeInit;
            historyData.HeightInit = Config.GetInstance().heightInit;
            historyData.AzimuthInit = Config.GetInstance().azimuthInit;
            historyData.PlacementHeight = Config.GetInstance().placementHeight;
            historyData.Flightshot = Config.GetInstance().flightshot;
            historyData.ForwardLine = Config.GetInstance().forwardLine;
            historyData.BackwardLine = Config.GetInstance().backwardLine;
            historyData.SideLine = Config.GetInstance().sideLine;
            historyData.StrMultiCastIpAddr = Config.GetInstance().strRadarMultiCastIpAddr;
            historyData.Port = Config.GetInstance().radarPort;
            historyData.StationId = Config.GetInstance().stationId;
            historyData.SpeedError = Config.GetInstance().speedError;
            historyData.PointError = Config.GetInstance().pointError;
            historyData.MaxPointCount = Config.GetInstance().maxPointCount;

            try
            {
                using (FileStream fs = new FileStream(TestInfo.GetInstance().strHistoryFile, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, historyData);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("保存历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            historyData.Clear();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm();
            historyForm.ShowDialog();
        }

        protected override void DefWndProc(ref Message m)
        {
            IntPtr ptr = m.LParam;
            switch (m.Msg)
            {
                case WM_RADAR_DATA:
                    if (Config.GetInstance().source == 0)
                    {
                        S_OBJECT sObject = Marshal.PtrToStructure<S_OBJECT>(ptr);
                        historyData.AddObject(sObject);
                        FallPoint fallPoint = null;
                        double distance = 0;
                        double fallTime = 0;
                        try
                        {
                            algorithm.CalcResultLd(Config.GetInstance().longitudeInit, sObject.X, sObject.Y, sObject.Z,
                                sObject.VX, sObject.VY, sObject.VZ, constLaunchFsx,
                                Config.GetInstance().placementHeight, out fallPoint, out fallTime, out distance);
                            CheckFallPoint(fallPoint, fallTime, 1);
                            historyData.AddFallPoint(fallPoint);
                        }
                        catch (Exception) { }

                        AddDisplayData(sObject.time, sObject.X, sObject.Y, sObject.Z, sObject.VX, sObject.VY, sObject.VZ,
                            sObject.MinX, sObject.MaxX, sObject.MinY, sObject.MaxY, sObject.MinZ, sObject.MaxZ,
                            sObject.MinVx, sObject.MaxVx, sObject.MinVy, sObject.MaxVy, sObject.MinVz, sObject.MaxVz,
                            fallPoint, fallTime, distance, 1);
                        CheckPosition(sObject.X, sObject.Y, sObject.Z, sObject.MinX, sObject.MaxX, sObject.MinY, sObject.MaxY, sObject.MinZ, sObject.MaxZ, 1);
                        CheckSpeed(sObject.VX, sObject.VY, sObject.VZ, sObject.MinVx, sObject.MaxVx, sObject.MinVy, sObject.MaxVy, sObject.MinVz, sObject.MaxVz, 1);
                    }
                    Marshal.FreeHGlobal(ptr);
                    break;
                case WM_RADAR_DATA_COMMING:
                    recvRaderNetworkDataTime = DateTime.Now;
                    picRadarNetwork.Image = greenLedImage;
                    break;
                case WM_TELEMETRY_DATA_COMMING:
                    recvTelemetryNetworkDataTime = DateTime.Now;
                    picTelemetryNetwork.Image = greenLedImage;
                    break;
                case WM_T0:
                    editT0.Enabled = btnStartT0.Enabled = false;
                    break;
                case WM_YC_I:
                    if (Config.GetInstance().source == 1)
                    {

                        YcMessage msg = Marshal.PtrToStructure<YcMessage>(ptr);
                        historyData.AddObject(msg.sObject);
                        FallPoint fallPoint = msg.fallPoint;
                        double fallTime = msg.fallTime;
                        double distance = msg.distance;
                        historyData.AddFallPoint(fallPoint);
                        AddDisplayData(msg.sObject.time, msg.sObject.X, msg.sObject.Y, msg.sObject.Z, msg.sObject.VX, msg.sObject.VY, msg.sObject.VZ,
                            msg.sObject.MinX, msg.sObject.MaxX, msg.sObject.MinY, msg.sObject.MaxY, msg.sObject.MinZ, msg.sObject.MaxZ,
                            msg.sObject.MinVx, msg.sObject.MaxVx, msg.sObject.MinVy, msg.sObject.MaxVy, msg.sObject.MinVz, msg.sObject.MaxVz,
                            fallPoint, fallTime, distance, 1);
                        CheckFallPoint(fallPoint, fallTime, 1);
                        CheckPosition(msg.sObject.X, msg.sObject.Y, msg.sObject.Z, msg.sObject.MinX, msg.sObject.MaxX, msg.sObject.MinY, msg.sObject.MaxY, msg.sObject.MinZ, msg.sObject.MaxZ, 1);
                        CheckSpeed(msg.sObject.VX, msg.sObject.VY, msg.sObject.VZ, msg.sObject.MinVx, msg.sObject.MaxVx, msg.sObject.MinVy, msg.sObject.MaxVy, msg.sObject.MinVz, msg.sObject.MaxVz, 1);
                    }
                    Marshal.FreeHGlobal(ptr);
                    break;
                case WM_YC_II:
                    if (Config.GetInstance().source == 1)
                    {

                        YcMessage msg = Marshal.PtrToStructure<YcMessage>(ptr);
                        historyData.AddObject(msg.sObject);
                        FallPoint fallPoint = msg.fallPoint;
                        double fallTime = msg.fallTime;
                        double distance = msg.distance;
                        historyData.AddFallPoint(fallPoint);
                        AddDisplayData(msg.sObject.time, msg.sObject.X, msg.sObject.Y, msg.sObject.Z, msg.sObject.VX, msg.sObject.VY, msg.sObject.VZ,
                            msg.sObject.MinX, msg.sObject.MaxX, msg.sObject.MinY, msg.sObject.MaxY, msg.sObject.MinZ, msg.sObject.MaxZ,
                            msg.sObject.MinVx, msg.sObject.MaxVx, msg.sObject.MinVy, msg.sObject.MaxVy, msg.sObject.MinVz, msg.sObject.MaxVz,
                            fallPoint, fallTime, distance, 2);
                        CheckFallPoint(fallPoint, fallTime, 2);
                        CheckPosition(msg.sObject.X, msg.sObject.Y, msg.sObject.Z, msg.sObject.MinX, msg.sObject.MaxX, msg.sObject.MinY, msg.sObject.MaxY, msg.sObject.MinZ, msg.sObject.MaxZ, 2);
                        CheckSpeed(msg.sObject.VX, msg.sObject.VY, msg.sObject.VZ, msg.sObject.MinVx, msg.sObject.MaxVx, msg.sObject.MinVy, msg.sObject.MaxVy, msg.sObject.MinVz, msg.sObject.MaxVz, 2);
                    }
                    Marshal.FreeHGlobal(ptr);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void AddDisplayData(int time, double x, double y, double z, double vx, double vy, double vz, 
            double minX, double maxX, double minY, double maxY, double minZ, double maxZ,
            double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz,
            FallPoint fallPoint, double fallTime, double distance, int suit)
        {
            if(suit == 1)
            {
                myChartControl1.AddDisplayData(time, x, y, z, vx, vy, vz,
                    minX, maxX, minY, maxY, minZ, maxZ,
                    minVx, maxVx, minVy, maxVy, minVz, maxVz,
                    fallPoint, fallTime, distance);
            }
            else
            {
                myChartControl2.AddDisplayData(time, x, y, z, vx, vy, vz,
                    minX, maxX, minY, maxY, minZ, maxZ,
                    minVx, maxVx, minVy, maxVy, minVz, maxVz,
                    fallPoint, fallTime, distance);
            }
        }

        private void SetFallPoint(FallPoint fallPoint, int suit)
        {
            if (suit == 1)
            {
                myChartControl1.SetFallPoint(fallPoint);
            }
            else
            {
                myChartControl2.SetFallPoint(fallPoint);
            }
        }

        private void CheckPosition(double x, double y, double z, double minX, double maxX, double minY, double maxY, double minZ, double maxZ, int suit)
        {
            if ((DateTime.Now - positionAlertTime).TotalSeconds > 5)
            {
                if (x > maxX || x < minX)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置X超出范围:" + x.ToString());
                    positionAlertTime = DateTime.Now;
                }
                if (y > maxY || y < minY)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Y超出范围:" + y.ToString());
                    positionAlertTime = DateTime.Now;
                }
                if (z > maxZ || z < minZ)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Z超出范围:" + z.ToString());
                    positionAlertTime = DateTime.Now;
                }
                if (suit == 1)
                {
                    myChartControl1.CheckPosition(x, y, z, minX, maxX, minY, maxY, minZ, maxZ);
                }
                else
                {
                    myChartControl2.CheckPosition(x, y, z, minX, maxX, minY, maxY, minZ, maxZ);
                }
            }
        }

        private void CheckSpeed(double vx, double vy, double vz, double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz, int suit)
        {
            if ((DateTime.Now - speedAlertTime).TotalSeconds > 5)
            {
                if (vx > maxVx || vx < minVx)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:" + vx.ToString());
                    speedAlertTime = DateTime.Now;
                }
                if (vy > maxVy || vy < minVy)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:" + vy.ToString());
                    speedAlertTime = DateTime.Now;
                }
                if (vz > maxVz || vz < minVz)
                {
                    ShowAlert();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:" + vz.ToString());
                    speedAlertTime = DateTime.Now;
                }
                if (suit == 1)
                {
                    myChartControl1.CheckSpeed(vx, vy, vz, minVx, maxVx, minVy, maxVy, minVz, maxVz);
                }
                else
                {
                    myChartControl2.CheckSpeed(vx, vy, vz, minVx, maxVx, minVy, maxVy, minVz, maxVz);
                }
            }
        }

        private void CheckFallPoint(FallPoint fallPoint, double fallTime, int suit)
        {
            if (fallPoint.x < -Config.GetInstance().sideLine ||
                fallPoint.x > Config.GetInstance().sideLine ||
                fallPoint.y < Config.GetInstance().backwardLine ||
                fallPoint.y > Config.GetInstance().forwardLine)
            {
                if ((DateTime.Now - fallPointAlertTime).TotalSeconds > 5)
                {
                    ShowAlert();
                    alertForm.SetAlert(ideaPoint, fallPoint, fallTime);
                    alertForm.Show();
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_SELF_DESTRUCT, String.Format("落点超出范围:X={0},Y={1}", fallPoint.x, fallPoint.y));
                    fallPointAlertTime = DateTime.Now;
                }
            }
            else
            {
                alertForm.Hide();
            }
            if (suit == 1)
            {
                myChartControl1.CheckFallPoint(fallPoint, fallTime);
            }
            else
            {
                myChartControl2.CheckFallPoint(fallPoint, fallTime);
            }
        }

        private void ClearAllChart()
        {
            myChartControl1.ClearAllChart();
            myChartControl2.ClearAllChart();
        }

        private void InitChartPoints()
        {
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            myChartControl1.InitChartPoints(ideaPoint);
            myChartControl2.InitChartPoints(ideaPoint);
        }

        private void InitPositionSpeedMaxMin()
        {
            myChartControl1.InitPositionSpeedMaxMin();
            myChartControl2.InitPositionSpeedMaxMin();
        }

        private void ShowAlert()
        {
            if (!isAlertPlaying)
            {
                isAlertPlaying = true;
                player.Position = new TimeSpan(0);
                player.Play();
            }
            FlashWindow(Handle, FlashType.FLASHW_ALL);
        }

        public static bool FlashWindow(IntPtr hWnd, FlashType type)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            fInfo.dwFlags = (uint)type;//闪烁的类型
            fInfo.uCount = 3;//闪烁窗口的次数
            fInfo.dwTimeout = 0; //窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            return FlashWindowEx(ref fInfo);
        }

        private void logTimer_Tick(object sender, EventArgs e)
        {
            if (logItemList.Count > 0)
            {
                LogListView.BeginUpdate();
                while (LogListView.Items.Count > 100)
                {
                    LogListView.Items.RemoveAt(0);
                }
                LogListView.Items.AddRange(logItemList.ToArray());
                logItemList.Clear();
                if (LogListView.Items.Count > 0)
                {
                    LogListView.EnsureVisible(LogListView.Items.Count - 1);
                }
                LogListView.EndUpdate();
            }
        }

        private void netWorkTimer_Tick(object sender, EventArgs e)
        {
            if(DateTime.Now - recvRaderNetworkDataTime > new TimeSpan(0, 0, 0, 0, 200))
            {
                picRadarNetwork.Image = redLedImage;
            }
            if(DateTime.Now - recvTelemetryNetworkDataTime > new TimeSpan(0,0,0,0,200))
            {
                picTelemetryNetwork.Image = redLedImage;
            }
        }

        private void btnStartT0_Click(object sender, EventArgs e)
        {
            Config.GetInstance().delayT0 = int.Parse(editT0.Text);
            Config.GetInstance().SaveConfig(out String _);
            dataParser.StartGetT0(Config.GetInstance().delayT0);
            btnStartT0.Enabled = false;
            editT0.Enabled = false;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            isAlertPlaying = false;
        }

        private void DisplayFallTimeAndDistance(double fallTime, double distance, int suit)
        {
            if (suit == 1)
            {
                myChartControl1.DisplayFallTimeAndDistance(fallTime, distance);
            }
            else
            {
                myChartControl2.DisplayFallTimeAndDistance(fallTime, distance);
            }
        }
    }
}
