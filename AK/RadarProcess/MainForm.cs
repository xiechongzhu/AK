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
using System.Media;
using System.Resources;

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        public struct DisplayData
        {
            public int coordinate;
            public double x;
            public double y;
            public double z;
            public double vx;
            public double vy;
            public double vz;
            public FallPoint fallPoint;
            public double fallTime;
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
        private int CHART_ITEM_INDEX = 0;
        private const int WM_USER = 0x400;
        public const int WM_RADAR_DATA = WM_USER + 100;
        private List<DisplayData> displayDataList = new List<DisplayData>();
        private List<FallPoint> fallPoints = new List<FallPoint>();

        private UdpClient udpClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        private HistoryData historyData = new HistoryData();
        private DateTime positionAlertTime, speedAlertTime, fallPointAlertTime;
        private FallPoint ideaPoint;
        private ConstantCalculateOutput constantCalculateOutput;
        private AlertForm alertForm = new AlertForm();
        List<ListViewItem> logItemList = new List<ListViewItem>();

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
            }
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
            ClearAllChart();
            InitChartPoints();
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            constantCalculateOutput = Algorithm.ConstantCalculate(new ConstantCalculateInput
            {
                dLambd0_c = Config.GetInstance().longitudeInit,
                dB0 = Config.GetInstance().latitudeInit,
                dA0 = Config.GetInstance().azimuthInit,
                dh0 = Config.GetInstance().heightInit
            });
            chartUpateTimer.Start();
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
            chartUpateTimer.Stop();
            udpClient?.Close();
            dataParser.Stop();
            dataLogger.Stop();
            Logger.GetInstance().Close();
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "退出组播组成功");
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "关闭套接字成功");
            SaveTestInfo();
            btnSetting.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            displayDataList.Clear();
            alertForm.Hide();
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
            historyData.LocMaxX = Config.GetInstance().locMaxX;
            historyData.LocMinX = Config.GetInstance().locMinX;
            historyData.LocMaxY = Config.GetInstance().locMaxY;
            historyData.LocMinY = Config.GetInstance().locMinY;
            historyData.LocMaxZ = Config.GetInstance().locMaxZ;
            historyData.LocMinZ = Config.GetInstance().locMinZ;
            historyData.SpeedMaxX = Config.GetInstance().speedMaxX;
            historyData.SpeedMinX = Config.GetInstance().speedMinX;
            historyData.SpeedMaxY = Config.GetInstance().speedMaxY;
            historyData.SpeedMinY = Config.GetInstance().speedMinY;
            historyData.SpeedMaxZ = Config.GetInstance().speedMaxZ;
            historyData.SpeedMinZ = Config.GetInstance().speedMinZ;
            historyData.ForwardLine = Config.GetInstance().forwardLine;
            historyData.BackwardLine = Config.GetInstance().backwardLine;
            historyData.SideLine = Config.GetInstance().sideLine;
            historyData.StrMultiCastIpAddr = Config.GetInstance().strMultiCastIpAddr;
            historyData.Port = Config.GetInstance().port;

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
                    FallPoint fallPoint;
                    double fallTime;
                    if (Algorithm.CalcResult(Config.GetInstance().flightshot, constantCalculateOutput, sObject.X, sObject.Y, sObject.Z,
                        sObject.VX, sObject.VY, sObject.VZ, Config.GetInstance().placementHeight,
                        Config.GetInstance().azimuthInit,out fallPoint, out fallTime))
                    {
                        CheckFallPoint(fallPoint, fallTime);
                        historyData.AddFallPoint(fallPoint);
                    }
                    AddDisplayData(CHART_ITEM_INDEX++, sObject.X, sObject.Y, sObject.Z, sObject.VX, sObject.VY, sObject.VZ, fallPoint, fallTime);
                    CheckPosition(sObject.X, sObject.Y, sObject.Z);
                    CheckSpeed(sObject.VX, sObject.VY, sObject.VZ);
                    Marshal.FreeHGlobal(ptr);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void AddDisplayData(int coordinate, double x, double y, double z, double vx, double vy, double vz, FallPoint fallPoint, double fallTime)
        {
            displayDataList.Add(new DisplayData
            {
                coordinate = coordinate,
                x = x,
                y = y,
                z = z,
                vx = vx,
                vy = vy,
                vz = vz,
                fallPoint = fallPoint,
                fallTime = fallTime
            });
        }

        private void AddPointOfFall(FallPoint fallPoint)
        {
            if(fallPoints.Count == 5)
            {
                fallPoints.RemoveAt(0);
            }
            fallPoints.Add(fallPoint);
            List<FallPoint> temp = new List<FallPoint>(fallPoints.ToArray());
            int i = 0;
            while(temp.Count > 0)
            {
                FallPoint point = temp[0];
                chartPoints.Series[String.Format("落点{0}", i+1)].Points.Clear();
                chartPoints.Series[String.Format("落点{0}", i+1)].Points.Add(new SeriesPoint(point.x, point.y));
                i++;
                temp.RemoveAt(0);
            }
        }

        private void CheckPosition(double x, double y, double z)
        {
            if(x > Config.GetInstance().locMaxX || x < Config.GetInstance().locMinX)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置X超出范围:" + x.ToString());
                positionAlertTime = DateTime.Now;
            }
            if (y > Config.GetInstance().locMaxY || y < Config.GetInstance().locMinY)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Y超出范围:" + y.ToString());
                positionAlertTime = DateTime.Now;
            }
            if (z > Config.GetInstance().locMaxZ || z < Config.GetInstance().locMinZ)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "位置Z超出范围:" + z.ToString());
                positionAlertTime = DateTime.Now;
            }
        }

        private void chartUpateTimer_Tick(object sender, EventArgs e)
        {
            if (displayDataList.Count == 0)
            {
                return;
            }

            List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
            List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
            List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();

            foreach(DisplayData displayData in displayDataList)
            {
                positionXBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.x));
                positionYBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.y));
                positionZBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.z));
                speedVxBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.vx));
                speedVyBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.vy));
                speedVzBuffer.Add(new SeriesPoint(displayData.coordinate, displayData.vz));
                AddPointOfFall(displayData.fallPoint);   
            }
            DisplayFallTime(displayDataList[displayDataList.Count-1].fallTime);

            chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
            chartX.Series["位置X上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionXBuffer[0].Argument, Config.GetInstance().locMaxX),
                new SeriesPoint(positionXBuffer[positionXBuffer.Count-1].Argument, Config.GetInstance().locMaxX) });
            chartX.Series["位置X下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionXBuffer[0].Argument, Config.GetInstance().locMinX),
                new SeriesPoint(positionXBuffer[positionXBuffer.Count-1].Argument, Config.GetInstance().locMinX) });
            double distanceHigh = Config.GetInstance().locMaxX - positionXBuffer[positionXBuffer.Count - 1].Values[0];
            double distanceLow = positionXBuffer[positionXBuffer.Count - 1].Values[0] - Config.GetInstance().locMinX;
            chartX.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartX.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartX.Annotations[0]).Text = String.Format("{0:F}", positionXBuffer[positionXBuffer.Count - 1].Values[0]);
                
            chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
            chartY.Series["位置Y上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionYBuffer[0].Argument, Config.GetInstance().locMaxY),
                new SeriesPoint(positionYBuffer[positionYBuffer.Count-1].Argument, Config.GetInstance().locMaxY) });
            chartY.Series["位置Y下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionYBuffer[0].Argument, Config.GetInstance().locMinY),
                new SeriesPoint(positionYBuffer[positionYBuffer.Count-1].Argument, Config.GetInstance().locMinY) });
            distanceHigh = Config.GetInstance().locMaxY - positionYBuffer[positionYBuffer.Count - 1].Values[0];
            distanceLow = positionYBuffer[positionYBuffer.Count - 1].Values[0] - Config.GetInstance().locMinY;
            chartY.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartY.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartY.Annotations[0]).Text = String.Format("{0:F}", positionYBuffer[positionYBuffer.Count - 1].Values[0]);

            chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
            chartZ.Series["位置Z上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionZBuffer[0].Argument, Config.GetInstance().locMaxZ),
                new SeriesPoint(positionZBuffer[positionZBuffer.Count-1].Argument, Config.GetInstance().locMaxZ) });
            chartZ.Series["位置Z下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(positionZBuffer[0].Argument, Config.GetInstance().locMinZ),
                new SeriesPoint(positionZBuffer[positionZBuffer.Count-1].Argument, Config.GetInstance().locMinZ) });
            distanceHigh = Config.GetInstance().locMaxZ - positionZBuffer[positionXBuffer.Count - 1].Values[0];
            distanceLow = positionZBuffer[positionZBuffer.Count - 1].Values[0] - Config.GetInstance().locMinZ;
            chartZ.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartZ.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartZ.Annotations[0]).Text = String.Format("{0:F}", positionZBuffer[positionZBuffer.Count - 1].Values[0]);

            chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());
            chartVx.Series["速度VX上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVxBuffer[0].Argument, Config.GetInstance().speedMaxX),
                new SeriesPoint(speedVxBuffer[speedVxBuffer.Count-1].Argument, Config.GetInstance().speedMaxX) });
            chartVx.Series["速度VX下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVxBuffer[0].Argument, Config.GetInstance().speedMinX),
                new SeriesPoint(speedVxBuffer[speedVxBuffer.Count-1].Argument, Config.GetInstance().speedMinX) });
            distanceHigh = Config.GetInstance().speedMaxX - speedVxBuffer[speedVxBuffer.Count - 1].Values[0];
            distanceLow = speedVxBuffer[speedVxBuffer.Count - 1].Values[0] - Config.GetInstance().speedMinX;
            chartVx.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartVx.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartVx.Annotations[0]).Text = String.Format("{0:F}", speedVxBuffer[speedVxBuffer.Count - 1].Values[0]);

            chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());
            chartVy.Series["速度VY上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVyBuffer[0].Argument, Config.GetInstance().speedMaxY),
                new SeriesPoint(speedVyBuffer[speedVyBuffer.Count-1].Argument, Config.GetInstance().speedMaxY) });
            chartVy.Series["速度VY下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVyBuffer[0].Argument, Config.GetInstance().speedMinY),
                new SeriesPoint(speedVyBuffer[speedVyBuffer.Count-1].Argument, Config.GetInstance().speedMinY) });
            distanceHigh = Config.GetInstance().speedMaxY - speedVyBuffer[speedVyBuffer.Count - 1].Values[0];
            distanceLow = speedVyBuffer[speedVyBuffer.Count - 1].Values[0] - Config.GetInstance().speedMinY;
            chartVy.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartVy.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartVy.Annotations[0]).Text = String.Format("{0:F}", speedVyBuffer[speedVyBuffer.Count - 1].Values[0]);

            chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());
            chartVz.Series["速度VZ上限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVzBuffer[0].Argument, Config.GetInstance().speedMaxZ),
                new SeriesPoint(speedVzBuffer[speedVzBuffer.Count-1].Argument, Config.GetInstance().speedMaxZ) });
            chartVz.Series["速度VZ下限"].Points.AddRange(new SeriesPoint[] {
                new SeriesPoint(speedVzBuffer[0].Argument, Config.GetInstance().speedMinZ),
                new SeriesPoint(speedVzBuffer[speedVzBuffer.Count-1].Argument, Config.GetInstance().speedMinZ) });
            distanceHigh = Config.GetInstance().speedMaxZ - speedVzBuffer[speedVzBuffer.Count - 1].Values[0];
            distanceLow = speedVzBuffer[speedVzBuffer.Count - 1].Values[0] - Config.GetInstance().speedMinZ;
            chartVz.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            chartVz.Titles[0].TextColor = distanceHigh < 0 || distanceLow < 0 ? Color.Red : Color.Black;
            ((TextAnnotation)chartVz.Annotations[0]).Text = String.Format("{0:F}", speedVzBuffer[speedVzBuffer.Count - 1].Values[0]);

            displayDataList.Clear();
        }

        private void CheckSpeed(double vx, double vy, double vz)
        {
            if (vx > Config.GetInstance().speedMaxX || vx < Config.GetInstance().speedMinX)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:" + vx.ToString());
                speedAlertTime = DateTime.Now;
            }
            if (vy > Config.GetInstance().speedMaxY || vy < Config.GetInstance().speedMinY)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:" + vy.ToString());
                speedAlertTime = DateTime.Now;
            }
            if (vz > Config.GetInstance().speedMaxZ || vz < Config.GetInstance().speedMinZ)
            {
                ShowAlert();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:" + vz.ToString());
                speedAlertTime = DateTime.Now;
            }
        }

        private void CheckFallPoint(FallPoint fallPoint, double fallTime)
        {
            if(fallPoint.x < ideaPoint.x - Config.GetInstance().sideLine ||
                fallPoint.x > ideaPoint.x + Config.GetInstance().sideLine ||
                fallPoint.y < ideaPoint.y - Config.GetInstance().backwardLine ||
                fallPoint.y > ideaPoint.y + Config.GetInstance().forwardLine)
            {
                ShowAlert();
                alertForm.SetAlert(ideaPoint, fallPoint, fallTime);
                alertForm.Show();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, String.Format("落点超出范围:X={0},Y={1}", fallPoint.x, fallPoint.y));
            }
            else
            {
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
        }

        private void InitChartPoints()
        {
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(ideaPoint.x, ideaPoint.y));
            historyData.IdeaFallPoint = ideaPoint;
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(ideaPoint.x - Config.GetInstance().sideLine,
                ideaPoint.y + Config.GetInstance().forwardLine, ideaPoint.y - Config.GetInstance().backwardLine));
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(ideaPoint.x + Config.GetInstance().sideLine,
                ideaPoint.y + Config.GetInstance().forwardLine, ideaPoint.y - Config.GetInstance().backwardLine));
        }

        private void PlayAlertSound()
        {
            if(!isAlertPlaying)
            {
                isAlertPlaying = true;
                player.Position = new TimeSpan(0);
                player.Play();
            }
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

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            isAlertPlaying = false;
        }

        private void DisplayFallTime(double fallTime)
        {
            chartPoints.Titles[0].Text = String.Format("预计落地时间:{0:F}s", fallTime);
        }
    }
}
