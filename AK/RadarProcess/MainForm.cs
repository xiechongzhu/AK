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

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        public struct DisplayData
        {
            public int time;
            public double x;
            public double y;
            public double z;
            public double vx;
            public double vy;
            public double vz;
            public double minX;
            public double maxX;
            public double minY;
            public double maxY;
            public double minZ;
            public double maxZ;
            public double minVx;
            public double maxVx;
            public double minVy;
            public double maxVy;
            public double minVz;
            public double maxVz;
            public FallPoint fallPoint;
            public double fallTime;
            public double distance;
        }

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
        private List<DisplayData> displayDataList = new List<DisplayData>();
        private List<FallPoint> fallPoints = new List<FallPoint>();

        private UdpClient udpRadarClient;
        private UdpClient udpTelemetryClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        private HistoryData historyData = new HistoryData();
        private DateTime positionAlertTime, speedAlertTime, fallPointAlertTime;
        private FallPoint ideaPoint;
        Algorithm algorithm = new Algorithm();
        private ConstLaunchFsx constLaunchFsx;
        private AlertForm alertForm = new AlertForm();
        List<ListViewItem> logItemList = new List<ListViewItem>();
        private readonly int MAX_CHART_POINTS = 1000;

        private Image grayLedImage;
        private Image greenLedImage;
        private Image redLedImage;
        private DateTime recvRaderNetworkDataTime;
        private DateTime recvTelemetryNetworkDataTime;

        public MainForm()
        {
            dataParser = new DataParser(Handle);
            InitializeComponent();
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
                    ClearChartData(chartPoints);
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
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功");
            try
            { 
                udpRadarClient = new UdpClient(Config.GetInstance().radarPort);
                udpRadarClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strRadarMultiCastIpAddr));
                udpTelemetryClient = new UdpClient(Config.GetInstance().telemetryPort);
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
            btnStartT0.Enabled = true;
            editT0.Enabled = true;
            constLaunchFsx = algorithm.calc_const_launch_fsx(
                Config.GetInstance().latitudeInit,
                Config.GetInstance().longitudeInit,
                Config.GetInstance().heightInit,
                Config.GetInstance().azimuthInit);
            chartUpateTimer.Start();
            udpRadarClient.BeginReceive(EndRadarUdpReceive, null);
            udpTelemetryClient.BeginReceive(EndTelemetryUdpReceive, null);
            netWorkTimer.Start();
        }

        private void EndRadarUdpReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] recvBuffer = udpRadarClient?.EndReceive(ar, ref endPoint);
                dataParser.Enqueue(DataSourceType.DATA_RADER ,recvBuffer);
                dataLogger.Enqueue(DataLogger.DataSourceType.DATA_RADER, recvBuffer);
                udpRadarClient.BeginReceive(EndRadarUdpReceive, null);
            }
            catch(Exception)
            { }
        }

        private void EndTelemetryUdpReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] recvBuffer = udpTelemetryClient?.EndReceive(ar, ref endPoint);
                dataParser.Enqueue(DataSourceType.DATA_TELEMETRY, recvBuffer);
                dataLogger.Enqueue(DataLogger.DataSourceType.DATA_TELEMETRY, recvBuffer);
                udpTelemetryClient.BeginReceive(EndTelemetryUdpReceive, null);
            }
            catch(Exception)
            { }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            chartUpateTimer.Stop();
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
            displayDataList.Clear();
            fallPoints.Clear();
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
            switch(m.Msg)
            {
                case WM_RADAR_DATA:
                    IntPtr ptr = m.LParam;
                    S_OBJECT sObject = Marshal.PtrToStructure<S_OBJECT>(ptr);
                    historyData.AddObject(sObject);
                    FallPoint fallPoint = null;
                    double distance = 0;
                    double fallTime = 0;
                    try
                    {
                        algorithm.CalcResult(Config.GetInstance().longitudeInit, sObject.X, sObject.Y, sObject.Z,
                            sObject.VX, sObject.VY, sObject.VZ, constLaunchFsx,
                            Config.GetInstance().placementHeight, out fallPoint, out fallTime, out distance);
                        CheckFallPoint(fallPoint, fallTime);
                        historyData.AddFallPoint(fallPoint);
                    }
                    catch (Exception) { }
                    
                    AddDisplayData(sObject.time, sObject.X, sObject.Y, sObject.Z,sObject.VX, sObject.VY, sObject.VZ,
                        sObject.MinX, sObject.MaxX, sObject.MinY, sObject.MaxY, sObject.MinZ, sObject.MaxZ,
                        sObject.MinVx, sObject.MaxVx, sObject.MinVy, sObject.MaxVy, sObject.MinVz, sObject.MaxVz,
                        fallPoint, fallTime, distance);
                    CheckPosition(sObject.X, sObject.Y, sObject.Z, sObject.MinX, sObject.MaxX, sObject.MinY, sObject.MaxY, sObject.MinZ, sObject.MaxZ);
                    CheckSpeed(sObject.VX, sObject.VY, sObject.VZ, sObject.MinVx, sObject.MaxVx, sObject.MinVy, sObject.MaxVy, sObject.MinVz, sObject.MaxVz);
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
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void AddDisplayData(int time, double x, double y, double z, double vx, double vy, double vz, 
            double minX, double maxX, double minY, double maxY, double minZ, double maxZ,
            double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz,
            FallPoint fallPoint, double fallTime, double distance)
        {
            displayDataList.Add(new DisplayData
            {
                time = time,
                x = x,
                y = y,
                z = z,
                vx = vx,
                vy = vy,
                vz = vz,
                minX = minX,
                maxX = maxX,
                minY = minY,
                maxY = maxY,
                minZ = minZ,
                maxZ = maxZ,
                minVx = minVx,
                maxVx = maxVx,
                minVy = minVy,
                maxVy = maxVy,
                minVz = minVz,
                maxVz = maxVz,
                fallPoint = fallPoint,
                fallTime = fallTime,
                distance = distance
            });;
        }

        private void SetFallPoint(FallPoint fallPoint)
        {
            if (fallPoint != null)
            {
                chartPoints.Series["预测落点"].Points.Clear();
                chartPoints.Series["预测落点"].Points.Add(new SeriesPoint(fallPoint.x, fallPoint.y));
                chartPoints.Series["落点误差"].Points.Clear();
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x - Config.GetInstance().pointError, 
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x + Config.GetInstance().pointError,
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));
            }
        }

        private void CheckPosition(double x, double y, double z, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            if(x > maxX|| x < minX)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置X超出范围:" + x.ToString());
                positionAlertTime = DateTime.Now;
                chartX.BackColor = Color.Red;
            }
            else
            {
                chartX.BackColor = Color.White;
            }
            if (y > maxY || y < minY)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Y超出范围:" + y.ToString());
                positionAlertTime = DateTime.Now;
                chartY.BackColor = Color.Red;
            }
            else
            {
                chartY.BackColor = Color.White;
            }
            if (z > maxZ || z < minZ)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Z超出范围:" + z.ToString());
                positionAlertTime = DateTime.Now;
                chartZ.BackColor = Color.Red;
            }
            else
            {
                chartZ.BackColor = Color.White;
            }
        }

        private void chartUpateTimer_Tick(object sender, EventArgs e)
        {
            if (displayDataList.Count == 0)
            {
                return;
            }

            List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMinXBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMaxXBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMinYBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMaxYBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMinZBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionMaxZBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMinVxBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMaxVxBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMinVyBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMaxVyBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMinVzBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedMaxVzBuffer = new List<SeriesPoint>();

            foreach (DisplayData displayData in displayDataList)
            {
                positionXBuffer.Add(new SeriesPoint(displayData.time, displayData.x));
                positionMinXBuffer.Add(new SeriesPoint(displayData.time, displayData.minX));
                positionMaxXBuffer.Add(new SeriesPoint(displayData.time, displayData.maxX));
                positionYBuffer.Add(new SeriesPoint(displayData.time, displayData.y));
                positionMinYBuffer.Add(new SeriesPoint(displayData.time, displayData.minY));
                positionMaxYBuffer.Add(new SeriesPoint(displayData.time, displayData.maxY));
                positionZBuffer.Add(new SeriesPoint(displayData.time, displayData.z));
                positionMinZBuffer.Add(new SeriesPoint(displayData.time, displayData.minZ));
                positionMaxZBuffer.Add(new SeriesPoint(displayData.time, displayData.maxZ));
                speedVxBuffer.Add(new SeriesPoint(displayData.time, displayData.vx));
                speedMinVxBuffer.Add(new SeriesPoint(displayData.time, displayData.minVx));
                speedMaxVxBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVx));
                speedVyBuffer.Add(new SeriesPoint(displayData.time, displayData.vy));
                speedMinVyBuffer.Add(new SeriesPoint(displayData.time, displayData.minVy));
                speedMaxVyBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVy));
                speedVzBuffer.Add(new SeriesPoint(displayData.time, displayData.vz));
                speedMinVzBuffer.Add(new SeriesPoint(displayData.time, displayData.minVz));
                speedMaxVzBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVz));

                SetFallPoint(displayData.fallPoint);
            }

            DisplayFallTimeAndDistance(displayDataList[displayDataList.Count - 1].fallTime, displayDataList[displayDataList.Count - 1].distance);

            chartX.BeginInit();
            chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
            if (chartX.Series["位置X"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X"].Points.RemoveRange(0, chartX.Series["位置X"].Points.Count - MAX_CHART_POINTS);
            }
            chartX.Series["位置X上限"].Points.AddRange(positionMaxXBuffer.ToArray());
            if (chartX.Series["位置X上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X上限"].Points.RemoveRange(0, chartX.Series["位置X上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartX.Series["位置X下限"].Points.AddRange(positionMinXBuffer.ToArray());
            if (chartX.Series["位置X下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X下限"].Points.RemoveRange(0, chartX.Series["位置X下限"].Points.Count - MAX_CHART_POINTS);
            }
            double distanceHigh = displayDataList[displayDataList.Count-1].maxX - displayDataList[displayDataList.Count - 1].x;
            double distanceLow = displayDataList[displayDataList.Count - 1].x - displayDataList[displayDataList.Count - 1].minX;
            chartX.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartX.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].x);
            chartX.EndInit();

            chartY.BeginInit();
            chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
            if (chartY.Series["位置Y"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y"].Points.RemoveRange(0, chartY.Series["位置Y"].Points.Count - MAX_CHART_POINTS);
            }
            chartY.Series["位置Y上限"].Points.AddRange(positionMaxYBuffer.ToArray());
            if (chartY.Series["位置Y上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y上限"].Points.RemoveRange(0, chartY.Series["位置Y上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartY.Series["位置Y下限"].Points.AddRange(positionMinYBuffer.ToArray());
            if (chartY.Series["位置Y下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y下限"].Points.RemoveRange(0, chartY.Series["位置Y下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxY - displayDataList[displayDataList.Count - 1].y;
            distanceLow = displayDataList[displayDataList.Count - 1].y - displayDataList[displayDataList.Count - 1].minY;
            chartY.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartY.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].y);
            chartY.EndInit();

            chartZ.BeginInit();
            chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
            if (chartZ.Series["位置Z"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z"].Points.RemoveRange(0, chartZ.Series["位置Z"].Points.Count - MAX_CHART_POINTS);
            }
            chartZ.Series["位置Z上限"].Points.AddRange(positionMaxZBuffer.ToArray());
            if (chartZ.Series["位置Z上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z上限"].Points.RemoveRange(0, chartZ.Series["位置Z上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartZ.Series["位置Z下限"].Points.AddRange(positionMinZBuffer.ToArray());
            if (chartZ.Series["位置Z下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z下限"].Points.RemoveRange(0, chartZ.Series["位置Z下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxZ - displayDataList[displayDataList.Count - 1].z;
            distanceLow = displayDataList[displayDataList.Count - 1].z - displayDataList[displayDataList.Count - 1].minZ;
            chartZ.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartZ.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].z);
            chartZ.EndInit();

            chartVx.BeginInit();
            chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());
            if (chartVx.Series["速度VX"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX"].Points.RemoveRange(0, chartVx.Series["速度VX"].Points.Count - MAX_CHART_POINTS);
            }
            chartVx.Series["速度VX上限"].Points.AddRange(speedMaxVxBuffer.ToArray());
            if (chartVx.Series["速度VX上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX上限"].Points.RemoveRange(0, chartVx.Series["速度VX上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVx.Series["速度VX下限"].Points.AddRange(speedMinVxBuffer.ToArray());
            if (chartVx.Series["速度VX下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX下限"].Points.RemoveRange(0, chartVx.Series["速度VX下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVx - displayDataList[displayDataList.Count - 1].vx;
            distanceLow = displayDataList[displayDataList.Count - 1].vx - displayDataList[displayDataList.Count - 1].minVx;
            chartVx.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVx.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vx);
            chartVx.Series["速度VX误差"].Points.Clear();
            chartVx.Series["速度VX误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time, 
                displayDataList[displayDataList.Count - 1].vx + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vx - Config.GetInstance().speedError));
            chartVx.EndInit();

            chartVy.BeginInit();
            chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());
            if (chartVy.Series["速度VY"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY"].Points.RemoveRange(0, chartVy.Series["速度VY"].Points.Count - MAX_CHART_POINTS);
            }
            chartVy.Series["速度VY上限"].Points.AddRange(speedMaxVyBuffer.ToArray());
            if (chartVy.Series["速度VY上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY上限"].Points.RemoveRange(0, chartVy.Series["速度VY上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVy.Series["速度VY下限"].Points.AddRange(speedMinVyBuffer.ToArray());
            if (chartVy.Series["速度VY下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY下限"].Points.RemoveRange(0, chartVy.Series["速度VY下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVy - displayDataList[displayDataList.Count - 1].vy;
            distanceLow = displayDataList[displayDataList.Count - 1].vy - displayDataList[displayDataList.Count - 1].minVy;
            chartVy.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVy.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vy);
            chartVy.Series["速度VY误差"].Points.Clear();
            chartVy.Series["速度VY误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time,
                displayDataList[displayDataList.Count - 1].vy + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vy - Config.GetInstance().speedError));
            chartVy.EndInit();

            chartVz.BeginInit();
            chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());
            if (chartVz.Series["速度VZ"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ"].Points.RemoveRange(0, chartVz.Series["速度VZ"].Points.Count - MAX_CHART_POINTS);
            }
            chartVz.Series["速度VZ上限"].Points.AddRange(speedMaxVzBuffer.ToArray());
            if (chartVz.Series["速度VZ上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ上限"].Points.RemoveRange(0, chartVz.Series["速度VZ上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVz.Series["速度VZ下限"].Points.AddRange(speedMinVzBuffer.ToArray());
            if (chartVz.Series["速度VZ下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ下限"].Points.RemoveRange(0, chartVz.Series["速度VZ下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVz - displayDataList[displayDataList.Count - 1].vz;
            distanceLow = displayDataList[displayDataList.Count - 1].vz - displayDataList[displayDataList.Count - 1].minVz;
            chartVz.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVz.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vz);
            chartVz.Series["速度VZ误差"].Points.Clear();
            chartVz.Series["速度VZ误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time,
                displayDataList[displayDataList.Count - 1].vz + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vz - Config.GetInstance().speedError));
            chartVz.EndInit();

            displayDataList.Clear();
        }

        private void CheckSpeed(double vx, double vy, double vz, double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz)
        {
            if (vx > maxVx|| vx < minVx)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:" + vx.ToString());
                speedAlertTime = DateTime.Now;
                chartVx.BackColor = Color.Red;
            }
            else
            {
                chartVx.BackColor = Color.White;
            }
            if (vy > maxVy || vy < minVy)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:" + vy.ToString());
                speedAlertTime = DateTime.Now;
                chartVy.BackColor = Color.Red;
            }
            else
            {
                chartVy.BackColor = Color.White;
            }
            if (vz > maxVz || vz < minVz)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:" + vz.ToString());
                speedAlertTime = DateTime.Now;
                chartVz.BackColor = Color.Red;
            }
            else
            {
                chartVz.BackColor = Color.White;
            }
        }

        private void CheckFallPoint(FallPoint fallPoint, double fallTime)
        {
            if(fallPoint.x < -Config.GetInstance().sideLine ||
                fallPoint.x >Config.GetInstance().sideLine ||
                fallPoint.y < Config.GetInstance().backwardLine ||
                fallPoint.y > Config.GetInstance().forwardLine)
            {
                ShowAlert();
                alertForm.SetAlert(ideaPoint, fallPoint, fallTime);
                alertForm.Show();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_SELF_DESTRUCT, String.Format("落点超出范围:X={0},Y={1}", fallPoint.x, fallPoint.y));
                chartPoints.BackColor = Color.Red;
            }
            else
            {
                chartPoints.BackColor = Color.White;
                alertForm.Hide();
            }
        }

        private void ClearAllChart()
        {
            ClearChartData(chartX);
            ClearChartData(chartY);
            ClearChartData(chartZ);
            ClearChartData(chartVx);
            ClearChartData(chartVy);
            ClearChartData(chartVz);
            ClearChartData(chartPoints);
        }

        private void ClearChartData(ChartControl chartControl)
        {
            foreach (Series series in chartControl.Series)
            {
                series.Points.Clear();
            }
            foreach (ChartTitle title in chartControl.Titles)
            {
                title.Text = String.Empty;
            }
            foreach(TextAnnotation textAnnotation in chartControl.Annotations)
            {
                textAnnotation.Text = String.Empty;
            }
            chartControl.BackColor = Color.White;
        }

        private void InitChartPoints()
        {
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(ideaPoint.x, ideaPoint.y));
            historyData.IdeaFallPoint = ideaPoint;
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(- Config.GetInstance().sideLine,
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(Config.GetInstance().sideLine,
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));
        }

        private void InitPositionSpeedMaxMin()
        {
            chartX.BeginInit();
            chartX.Series["位置X上限"].Points.Clear();
            chartX.Series["位置X下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartX.Series["位置X上限"].Points.AddPoint(item.Time, item.MaxX);
                chartX.Series["位置X下限"].Points.AddPoint(item.Time, item.MinX);
            });
            chartX.EndInit();

            chartY.BeginInit();
            chartY.Series["位置Y上限"].Points.Clear();
            chartY.Series["位置Y下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartY.Series["位置Y上限"].Points.AddPoint(item.Time, item.MaxY);
                chartY.Series["位置Y下限"].Points.AddPoint(item.Time, item.MinY);
            });
            chartY.EndInit();

            chartZ.BeginInit();
            chartZ.Series["位置Z上限"].Points.Clear();
            chartZ.Series["位置Z下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartZ.Series["位置Z上限"].Points.AddPoint(item.Time, item.MaxZ);
                chartZ.Series["位置Z下限"].Points.AddPoint(item.Time, item.MinZ);
            });
            chartZ.EndInit();

            chartVx.BeginInit();
            chartVx.Series["速度VX上限"].Points.Clear();
            chartVx.Series["速度VX下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVx.Series["速度VX上限"].Points.AddPoint(item.Time, item.MaxVx);
                chartVx.Series["速度VX下限"].Points.AddPoint(item.Time, item.MinVx);
            });
            chartVx.EndInit();

            chartVy.BeginInit();
            chartVy.Series["速度VY上限"].Points.Clear();
            chartVy.Series["速度VY下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVy.Series["速度VY上限"].Points.AddPoint(item.Time, item.MaxVy);
                chartVy.Series["速度VY下限"].Points.AddPoint(item.Time, item.MinVy);
            });
            chartVy.EndInit();

            chartVz.BeginInit();
            chartVz.Series["速度VZ上限"].Points.Clear();
            chartVz.Series["速度VZ下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVz.Series["速度VZ上限"].Points.AddPoint(item.Time, item.MaxVz);
                chartVz.Series["速度VZ下限"].Points.AddPoint(item.Time, item.MinVz);
            });
            chartVz.EndInit();
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

        private void DisplayFallTimeAndDistance(double fallTime, double distance)
        {
            chartPoints.Titles[0].Text = String.Format("剩余落地时间:{0:F}s, 射程:{1:F}m", fallTime, distance);
        }
    }
}
