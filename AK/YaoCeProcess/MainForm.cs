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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YaoCeProcess
{
    public partial class MainForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量

        //--------//
        // 创建曲线X轴索引值
        private int xiTong_CHART_ITEM_INDEX = 0;
        // 系统判决状态绘图数据缓存
        private List<SeriesPoint> xiTong_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();       // 经度
        private List<SeriesPoint> xiTong_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();        // 纬度
        private List<SeriesPoint> xiTong_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();        // 海拔高度

        private List<SeriesPoint> xiTong_SuDu_DongXiang_Buffer = new List<SeriesPoint>();       // 东向速度
        private List<SeriesPoint> xiTong_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();        // 北向速度
        private List<SeriesPoint> xiTong_SuDu_TianXiang_Buffer = new List<SeriesPoint>();       // 天向速度

        private List<SeriesPoint> xiTong_JiaoSuDu_Wx_Buffer = new List<SeriesPoint>();          // Wx角速度
        private List<SeriesPoint> xiTong_JiaoSuDu_Wy_Buffer = new List<SeriesPoint>();          // Wy角速度
        private List<SeriesPoint> xiTong_JiaoSuDu_Wz_Buffer = new List<SeriesPoint>();          // Wz角速度

        private List<SeriesPoint> xiTong_ZhouXiangGuoZai_Buffer = new List<SeriesPoint>();      // 轴向过载
        private List<SeriesPoint> xiTong_FaSheXi_x_Buffer = new List<SeriesPoint>();            // 发射系X
        private List<SeriesPoint> xiTong_FaSheXi_y_Buffer = new List<SeriesPoint>();            // 发射系Y
        private List<SeriesPoint> xiTong_FaSheXi_z_Buffer = new List<SeriesPoint>();            // 发射系Z

        private List<SeriesPoint> xiTong_YuShiLuoDian_SheCheng_Buffer = new List<SeriesPoint>();// 预示落点射程
        private List<SeriesPoint> xiTong_YuShiLuoDian_Z_Buffer = new List<SeriesPoint>();       // 预示落点Z

        //-----------------------------------------------------//

        private const int WM_USER = 0x400;
        // 系统判决状态数据标识
        public const int WM_YAOCE_SystemStatus_DATA = WM_USER + 102;
        // 导航数据（快速）标识
        public const int WM_YAOCE_daoHangKuaiSu_DATA = WM_USER + 103;
        // 导航数据（慢速）标识
        public const int WM_YAOCE_daoHangManSu_DATA = WM_USER + 104;

        // UDP
        private UdpClient udpClient;
        // 码流记录
        private DataLogger dataLogger = new DataLogger();
        // 网络消息处理
        private DataParser dataParser;

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

                // 关闭绘图定时器刷新数据
                timerUpdateChart.Stop();
            }
            base.WndProc(ref m);
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_YAOCE_SystemStatus_DATA:
                    IntPtr ptr = m.LParam;
                    SYSTEMPARSE_STATUS sObject = Marshal.PtrToStructure<SYSTEMPARSE_STATUS>(ptr);

                    //----------------------------------------------------------//
                    // 填充实时数据
                    showSystemTimeStatus(ref sObject);
                    //----------------------------------------------------------//
                    // 绘图
                    xiTong_CHART_ITEM_INDEX++;

                    // 添加系统坐标点集
                    AddXiTongZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu);
                    // 添加系统速度点集
                    AddXiTongSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu);
                    // 添加系统角速度点集
                    AddXiTongJiaoSuDu(sObject.WxJiaoSuDu, sObject.WyJiaoSuDu, sObject.WzJiaoSuDu);
                    // 添加系统发射系点集
                    AddXiTongFaSheXi(sObject.zhouXiangGuoZai, sObject.curFaSheXi_X, sObject.curFaSheXi_Y, sObject.curFaSheXi_Z);
                    // 添加系统预示落点点集
                    AddXiTongYuShiLuoDian(sObject.yuShiLuoDianSheCheng, sObject.yuShiLuoDianZ);

                    Marshal.FreeHGlobal(ptr);

                    //----------------------------------------------------------//

                    break;
                case WM_YAOCE_daoHangKuaiSu_DATA:
                    break;
                case WM_YAOCE_daoHangManSu_DATA:
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void showSystemTimeStatus(ref SYSTEMPARSE_STATUS sObject)
        {
            // GNSS时间
            XiTong_GNSSTime.Text = sObject.GNSSTime.ToString();
            // 飞行总时间
            XiTong_ZongFeiXingTime.Text = sObject.feiXingZongShiJian.ToString();
            // 策略阶段(0-准备 1-起飞 2-一级 3-二级 4-结束)
            string ceLueJieDuanValue = "";
            switch (sObject.ceLueJieDuan)
            {
                case 0:
                    ceLueJieDuanValue = "准备";
                    break;
                case 1:
                    ceLueJieDuanValue = "起飞";
                    break;
                case 2:
                    ceLueJieDuanValue = "一级";
                    break;
                case 3:
                    ceLueJieDuanValue = "二级";
                    break;
                case 4:
                    ceLueJieDuanValue = "结束";
                    break;
                default:
                    break;
            }
            XiTong_CeLueJieDuan.Text = ceLueJieDuanValue;
            // 弹头状态(0-状态异常 1-产品遥测上电正常 2-初始化正常 3-一级保险解除
            string danTouZhuangTaiValue = "";
            switch (sObject.danTouZhuangTai)
            {
                case 0:
                    danTouZhuangTaiValue = "状态异常";
                    break;
                case 1:
                    danTouZhuangTaiValue = "产品遥测上电正常";
                    break;
                case 2:
                    danTouZhuangTaiValue = "初始化正常";
                    break;
                case 3:
                    danTouZhuangTaiValue = "一级保险解除";
                    break;
                default:
                    break;
            }
            XiTong_DanTouZhuangTai.Text = danTouZhuangTaiValue;

            // 导航状态指示1
            byte daoHangTip1 = sObject.daoHangTip1;
            // 导航数据选择
            XiTong_DaoHangShuJuXuanZe.Text = (daoHangTip1 & 0x1) == 0 ? "数据不可用" : "数据可用";
            // 陀螺数据融合结果（0：所有数据不可用 1：数据可用）
            XiTong_TuoLuoShuJuRongHe.Text = ((daoHangTip1 >> 1) & 0x1) == 0 ? "所有数据不可用" : "数据可用";
            // bit2 bit3 数据未更新标志（00：均无数据; 01：1号输入无数据，2号输入有数据; 10：1号输入有数据，2号输入无数据; 11：均有数据）
            byte tempValue = (byte)((daoHangTip1 >> 2) & 0x3);
            string tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "均无数据";
                    break;
                case 1:
                    tempSTR = "1号输入无数据，2号输入有数据";
                    break;
                case 2:
                    tempSTR = "1号输入有数据，2号输入无数据";
                    break;
                case 3:
                    tempSTR = "均有数据";
                    break;
                default:
                    break;
            }
            XiTong_ShuJuWeiGengXin.Text = tempSTR;

            // bit4 bit5 时间间隔异常标志（00：时间间隔均正常; 01：1号时间间隔异常，2号时间间隔正常； 10：1号时间间隔正常，2号时间间隔异常； 00：时间间隔均不正常）
            tempValue = (byte)((daoHangTip1 >> 4) & 0x3);
            tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "时间间隔均正常";
                    break;
                case 1:
                    tempSTR = "1号时间间隔异常，2号时间间隔正常";
                    break;
                case 2:
                    tempSTR = "1号时间间隔正常，2号时间间隔异常";
                    break;
                case 3:
                    tempSTR = "时间间隔均不正常";
                    break;
                default:
                    break;
            }
            XiTong_ShiJianJianGeYiChang.Text = tempSTR;

            // bit6 弹头组合无效标志（1表示无效）
            XiTong_DanTouZuHe.Text = (daoHangTip1 >> 6 & 0x1) == 1 ? "无效" : "有效";
            // bit7 弹体组合无效标志（1表示无效）
            XiTong_DanTiZuHe.Text = (daoHangTip1 >> 6 & 0x1) == 1 ? "无效" : "有效";

            //----------------------------------------------------------------------//
            // 导航状态指示2
            byte daoHangTip2 = sObject.daoHangTip2;
            Dictionary<byte, string> dicTip2 = new Dictionary<byte, string>();
            dicTip2.Add(0, "不是野值");
            dicTip2.Add(1, "无数据");
            dicTip2.Add(2, "数据用于初始化");
            dicTip2.Add(3, "是野值");
            // bit0 bit1 1号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuJingDu.Text = dicTip2[(byte)(daoHangTip2 & 0x2)];
            // bit2 bit3 1号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuWeiDu.Text = dicTip2[(byte)(daoHangTip2 >> 2 & 0x2)];
            // bit4 bit5 1号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuGaoDu.Text = dicTip2[(byte)(daoHangTip2 >> 4 & 0x2)];
            // bit6 bit7 1号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuDongXiangSuDu.Text = dicTip2[(byte)(daoHangTip2 >> 6 & 0x2)];
        }

        // 添加系统坐标点集
        private void AddXiTongZuoBiao(double jingDu, double weiDu, double gaoDu)
        {
            xiTong_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, jingDu));
            xiTong_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, weiDu));
            xiTong_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, gaoDu));
        }

        // 添加系统速度点集
        private void AddXiTongSuDu(double dongXiangSuDu, double beiXiangSuDu, double tianXiangSuDu)
        {
            xiTong_SuDu_DongXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, dongXiangSuDu));
            xiTong_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, beiXiangSuDu));
            xiTong_SuDu_TianXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, tianXiangSuDu));
        }

        // 添加系统角速度点集
        private void AddXiTongJiaoSuDu(double Wx, double Wy, double Wz)
        {
            xiTong_JiaoSuDu_Wx_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wx));
            xiTong_JiaoSuDu_Wy_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wy));
            xiTong_JiaoSuDu_Wz_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wz));
        }

        // 添加系统发射系点集
        private void AddXiTongFaSheXi(double zhouXiangGuoZai, double x, double y, double z)
        {
            xiTong_ZhouXiangGuoZai_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, zhouXiangGuoZai));
            xiTong_FaSheXi_x_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, x));
            xiTong_FaSheXi_y_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, y));
            xiTong_FaSheXi_z_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, z));
        }

        // 添加系统预示落点点集
        private void AddXiTongYuShiLuoDian(double sheCheng, double z)
        {
            xiTong_YuShiLuoDian_SheCheng_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, sheCheng));
            xiTong_YuShiLuoDian_Z_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, z));
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
                return;
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

            //-----------------------------------------------------//

            // 清空所有的曲线
            xiTong_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_XiTong_ZuoBiao.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_XiTong_SuDu.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_XiTong_JiaoSuDu.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_XiTong_FaSheXi.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_XiTong_YuShiLuoDian.Series)
            {
                series.Points.Clear();
            }

            // 禁用按钮
            // btnSetting.Enabled = false;
            // btnStop.Enabled = true;
            // btnStart.Enabled = false;

            // 启动绘图定时器刷新数据
            timerUpdateChart.Start();

            // 绘图时上下限
            //chart_XiTong_ZuoBiao.Titles[0].Text = String.Format("X范围:{0}-{1}, Y范围{2}-{3}, Z范围{4}-{5}", Config.GetInstance().locMinX, Config.GetInstance().locMaxX,
            //    Config.GetInstance().locMinY, Config.GetInstance().locMaxY, Config.GetInstance().locMinZ, Config.GetInstance().locMaxZ);
            //speedChart.Titles[0].Text = String.Format("Vx范围:{0}-{1}, Vy范围{2}-{3}, Vz范围{4}-{5}", Config.GetInstance().speedMinX, Config.GetInstance().speedMaxX,
            //    Config.GetInstance().speedMinY, Config.GetInstance().speedMaxY, Config.GetInstance().speedMinZ, Config.GetInstance().speedMaxZ);
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

        // 绘图定时器，每隔100ms刷新绘图区域
        private void timerUpdateChart_Tick(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------------//
            // 坐标
            chart_XiTong_ZuoBiao.Series["经度"].Points.AddRange(xiTong_ZuoBiao_JingDu_Buffer.ToArray());
            chart_XiTong_ZuoBiao.Series["纬度"].Points.AddRange(xiTong_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_XiTong_ZuoBiao.Series["海拔高度"].Points.AddRange(xiTong_ZuoBiao_GaoDu_Buffer.ToArray());
            xiTong_ZuoBiao_JingDu_Buffer.Clear();
            xiTong_ZuoBiao_WeiDu_Buffer.Clear();
            xiTong_ZuoBiao_GaoDu_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 速度
            chart_XiTong_SuDu.Series["东向速度"].Points.AddRange(xiTong_SuDu_DongXiang_Buffer.ToArray());
            chart_XiTong_SuDu.Series["北向速度"].Points.AddRange(xiTong_SuDu_BeiXiang_Buffer.ToArray());
            chart_XiTong_SuDu.Series["天向速度"].Points.AddRange(xiTong_SuDu_TianXiang_Buffer.ToArray());
            xiTong_SuDu_DongXiang_Buffer.Clear();
            xiTong_SuDu_BeiXiang_Buffer.Clear();
            xiTong_SuDu_TianXiang_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 角速度
            chart_XiTong_JiaoSuDu.Series["Wx角速度"].Points.AddRange(xiTong_JiaoSuDu_Wx_Buffer.ToArray());
            chart_XiTong_JiaoSuDu.Series["Wy角速度"].Points.AddRange(xiTong_JiaoSuDu_Wy_Buffer.ToArray());
            chart_XiTong_JiaoSuDu.Series["Wz角速度"].Points.AddRange(xiTong_JiaoSuDu_Wz_Buffer.ToArray());
            xiTong_JiaoSuDu_Wx_Buffer.Clear();
            xiTong_JiaoSuDu_Wy_Buffer.Clear();
            xiTong_JiaoSuDu_Wz_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 发射系
            chart_XiTong_FaSheXi.Series["轴向过载"].Points.AddRange(xiTong_ZhouXiangGuoZai_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系X"].Points.AddRange(xiTong_FaSheXi_x_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系Y"].Points.AddRange(xiTong_FaSheXi_y_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系Z"].Points.AddRange(xiTong_FaSheXi_z_Buffer.ToArray());
            xiTong_ZhouXiangGuoZai_Buffer.Clear();
            xiTong_FaSheXi_x_Buffer.Clear();
            xiTong_FaSheXi_y_Buffer.Clear();
            xiTong_FaSheXi_z_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 预示落点
            chart_XiTong_YuShiLuoDian.Series["预示落点射程"].Points.AddRange(xiTong_YuShiLuoDian_SheCheng_Buffer.ToArray());
            chart_XiTong_YuShiLuoDian.Series["预示落点Z"].Points.AddRange(xiTong_YuShiLuoDian_Z_Buffer.ToArray());
            xiTong_YuShiLuoDian_SheCheng_Buffer.Clear();
            xiTong_YuShiLuoDian_Z_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 测试 绘图功能
            Random rnd = new Random();
            chart_XiTong_ZuoBiao.Series["经度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_XiTong_ZuoBiao.Series["纬度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_XiTong_ZuoBiao.Series["海拔高度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            xiTong_CHART_ITEM_INDEX++;
            //-----------------------------------------------------------------------------------//
        }
    }
}
