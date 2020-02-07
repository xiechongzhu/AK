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

        //--------//
        // 创建曲线X轴索引值
        private int DHKuaiSu_CHART_ITEM_INDEX = 0;
        // 系统判决状态绘图数据缓存
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();     // 经度
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();      // 纬度
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();      // 海拔高度

        private List<SeriesPoint> DHKuaiSu_SuDu_DongXiang_Buffer = new List<SeriesPoint>();     // 东向速度
        private List<SeriesPoint> DHKuaiSu_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();      // 北向速度
        private List<SeriesPoint> DHKuaiSu_SuDu_TianXiang_Buffer = new List<SeriesPoint>();     // 天向速度

        //--------//
        // 创建曲线X轴索引值
        private int DHManSu_CHART_ITEM_INDEX = 0;
        // 系统判决状态绘图数据缓存
        private List<SeriesPoint> DHManSu_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();      // 经度
        private List<SeriesPoint> DHManSu_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();       // 纬度
        private List<SeriesPoint> DHManSu_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();       // 海拔高度

        private List<SeriesPoint> DHManSu_SuDu_DongXiang_Buffer = new List<SeriesPoint>();      // 东向速度
        private List<SeriesPoint> DHManSu_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();       // 北向速度
        private List<SeriesPoint> DHManSu_SuDu_TianXiang_Buffer = new List<SeriesPoint>();      // 天向速度

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
                    {
                        //----------------------------------------------------------//
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
                    }

                    break;
                case WM_YAOCE_daoHangKuaiSu_DATA:
                    {
                        //----------------------------------------------------------//

                        IntPtr ptr = m.LParam;
                        DAOHANGSHUJU_KuaiSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_KuaiSu>(ptr);

                        //----------------------------------------------------------//
                        // 填充实时数据
                        showDHKuaiSuTimeStatus(ref sObject);
                        //----------------------------------------------------------//
                        // 绘图
                        DHKuaiSu_CHART_ITEM_INDEX++;

                        // 添加导航数据快速坐标点集
                        AddDHKuaiSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu);
                        // 添加导航数据快速速度点集
                        AddDHKuaiSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu);

                        Marshal.FreeHGlobal(ptr);

                        //----------------------------------------------------------//
                    }
                    break;
                case WM_YAOCE_daoHangManSu_DATA:
                    {
                        //----------------------------------------------------------//

                        IntPtr ptr = m.LParam;
                        DAOHANGSHUJU_ManSu sObject = Marshal.PtrToStructure<DAOHANGSHUJU_ManSu>(ptr);

                        //----------------------------------------------------------//
                        // 填充实时数据
                        showDHManSuTimeStatus(ref sObject);
                        //----------------------------------------------------------//
                        // 绘图
                        DHManSu_CHART_ITEM_INDEX++;

                        // 添加导航数据慢速坐标点集
                        AddDHManSuZuoBiao(sObject.jingDu, sObject.weiDu, sObject.haiBaGaoDu);
                        // 添加导航数据慢速速度点集
                        AddDHManSuSuDu(sObject.dongXiangSuDu, sObject.beiXiangSuDu, sObject.tianXiangSuDu);

                        Marshal.FreeHGlobal(ptr);

                        //----------------------------------------------------------//
                    }
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

            //----------------------------------------------------------------------//
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
            Dictionary<byte, string> dicTip = new Dictionary<byte, string>();
            dicTip.Add(0, "不是野值");
            dicTip.Add(1, "无数据");
            dicTip.Add(2, "数据用于初始化");
            dicTip.Add(3, "是野值");
            // bit0 bit1 1号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuJingDu.Text = dicTip[(byte)(daoHangTip2 & 0x2)];
            // bit2 bit3 1号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuWeiDu.Text = dicTip[(byte)(daoHangTip2 >> 2 & 0x2)];
            // bit4 bit5 1号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuGaoDu.Text = dicTip[(byte)(daoHangTip2 >> 4 & 0x2)];
            // bit6 bit7 1号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuDongXiangSuDu.Text = dicTip[(byte)(daoHangTip2 >> 6 & 0x2)];

            //----------------------------------------------------------------------//
            // 导航状态指示3
            byte daoHangTip3 = sObject.daoHangTip3;
            // bit0 bit1 1号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuBeiXiangSuDu.Text = dicTip[(byte)(daoHangTip3 & 0x2)];
            // bit2 bit3 1号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_1HaoShuJuTianXiangSuDu.Text = dicTip[(byte)(daoHangTip3 >> 2 & 0x2)];
            // bit4 bit5 2号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuJingDu.Text = dicTip[(byte)(daoHangTip3 >> 4 & 0x2)];
            // bit6 bit7 2号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuWeiDu.Text = dicTip[(byte)(daoHangTip3 >> 6 & 0x2)];

            //----------------------------------------------------------------------//
            // 导航状态指示3
            byte daoHangTip4 = sObject.daoHangTip4;
            // bit0 bit1 2号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuGaoDu.Text = dicTip[(byte)(daoHangTip4 & 0x2)];
            // bit2 bit3 2号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuDongXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 2 & 0x2)];
            // bit4 bit5 2号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuBeiXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 4 & 0x2)];
            // bit6 bit7 2号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
            XiTong_2HaoShuJuTianXiangSuDu.Text = dicTip[(byte)(daoHangTip4 >> 6 & 0x2)];

            //----------------------------------------------------------------------//

            // 系统状态指示
            byte sysyemStatusTip = sObject.sysyemStatusTip;
            // bit0 功率输出闭合（1有效）
            XiTong_GongLvShuChuBiHe.Text = (sysyemStatusTip & 0x1) == 1 ? "有效" : "无效";
            // bit1 解保指令发出（1有效）
            XiTong_JieBaoZhiLingFaChu.Text = (sysyemStatusTip >> 1 & 0x1) == 1 ? "有效" : "无效";
            // bit2 自毁指令发出（1有效）
            XiTong_ZiHuiZhiLingFaChu.Text = (sysyemStatusTip >> 2 & 0x1) == 1 ? "有效" : "无效";
            // bit3 复位信号（1有效）
            XiTong_FuWeiXinHao.Text = (sysyemStatusTip >> 3 & 0x1) == 1 ? "有效" : "无效";
            // bit4 对外供电（1有效）
            XiTong_DuiWaiGongDian.Text = (sysyemStatusTip >> 4 & 0x1) == 1 ? "有效" : "无效";
            // bit5 模拟自毁指令1（1有效）
            XiTong_MoNiZiHui1.Text = (sysyemStatusTip >> 5 & 0x1) == 1 ? "有效" : "无效";
            // bit6 模拟自毁指令2（1有效）
            XiTong_MoNiZiHui2.Text = (sysyemStatusTip >> 6 & 0x1) == 1 ? "有效" : "无效";
            // bit7 回路检测 ?? 待定
            XiTong_HuiLuJianCe.Text = (sysyemStatusTip >> 7 & 0x1) == 1 ? "有效" : "无效";

            //----------------------------------------------------------------------//

            // 触点状态指示
            byte chuDianZhuangTai = sObject.chuDianZhuangTai;
            // bit0 起飞分离脱插信号（0有效）
            XiTong_QiFeiFenLiTuoCha.Text = (chuDianZhuangTai >> 0 & 0x1) == 0 ? "有效" : "无效";
            // bit1 一级分离脱插信号（0有效）
            XiTong_YiJiFenLiTuoCha.Text = (chuDianZhuangTai >> 1 & 0x1) == 0 ? "有效" : "无效";
            // bit2 安控接收机预令（1有效）
            XiTong_AnKongJieShouJiYuLing.Text = (chuDianZhuangTai >> 2 & 0x1) == 1 ? "有效" : "无效";
            // bit3 安控接收机动令（1有效）
            XiTong_AnKongJieShouJiDongLing.Text = (chuDianZhuangTai >> 3 & 0x1) == 1 ? "有效" : "无效";
            // bit4 一级自毁工作状态A（1有效）
            XiTong_1ZiHuiWorkA.Text = (chuDianZhuangTai >> 4 & 0x1) == 1 ? "有效" : "无效";
            // bit5 一级自毁工作状态B（1有效）
            XiTong_1ZiHuiWorkB.Text = (chuDianZhuangTai >> 5 & 0x1) == 1 ? "有效" : "无效";
            // bit6 二级自毁工作状态A（1有效）
            XiTong_2ZiHuiWorkA.Text = (chuDianZhuangTai >> 6 & 0x1) == 1 ? "有效" : "无效";
            // bit7 二级自毁工作状态B（1有效）
            XiTong_2ZiHuiWorkB.Text = (chuDianZhuangTai >> 7 & 0x1) == 1 ? "有效" : "无效";

            //----------------------------------------------------------------------//

            // 策略判决结果1
            byte jueCePanJueJieGuo1 = sObject.jueCePanJueJieGuo1;
            // bit0 总飞行时间（1：有效
            XiTong_ZongFeiXingShiJian.Text = (jueCePanJueJieGuo1 >> 0 & 0x1) == 1 ? "有效" : "无效";
            // bit1 侧向（1：有效）
            XiTong_CeXiang.Text = (jueCePanJueJieGuo1 >> 1 & 0x1) == 1 ? "有效" : "无效";
            // bit2 Wx角速度（1：有效）
            XiTong_WxJiaoSuDu.Text = (jueCePanJueJieGuo1 >> 2 & 0x1) == 1 ? "有效" : "无效";
            // bit3 Wy角速度（1：有效）
            XiTong_WyJiaoSuDu.Text = (jueCePanJueJieGuo1 >> 3 & 0x1) == 1 ? "有效" : "无效";
            // bit4 Wz角速度（1：有效）
            XiTong_WzJiaoSuDu.Text = (jueCePanJueJieGuo1 >> 4 & 0x1) == 1 ? "有效" : "无效";
            // bit5 后向（1：有效）
            XiTong_HouXiang.Text = (jueCePanJueJieGuo1 >> 5 & 0x1) == 1 ? "有效" : "无效";
            // bit6 坠落（1：有效）
            XiTong_ZhuiLuo.Text = (jueCePanJueJieGuo1 >> 6 & 0x1) == 1 ? "有效" : "无效";
            // bit7 分离时间（1：有效）
            XiTong_FenLiShiTian.Text = (jueCePanJueJieGuo1 >> 7 & 0x1) == 1 ? "有效" : "无效";

            //----------------------------------------------------------------------//

            // 策略判决结果2
            byte jueCePanJueJieGuo2 = sObject.jueCePanJueJieGuo2;
            // bit0 控制区下限（1：有效）
            XiTong_KongZhiQuXiaXian.Text = (jueCePanJueJieGuo2 >> 0 & 0x1) == 1 ? "有效" : "无效";
            // bit1 控制区上限（1：有效）
            XiTong_KongZhiQuShangXian.Text = (jueCePanJueJieGuo2 >> 1 & 0x1) == 1 ? "有效" : "无效";

            //----------------------------------------------------------------------//

            // 输出开关状态1
            byte shuChuKaiGuanStatus1 = sObject.shuChuKaiGuanStatus1;
            // bit0 弹头保险（1：闭合）
            XiTong_DanTouBaoXian.Text = (jueCePanJueJieGuo2 >> 0 & 0x1) == 1 ? "闭合" : "断开";
            // bit1 弹头起爆（1：闭合）
            XiTong_DanTouQiBao.Text = (jueCePanJueJieGuo2 >> 1 & 0x1) == 1 ? "闭合" : "断开";
            // bit2 一级保险1（1：闭合）
            XiTong_1JiBaoXian1.Text = (jueCePanJueJieGuo2 >> 2 & 0x1) == 1 ? "闭合" : "断开";
            // bit3 一级保险2（1：闭合）
            XiTong_1JiBaoXian2.Text = (jueCePanJueJieGuo2 >> 3 & 0x1) == 1 ? "闭合" : "断开";
            // bit4 一级起爆1（1：闭合）
            XiTong_1JiQiBao1.Text = (jueCePanJueJieGuo2 >> 4 & 0x1) == 1 ? "闭合" : "断开";
            // bit5 一级起爆2（1：闭合）
            XiTong_1JiQiBao2.Text = (jueCePanJueJieGuo2 >> 5 & 0x1) == 1 ? "闭合" : "断开";
            // bit6 二级保险1（1：闭合）
            XiTong_2JiBaoXian1.Text = (jueCePanJueJieGuo2 >> 6 & 0x1) == 1 ? "闭合" : "断开";
            // bit7 二级保险2（1：闭合）
            XiTong_2JiBaoXian2.Text = (jueCePanJueJieGuo2 >> 7 & 0x1) == 1 ? "闭合" : "断开";

            //----------------------------------------------------------------------//

            // 输出开关状态2
            byte shuChuKaiGuanStatus2 = sObject.shuChuKaiGuanStatus2;
            // bit0 二级起爆1（1：闭合）
            XiTong_2JiQiBao1.Text = (shuChuKaiGuanStatus2 >> 0 & 0x1) == 1 ? "闭合" : "断开";
            // bit1 二级起爆2（1：闭合）
            XiTong_2JiQiBao2.Text = (shuChuKaiGuanStatus2 >> 1 & 0x1) == 1 ? "闭合" : "断开";
            // bit2 bit3 参试状态（00：测试1，数据输出状态；01：测试2，低压输出状态；10：保留状态；11：正式实验）
            tempValue = (byte)(shuChuKaiGuanStatus2 >> 2 & 0x3);
            tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "测试1，数据输出状态";
                    break;
                case 1:
                    tempSTR = "测试2，低压输出状态";
                    break;
                case 2:
                    tempSTR = "保留状态";
                    break;
                case 3:
                    tempSTR = "正式实验";
                    break;
                default:
                    break;
            }
            XiTong_CanShiZhuangTai.Text = tempSTR;
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

        //-----------------------导航数据（快速）-----------------------//
        private void showDHKuaiSuTimeStatus(ref DAOHANGSHUJU_KuaiSu sObject)
        {
            // 导航系统时间
            DHKuaiSu_DaoHangXiTongShiJian.Text = sObject.daoHangXiTongShiJian.ToString();

            // sObject.jingDu;              // 经度（组合结果）当量：1e-7
            // sObject.weiDu;               // 纬度（组合结果）当量：1e-7
            // sObject.haiBaGaoDu;          // 海拔高度（组合结果）当量：1e-2

            //sObject.dongXiangSuDu;        // 东向速度（组合结果）当量：1e-2
            //sObject.beiXiangSuDu;         // 北向速度（组合结果）当量：1e-2
            //sObject.tianXiangSuDu;        // 天向速度（组合结果）当量：1e-2

            // GNSS时间 单位s,UTC秒部
            DHKuaiSu_GNSSTime.Text = sObject.GNSSTime.ToString();
            // 俯仰角
            DHKuaiSu_FuYangJiao.Text = sObject.fuYangJiao.ToString();
            // 滚转角
            DHKuaiSu_GunZhuanJiao.Text = sObject.gunZhuanJiao.ToString();
            // 偏航角
            DHKuaiSu_PianHangJiao.Text = sObject.pianHangJiao.ToString();

            // 陀螺X数据
            DHKuaiSu_TuoLuoXShuJuShang.Text = sObject.tuoLuoShuJu_X.ToString();
            // 陀螺Y数据
            DHKuaiSu_TuoLuoYShuJuShang.Text = sObject.tuoLuoShuJu_Y.ToString();
            // 陀螺Z数据
            DHKuaiSu_TuoLuoZShuJuShang.Text = sObject.tuoLuoShuJu_Z.ToString();

            // 加速度计X数据
            DHKuaiSu_JiaSuDuJiXShang.Text = sObject.jiaSuDuJiShuJu_X.ToString();
            // 加速度计Y数据
            DHKuaiSu_JiaSuDuJiYShang.Text = sObject.jiaSuDuJiShuJu_Y.ToString();
            // 加速度计Z数据
            DHKuaiSu_JiaSuDuJiZShang.Text = sObject.jiaSuDuJiShuJu_Z.ToString();

            // 陀螺X数据2
            DHKuaiSu_TuoLuoXShuJuBen.Text = sObject.tuoLuoShuJu_X2.ToString();
            // 陀螺Y数据2
            DHKuaiSu_TuoLuoYShuJuBen.Text = sObject.tuoLuoShuJu_Y2.ToString();
            // 陀螺Z数据2
            DHKuaiSu_TuoLuoZShuJuBen.Text = sObject.tuoLuoShuJu_Z2.ToString();

            // 加速度计X数据2
            DHKuaiSu_JiaSuDuJiXBen.Text = sObject.jiaSuDuJiShuJu_X2.ToString();
            // 加速度计Y数据2
            DHKuaiSu_JiaSuDuJiYBen.Text = sObject.jiaSuDuJiShuJu_Y2.ToString();
            // 加速度计Z数据2
            DHKuaiSu_JiaSuDuJiZBen.Text = sObject.jiaSuDuJiShuJu_Z2.ToString();

            // 状态标志位
            byte zhuangTaiBiaoZhiWei = sObject.zhuangTaiBiaoZhiWei;
            // bit0 点火标志（0：未点火 1：已点火）
            DHKuaiSu_DianHuo.Text = (zhuangTaiBiaoZhiWei >> 0 & 0x1) == 1 ? "已点火" : "未点火";
            // bit1 分离标志（0：已分离 1：未分离）
            DHKuaiSu_FenLi.Text = (zhuangTaiBiaoZhiWei >> 1 & 0x1) == 1 ? "未分离" : "已分离";
            // bit2 bit3 00:准备阶段 01：对准阶段 10：导航阶段
            byte tempValue = (byte)(zhuangTaiBiaoZhiWei >> 2 & 0x3);
            string tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "准备阶段";
                    break;
                case 1:
                    tempSTR = "对准阶段";
                    break;
                case 2:
                    tempSTR = "导航阶段";
                    break;
                default:
                    break;
            }
            DHKuaiSu_GongZuoJieDuan.Text = tempSTR;
            // bit4 bit5 00:GPS无更新 01：GPS有更新 10：GPS更新过
            tempValue = (byte)(zhuangTaiBiaoZhiWei >> 4 & 0x3);
            tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "GPS无更新";
                    break;
                case 1:
                    tempSTR = "GPS有更新";
                    break;
                case 2:
                    tempSTR = "GPS更新过";
                    break;
                default:
                    break;
            }
            DHKuaiSu_GPSShuJuGengXin.Text = tempSTR;
            // GPS组合标志 (00：上5ms惯导，本5ms惯导; 01：上5ms惯导，本5ms组合; 10：上5ms组合，本5ms组合; 11：上5ms组合，本5ms惯导;)
            tempValue = (byte)(zhuangTaiBiaoZhiWei >> 6 & 0x3);
            tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "上5ms惯导，本5ms惯导";
                    break;
                case 1:
                    tempSTR = "上5ms惯导，本5ms组合";
                    break;
                case 2:
                    tempSTR = "上5ms组合，本5ms组合";
                    break;
                case 3:
                    tempSTR = "上5ms组合，本5ms惯导";
                    break;
                default:
                    break;
            }
            DHKuaiSu_GPSZuHe.Text = tempSTR;

            // 陀螺故障标志
            byte tuoLuoGuZhangBiaoZhi = sObject.tuoLuoGuZhangBiaoZhi;
            // bit5 陀螺x故障标志（0：正常）
            DHKuaiSu_TuoLuoXGuZhang.Text = (tuoLuoGuZhangBiaoZhi >> 5 & 0x1) == 0 ? "正常" : "异常";
            // bit6 陀螺y故障标志（0：正常）
            DHKuaiSu_TuoLuoYGuZhang.Text = (tuoLuoGuZhangBiaoZhi >> 6 & 0x1) == 0 ? "正常" : "异常";
            // bit7 陀螺z故障标志（0：正常）
            DHKuaiSu_TuoLuoZGuZhang.Text = (tuoLuoGuZhangBiaoZhi >> 7 & 0x1) == 0 ? "正常" : "异常";
        }

        // 添加坐标点集
        private void AddDHKuaiSuZuoBiao(Int32 jingDu, Int32 weiDu, Int32 gaoDu)
        {
            DHKuaiSu_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, jingDu * Math.Pow(10, -7)));
            DHKuaiSu_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, weiDu * Math.Pow(10, -7)));
            DHKuaiSu_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, gaoDu * Math.Pow(10, -2)));
        }

        // 添加速度点集
        private void AddDHKuaiSuSuDu(Int32 dongXiangSuDu, Int32 beiXiangSuDu, Int32 tianXiangSuDu)
        {
            DHKuaiSu_SuDu_DongXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, dongXiangSuDu * Math.Pow(10, -2)));
            DHKuaiSu_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, beiXiangSuDu * Math.Pow(10, -2)));
            DHKuaiSu_SuDu_TianXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, tianXiangSuDu * Math.Pow(10, -2)));
        }

        //-----------------------导航数据（慢速）-----------------------//

        // 时间转UTC时间
        public double ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }

        // UTC 时间转北京时间
        public DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            startTime = startTime.AddHours(8);  // 转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        private void showDHManSuTimeStatus(ref DAOHANGSHUJU_ManSu sObject)
        {
            // GPS时间 单位s,UTC秒部
            DHManSu_GPSTime.Text = ConvertIntDatetime(sObject.GPSTime).ToString();
            // GPS定位模式
            byte GPSDingWeiMoShi = sObject.GPSDingWeiMoShi;
            string tempValueSTR = "";

            // bit0 (1:采用GPS定位 0:没有采用GPS定位)
            tempValueSTR += (GPSDingWeiMoShi >> 0 & 0x01) == 1 ? "采用GPS定位;" : "没有采用GPS定位;";
            // bit1 (1:采用BD2定位 0:没有采用BD2定位)
            tempValueSTR += (GPSDingWeiMoShi >> 1 & 0x01) == 1 ? "采用BD2定位;" : "没有采用BD2定位;";
            // bit2 1：采用GLONASS定位 0：没有采用GLONASS定位
            tempValueSTR += (GPSDingWeiMoShi >> 2 & 0x01) == 1 ? "采用GLONASS定位;" : "没有采用GLONASS定位;";
            // bit3 0:没有DGNSS可用 1：DGNSS可用
            tempValueSTR += (GPSDingWeiMoShi >> 3 & 0x01) == 1 ? "DGNSS可用;" : "没有DGNSS可用;";
            // bit4 bit5 (00:No Fix 01:2DFix 11:3D Fix)
            byte tempValue = (byte)(GPSDingWeiMoShi >> 4 & 0x03);
            tempValueSTR += tempValue == 0 ? "No Fix;" : (tempValue == 1 ? "2DFix" : (tempValue == 3 ? "3D Fix" : ""));
            // bit6 0:GNSS修正无效 1：GNSS修正有效
            tempValueSTR += (GPSDingWeiMoShi >> 6 & 0x01) == 1 ? "GNSS修正有效;" : "GNSS修正无效;";
            // bit7 0:BD2修正无效 1：BD2修正有效
            tempValueSTR += (GPSDingWeiMoShi >> 7 & 0x01) == 1 ? "BD2修正有效;" : "BD2修正无效;";
            DHManSu_GPSDingWeiZhuangTai.Text = tempValueSTR;


            // GPS SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
            tempValue = sObject.GPS_SV;
            DHManSu_GPSSVKeYong.Text = ((byte)(tempValue & 0xF)).ToString();
            DHManSu_GPSCanYuDingWei.Text = ((byte)(tempValue >> 4 & 0xF)).ToString();
            // BD2 SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
            tempValue = sObject.BD2_SV;
            textEdit32.Text = ((byte)(tempValue & 0xF)).ToString();
            DHManSu_BD2CanYuDingWei.Text = ((byte)(tempValue >> 4 & 0xF)).ToString();

            // 经度（GPS测量）当量：1e-7
            // 纬度（GPS测量）当量：1e-7
            // 海拔高度（GPS测量）当量：1e-2

            // 东向速度（GPS测量）当量：1e-2
            // 北向速度（GPS测量）当量：1e-2
            // 天向速度（GPS测量）当量：1e-2

            // PDOP 当量0.01
            DHManSu_PDOP.Text = ((double)(sObject.PDOP * 0.01)).ToString();
            // HDOP 当量0.01
            DHManSu_HDOP.Text = ((double)(sObject.HDOP * 0.01)).ToString(); 
            // VDOP 当量0.01
            DHManSu_VDOP.Text = ((double)(sObject.VDOP * 0.01)).ToString();

            // X陀螺温度
            DHManSu_XTuoLuoWenDu.Text = sObject.tuoLuoWenDu_X.ToString();
            // Y陀螺温度
            DHManSu_YTuoLuoWenDu.Text = sObject.tuoLuoWenDu_Y.ToString();
            // Z陀螺温度
            DHManSu_ZTuoLuoWenDu.Text = sObject.tuoLuoWenDu_Z.ToString();

            // X加计温度
            DHManSu_XJiaJiWenDu.Text = sObject.jiaJiWenDu_X.ToString();
            // Y加计温度
            DHManSu_YJiaJiWenDu.Text = sObject.jiaJiWenDu_Y.ToString();
            // Z加计温度
            DHManSu_ZJiaJiWenDu.Text = sObject.jiaJiWenDu_Z.ToString();

            // +5V电压值     当量0.05
            DHManSu_Zheng5VDianYa.Text = ((double)(sObject.dianYaZhi_zheng5V * 0.05)).ToString();
            // -5V电压值     当量0.05
            DHManSu_Fu5VDianYa.Text = ((double)(sObject.dianYaZhi_fu5V * 0.05)).ToString(); 

            // +15V电压值    当量0.02
            DHManSu_Zheng15VDianYa.Text = ((double)(sObject.dianYaZhi_zheng15V * 0.2)).ToString(); 
            // -15V电压值    当量0.02
            DHManSu_Fu15VDianYa.Text = ((double)(sObject.dianYaZhi_fu15V * 0.2)).ToString(); 

            // X陀螺+5V电压值     当量0.05
            DHManSu_XTuoLuoZheng5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_X_zheng5V * 0.05)).ToString(); 
            // X陀螺-5V电压值     当量0.05
            DHManSu_XTuoLuoFu5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_X_fu5V * 0.05)).ToString(); 

            // Y陀螺+5V电压值     当量0.05
            DHManSu_YTuoLuoZheng5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_Y_zheng5V * 0.05)).ToString(); 
            // Y陀螺-5V电压值     当量0.05
            DHManSu_YTuoLuoFu5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_Y_fu5V * 0.05)).ToString(); 

             // Z陀螺+5V电压值     当量0.05
            DHManSu_ZTuoLuoZheng5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_Z_zheng5V * 0.05)).ToString(); 
            // Z陀螺-5V电压值     当量0.05
            DHManSu_ZTuoLuoFu5VDianYa.Text = ((double)(sObject.tuoLuoDianYaZhi_Z_fu5V * 0.05)).ToString(); 

            // 与X陀螺通信错误计数（一直循环计数）
            DHManSu_XTuoLuoTongXinError.Text = sObject.yuTuoLuoTongXingCuoWuJiShu_X.ToString();
            // 与Y陀螺通信错误计数（一直循环计数）
            DHManSu_YTuoLuoTongXinError.Text = sObject.yuTuoLuoTongXingCuoWuJiShu_Y.ToString();
            // 与Z陀螺通信错误计数（一直循环计数）
            DHManSu_ZTuoLuoTongXinError.Text = sObject.yuTuoLuoTongXingCuoWuJiShu_Z.ToString();
            // 与GPS接收机通信错误计数（一直循环计数）
            DHManSu_GPSJieShouJiTongXinError.Text = sObject.yuGPSJieShouJiTongXingCuoWuJiShu.ToString();

            // IMU进入中断次数（每800次+1 循环计数）
            DHManSu_IMUZhongDuan.Text = sObject.IMUJinRuZhongDuanCiShu.ToString();
            // GPS中断次数（每10次+1 循环计数）
            DHManSu_GPSZhongDuan.Text = sObject.GPSZhongDuanCiShu.ToString();

            // 标志位1
            byte biaoZhiWei1 = sObject.biaoZhiWei1;
            // bit0 导航初始值装订标志（0:未装订 1：已装订）
            DHManSu_DaoHangChuZhiZhuangDing.Text = (biaoZhiWei1 >> 0 & 0x1) == 0 ? "未装订" : "已装订";
            // bit1 发送1553数据标志（0：未发送 1：已发送）
            DHManSu_1553ShuJuFaSong.Text = (biaoZhiWei1 >> 1 & 0x1) == 0 ? "未发送" : "已发送";
            // bit2 导航标志（0：未导航 1：已导航）
            DHManSu_DaoHangBiaoZhi.Text = (biaoZhiWei1 >> 2 & 0x1) == 0 ? "未导航" : "已导航";
            // bit3 对准完成标志(0:未对准 1：已对准)
            DHManSu_DuiZhunWanCheng.Text = (biaoZhiWei1 >> 3 & 0x1) == 0 ? "未对准" : "已对准";
            // bit4 装订参数读取标志(0:未装订 1：已装订)
            DHManSu_ZhuangDingCanShuDuQu.Text = (biaoZhiWei1 >> 4 & 0x1) == 0 ? "未装订" : "已装订";

            // 标志位2
            byte biaoZhiWei2 = sObject.biaoZhiWei2;
            // bit0 bit1 工作模式（00：飞行模式 01：仿真模式1 10：仿真模式2 11：调试模式）
            tempValue = (byte)(biaoZhiWei2 >> 0 & 0x3);
            string tempSTR = "";
            switch (tempValue)
            {
                case 0:
                    tempSTR = "飞行模式";
                    break;
                case 1:
                    tempSTR = "仿真模式1";
                    break;
                case 2:
                    tempSTR = "仿真模式2";
                    break;
                case 3:
                    tempSTR = "调试模式";
                    break;
                default:
                    break;
            }
            DHManSu_GongZuoMoShi.Text = tempSTR;

            // bit5 GPS组合标志（0：惯性 1：组合）
            DHManSu_GPSZuHe.Text = (biaoZhiWei2 >> 5 & 0x1) == 0 ? "惯性" : "组合";
            // bit6 点火标志(0：未点火 1：已点火)
            DHManSu_DianHuo.Text = (biaoZhiWei2 >> 6 & 0x1) == 0 ? "未点火" : "已点火";
            // bit7 分离标志（0：已分离 1：未分离）
            DHManSu_FenLi.Text = (biaoZhiWei2 >> 7 & 0x1) == 0 ? "已分离" : "未分离";
        }

    // 添加坐标点集
    private void AddDHManSuZuoBiao(Int32 jingDu, Int32 weiDu, Int32 gaoDu)
        {
            DHManSu_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, jingDu * Math.Pow(10, -7)));
            DHManSu_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, weiDu * Math.Pow(10, -7)));
            DHManSu_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, gaoDu * Math.Pow(10, -2)));
        }

        // 添加速度点集
        private void AddDHManSuSuDu(Int32 dongXiangSuDu, Int32 beiXiangSuDu, Int32 tianXiangSuDu)
        {
            DHManSu_SuDu_DongXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, dongXiangSuDu * Math.Pow(10, -2)));
            DHManSu_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, beiXiangSuDu * Math.Pow(10, -2)));
            DHManSu_SuDu_TianXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, tianXiangSuDu * Math.Pow(10, -2)));
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
            //清空所有的曲线

            // 系统判决状态曲线
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

            // 导航数据（快速）状态曲线
            DHKuaiSu_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_DHKuaiSu_ZuoBiao.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_DHKuaiSu_SuDu.Series)
            {
                series.Points.Clear();
            }

            // 导航数据（慢速）状态曲线
            DHManSu_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_DHManSu_ZuoBiao.Series)
            {
                series.Points.Clear();
            }
            foreach (Series series in chart_DHManSu_SuDu.Series)
            {
                series.Points.Clear();
            }

            //-----------------------------------------------------//

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
            // 系统判决状态
            //---------------------------------------//
            // 坐标
            chart_XiTong_ZuoBiao.Series["经度"].Points.AddRange(xiTong_ZuoBiao_JingDu_Buffer.ToArray());
            chart_XiTong_ZuoBiao.Series["纬度"].Points.AddRange(xiTong_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_XiTong_ZuoBiao.Series["海拔高度"].Points.AddRange(xiTong_ZuoBiao_GaoDu_Buffer.ToArray());
            xiTong_ZuoBiao_JingDu_Buffer.Clear();
            xiTong_ZuoBiao_WeiDu_Buffer.Clear();
            xiTong_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_XiTong_SuDu.Series["东向速度"].Points.AddRange(xiTong_SuDu_DongXiang_Buffer.ToArray());
            chart_XiTong_SuDu.Series["北向速度"].Points.AddRange(xiTong_SuDu_BeiXiang_Buffer.ToArray());
            chart_XiTong_SuDu.Series["天向速度"].Points.AddRange(xiTong_SuDu_TianXiang_Buffer.ToArray());
            xiTong_SuDu_DongXiang_Buffer.Clear();
            xiTong_SuDu_BeiXiang_Buffer.Clear();
            xiTong_SuDu_TianXiang_Buffer.Clear();

            //---------------------------------------//
            // 角速度
            chart_XiTong_JiaoSuDu.Series["Wx角速度"].Points.AddRange(xiTong_JiaoSuDu_Wx_Buffer.ToArray());
            chart_XiTong_JiaoSuDu.Series["Wy角速度"].Points.AddRange(xiTong_JiaoSuDu_Wy_Buffer.ToArray());
            chart_XiTong_JiaoSuDu.Series["Wz角速度"].Points.AddRange(xiTong_JiaoSuDu_Wz_Buffer.ToArray());
            xiTong_JiaoSuDu_Wx_Buffer.Clear();
            xiTong_JiaoSuDu_Wy_Buffer.Clear();
            xiTong_JiaoSuDu_Wz_Buffer.Clear();

            //---------------------------------------//
            // 发射系
            chart_XiTong_FaSheXi.Series["轴向过载"].Points.AddRange(xiTong_ZhouXiangGuoZai_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系X"].Points.AddRange(xiTong_FaSheXi_x_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系Y"].Points.AddRange(xiTong_FaSheXi_y_Buffer.ToArray());
            chart_XiTong_FaSheXi.Series["当前发射系Z"].Points.AddRange(xiTong_FaSheXi_z_Buffer.ToArray());
            xiTong_ZhouXiangGuoZai_Buffer.Clear();
            xiTong_FaSheXi_x_Buffer.Clear();
            xiTong_FaSheXi_y_Buffer.Clear();
            xiTong_FaSheXi_z_Buffer.Clear();

            //---------------------------------------//
            // 预示落点
            chart_XiTong_YuShiLuoDian.Series["预示落点射程"].Points.AddRange(xiTong_YuShiLuoDian_SheCheng_Buffer.ToArray());
            chart_XiTong_YuShiLuoDian.Series["预示落点Z"].Points.AddRange(xiTong_YuShiLuoDian_Z_Buffer.ToArray());
            xiTong_YuShiLuoDian_SheCheng_Buffer.Clear();
            xiTong_YuShiLuoDian_Z_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 导航数据（快速）
            //---------------------------------------//
            // 坐标
            chart_DHKuaiSu_ZuoBiao.Series["经度"].Points.AddRange(DHKuaiSu_ZuoBiao_JingDu_Buffer.ToArray());
            chart_DHKuaiSu_ZuoBiao.Series["纬度"].Points.AddRange(DHKuaiSu_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_DHKuaiSu_ZuoBiao.Series["海拔高度"].Points.AddRange(DHKuaiSu_ZuoBiao_GaoDu_Buffer.ToArray());
            DHKuaiSu_ZuoBiao_JingDu_Buffer.Clear();
            DHKuaiSu_ZuoBiao_WeiDu_Buffer.Clear();
            DHKuaiSu_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_DHKuaiSu_SuDu.Series["东向速度"].Points.AddRange(DHKuaiSu_SuDu_DongXiang_Buffer.ToArray());
            chart_DHKuaiSu_SuDu.Series["北向速度"].Points.AddRange(DHKuaiSu_SuDu_BeiXiang_Buffer.ToArray());
            chart_DHKuaiSu_SuDu.Series["天向速度"].Points.AddRange(DHKuaiSu_SuDu_TianXiang_Buffer.ToArray());
            DHKuaiSu_SuDu_DongXiang_Buffer.Clear();
            DHKuaiSu_SuDu_BeiXiang_Buffer.Clear();
            DHKuaiSu_SuDu_TianXiang_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 导航数据（慢速）
            //---------------------------------------//
            // 坐标
            chart_DHManSu_ZuoBiao.Series["经度"].Points.AddRange(DHManSu_ZuoBiao_JingDu_Buffer.ToArray());
            chart_DHManSu_ZuoBiao.Series["纬度"].Points.AddRange(DHManSu_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_DHManSu_ZuoBiao.Series["海拔高度"].Points.AddRange(DHManSu_ZuoBiao_GaoDu_Buffer.ToArray());
            DHManSu_ZuoBiao_JingDu_Buffer.Clear();
            DHManSu_ZuoBiao_WeiDu_Buffer.Clear();
            DHManSu_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_DHManSu_SuDu.Series["东向速度"].Points.AddRange(DHManSu_SuDu_DongXiang_Buffer.ToArray());
            chart_DHManSu_SuDu.Series["北向速度"].Points.AddRange(DHManSu_SuDu_BeiXiang_Buffer.ToArray());
            chart_DHManSu_SuDu.Series["天向速度"].Points.AddRange(DHManSu_SuDu_TianXiang_Buffer.ToArray());
            DHManSu_SuDu_DongXiang_Buffer.Clear();
            DHManSu_SuDu_BeiXiang_Buffer.Clear();
            DHManSu_SuDu_TianXiang_Buffer.Clear();

            //-----------------------------------------------------------------------------------//
            // 测试 绘图功能
            Random rnd = new Random();
            chart_XiTong_ZuoBiao.Series["经度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_XiTong_ZuoBiao.Series["纬度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_XiTong_ZuoBiao.Series["海拔高度"].Points.AddPoint(xiTong_CHART_ITEM_INDEX, rnd.Next(1, 100));
            xiTong_CHART_ITEM_INDEX++;

            chart_DHKuaiSu_ZuoBiao.Series["经度"].Points.AddPoint(DHKuaiSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_DHKuaiSu_ZuoBiao.Series["纬度"].Points.AddPoint(DHKuaiSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_DHKuaiSu_ZuoBiao.Series["海拔高度"].Points.AddPoint(DHKuaiSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            DHKuaiSu_CHART_ITEM_INDEX++;

            chart_DHManSu_ZuoBiao.Series["经度"].Points.AddPoint(DHManSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_DHManSu_ZuoBiao.Series["纬度"].Points.AddPoint(DHManSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            chart_DHManSu_ZuoBiao.Series["海拔高度"].Points.AddPoint(DHManSu_CHART_ITEM_INDEX, rnd.Next(1, 100));
            DHManSu_CHART_ITEM_INDEX++;
            //-----------------------------------------------------------------------------------//
        }
    }
}
