// 
using DevExpress.XtraCharts; //
// 
using DevExpress.XtraEditors; //
// 
using System; //
// 
using System.Collections.Generic; //
// 
using System.ComponentModel; //
// 
using System.Data; //
// 
using System.Drawing; //
// 
using System.IO; //
// 
using System.Linq; //
// 
using System.Net; //
// 
using System.Net.Sockets; //
// 
using System.Runtime.InteropServices; //
// 
using System.Text; //
// 
using System.Threading; //
// 
using System.Threading.Tasks; //
// 
using System.Timers; //
// 
using System.Windows.Forms; //
// 

// 
/// <summary>
// 
/// YaoCeProcess
// 
/// </summary>
// 
namespace YaoCeProcess
// 
{
// 
    /// <summary>
// 
    /// 文件名:MainForm/
// 
    /// 文件功能描述:软件主界面/
// 
    /// 创建人:yangy
// 
    /// 版权所有:Copyright (C) ZGM/
// 
    /// 创建标识:2020.03.12/     
// 
    /// 修改描述:/
// 
    /// </summary>
// 
    public partial class MainForm : Form
// 
    {
// 
        /// <summary>
// 
        /// 每一个UDP帧固定长度651
// 
        /// </summary>
// 
        private const int UDPLENGTH = 651; //
// 
        /// <summary>
// 
        /// 自定义消息
// 
        /// </summary>
// 
        private const int WM_USER = 0x400; //
// 
        /// <summary>
// 
        /// 系统判决状态数据标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_SystemStatus_DATA = WM_USER + 102; //
// 
        /// <summary>
// 
        /// 导航数据（快速——弹体）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_daoHangKuaiSu_Ti_DATA = WM_USER + 103; //
// 
        /// <summary>
// 
        /// 导航数据（快速——弹头）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_daoHangKuaiSu_Tou_DATA = WM_USER + 104; //
// 
        /// <summary>
// 
        /// 导航数据（慢速——弹体）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_daoHangManSu_Ti_DATA = WM_USER + 105; //
// 
        /// <summary>
// 
        /// 导航数据（慢速——弹头）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_daoHangManSu_Tou_DATA = WM_USER + 106; //
// 
        /// <summary>
// 
        /// 回路检测数据标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_HuiLuJianCe_DATA = WM_USER + 107; //
// 
        // TODO 20200219 新增
// 
        /// <summary>
// 
        /// 系统状态即时反馈数据（弹体）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_XiTongJiShi_Ti_DATA = WM_USER + 108; //
// 
        /// <summary>
// 
        /// 系统状态即时反馈（弹头）标识
// 
        /// </summary>
// 
        public const int WM_YAOCE_XiTongJiShi_Tou_DATA = WM_USER + 109; //
// 
        /// <summary>
// 
        /// 数据帧信息
// 
        /// </summary>
// 
        public const int WM_YAOCE_FRAMEPROPERTY_DATA = WM_USER + 110; //
// 
        /// <summary>
// 
        /// UDP包状态
// 
        /// </summary>
// 
        public const int WM_YAOCE_UDPPROPERTY_DATA = WM_USER + 111; //
// 

// 
        /// <summary>
// 
        /// 读取完成后休眠时间
// 
        /// </summary>
// 
        private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(500); //
// 

// 
        /// 读取文件动作
// 
        /// <summary>
// 
        /// E_LOADFILE_START
// 
        /// </summary>
// 
        public const int E_LOADFILE_START = 0; //
// 

// 
        /// <summary>
// 
        /// E_LOADFILE_PAUSE
// 
        /// </summary>
// 
        public const int E_LOADFILE_PAUSE = 1; //
// 

// 
        /// <summary>
// 
        /// E_LOADFILE_CONTINUE
// 
        /// </summary>
// 
        public const int E_LOADFILE_CONTINUE = 2; //
// 

// 
        /// <summary>
// 
        /// E_LOADFILE_STOP
// 
        /// </summary>
// 
        public const int E_LOADFILE_STOP = 3; //
// 

// 
        /// <summary>
// 
        /// E_LOADFILE_SKIPPROGRAM
// 
        /// </summary>
// 
        public const int E_LOADFILE_SKIPPROGRAM = 4; //
// 

// 
        //-----------------------------------------------------//
// 
        // 成员变量
// 

// 
        /// <summary>
// 
        /// 是否开启Socket接收网络数据
// 
        /// </summary>
// 
        bool bStartRecvNetworkData = false; //
// 
        /// <summary>
// 
        /// 读取文件定时器
// 
        /// </summary>
// 
        System.Timers.Timer readFileTimer = new System.Timers.Timer(); //
// 
        // 以文本流的形式读取文件
// 
        // StreamReader srFileRead = null; //
// 
        /// <summary>
// 
        /// 直接读取二进制文件
// 
        /// </summary>
// 
        FileStream srFileRead = null; //
// 
        /// <summary>
// 
        /// 文件大小
// 
        /// </summary>
// 
        long loadFileLength = 0; //
// 
        /// <summary>
// 
        /// 已经读取的文件大小
// 
        /// </summary>
// 
        long alreadReadFileLength = 0; //
// 

// 
        //--------//
// 
        /// <summary>
// 
        /// 创建曲线X轴索引值
// 
        /// </summary>
// 
        private int xiTong_CHART_ITEM_INDEX = 0; //
// 

// 
        /// 系统判决状态绘图数据缓存
// 

// 
        /// <summary>
// 
        /// 经度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>(); //       // 经度
// 
        /// <summary>
// 
        /// 纬度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>(); //        // 纬度
// 
        /// <summary>
// 
        /// 海拔高度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>(); //        // 海拔高度
// 

// 
        /// <summary>
// 
        /// 东向速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_SuDu_DongXiang_Buffer = new List<SeriesPoint>(); //       // 东向速度
// 
        /// <summary>
// 
        /// 北向速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_SuDu_BeiXiang_Buffer = new List<SeriesPoint>(); //        // 北向速度
// 
        /// <summary>
// 
        /// 天向速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_SuDu_TianXiang_Buffer = new List<SeriesPoint>(); //       // 天向速度
// 

// 
        /// <summary>
// 
        /// Wx角速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_JiaoSuDu_Wx_Buffer = new List<SeriesPoint>(); //          // Wx角速度
// 
        /// <summary>
// 
        /// Wy角速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_JiaoSuDu_Wy_Buffer = new List<SeriesPoint>(); //          // Wy角速度
// 
        /// <summary>
// 
        /// Wz角速度
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_JiaoSuDu_Wz_Buffer = new List<SeriesPoint>(); //          // Wz角速度
// 

// 
        /// <summary>
// 
        /// 轴向过载
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_ZhouXiangGuoZai_Buffer = new List<SeriesPoint>(); //      // 轴向过载
// 
        /// <summary>
// 
        /// 发射系X
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_FaSheXi_x_Buffer = new List<SeriesPoint>(); //            // 发射系X
// 
        /// <summary>
// 
        /// 发射系Y
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_FaSheXi_y_Buffer = new List<SeriesPoint>(); //            // 发射系Y
// 
        /// <summary>
// 
        /// 发射系Z
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_FaSheXi_z_Buffer = new List<SeriesPoint>(); //            // 发射系Z
// 

// 
        /// <summary>
// 
        /// 预示落点射程
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_YuShiLuoDian_SheCheng_Buffer = new List<SeriesPoint>(); //// 预示落点射程
// 
        /// <summary>
// 
        /// 预示落点Z
// 
        /// </summary>
// 
        private List<SeriesPoint> xiTong_YuShiLuoDian_Z_Buffer = new List<SeriesPoint>(); //       // 预示落点Z
// 

// 
        //-----------------------------------------------------//
// 
        // 状态数据缓存
// 

// 
        /// <summary>
// 
        /// sObject_XiTong
// 
        /// </summary>
// 
        SYSTEMPARSE_STATUS sObject_XiTong; //
// 

// 
        /// <summary>
// 
        /// sObject_huiLuJianCe
// 
        /// </summary>
// 
        HUILUJIANCE_STATUS sObject_huiLuJianCe; //
// 

// 
        // 是否收到数据
// 

// 
        /// <summary>
// 
        /// bRecvStatusData_XiTong
// 
        /// </summary>
// 
        bool bRecvStatusData_XiTong = false; //
// 
        /// <summary>
// 
        /// bRecvStatusData_HuiLuJianCe
// 
        /// </summary>
// 
        bool bRecvStatusData_HuiLuJianCe = false; //
// 
        /// <summary>
// 
        /// bRecvStatusData_UDP
// 
        /// </summary>
// 
        bool bRecvStatusData_UDP = false; //
// 
        //-----------------------------------------------------//
// 

// 
        /// <summary>
// 
        /// UDP
// 
        /// </summary>
// 
        private UdpClient udpClient; //
// 
        /// <summary>
// 
        /// 码流记录
// 
        /// </summary>
// 
        private DataLogger dataLogger = new DataLogger(); //
// 
        /// <summary>
// 
        /// 网络消息处理
// 
        /// </summary>
// 
        private DataParser dataParser; //
// 

// 
        //-----------------------------------------------------//
// 
        // 导航快速子窗口
// 

// 
        /// <summary>
// 
        /// dHKSubForm_Ti
// 
        /// </summary>
// 
        public DHKSubForm dHKSubForm_Ti; //
// 

// 
        /// <summary>
// 
        /// dHKSubForm_Tou
// 
        /// </summary>
// 
        public DHKSubForm dHKSubForm_Tou; //
// 

// 
        // 导航慢速子窗口
// 

// 
        /// <summary>
// 
        /// dHMSubForm_Ti
// 
        /// </summary>
// 
        public DHMSubForm dHMSubForm_Ti; //
// 

// 
        /// <summary>
// 
        /// dHMSubForm_Tou
// 
        /// </summary>
// 
        public DHMSubForm dHMSubForm_Tou; //
// 

// 
        // 系统状态即时反馈子窗口
// 

// 
        /// <summary>
// 
        /// xiTongJiShiSubForm_Ti
// 
        /// </summary>
// 
        public XiTongJiShiSubForm xiTongJiShiSubForm_Ti; //
// 

// 
        /// <summary>
// 
        /// xiTongJiShiSubForm_Tou
// 
        /// </summary>
// 
        public XiTongJiShiSubForm xiTongJiShiSubForm_Tou; //
// 

// 
        /// <summary>
// 
        /// 帧序号
// 
        /// </summary>
// 
        public FrameInfoSubForm frameInfoForm; //
// 

// 
        /// <summary>
// 
        /// 加载离线文件的子窗口
// 
        /// </summary>
// 
        public LoadDataForm loadFileForm = new LoadDataForm(); //
// 
        //-----------------------------------------------------//
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_XiTong
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_XiTong = 0x01; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_HuiLuJianCe
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_HuiLuJianCe = 0x02; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_DaoHangKuaiSu_Ti
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_DaoHangKuaiSu_Ti = 0x03; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_DaoHangKuaiSu_Tou
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_DaoHangKuaiSu_Tou = 0x04; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_DaoHangManSu_Ti
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_DaoHangManSu_Ti = 0x05; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_DaoHangManSu_Tou
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_DaoHangManSu_Tou = 0x06; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_XiTongJiShi_Ti
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_XiTongJiShi_Ti = 0x07; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_XiTongJiShi_Tou
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_XiTongJiShi_Tou = 0x08; //
// 

// 
        /// <summary>
// 
        /// E_STATUSTYPE_dataConnect
// 
        /// </summary>
// 
        public const uint E_STATUSTYPE_dataConnect = 0x09; //
// 

// 
        /// <summary>
// 
        /// bDaoHangKuaiSuOnLine_Ti
// 
        /// </summary>
// 
        bool bDaoHangKuaiSuOnLine_Ti = false; //
// 

// 
        /// <summary>
// 
        /// bDaoHangKuaiSuOnLine_Tou
// 
        /// </summary>
// 
        bool bDaoHangKuaiSuOnLine_Tou = false; //
// 

// 
        /// <summary>
// 
        /// bDaoHangManSuOnLine_Ti
// 
        /// </summary>
// 
        bool bDaoHangManSuOnLine_Ti = false; //
// 

// 
        /// <summary>
// 
        /// bDaoHangManSuOnLine_Tou
// 
        /// </summary>
// 
        bool bDaoHangManSuOnLine_Tou = false; //
// 

// 
        /// <summary>
// 
        /// bXiTongJiShiOnLine_Ti
// 
        /// </summary>
// 
        bool bXiTongJiShiOnLine_Ti = false; //
// 

// 
        /// <summary>
// 
        /// bXiTongJiShiOnLine_Tou
// 
        /// </summary>
// 
        bool bXiTongJiShiOnLine_Tou = false; //
// 

// 
        /// <summary>
// 
        /// setDaoHangStatusOnOffLine
// 
        /// </summary>
// 
        /// <param name="statusType"></param>
// 
        /// <param name="bOn"></param>
// 
        public void setDaoHangStatusOnOffLine(uint statusType, bool bOn)
// 
        {
// 
            switch (statusType)
// 
            {
// 
                case E_STATUSTYPE_DaoHangKuaiSu_Ti:
// 
                    bDaoHangKuaiSuOnLine_Ti = bOn; //
// 
                    break; //
// 
                case E_STATUSTYPE_DaoHangKuaiSu_Tou:
// 
                    bDaoHangKuaiSuOnLine_Tou = bOn; //
// 
                    break; //
// 
                case E_STATUSTYPE_DaoHangManSu_Ti:
// 
                    bDaoHangManSuOnLine_Ti = bOn; //
// 
                    break; //
// 
                case E_STATUSTYPE_DaoHangManSu_Tou:
// 
                    bDaoHangManSuOnLine_Tou = bOn; //
// 
                    break; //
// 
                // TODO 20200219 新增
// 
                case E_STATUSTYPE_XiTongJiShi_Ti:
// 
                    bXiTongJiShiOnLine_Ti = bOn; //
// 
                    break; //
// 
                case E_STATUSTYPE_XiTongJiShi_Tou:
// 
                    bXiTongJiShiOnLine_Tou = bOn; //
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
            setStatusOnOffLine(E_STATUSTYPE_DaoHangKuaiSu_Ti, bDaoHangKuaiSuOnLine_Ti | bDaoHangKuaiSuOnLine_Tou); //
// 
            setStatusOnOffLine(E_STATUSTYPE_DaoHangManSu_Ti, bDaoHangManSuOnLine_Ti | bDaoHangManSuOnLine_Tou); //
// 
            setStatusOnOffLine(E_STATUSTYPE_XiTongJiShi_Ti, bXiTongJiShiOnLine_Ti | bXiTongJiShiOnLine_Tou); //
// 
        }
// 

// 
        /// <summary>
// 
        /// setStatusOnOffLine
// 
        /// </summary>
// 
        /// <param name="statusType"></param>
// 
        /// <param name="bOn"></param>
// 
        public void setStatusOnOffLine(uint statusType, bool bOn)
// 
        {
// 
            switch (statusType)
// 
            {
// 
                case E_STATUSTYPE_XiTong:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_XiTong.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_XiTong.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                case E_STATUSTYPE_HuiLuJianCe:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_HuiLu.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_HuiLu.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                case E_STATUSTYPE_DaoHangKuaiSu_Ti:
// 
                case E_STATUSTYPE_DaoHangKuaiSu_Tou:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_DHK.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_DHK.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                case E_STATUSTYPE_DaoHangManSu_Ti:
// 
                case E_STATUSTYPE_DaoHangManSu_Tou:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_DHM.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_DHM.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                // TODO 20200219 新增
// 
                case E_STATUSTYPE_XiTongJiShi_Ti:
// 
                case E_STATUSTYPE_XiTongJiShi_Tou:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_XiTongJiShi.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_XiTongJiShi.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                // TODO 20200316 新增
// 
                case E_STATUSTYPE_dataConnect:
// 
                    if (bOn)
// 
                    {
// 
                        pictureEdit_dataConnect.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_green.png"); //
// 
                    }
// 
                    else
// 
                    {
// 
                        pictureEdit_dataConnect.Image = Image.FromFile(Application.StartupPath + @"\Image\LED_gray.png"); //
// 
                    }
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// MainForm
// 
        /// </summary>
// 
        public MainForm()
// 
        {
// 
            InitializeComponent(); //
// 

// 
            // 导航快速子窗口初始化
// 
            dHKSubForm_Ti = new DHKSubForm(); //
// 
            dHKSubForm_Ti.Into(xtraTabPage_DHK_DanTi); //
// 
            dHKSubForm_Ti.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            dHKSubForm_Ti.statusType = E_STATUSTYPE_DaoHangKuaiSu_Ti; //
// 
            dHKSubForm_Tou = new DHKSubForm(); //
// 
            dHKSubForm_Tou.Into(xtraTabPage_DHK_DanTou); //
// 
            dHKSubForm_Tou.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            dHKSubForm_Tou.statusType = E_STATUSTYPE_DaoHangKuaiSu_Tou; //
// 

// 
            // 导航慢速子窗口初始化
// 
            dHMSubForm_Ti = new DHMSubForm(); //
// 
            dHMSubForm_Ti.Into(xtraTabPage_DHM_DanTi); //
// 
            dHMSubForm_Ti.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            dHMSubForm_Ti.statusType = E_STATUSTYPE_DaoHangManSu_Ti; //
// 
            dHMSubForm_Tou = new DHMSubForm(); //
// 
            dHMSubForm_Tou.Into(xtraTabPage_DHM_DanTou); //
// 
            dHMSubForm_Tou.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            dHMSubForm_Tou.statusType = E_STATUSTYPE_DaoHangManSu_Tou; //
// 

// 
            // TODO 20200219 新增
// 
            // 系统状态即时反馈初始化
// 
            xiTongJiShiSubForm_Ti = new XiTongJiShiSubForm(); //
// 
            xiTongJiShiSubForm_Ti.Into(xtraTabPage_XiTongJiShi_DanTi); //
// 
            xiTongJiShiSubForm_Ti.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            xiTongJiShiSubForm_Ti.statusType = E_STATUSTYPE_XiTongJiShi_Ti; //
// 
            xiTongJiShiSubForm_Tou = new XiTongJiShiSubForm(); //
// 
            xiTongJiShiSubForm_Tou.Into(xtraTabPage_XiTongJiShi_DanTou); //
// 
            xiTongJiShiSubForm_Tou.testFunDelegate = setDaoHangStatusOnOffLine; //
// 
            xiTongJiShiSubForm_Tou.statusType = E_STATUSTYPE_XiTongJiShi_Tou; //
// 

// 
            // 帧序号
// 
            frameInfoForm = new FrameInfoSubForm(); //
// 
            frameInfoForm.Into(xtraTabPage_FrameInfo); //
// 

// 
            // 加载离线文件的子窗口
// 
            loadFileForm.setPlayStatus = setOffLineFilePlayStatus; //
// 
            loadFileForm.StartPosition = FormStartPosition.CenterScreen; //
// 

// 
            // 窗口居中显示
// 
            this.StartPosition = FormStartPosition.CenterScreen; //
// 
            // 最大化: 
// 
            this.WindowState = FormWindowState.Maximized; //
// 

// 
            // 离线定时器初始即启动
// 
            timerOffLineXiTongStatus.Start(); //
// 
            timerOffLineHuiLuJianCe.Start(); //
// 
            timerOffLineUDP.Start(); //
// 

// 
            // 初始清空数据
// 
            GenericFunction.reSetAllTextEdit(TabPage_XiTongPanJue); //
// 
            GenericFunction.reSetAllTextEdit(xtraTabPage_HuiLuJianCe); //
// 

// 
            //------------------------------------------------------//
// 

// 
            pictureEdit_XiTong.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 
            pictureEdit_HuiLu.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 
            pictureEdit_DHK.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 
            pictureEdit_DHM.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 
            pictureEdit_XiTongJiShi.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 
            pictureEdit_dataConnect.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom; //
// 

// 
            //------------------------------------------------------//
// 

// 
            // 传递窗口句柄
// 
            dataParser = new DataParser(Handle); //
// 

// 
            // 按钮暂时未使用，隐藏(?? 无效果，待确定 )
// 
            BtnStartStop.Visible = false; //
// 

// 
            // 传递主窗口指针
// 
            Logger.GetInstance().SetMainForm(this); //
// 

// 
            // 更改是否运行的图片
// 
            setRunPic(false); //
// 

// 
            // 更改按钮Tip
// 
            toolTip1.SetToolTip(BtnStartStop, "开始"); //
// 

// 
            // 创建新的日志文件
// 
            Logger.GetInstance().NewFile(); //
// 
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "程序开始启动！"); //
// 
        }
// 

// 
        /// <summary>
// 
        /// MainForm
// 
        /// </summary>
// 
        ~MainForm()
// 
        {
// 
            timerOffLineXiTongStatus.Stop(); //
// 
            timerOffLineHuiLuJianCe.Stop(); //
// 
            timerOffLineUDP.Stop(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// setRunPic
// 
        /// </summary>
// 
        /// <param name="bRun"></param>
// 
        public void setRunPic(bool bRun)
// 
        {
// 
            if (bRun)
// 
            {
// 
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Image" + @"\Stop_32x32.png"; //
// 
                if (File.Exists(filePath))
// 
                {
// 
                    BtnStartStop.Image = Image.FromFile(filePath); //
// 
                }
// 
            }
// 
            else
// 
            {
// 
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Image" + @"\Play_32x32.png"; //
// 
                if (File.Exists(filePath))
// 
                {
// 
                    BtnStartStop.Image = Image.FromFile(filePath); //
// 
                }
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// FormClosing事件 先停下定时器
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
// 
        {
// 
            // 读取文件定时器
// 
            readFileTimer.Stop(); //
// 

// 
            // 停止加载文件进度
// 
            timerUpdateLoadFileProgress.Stop(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// FormClosed事件 彻底关闭程序
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
// 
        {
// 
            // TODO ?? 退出时出现异常
// 
            // System.Environment.Exit(0); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 窗口事件
// 
        /// </summary>
// 
        /// <param name="m"></param>
// 
        protected override void WndProc(ref Message m)
// 
        {
// 
            const int WM_SYSCOMMAND = 0x0112; //
// 
            const int SC_CLOSE = 0xF060; //
// 

// 
            // 捕捉关闭窗体消息(用户点击关闭窗体控制按钮) 
// 
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
// 
            {
// 
                // 读取文件定时器
// 
                readFileTimer.Stop(); //
// 

// 
                // 关闭日志文件
// 
                Logger.GetInstance().closeFile(); //
// 

// 
                // 关闭码流记录
// 
                dataLogger.Stop(); //
// 

// 
                // 关闭消息处理
// 
                dataParser.Stop(); //
// 

// 
                // 关闭绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(false); //
// 

// 
                // 停止加载文件进度
// 
                timerUpdateLoadFileProgress.Stop(); //
// 
            }
// 
            base.WndProc(ref m); //
// 
        }
// 

// 
        /// <summary>
// 
        /// DefWndProc
// 
        /// </summary>
// 
        /// <param name="m"></param>
// 
        protected override void DefWndProc(ref Message m)
// 
        {
// 
            switch (m.Msg)
// 
            {
// 
                case WM_YAOCE_SystemStatus_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 
                        IntPtr ptr = m.LParam; //
// 
                        SYSTEMPARSE_STATUS sObject = Marshal.PtrToStructure<SYSTEMPARSE_STATUS>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        sObject_XiTong = sObject; //
// 

// 
                        // 重新启动离线定时器
// 
                        timerOffLineXiTongStatus.Stop(); //
// 
                        timerOffLineXiTongStatus.Start(); //
// 

// 
                        // 是否收到数据
// 
                        bRecvStatusData_XiTong = true; //
// 
                        //----------------------------------------------------------//
// 
                        // 填充实时数据(更改成通过定时器来刷新实时数据)
// 
                        // showSystemTimeStatus(ref sObject); //
// 
                        //----------------------------------------------------------//
// 
                        // 绘图
// 
                        xiTong_CHART_ITEM_INDEX++; //
// 

// 
                        // 添加系统坐标点集
// 
                        AddXiTongZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        // 添加系统速度点集
// 
                        AddXiTongSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 
                        // 添加系统角速度点集
// 
                        AddXiTongJiaoSuDu(sObject.WxJiaoSuDu, sObject.WyJiaoSuDu, sObject.WzJiaoSuDu); //
// 
                        // 添加系统发射系点集
// 
                        AddXiTongFaSheXi(sObject.zhouXiangGuoZai, sObject.curFaSheXi_X, sObject.curFaSheXi_Y, sObject.curFaSheXi_Z); //
// 
                        // 添加系统预示落点点集
// 
                        AddXiTongYuShiLuoDian(sObject.yuShiLuoDianSheCheng, sObject.yuShiLuoDianZ); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 

// 
                    break; //
// 
                case WM_YAOCE_daoHangKuaiSu_Ti_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        DAOHANGSHUJU_KuaiSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_KuaiSu>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        dHKSubForm_Ti.SetDHKStatus(ref sObject); //
// 
                        // 绘图
// 
                        dHKSubForm_Ti.setCHARTITEMINDEXAdd(); //
// 

// 
                        // 添加导航数据快速坐标点集
// 
                        dHKSubForm_Ti.AddDHKuaiSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        // 添加导航数据快速速度点集
// 
                        dHKSubForm_Ti.AddDHKuaiSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_daoHangKuaiSu_Tou_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        DAOHANGSHUJU_KuaiSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_KuaiSu>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        dHKSubForm_Tou.SetDHKStatus(ref sObject); //
// 
                        // 绘图
// 
                        dHKSubForm_Tou.setCHARTITEMINDEXAdd(); //
// 

// 
                        // 添加导航数据快速坐标点集
// 
                        dHKSubForm_Tou.AddDHKuaiSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        // 添加导航数据快速速度点集
// 
                        dHKSubForm_Tou.AddDHKuaiSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_daoHangManSu_Ti_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        DAOHANGSHUJU_ManSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_ManSu>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        dHMSubForm_Ti.SetDHMStatus(ref sObject); //
// 

// 
                        // 绘图
// 
                        dHMSubForm_Ti.setCHARTITEMINDEXAdd(); //
// 

// 
                        // 添加导航数据慢速坐标点集
// 
                        dHMSubForm_Ti.AddDHManSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        // 添加导航数据慢速速度点集
// 
                        dHMSubForm_Ti.AddDHManSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_daoHangManSu_Tou_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        DAOHANGSHUJU_ManSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_ManSu>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        dHMSubForm_Tou.SetDHMStatus(ref sObject); //
// 

// 
                        // 绘图
// 
                        dHMSubForm_Tou.setCHARTITEMINDEXAdd(); //
// 

// 
                        // 添加导航数据慢速坐标点集
// 
                        dHMSubForm_Tou.AddDHManSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        // 添加导航数据慢速速度点集
// 
                        dHMSubForm_Tou.AddDHManSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_HuiLuJianCe_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 
                        IntPtr ptr = m.LParam; //
// 
                        HUILUJIANCE_STATUS sObject = Marshal.PtrToStructure<HUILUJIANCE_STATUS>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        sObject_huiLuJianCe = sObject; //
// 

// 
                        // 重新启动离线定时器
// 
                        timerUpdateHuiLuJianCe.Stop(); //
// 
                        timerUpdateHuiLuJianCe.Start(); //
// 

// 
                        // 是否收到数据
// 
                        bRecvStatusData_HuiLuJianCe = true; //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                // TODO 20200219 新增
// 
                case WM_YAOCE_XiTongJiShi_Ti_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        SYSTEMImmediate_STATUS sObject = Marshal.PtrToStructure<SYSTEMImmediate_STATUS>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        xiTongJiShiSubForm_Ti.SetXiTongJiShiStatus(ref sObject); //
// 

// 
                        // 绘图
// 
                        xiTongJiShiSubForm_Ti.setCHARTITEMINDEXAdd(); //
// 

// 
                        xiTongJiShiSubForm_Ti.AddXiTongJiShiZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        xiTongJiShiSubForm_Ti.AddXiTongJiShiSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 
                        xiTongJiShiSubForm_Ti.AddXiTongJiShiJiaoSuDu(sObject.WxJiaoSuDu, sObject.WyJiaoSuDu, sObject.WzJiaoSuDu); //
// 
                        xiTongJiShiSubForm_Ti.AddXiTongJiShiGuoZai(sObject.zhouXiangGuoZai, sObject.faXiangGuoZai, sObject.ceXiangGuoZai); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_XiTongJiShi_Tou_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        SYSTEMImmediate_STATUS sObject = Marshal.PtrToStructure<SYSTEMImmediate_STATUS>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        xiTongJiShiSubForm_Tou.SetXiTongJiShiStatus(ref sObject); //
// 

// 
                        // 绘图
// 
                        xiTongJiShiSubForm_Tou.setCHARTITEMINDEXAdd(); //
// 

// 
                        xiTongJiShiSubForm_Tou.AddXiTongJiShiZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu); //
// 
                        xiTongJiShiSubForm_Tou.AddXiTongJiShiSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu); //
// 
                        xiTongJiShiSubForm_Tou.AddXiTongJiShiJiaoSuDu(sObject.WxJiaoSuDu, sObject.WyJiaoSuDu, sObject.WzJiaoSuDu); //
// 
                        xiTongJiShiSubForm_Tou.AddXiTongJiShiGuoZai(sObject.zhouXiangGuoZai, sObject.faXiangGuoZai, sObject.ceXiangGuoZai); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_FRAMEPROPERTY_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        FRAME_PROPERTY sObject = Marshal.PtrToStructure<FRAME_PROPERTY>(ptr); //
// 

// 
                        // 缓存状态数据
// 
                        frameInfoForm.addFrameInfo(ref sObject); //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                case WM_YAOCE_UDPPROPERTY_DATA:
// 
                    {
// 
                        //----------------------------------------------------------//
// 

// 
                        IntPtr ptr = m.LParam; //
// 
                        UDP_PROPERTY sObject = Marshal.PtrToStructure<UDP_PROPERTY>(ptr); //
// 

// 
                        // 重新启动离线定时器
// 
                        timerOffLineUDP.Stop(); //
// 
                        timerOffLineUDP.Start(); //
// 

// 
                        // 更改状态灯颜色
// 
                        bRecvStatusData_UDP = true; //
// 

// 
                        Marshal.FreeHGlobal(ptr); //
// 

// 
                        //----------------------------------------------------------//
// 
                    }
// 
                    break; //
// 
                default:
// 
                    base.DefWndProc(ref m); //
// 
                    break; //
// 
            }
// 
        }
// 
        //----------------------------------------------------------//
// 
        /// <summary>
// 
        /// 系统实时状态显示
// 
        /// </summary>
// 
        /// <param name="sObject"></param>
// 
        private void showSystemTimeStatus(ref SYSTEMPARSE_STATUS sObject)
// 
        {
// 
            // 经度
// 
            XiTong_JingDu.Text = sObject.jingDu.ToString(); //
// 
            // 纬度
// 
            XiTong_WeiDu.Text = sObject.weiDu.ToString(); //
// 
            // 海拔高度
// 
            XiTong_GaoDu.Text = sObject.haiBaGaoDu.ToString(); //
// 

// 
            // 东向速度
// 
            XiTong_DongXiangSuDu.Text = sObject.dongXiangSuDu.ToString(); //
// 
            // 北向速度
// 
            XiTong_BeiXiangSuDu.Text = sObject.beiXiangSuDu.ToString(); //
// 
            // 天向速度
// 
            XiTong_TianXiangSuDu.Text = sObject.tianXiangSuDu.ToString(); //
// 

// 
            // Wx角速度
// 
            XiTong_WxJiaoSuDuValue.Text = sObject.WxJiaoSuDu.ToString(); //
// 
            // Wy角速度
// 
            XiTong_WyJiaoSuDuValue.Text = sObject.WyJiaoSuDu.ToString(); //
// 
            // Wz角速度
// 
            XiTong_WzJiaoSuDuValue.Text = sObject.WzJiaoSuDu.ToString(); //
// 

// 
            // 当前发射系X
// 
            XiTong_XFaSheXi.Text = sObject.curFaSheXi_X.ToString(); //
// 
            // 当前发射系Y
// 
            XiTong_YFaSheXi.Text = sObject.curFaSheXi_Y.ToString(); //
// 
            // 当前发射系Z
// 
            XiTong_ZFaSheXi.Text = sObject.curFaSheXi_Z.ToString(); //
// 

// 

// 
            // GNSS时间
// 
            XiTong_GNSSTime.Text = sObject.GNSSTime.ToString(); //
// 
            // 飞行总时间
// 
            XiTong_ZongFeiXingTime.Text = sObject.feiXingZongShiJian.ToString(); //
// 
            // 策略阶段(0-准备 1-起飞 2-一级 3-二级 4-结束)
// 
            string ceLueJieDuanValue = ""; //
// 
            switch (sObject.ceLueJieDuan)
// 
            {
// 
                case 0:
// 
                    ceLueJieDuanValue = "准备"; //
// 
                    break; //
// 
                case 1:
// 
                    ceLueJieDuanValue = "起飞"; //
// 
                    break; //
// 
                case 2:
// 
                    ceLueJieDuanValue = "一级"; //
// 
                    break; //
// 
                case 3:
// 
                    ceLueJieDuanValue = "二级"; //
// 
                    break; //
// 
                case 4:
// 
                    ceLueJieDuanValue = "结束"; //
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
            XiTong_CeLueJieDuan.Text = ceLueJieDuanValue; //
// 
            // 弹头状态(0-状态异常 1-产品遥测上电正常 2-初始化正常 3-一级保险解除
// 
            // 4-二级保险解除 5-收到保险解除信号 6-三级保险解除 7-充电 8-起爆
// 
            string danTouZhuangTaiValue = ""; //
// 
            switch (sObject.danTouZhuangTai)
// 
            {
// 
                case 0:
// 
                    danTouZhuangTaiValue = "状态异常"; //
// 
                    break; //
// 
                case 1:
// 
                    danTouZhuangTaiValue = "产品遥测上电正常"; //
// 
                    break; //
// 
                case 2:
// 
                    danTouZhuangTaiValue = "初始化正常"; //
// 
                    break; //
// 
                case 3:
// 
                    danTouZhuangTaiValue = "一级保险解除"; //
// 
                    break; //
// 
                case 4:
// 
                    danTouZhuangTaiValue = "二级保险解除"; //
// 
                    break; //
// 
                case 5:
// 
                    danTouZhuangTaiValue = "收到保险解除信号"; //
// 
                    break; //
// 
                case 6:
// 
                    danTouZhuangTaiValue = "三级保险解除"; //
// 
                    break; //
// 
                case 7:
// 
                    danTouZhuangTaiValue = "充电"; //
// 
                    break; //
// 
                case 8:
// 
                    danTouZhuangTaiValue = "起爆"; //
// 
                    break; //
// 
                default:
// 
                    danTouZhuangTaiValue = "未知"; //
// 
                    break; //
// 
            }
// 
            XiTong_DanTouZhuangTai.Text = danTouZhuangTaiValue; //
// 

// 
            //----------------------------------------------------------------------//
// 
            // 导航状态指示1
// 
            byte daoHangTip1 = sObject.daoHangTip1; //
// 
            // 导航数据选择
// 
            XiTong_DaoHangShuJuXuanZe.Text = (daoHangTip1 & 0x1) == 0 ? "数据不可用" : "数据可用"; //
// 
            // 陀螺数据融合结果（0：所有数据不可用 1：数据可用）
// 
            XiTong_TuoLuoShuJuRongHe.Text = ((daoHangTip1 >> 1) & 0x1) == 0 ? "所有数据不可用" : "数据可用"; //
// 
            // bit2 bit3 数据未更新标志（00：均无数据; // 01：1号输入无数据，2号输入有数据; // 10：1号输入有数据，2号输入无数据; // 11：均有数据）
// 
            byte tempValue = (byte)((daoHangTip1 >> 2) & 0x3); //
// 
            string tempSTR = ""; //
// 
            switch (tempValue)
// 
            {
// 
                case 0:
// 
                    tempSTR = "均无数据"; //
// 
                    break; //
// 
                case 1:
// 
                    // tempSTR = "1号输入无数据，2号输入有数据"; //
// 
                    tempSTR = "1号无数据，2号有数据"; //
// 
                    break; //
// 
                case 2:
// 
                    // tempSTR = "1号输入有数据，2号输入无数据"; //
// 
                    tempSTR = "1号有数据，2号无数据"; //
// 
                    break; //
// 
                case 3:
// 
                    tempSTR = "均有数据"; //
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
            XiTong_ShuJuWeiGengXin.Text = tempSTR; //
// 

// 
            // bit4 bit5 时间间隔异常标志（00：时间间隔均正常; // 01：1号时间间隔异常，2号时间间隔正常； 10：1号时间间隔正常，2号时间间隔异常； 00：时间间隔均不正常）
// 
            tempValue = (byte)((daoHangTip1 >> 4) & 0x3); //
// 
            tempSTR = ""; //
// 
            switch (tempValue)
// 
            {
// 
                case 0:
// 
                    // tempSTR = "时间间隔均正常"; //
// 
                    tempSTR = "无输出"; //
// 
                    break; //
// 
                case 1:
// 
                    // tempSTR = "1号异常，2号正常"; //
// 
                    tempSTR = "1号"; //
// 
                    break; //
// 
                case 2:
// 
                    // tempSTR = "1号正常，2号异常"; //
// 
                    tempSTR = "2号"; //
// 
                    break; //
// 
                case 3:
// 
                    // tempSTR = "时间间隔均不正常"; //
// 
                    tempSTR = "1号和2号"; //
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
            XiTong_ShiJianJianGeYiChang.Text = tempSTR; //
// 

// 
            // bit6 弹头组合无效标志（1表示无效）
// 
            XiTong_DanTouZuHe.Text = (daoHangTip1 >> 6 & 0x1) == 1 ? "无效" : "有效"; //
// 
            // bit7 弹体组合无效标志（1表示无效）
// 
            XiTong_DanTiZuHe.Text = (daoHangTip1 >> 7 & 0x1) == 1 ? "无效" : "有效"; //
// 

// 
            //----------------------------------------------------------------------//
// 
            // 导航状态指示2
// 
            byte daoHangTip2 = sObject.daoHangTip2; //
// 
            Dictionary<byte, string> dicTip = new Dictionary<byte, string>(); //
// 
            dicTip.Add(0, "不是野值"); //
// 
            dicTip.Add(1, "无数据"); //
// 
            dicTip.Add(2, "数据用于初始化"); //
// 
            dicTip.Add(3, "是野值"); //
// 
            // bit0 bit1 1号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuJingDu.Text = dicTip[(byte)(daoHangTip2 & 0x2)]; //
// 
            // bit2 bit3 1号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuWeiDu.Text = dicTip[(byte)(daoHangTip2 >> 2 & 0x2)]; //
// 
            // bit4 bit5 1号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuGaoDu.Text = dicTip[(byte)(daoHangTip2 >> 4 & 0x2)]; //
// 
            // bit6 bit7 1号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuDongXiangSuDu.Text = dicTip[(byte)(daoHangTip2 >> 6 & 0x2)]; //
// 

// 
            //----------------------------------------------------------------------//
// 
            // 导航状态指示3
// 
            byte daoHangTip3 = sObject.daoHangTip3; //
// 
            // bit0 bit1 1号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuBeiXiangSuDu.Text = dicTip[(byte)(daoHangTip3 & 0x2)]; //
// 
            // bit2 bit3 1号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_1HaoShuJuTianXiangSuDu.Text = dicTip[(byte)(daoHangTip3 >> 2 & 0x2)]; //
// 
            // bit4 bit5 2号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuJingDu.Text = dicTip[(byte)(daoHangTip3 >> 4 & 0x2)]; //
// 
            // bit6 bit7 2号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuWeiDu.Text = dicTip[(byte)(daoHangTip3 >> 6 & 0x2)]; //
// 

// 
            //----------------------------------------------------------------------//
// 
            // 导航状态指示3
// 
            byte daoHangTip4 = sObject.daoHangTip4; //
// 
            // bit0 bit1 2号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuGaoDu.Text = dicTip[(byte)(daoHangTip4 & 0x2)]; //
// 
            // bit2 bit3 2号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuDongXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 2 & 0x2)]; //
// 
            // bit4 bit5 2号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuBeiXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 4 & 0x2)]; //
// 
            // bit6 bit7 2号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
// 
            XiTong_2HaoShuJuTianXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 6 & 0x2)]; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 系统状态指示
// 
            byte sysyemStatusTip = sObject.sysyemStatusTip; //
// 
            // bit0 功率输出闭合（1有效）
// 
            XiTong_GongLvShuChuBiHe.Text = (sysyemStatusTip & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit1 解保指令发出（1有效）
// 
            XiTong_JieBaoZhiLingFaChu.Text = (sysyemStatusTip >> 1 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit2 自毁指令发出（1有效）
// 
            XiTong_ZiHuiZhiLingFaChu.Text = (sysyemStatusTip >> 2 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit3 复位信号（1有效）
// 
            XiTong_FuWeiXinHao.Text = (sysyemStatusTip >> 3 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit4 对外供电（1有效）
// 
            XiTong_DuiWaiGongDian.Text = (sysyemStatusTip >> 4 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit5 模拟自毁指令1（1有效）
// 
            XiTong_MoNiZiHui1.Text = (sysyemStatusTip >> 5 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit6 模拟自毁指令2（1有效）
// 
            XiTong_MoNiZiHui2.Text = (sysyemStatusTip >> 6 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit7 回路检测 ?? 待定
// 
            XiTong_HuiLuJianCe.Text = (sysyemStatusTip >> 7 & 0x1) == 1 ? "数据可用" : "数据不可用"; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 触点状态指示
// 
            byte chuDianZhuangTai = sObject.chuDianZhuangTai; //
// 
            // bit0 起飞分离脱插信号（0有效）
// 
            XiTong_QiFeiFenLiTuoCha.Text = (chuDianZhuangTai >> 0 & 0x1) == 0 ? "有效" : "无效"; //
// 
            // bit1 一级分离脱插信号（0有效）
// 
            XiTong_YiJiFenLiTuoCha.Text = (chuDianZhuangTai >> 1 & 0x1) == 0 ? "有效" : "无效"; //
// 
            // bit2 安控接收机预令（1有效）
// 
            XiTong_AnKongJieShouJiYuLing.Text = (chuDianZhuangTai >> 2 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit3 安控接收机动令（1有效）
// 
            XiTong_AnKongJieShouJiDongLing.Text = (chuDianZhuangTai >> 3 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit4 一级自毁工作状态A（1有效）
// 
            XiTong_1ZiHuiWorkA.Text = (chuDianZhuangTai >> 4 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit5 一级自毁工作状态B（1有效）
// 
            XiTong_1ZiHuiWorkB.Text = (chuDianZhuangTai >> 5 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit6 二级自毁工作状态A（1有效）
// 
            XiTong_2ZiHuiWorkA.Text = (chuDianZhuangTai >> 6 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit7 二级自毁工作状态B（1有效）
// 
            XiTong_2ZiHuiWorkB.Text = (chuDianZhuangTai >> 7 & 0x1) == 1 ? "有效" : "无效"; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 策略判决结果1
// 
            byte jueCePanJueJieGuo1 = sObject.jueCePanJueJieGuo1; //
// 
            // bit0 总飞行时间（1：有效
// 
            XiTong_ZongFeiXingShiJian.Text = (jueCePanJueJieGuo1 >> 0 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit1 侧向（1：有效）
// 
            XiTong_CeXiang.Text = (jueCePanJueJieGuo1 >> 1 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit2 Wx角速度（1：有效）
// 
            XiTong_WxJiaoSuDuStatus.Text = (jueCePanJueJieGuo1 >> 2 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit3 Wy角速度（1：有效）
// 
            XiTong_WyJiaoSuDuStatus.Text = (jueCePanJueJieGuo1 >> 3 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit4 Wz角速度（1：有效）
// 
            XiTong_WzJiaoSuDuStatus.Text = (jueCePanJueJieGuo1 >> 4 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit5 后向（1：有效）
// 
            XiTong_HouXiang.Text = (jueCePanJueJieGuo1 >> 5 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit6 坠落（1：有效）
// 
            XiTong_ZhuiLuo.Text = (jueCePanJueJieGuo1 >> 6 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit7 分离时间（1：有效）
// 
            XiTong_FenLiShiTian.Text = (jueCePanJueJieGuo1 >> 7 & 0x1) == 1 ? "有效" : "无效"; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 策略判决结果2
// 
            byte jueCePanJueJieGuo2 = sObject.jueCePanJueJieGuo2; //
// 
            // bit0 控制区下限（1：有效）
// 
            XiTong_KongZhiQuXiaXian.Text = (jueCePanJueJieGuo2 >> 0 & 0x1) == 1 ? "有效" : "无效"; //
// 
            // bit1 控制区上限（1：有效）
// 
            XiTong_KongZhiQuShangXian.Text = (jueCePanJueJieGuo2 >> 1 & 0x1) == 1 ? "有效" : "无效"; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 输出开关状态1
// 
            byte shuChuKaiGuanStatus1 = sObject.shuChuKaiGuanStatus1; //
// 
            // bit0 弹头保险（1：闭合）
// 
            XiTong_DanTouBaoXian.Text = (jueCePanJueJieGuo2 >> 0 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit1 弹头起爆（1：闭合）
// 
            XiTong_DanTouQiBao.Text = (jueCePanJueJieGuo2 >> 1 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit2 一级保险1（1：闭合）
// 
            XiTong_1JiBaoXian1.Text = (jueCePanJueJieGuo2 >> 2 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit3 一级保险2（1：闭合）
// 
            XiTong_1JiBaoXian2.Text = (jueCePanJueJieGuo2 >> 3 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit4 一级起爆1（1：闭合）
// 
            XiTong_1JiQiBao1.Text = (jueCePanJueJieGuo2 >> 4 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit5 一级起爆2（1：闭合）
// 
            XiTong_1JiQiBao2.Text = (jueCePanJueJieGuo2 >> 5 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit6 二级保险1（1：闭合）
// 
            XiTong_2JiBaoXian1.Text = (jueCePanJueJieGuo2 >> 6 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit7 二级保险2（1：闭合）
// 
            XiTong_2JiBaoXian2.Text = (jueCePanJueJieGuo2 >> 7 & 0x1) == 1 ? "闭合" : "断开"; //
// 

// 
            //----------------------------------------------------------------------//
// 

// 
            // 输出开关状态2
// 
            byte shuChuKaiGuanStatus2 = sObject.shuChuKaiGuanStatus2; //
// 
            // bit0 二级起爆1（1：闭合）
// 
            XiTong_2JiQiBao1.Text = (shuChuKaiGuanStatus2 >> 0 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit1 二级起爆2（1：闭合）
// 
            XiTong_2JiQiBao2.Text = (shuChuKaiGuanStatus2 >> 1 & 0x1) == 1 ? "闭合" : "断开"; //
// 
            // bit2 bit3 参试状态（00：测试1，数据输出状态；01：测试2，低压输出状态；10：保留状态；11：正式实验）
// 
            tempValue = (byte)(shuChuKaiGuanStatus2 >> 2 & 0x3); //
// 
            tempSTR = ""; //
// 
            switch (tempValue)
// 
            {
// 
                case 0:
// 
                    tempSTR = "测试1，数据输出状态"; //
// 
                    break; //
// 
                case 1:
// 
                    tempSTR = "测试2，低压输出状态"; //
// 
                    break; //
// 
                case 2:
// 
                    tempSTR = "保留状态"; //
// 
                    break; //
// 
                case 3:
// 
                    tempSTR = "正式实验"; //
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
            XiTong_CanShiZhuangTai.Text = tempSTR; //
// 

// 
            // 预示落点Z
// 
            XiTong_YuShiLuoDianZ.Text = sObject.yuShiLuoDianZ.ToString(); //
// 
            // 预示落点射程
// 
            XiTong_YuShiLuoDianSheCheng.Text = sObject.yuShiLuoDianSheCheng.ToString(); //
// 
            // 轴向过载
// 
            XiTong_ZhouXiangGuoZai.Text = sObject.zhouXiangGuoZai.ToString(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 添加系统坐标点集
// 
        /// </summary>
// 
        /// <param name="jingDu"></param>
// 
        /// <param name="weiDu"></param>
// 
        /// <param name="gaoDu"></param>
// 
        private void AddXiTongZuoBiao(double jingDu, double weiDu, double gaoDu)
// 
        {
// 
            xiTong_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, jingDu)); //
// 
            xiTong_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, weiDu)); //
// 
            xiTong_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, gaoDu)); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 添加系统速度点集
// 
        /// </summary>
// 
        /// <param name="dongXiangSuDu"></param>
// 
        /// <param name="beiXiangSuDu"></param>
// 
        /// <param name="tianXiangSuDu"></param>
// 
        private void AddXiTongSuDu(double dongXiangSuDu, double beiXiangSuDu, double tianXiangSuDu)
// 
        {
// 
            xiTong_SuDu_DongXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, dongXiangSuDu)); //
// 
            xiTong_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, beiXiangSuDu)); //
// 
            xiTong_SuDu_TianXiang_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, tianXiangSuDu)); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 添加系统角速度点集
// 
        /// </summary>
// 
        /// <param name="Wx"></param>
// 
        /// <param name="Wy"></param>
// 
        /// <param name="Wz"></param>
// 
        private void AddXiTongJiaoSuDu(double Wx, double Wy, double Wz)
// 
        {
// 
            xiTong_JiaoSuDu_Wx_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wx)); //
// 
            xiTong_JiaoSuDu_Wy_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wy)); //
// 
            xiTong_JiaoSuDu_Wz_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, Wz)); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 添加系统发射系点集
// 
        /// </summary>
// 
        /// <param name="zhouXiangGuoZai"></param>
// 
        /// <param name="x"></param>
// 
        /// <param name="y"></param>
// 
        /// <param name="z"></param>
// 
        private void AddXiTongFaSheXi(double zhouXiangGuoZai, double x, double y, double z)
// 
        {
// 
            xiTong_ZhouXiangGuoZai_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, zhouXiangGuoZai)); //
// 
            xiTong_FaSheXi_x_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, x)); //
// 
            xiTong_FaSheXi_y_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, y)); //
// 
            xiTong_FaSheXi_z_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, z)); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 添加系统预示落点点集
// 
        /// </summary>
// 
        /// <param name="sheCheng"></param>
// 
        /// <param name="z"></param>
// 
        private void AddXiTongYuShiLuoDian(double sheCheng, double z)
// 
        {
// 
            xiTong_YuShiLuoDian_SheCheng_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, sheCheng)); //
// 
            xiTong_YuShiLuoDian_Z_Buffer.Add(new SeriesPoint(xiTong_CHART_ITEM_INDEX, z)); //
// 
        }
// 

// 
        //----------------------------------------------------------//
// 
        /// <summary>
// 
        /// 回路检测反馈数据显示
// 
        /// </summary>
// 
        /// <param name="sObject"></param>
// 
        private void showHuiLuJianCeStatus(ref HUILUJIANCE_STATUS sObject)
// 
        {
// 
            edit_shuChu1HuiLuDianZu.Text = sObject.shuChu1HuiLuDianZu.ToString("2f"); //    // 电机驱动输出1回路电阻
// 
            edit_shuChu2HuiLuDianZu.Text = sObject.shuChu2HuiLuDianZu.ToString("2f"); //    // 电机驱动输出2回路电阻
// 
            edit_QBDH1AHuiLuDianZu.Text = sObject.QBDH1AHuiLuDianZu.ToString("2f"); //      // 起爆点火1A回路电阻
// 
            edit_QBDH1BHuiLuDianZu.Text = sObject.QBDH1BHuiLuDianZu.ToString("2f"); //      // 起爆点火1B回路电阻
// 
            edit_QBDH2AHuiLuDianZu.Text = sObject.QBDH2AHuiLuDianZu.ToString("2f"); //      // 起爆点火2A回路电阻
// 
            edit_QBDH2BHuiLuDianZu.Text = sObject.QBDH2BHuiLuDianZu.ToString("2f"); //      // 起爆点火2B回路电阻
// 
        }
// 
        //----------------------------------------------------------//
// 

// 
        /// <summary>
// 
        /// MainForm_Load
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void MainForm_Load(object sender, EventArgs e)
// 
        {
// 
            //--------------------------------------------------//
// 
            // 读取文件定时器
// 
            readFileTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnReadFileTimedEvent); //
// 
            readFileTimer.Interval = 1; //
// 
            readFileTimer.Enabled = true; //
// 

// 
            //--------------------------------------------------//
// 
        }
// 

// 
        /// <summary>
// 
        /// BtnSetting_Click
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void BtnSetting_Click(object sender, EventArgs e)
// 
        {
// 
            SettingForm settingForm = new SettingForm(); //
// 
            settingForm.StartPosition = FormStartPosition.CenterScreen; //
// 
            // settingForm.ShowDialog(); //
// 
            if (settingForm.ShowDialog() == DialogResult.OK)
// 
            {
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "设置网络配置成功！"); //
// 
                return; //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// EndReceive
// 
        /// </summary>
// 
        /// <param name="ar"></param>
// 
        private void EndReceive(IAsyncResult ar)
// 
        {
// 
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0); //
// 
            try
// 
            {
// 
                byte[] recvBuffer = udpClient?.EndReceive(ar, ref endPoint); //
// 
                dataParser.Enqueue(recvBuffer); //
// 
                dataLogger.Enqueue(recvBuffer); //
// 
                udpClient.BeginReceive(EndReceive, null); //
// 
            }
// 
            catch (Exception)
// 
            {
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// LogDelegate
// 
        /// </summary>
// 
        /// <param name="time"></param>
// 
        /// <param name="level"></param>
// 
        /// <param name="msg"></param>
// 
        delegate void LogDelegate(DateTime time, Logger.LOG_LEVEL level, String msg); //
// 
        public void Log(DateTime time, Logger.LOG_LEVEL level, String msg)
// 
        {
// 
            if (InvokeRequired)
// 
            {
// 
                Invoke(new LogDelegate(Log), time, level, msg); //
// 
            }
// 
            else
// 
            {
// 
                if (LogListView.Items.Count == 100)
// 
                {
// 
                    LogListView.Items.RemoveAt(0); //
// 
                }
// 
                ListViewItem item = new ListViewItem
// 
                {
// 
                    Text = time.ToString("G")
// 
                }; //
// 
                String strLevel; //
// 
                switch (level)
// 
                {
// 
                    case Logger.LOG_LEVEL.LOG_INFO:
// 
                        strLevel = "信息"; //
// 
                        item.BackColor = Color.White; //
// 
                        break; //
// 
                    case Logger.LOG_LEVEL.LOG_WARN:
// 
                        strLevel = "警告"; //
// 
                        item.BackColor = Color.Yellow; //
// 
                        break; //
// 
                    case Logger.LOG_LEVEL.LOG_ERROR:
// 
                        strLevel = "错误"; //
// 
                        item.BackColor = Color.Red; //
// 
                        break; //
// 
                    default:
// 
                        return; //
// 
                }
// 
                item.SubItems.Add(strLevel); //
// 
                item.SubItems.Add(msg); //
// 
                LogListView.Items.Add(item); //
// 
                LogListView.EnsureVisible(LogListView.Items.Count - 1); //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// 绘图定时器，每隔100ms刷新绘图区域
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerUpdateChart_Tick(object sender, EventArgs e)
// 
        {
// 
            //-----------------------------------------------------------------------------------//
// 
            // 系统判决状态
// 
            //---------------------------------------//
// 
            // 坐标
// 
            chart_XiTong_ZuoBiao_JingDu.Series["经度"].Points.AddRange(xiTong_ZuoBiao_JingDu_Buffer.ToArray()); //
// 
            chart_XiTong_ZuoBiao_WeiDu.Series["纬度"].Points.AddRange(xiTong_ZuoBiao_WeiDu_Buffer.ToArray()); //
// 
            chart_XiTong_ZuoBiao_GaoDu.Series["海拔高度"].Points.AddRange(xiTong_ZuoBiao_GaoDu_Buffer.ToArray()); //
// 
            xiTong_ZuoBiao_JingDu_Buffer.Clear(); //
// 
            xiTong_ZuoBiao_WeiDu_Buffer.Clear(); //
// 
            xiTong_ZuoBiao_GaoDu_Buffer.Clear(); //
// 

// 
            //---------------------------------------//
// 
            // 速度
// 
            chart_XiTong_SuDu_Dong.Series["东向速度"].Points.AddRange(xiTong_SuDu_DongXiang_Buffer.ToArray()); //
// 
            chart_XiTong_SuDu_Bei.Series["北向速度"].Points.AddRange(xiTong_SuDu_BeiXiang_Buffer.ToArray()); //
// 
            chart_XiTong_SuDu_Tian.Series["天向速度"].Points.AddRange(xiTong_SuDu_TianXiang_Buffer.ToArray()); //
// 
            xiTong_SuDu_DongXiang_Buffer.Clear(); //
// 
            xiTong_SuDu_BeiXiang_Buffer.Clear(); //
// 
            xiTong_SuDu_TianXiang_Buffer.Clear(); //
// 

// 
            //---------------------------------------//
// 
            // 角速度
// 
            chart_XiTong_JiaoSuDu_Wx.Series["Wx角速度"].Points.AddRange(xiTong_JiaoSuDu_Wx_Buffer.ToArray()); //
// 
            chart_XiTong_JiaoSuDu_Wy.Series["Wy角速度"].Points.AddRange(xiTong_JiaoSuDu_Wy_Buffer.ToArray()); //
// 
            chart_XiTong_JiaoSuDu_Wz.Series["Wz角速度"].Points.AddRange(xiTong_JiaoSuDu_Wz_Buffer.ToArray()); //
// 
            xiTong_JiaoSuDu_Wx_Buffer.Clear(); //
// 
            xiTong_JiaoSuDu_Wy_Buffer.Clear(); //
// 
            xiTong_JiaoSuDu_Wz_Buffer.Clear(); //
// 

// 
            //---------------------------------------//
// 
            // 发射系
// 
            chart_XiTong_FaSheXi_ZXGZ.Series["轴向过载"].Points.AddRange(xiTong_ZhouXiangGuoZai_Buffer.ToArray()); //
// 
            chart_XiTong_FaSheXi_X.Series["当前发射系X"].Points.AddRange(xiTong_FaSheXi_x_Buffer.ToArray()); //
// 
            chart_XiTong_FaSheXi_Y.Series["当前发射系Y"].Points.AddRange(xiTong_FaSheXi_y_Buffer.ToArray()); //
// 
            chart_XiTong_FaSheXi_Z.Series["当前发射系Z"].Points.AddRange(xiTong_FaSheXi_z_Buffer.ToArray()); //
// 
            xiTong_ZhouXiangGuoZai_Buffer.Clear(); //
// 
            xiTong_FaSheXi_x_Buffer.Clear(); //
// 
            xiTong_FaSheXi_y_Buffer.Clear(); //
// 
            xiTong_FaSheXi_z_Buffer.Clear(); //
// 

// 
            //---------------------------------------//
// 
            // 预示落点
// 
            chart_XiTong_YuShiLuoDian.Series["预示落点射程"].Points.AddRange(xiTong_YuShiLuoDian_SheCheng_Buffer.ToArray()); //
// 
            chart_XiTong_YuShiLuoDian.Series["预示落点Z"].Points.AddRange(xiTong_YuShiLuoDian_Z_Buffer.ToArray()); //
// 
            xiTong_YuShiLuoDian_SheCheng_Buffer.Clear(); //
// 
            xiTong_YuShiLuoDian_Z_Buffer.Clear(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// BtnStartStop_Click
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void BtnStartStop_Click(object sender, EventArgs e)
// 
        {
// 
            // 是否开启Socket接收网络数据
// 
            if (!bStartRecvNetworkData)
// 
            {
// 
                //-----------------------------------------------------//
// 

// 
                // 关闭已经存在的连接
// 
                udpClient?.Close(); //
// 

// 
                // 关闭码流记录
// 
                dataLogger.Stop(); //
// 

// 
                //-----------------------------------------------------//
// 

// 
                // 创建新的日志文件
// 
                // Logger.GetInstance().NewFile(); //
// 

// 
                String errMsg; //
// 
                if (!Config.GetInstance().LoadConfigFile(out errMsg))
// 
                {
// 
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加载配置文件失败," + errMsg); //
// 
                    XtraMessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 
                    return; //
// 
                }
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功"); //
// 

// 
                //-----------------------------------------------------//
// 

// 
                // 创建UDP Socket，加入组播，接收网络数据
// 
                try
// 
                {
// 
                    udpClient = new UdpClient(Config.GetInstance().port); //
// 
                    udpClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr)); //
// 

// 
                    dataParser.Start(); //
// 
                    dataLogger.Start(); //
// 
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加入组播组成功"); //
// 
                }
// 
                catch (Exception ex)
// 
                {
// 
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加入组播组失败，" + ex.Message); //
// 
                    XtraMessageBox.Show("加入组播组失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 
                    udpClient.Close(); //
// 

// 
                    dataParser.Stop(); //
// 
                    dataLogger.Stop(); //
// 
                    return; //
// 
                }
// 

// 
                //-----------------------------------------------------//
// 

// 
                // 开启UDP网络数据接收
// 
                udpClient.BeginReceive(EndReceive, null); //
// 

// 
                //-----------------------------------------------------//
// 
                //清空所有的曲线
// 
                clearAllChart(); //
// 

// 
                //-----------------------------------------------------//
// 

// 
                // 禁用按钮
// 
                BtnSetting.Enabled = false; //
// 
                btnLoadFile.Enabled = false; //
// 

// 
                // 启动绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(true); //
// 

// 
                //-----------------------------------------------------//
// 

// 
                // 日志打印
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开启数据接收"); //
// 
                // 是否开启Socket接收网络数据
// 
                bStartRecvNetworkData = true; //
// 

// 
                // 更改是否运行的图片
// 
                setRunPic(true); //
// 
                // 更改按钮Tip
// 
                toolTip1.SetToolTip(BtnStartStop, "停止"); //
// 

// 
                // 开启状态刷新定时器
// 
                setUpdateTimerStatus(true); //
// 
            }
// 
            else
// 
            {
// 
                //-----------------------------------------------------//
// 

// 
                // 关闭已经存在的连接
// 
                udpClient?.Close(); //
// 

// 
                // 关闭码流记录
// 
                dataLogger.Stop(); //
// 
                // 关闭数据解析
// 
                dataParser.Stop(); //
// 

// 
                // 禁用按钮
// 
                BtnSetting.Enabled = true; //
// 
                btnLoadFile.Enabled = true; //
// 

// 
                // 停止绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(false); //
// 

// 
                // 日志打印
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "停止数据接收"); //
// 

// 
                //-----------------------------------------------------//
// 
                // 是否开启Socket接收网络数据
// 
                bStartRecvNetworkData = false; //
// 

// 
                // 更改是否运行的图片
// 
                setRunPic(false); //
// 

// 
                // 更改按钮Tip
// 
                toolTip1.SetToolTip(BtnStartStop, "开始"); //
// 

// 
                // 关闭状态刷新定时器
// 
                setUpdateTimerStatus(false); //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// btnLoadFile_Click
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void btnLoadFile_Click(object sender, EventArgs e)
// 
        {
// 
            if (loadFileForm == null)
// 
            {
// 
                loadFileForm = new LoadDataForm(); //
// 
                loadFileForm.setPlayStatus = setOffLineFilePlayStatus; //
// 
            }
// 
            loadFileForm.Show(); //
// 
            return; //
// 

// 
            //-------------------------------------------------------//
// 
            // 直接加载文件
// 
            /*
// 
            OpenFileDialog dialog = new OpenFileDialog(); //
// 
            // 是否可以选择多个文件
// 
            dialog.Multiselect = false; //
// 
            dialog.Title = "请选择文件夹"; //
// 
            dialog.Filter = "数据文件(*.dat,*.bin)|*.dat; //*.bin"; //
// 
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log"; //
// 
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
// 
            {
// 
                string filePath = dialog.FileName; //
// 
                if (filePath == "")
// 
                {
// 
                    return; //
// 
                }
// 

// 
                // 创建新的日志文件
// 
                // Logger.GetInstance().NewFile(); //
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开始加载历史数据！"); //
// 

// 
                // 打开文件
// 
                // srFileRead = new StreamReader(filePath, Encoding.Default); //
// 
                srFileRead = new FileStream(filePath, FileMode.Open); //
// 

// 
                // 文件大小
// 
                FileInfo fileInfo = new FileInfo(filePath); //
// 
                loadFileLength = fileInfo.Length; //
// 
                // 已经读取的文件大小
// 
                alreadReadFileLength = 0; //
// 

// 
                // 打开文件读取定时器
// 
                readFileTimer.Start(); //
// 

// 
                //-------------------------------------------------------//
// 
                // 禁用按钮
// 
                BtnSetting.Enabled = false; //
// 
                BtnStartStop.Enabled = false; //
// 
                btnLoadFile.Enabled = false; //
// 

// 
                // 开启数据解析
// 
                dataParser.Start(); //
// 

// 
                //-----------------------------------------------------//
// 
                //清空所有的曲线
// 
                clearAllChart(); //
// 

// 
                // 启动绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(true); //
// 

// 
                // 刷新加载文件进度
// 
                timerUpdateLoadFileProgress.Start(); //
// 

// 
                // 开启状态刷新定时器
// 
                setUpdateTimerStatus(true); //
// 
            }
// 
            */
// 

// 
            //-------------------------------------------------------//
// 
            // 直接加载文件
// 
            /*
// 
            OpenFileDialog dialog = new OpenFileDialog(); //
// 
            // 是否可以选择多个文件
// 
            dialog.Multiselect = false; //
// 
            dialog.Title = "请选择文件夹"; //
// 
            dialog.Filter = "数据文件(*.dat,*.bin)|*.dat; //*.bin"; //
// 
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log"; //
// 
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
// 
            {
// 
                string filePath = dialog.FileName; //
// 
                if (filePath == "")
// 
                {
// 
                    return; //
// 
                }
// 

// 
                // 创建新的日志文件
// 
                // Logger.GetInstance().NewFile(); //
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开始加载历史数据！"); //
// 

// 
                // 打开文件
// 
                // srFileRead = new StreamReader(filePath, Encoding.Default); //
// 
                srFileRead = new FileStream(filePath, FileMode.Open); //
// 

// 
                // 文件大小
// 
                FileInfo fileInfo = new FileInfo(filePath); //
// 
                loadFileLength = fileInfo.Length; //
// 
                // 已经读取的文件大小
// 
                alreadReadFileLength = 0; //
// 

// 
                // 打开文件读取定时器
// 
                readFileTimer.Start(); //
// 

// 
                //-------------------------------------------------------//
// 
                // 禁用按钮
// 
                BtnSetting.Enabled = false; //
// 
                BtnStartStop.Enabled = false; //
// 
                btnLoadFile.Enabled = false; //
// 

// 
                // 开启数据解析
// 
                dataParser.Start(); //
// 

// 
                //-----------------------------------------------------//
// 
                //清空所有的曲线
// 
                clearAllChart(); //
// 

// 
                // 启动绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(true); //
// 

// 
                // 刷新加载文件进度
// 
                timerUpdateLoadFileProgress.Start(); //
// 

// 
                // 开启状态刷新定时器
// 
                setUpdateTimerStatus(true); //
// 
            }
// 
            */
// 

// 
            //-------------------------------------------------------//
// 
            // 直接加载文件
// 
            /*
// 
            OpenFileDialog dialog = new OpenFileDialog(); //
// 
            // 是否可以选择多个文件
// 
            dialog.Multiselect = false; //
// 
            dialog.Title = "请选择文件夹"; //
// 
            dialog.Filter = "数据文件(*.dat,*.bin)|*.dat; //*.bin"; //
// 
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log"; //
// 
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
// 
            {
// 
                string filePath = dialog.FileName; //
// 
                if (filePath == "")
// 
                {
// 
                    return; //
// 
                }
// 

// 
                // 创建新的日志文件
// 
                // Logger.GetInstance().NewFile(); //
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开始加载历史数据！"); //
// 

// 
                // 打开文件
// 
                // srFileRead = new StreamReader(filePath, Encoding.Default); //
// 
                srFileRead = new FileStream(filePath, FileMode.Open); //
// 

// 
                // 文件大小
// 
                FileInfo fileInfo = new FileInfo(filePath); //
// 
                loadFileLength = fileInfo.Length; //
// 
                // 已经读取的文件大小
// 
                alreadReadFileLength = 0; //
// 

// 
                // 打开文件读取定时器
// 
                readFileTimer.Start(); //
// 

// 
                //-------------------------------------------------------//
// 
                // 禁用按钮
// 
                BtnSetting.Enabled = false; //
// 
                BtnStartStop.Enabled = false; //
// 
                btnLoadFile.Enabled = false; //
// 

// 
                // 开启数据解析
// 
                dataParser.Start(); //
// 

// 
                //-----------------------------------------------------//
// 
                //清空所有的曲线
// 
                clearAllChart(); //
// 

// 
                // 启动绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(true); //
// 

// 
                // 刷新加载文件进度
// 
                timerUpdateLoadFileProgress.Start(); //
// 

// 
                // 开启状态刷新定时器
// 
                setUpdateTimerStatus(true); //
// 
            }
// 
            */
// 

// 
            //-------------------------------------------------------//
// 
            // 直接加载文件
// 
            /*
// 
            OpenFileDialog dialog = new OpenFileDialog(); //
// 
            // 是否可以选择多个文件
// 
            dialog.Multiselect = false; //
// 
            dialog.Title = "请选择文件夹"; //
// 
            dialog.Filter = "数据文件(*.dat,*.bin)|*.dat; //*.bin"; //
// 
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log"; //
// 
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
// 
            {
// 
                string filePath = dialog.FileName; //
// 
                if (filePath == "")
// 
                {
// 
                    return; //
// 
                }
// 

// 
                // 创建新的日志文件
// 
                // Logger.GetInstance().NewFile(); //
// 
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开始加载历史数据！"); //
// 

// 
                // 打开文件
// 
                // srFileRead = new StreamReader(filePath, Encoding.Default); //
// 
                srFileRead = new FileStream(filePath, FileMode.Open); //
// 

// 
                // 文件大小
// 
                FileInfo fileInfo = new FileInfo(filePath); //
// 
                loadFileLength = fileInfo.Length; //
// 
                // 已经读取的文件大小
// 
                alreadReadFileLength = 0; //
// 

// 
                // 打开文件读取定时器
// 
                readFileTimer.Start(); //
// 

// 
                //-------------------------------------------------------//
// 
                // 禁用按钮
// 
                BtnSetting.Enabled = false; //
// 
                BtnStartStop.Enabled = false; //
// 
                btnLoadFile.Enabled = false; //
// 

// 
                // 开启数据解析
// 
                dataParser.Start(); //
// 

// 
                //-----------------------------------------------------//
// 
                //清空所有的曲线
// 
                clearAllChart(); //
// 

// 
                // 启动绘图定时器刷新数据
// 
                setTimerUpdateChartStatus(true); //
// 

// 
                // 刷新加载文件进度
// 
                timerUpdateLoadFileProgress.Start(); //
// 

// 
                // 开启状态刷新定时器
// 
                setUpdateTimerStatus(true); //
// 
            }
// 
            */
// 
        }
// 

// 
        /// <summary>
// 
        /// OnReadFileTimedEventCallBack
// 
        /// </summary>
// 
        /// <param name="source"></param>
// 
        /// <param name="e"></param>
// 
        delegate void OnReadFileTimedEventCallBack(Object source, ElapsedEventArgs e); //
// 
        public void OnReadFileTimedEvent(Object source, ElapsedEventArgs e)
// 
        {
// 
            // Control.InvokeReauqired判断是否是创建控件线程，不是为true，
// 
            // 则需要invoke到创建控件的线程，是就为false，直接操作控件
// 
            if (this.InvokeRequired)
// 
            {
// 
                // TODO ?? 解决程序关闭时this会变得不可用问题
// 
                try
// 
                {
// 
                    // c# 关于invoke的无法访问已释放的对象
// 
                    OnReadFileTimedEventCallBack callBack = new OnReadFileTimedEventCallBack(OnReadFileTimedEvent); //
// 
                    this.Invoke(callBack, new object[] { source, e }); //
// 
                }
// 
                catch
// 
                {
// 
                    return; //
// 
                }
// 
            }
// 
            else
// 
            {
// 
                if (srFileRead == null)
// 
                {
// 
                    return; //
// 
                }
// 

// 
                /*
// 
                // 按行读取 line为每行的数据
// 
                String line; //
// 
                if ((line = srFileRead.ReadLine()) != null)
// 
                {
// 
                    // 处理数据
// 
                    byte[] byteArray = strToToHexByte(line); //
// 
                    dataParser.Enqueue(byteArray); //
// 
                }
// 
                */
// 
                // 按字节读取数据
// 
                const int fsLen = UDPLENGTH; //
// 
                byte[] heByte = new byte[fsLen]; //
// 
                int readLength = 0; //
// 
                if ((readLength = srFileRead.Read(heByte, 0, heByte.Length)) > 0)
// 
                {
// 
                    // 处理数据
// 
                    if (readLength < fsLen)
// 
                    {
// 
                        byte[] byteArray = new byte[readLength]; //
// 
                        Array.Copy(heByte, 0, byteArray, 0, readLength); //
// 
                        dataParser.Enqueue(byteArray); //
// 
                    }
// 
                    else
// 
                    {
// 
                        dataParser.Enqueue(heByte); //
// 
                    }
// 
                    // 已经读取的文件大小
// 
                    alreadReadFileLength += readLength; //
// 
                }
// 
                else
// 
                {
// 
                    // 关闭文件
// 
                    srFileRead.Close(); //
// 

// 
                    // 关闭文件读取定时器
// 
                    readFileTimer.Stop(); //
// 

// 
                    // 文件置空
// 
                    srFileRead = null; //
// 

// 
                    // 禁用按钮
// 
                    BtnSetting.Enabled = true; //
// 
                    BtnStartStop.Enabled = true; //
// 
                    btnLoadFile.Enabled = true; //
// 

// 
                    // 停止加载文件进度
// 
                    timerUpdateLoadFileProgress.Stop(); //
// 

// 
                    //------------------------------------------------------//
// 

// 
                    // 更新进度条
// 
                    loadFileForm.setProgressBarValue(0, loadFileLength, loadFileLength); //
// 
                    loadFileForm.loadFileFinish(); //
// 

// 
                    // 日志打印
// 
                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "历史数据加载完成！"); //
// 
                    //MessageBox.Show("文件读取完成！", "提示", MessageBoxButtons.OK); //
// 
                    XtraMessageBox.Show("文件读取完成！"); //
// 

// 
                    //-------------------------------------------------------//
// 

// 
                    // 线程休眠使用间隔时间(等待数据处理完成，而不是读取完毕，立即关闭定时器刷新)
// 
                    Thread.Sleep(Interval); //
// 

// 
                    // 关闭数据解析
// 
                    dataParser.Stop(); //
// 

// 
                    // 停止绘图定时器刷新数据
// 
                    setTimerUpdateChartStatus(false); //
// 

// 
                    // 关闭状态刷新定时器
// 
                    setUpdateTimerStatus(false); //
// 
                }
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// 字符串转16进制字节数组
// 
        /// </summary>
// 
        /// <param name="hexString"></param>
// 
        /// <returns></returns>
// 
        private static byte[] strToToHexByte(string hexString)
// 
        {
// 
            hexString = hexString.Replace(" ", ""); //
// 
            if ((hexString.Length % 2) != 0)
// 
                hexString += " "; //
// 
            byte[] returnBytes = new byte[hexString.Length / 2]; //
// 
            for (int i = 0; i < returnBytes.Length; i++)
// 
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16); //
// 
            return returnBytes; //
// 
        }
// 

// 
        /// <summary>
// 
        /// clearAllChart
// 
        /// </summary>
// 
        public void clearAllChart()
// 
        {
// 
            // 系统判决状态曲线
// 
            xiTong_CHART_ITEM_INDEX = 0; //
// 
            foreach (Series series in chart_XiTong_ZuoBiao_JingDu.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_ZuoBiao_JingDu.Update(); //
// 
            foreach (Series series in chart_XiTong_ZuoBiao_WeiDu.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_ZuoBiao_WeiDu.Update(); //
// 
            foreach (Series series in chart_XiTong_ZuoBiao_GaoDu.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_ZuoBiao_GaoDu.Update(); //
// 
            foreach (Series series in chart_XiTong_SuDu_Dong.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_SuDu_Dong.Update(); //
// 
            foreach (Series series in chart_XiTong_SuDu_Bei.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_SuDu_Bei.Update(); //
// 
            foreach (Series series in chart_XiTong_SuDu_Tian.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_SuDu_Tian.Update(); //
// 
            foreach (Series series in chart_XiTong_JiaoSuDu_Wx.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_JiaoSuDu_Wx.Update(); //
// 
            foreach (Series series in chart_XiTong_JiaoSuDu_Wy.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_JiaoSuDu_Wy.Update(); //
// 
            foreach (Series series in chart_XiTong_JiaoSuDu_Wz.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_JiaoSuDu_Wz.Update(); //
// 
            foreach (Series series in chart_XiTong_FaSheXi_ZXGZ.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_FaSheXi_ZXGZ.Update(); //
// 
            foreach (Series series in chart_XiTong_FaSheXi_X.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_FaSheXi_X.Update(); //
// 
            foreach (Series series in chart_XiTong_FaSheXi_Y.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_FaSheXi_Y.Update(); //
// 
            foreach (Series series in chart_XiTong_FaSheXi_Z.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_FaSheXi_Z.Update(); //
// 
            foreach (Series series in chart_XiTong_YuShiLuoDian.Series)
// 
            {
// 
                series.Points.Clear(); //
// 
            }
// 
            chart_XiTong_YuShiLuoDian.Update(); //
// 

// 
            // 导航数据（快速）状态曲线
// 
            dHKSubForm_Ti.clearAllChart(); //
// 
            dHKSubForm_Tou.clearAllChart(); //
// 

// 
            // 导航数据（慢速）状态曲线
// 
            dHMSubForm_Ti.clearAllChart(); //
// 
            dHMSubForm_Tou.clearAllChart(); //
// 

// 
            // 系统即时状态曲线
// 
            xiTongJiShiSubForm_Ti.clearAllChart(); //
// 
            xiTongJiShiSubForm_Tou.clearAllChart(); //
// 

// 
            // 帧信息曲线
// 
            frameInfoForm.clearAllChart(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// timerUpdateLoadFileProgress_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerUpdateLoadFileProgress_Tick(object sender, EventArgs e)
// 
        {
// 
            double percent = (double)alreadReadFileLength / (double)loadFileLength; //
// 
            percent *= 100; //
// 
            // 日志打印
// 
            // Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "数据加载：" + ((UInt32)percent).ToString() + "%"); //
// 
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "数据加载：" + percent.ToString("f2") + "%"); //    // 保留两位小数
// 

// 
            // 更新进度条
// 
            loadFileForm.setProgressBarValue(0, loadFileLength, alreadReadFileLength); //
// 
        }
// 

// 
        //-----------------------------------------------------------------------//
// 
        // 通过定时器定时刷新数据
// 

// 
        /// <summary>
// 
        /// 通过定时器定时刷新数据
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerUpdateXiTongStatus_Tick(object sender, EventArgs e)
// 
        {
// 
            // 是否收到数据
// 
            if (bRecvStatusData_XiTong)
// 
            {
// 
                // 填充实时数据
// 
                showSystemTimeStatus(ref sObject_XiTong); //
// 

// 
                // 更改状态背景颜色
// 
                GenericFunction.changeAllTextEditColor(TabPage_XiTongPanJue); //
// 

// 
                setStatusOnOffLine(E_STATUSTYPE_XiTong, true); //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// timerOffLineXiTongStatus_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerOffLineXiTongStatus_Tick(object sender, EventArgs e)
// 
        {
// 
            // 清空数据
// 
            // TODO 这里不需要清空最后一帧数据显示
// 
            // GenericFunction.reSetAllTextEdit(TabPage_XiTongPanJue); //
// 

// 
            // 是否收到数据
// 
            bRecvStatusData_XiTong = false; //
// 
            setStatusOnOffLine(E_STATUSTYPE_XiTong, false); //
// 
        }
// 

// 
        /// <summary>
// 
        /// setUpdateTimerStatus
// 
        /// </summary>
// 
        /// <param name="bOpen"></param>
// 
        private void setUpdateTimerStatus(bool bOpen)
// 
        {
// 
            if (bOpen)
// 
            {
// 
                timerUpdateXiTongStatus.Start(); //
// 
                timerUpdateHuiLuJianCe.Start(); //
// 
                timerUpdateUDP.Start(); //
// 
            }
// 
            else
// 
            {
// 
                timerUpdateXiTongStatus.Stop(); //
// 
                timerUpdateHuiLuJianCe.Stop(); //
// 
                timerUpdateUDP.Stop(); //
// 
            }
// 

// 
            dHKSubForm_Ti.setUpdateTimerStatus(bOpen); //
// 
            dHKSubForm_Tou.setUpdateTimerStatus(bOpen); //
// 

// 
            dHMSubForm_Ti.setUpdateTimerStatus(bOpen); //
// 
            dHMSubForm_Tou.setUpdateTimerStatus(bOpen); //
// 

// 
            xiTongJiShiSubForm_Ti.setUpdateTimerStatus(bOpen); //
// 
            xiTongJiShiSubForm_Tou.setUpdateTimerStatus(bOpen); //
// 
        }
// 

// 
        /// <summary>
// 
        /// setTimerUpdateChartStatus
// 
        /// </summary>
// 
        /// <param name="bOpen"></param>
// 
        private void setTimerUpdateChartStatus(bool bOpen)
// 
        {
// 
            if (bOpen)
// 
            {
// 
                timerUpdateChart.Start(); //
// 
            }
// 
            else
// 
            {
// 
                timerUpdateChart.Stop(); //
// 
            }
// 

// 
            dHKSubForm_Ti.startTimerUpdateChart(bOpen); //
// 
            dHKSubForm_Tou.startTimerUpdateChart(bOpen); //
// 

// 
            dHMSubForm_Ti.startTimerUpdateChart(bOpen); //
// 
            dHMSubForm_Tou.startTimerUpdateChart(bOpen); //
// 

// 
            xiTongJiShiSubForm_Ti.startTimerUpdateChart(bOpen); //
// 
            xiTongJiShiSubForm_Tou.startTimerUpdateChart(bOpen); //
// 

// 
            frameInfoForm.startTimerUpdateChart(bOpen); //
// 
        }
// 

// 
        /// <summary>
// 
        /// timerUpdateHuiLuJianCe_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerUpdateHuiLuJianCe_Tick(object sender, EventArgs e)
// 
        {
// 
            // 是否收到数据
// 
            if (bRecvStatusData_HuiLuJianCe)
// 
            {
// 
                // 填充实时数据
// 
                showHuiLuJianCeStatus(ref sObject_huiLuJianCe); //
// 

// 
                // 更改状态背景颜色
// 
                GenericFunction.changeAllTextEditColor(xtraTabPage_HuiLuJianCe); //
// 

// 
                setStatusOnOffLine(E_STATUSTYPE_HuiLuJianCe, true); //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// timerOffLineHuiLuJianCe_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerOffLineHuiLuJianCe_Tick(object sender, EventArgs e)
// 
        {
// 
            // 清空数据
// 
            // TODO 这里不需要清空最后一帧数据显示
// 
            // GenericFunction.reSetAllTextEdit(xtraTabPage_HuiLuJianCe); //
// 

// 
            // 是否收到数据
// 
            bRecvStatusData_HuiLuJianCe = false; //
// 
            setStatusOnOffLine(E_STATUSTYPE_HuiLuJianCe, false); //
// 
        }
// 

// 
        /// <summary>
// 
        /// 通过加载离线文件界面控制文件的读取与播放
// 
        /// </summary>
// 
        /// <param name="action"></param>
// 
        /// <param name="param1"></param>
// 
        public void setOffLineFilePlayStatus(int action, int param1 = 0)
// 
        {
// 
            switch (action)
// 
            {
// 
                case E_LOADFILE_START:
// 
                    {
// 
                        string fileName = loadFileForm.getLoadFileName(); //
// 
                        if (System.IO.File.Exists(fileName))
// 
                        {
// 
                            startLoadOffLineFile(fileName); //
// 
                        }
// 
                    }
// 
                    break; //
// 
                case E_LOADFILE_PAUSE:
// 
                    {
// 
                        Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "暂停加载历史数据！"); //
// 
                        // 停止文件读取定时器
// 
                        readFileTimer.Stop(); //
// 

// 
                        // 刷新加载文件进度
// 
                        timerUpdateLoadFileProgress.Stop(); //
// 
                    }
// 
                    break; //
// 
                case E_LOADFILE_CONTINUE:
// 
                    {
// 
                        Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "启动加载历史数据！"); //
// 
                        // 打开文件读取定时器
// 
                        readFileTimer.Start(); //
// 

// 
                        // 刷新加载文件进度
// 
                        timerUpdateLoadFileProgress.Start(); //
// 
                    }
// 
                    break; //
// 
                case E_LOADFILE_STOP:
// 
                    {
// 
                        Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "停止加载历史数据！"); //
// 

// 
                        // 关闭文件
// 
                        srFileRead.Close(); //
// 

// 
                        // 文件置空
// 
                        srFileRead = null; //
// 

// 
                        // 停止文件读取定时器
// 
                        readFileTimer.Stop(); //
// 

// 
                        // 刷新加载文件进度
// 
                        timerUpdateLoadFileProgress.Stop(); //
// 

// 
                        // 线程休眠使用间隔时间(等待数据处理完成，而不是读取完毕，立即关闭定时器刷新)
// 
                        Thread.Sleep(Interval); //
// 

// 
                        // 禁用按钮
// 
                        BtnSetting.Enabled = true; //
// 
                        BtnStartStop.Enabled = true; //
// 
                        btnLoadFile.Enabled = true; //
// 

// 
                        // 关闭数据解析
// 
                        dataParser.Stop(); //
// 

// 
                        // 关闭绘图定时器刷新数据
// 
                        setTimerUpdateChartStatus(false); //
// 

// 
                        // 关闭状态刷新定时器
// 
                        setUpdateTimerStatus(false); //
// 
                    }
// 
                    break; //
// 
                // 进度跳转
// 
                case E_LOADFILE_SKIPPROGRAM:
// 
                    {
// 
                        // 停止文件读取定时器
// 
                        readFileTimer.Stop(); //
// 

// 
                        // 取UDP长度的整数倍
// 
                        long skipValue = (long)((double)loadFileLength / 100.0f * param1); //
// 
                        skipValue = (long)(skipValue / UDPLENGTH) * UDPLENGTH; //
// 
                        // 文件读取指针偏移
// 
                        srFileRead.Seek(skipValue, 0); //
// 
                        // 更改已经读取的文件大小
// 
                        alreadReadFileLength = skipValue; //
// 

// 
                        // 开启文件读取定时器
// 
                        readFileTimer.Start(); //
// 
                    }
// 
                    break; //
// 
                default:
// 
                    break; //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// startLoadOffLineFile
// 
        /// </summary>
// 
        /// <param name="filePath"></param>
// 
        private void startLoadOffLineFile(string filePath)
// 
        {
// 
            // 创建新的日志文件
// 
            // Logger.GetInstance().NewFile(); //
// 
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "开始加载历史数据！"); //
// 

// 
            // 打开文件
// 
            // srFileRead = new StreamReader(filePath, Encoding.Default); //
// 
            srFileRead = new FileStream(filePath, FileMode.Open); //
// 

// 
            // 文件大小
// 
            FileInfo fileInfo = new FileInfo(filePath); //
// 
            loadFileLength = fileInfo.Length; //
// 
            // 已经读取的文件大小
// 
            alreadReadFileLength = 0; //
// 

// 
            // 打开文件读取定时器
// 
            readFileTimer.Start(); //
// 

// 
            //-------------------------------------------------------//
// 
            // 禁用按钮
// 
            BtnSetting.Enabled = false; //
// 
            BtnStartStop.Enabled = false; //
// 
            // btnLoadFile.Enabled = false; //
// 

// 
            // 开启数据解析
// 
            dataParser.Start(); //
// 

// 
            //-----------------------------------------------------//
// 
            //清空所有的曲线
// 
            clearAllChart(); //
// 

// 
            // 启动绘图定时器刷新数据
// 
            setTimerUpdateChartStatus(true); //
// 

// 
            // 刷新加载文件进度
// 
            timerUpdateLoadFileProgress.Start(); //
// 

// 
            // 开启状态刷新定时器
// 
            setUpdateTimerStatus(true); //
// 

// 
            //-----------------------------------------------------//
// 

// 
            // NOTE 20200525 每次重新回放重置数据显示界面
// 
            GenericFunction.reSetAllTextEdit(TabPage_XiTongPanJue); //
// 
            GenericFunction.reSetAllTextEdit(xtraTabPage_HuiLuJianCe); //
// 

// 
            // 是否收到数据
// 
            bRecvStatusData_XiTong = false; //
// 
            bRecvStatusData_HuiLuJianCe = false; //
// 

// 
            // 导航数据（快速）
// 
            dHKSubForm_Ti.initUI(); //
// 
            dHKSubForm_Tou.initUI(); //
// 

// 
            // 导航数据（慢速）
// 
            dHMSubForm_Ti.initUI(); //
// 
            dHMSubForm_Tou.initUI(); //
// 

// 
            // 系统即时状态
// 
            xiTongJiShiSubForm_Ti.initUI(); //
// 
            xiTongJiShiSubForm_Tou.initUI(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// timerUpdateUDP_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerUpdateUDP_Tick(object sender, EventArgs e)
// 
        {
// 
            // 是否收到数据
// 
            if (bRecvStatusData_UDP)
// 
            {
// 
                setStatusOnOffLine(E_STATUSTYPE_dataConnect, true); //
// 
            }
// 
        }
// 

// 
        /// <summary>
// 
        /// timerOffLineUDP_Tick
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void timerOffLineUDP_Tick(object sender, EventArgs e)
// 
        {
// 
            bRecvStatusData_UDP = false; //
// 
            setStatusOnOffLine(E_STATUSTYPE_dataConnect, false); //
// 
        }
// 
    }
// 

// 
    /// <summary>
// 
    /// Extensions
// 
    /// </summary>
// 
    static class Extensions
// 
    {
// 
        /// <summary>
// 
        /// Into
// 
        /// </summary>
// 
        /// <param name="form"></param>
// 
        /// <param name="control"></param>
// 
        public static void Into(this Form form, Control control)
// 
        {
// 
            if (form == null || form.InvokeRequired)
// 
            {
// 
                form?.Invoke((Action)(() => Into(form, control))); // // 防止跨线程异常
// 
                return; //
// 
            }
// 
            form.TopLevel = false; //
// 
            form.Parent = control; //
// 
            form.FormBorderStyle = FormBorderStyle.None; //
// 
            form.Dock = DockStyle.Fill; //
// 
            form.Show(); //
// 
        }
// 
    }
// 
}
// 
