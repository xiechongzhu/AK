using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YaoCeProcess
{
    public partial class XiTongJiShiSubForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量
        //--------//
        // 创建曲线X轴索引值
        private int XiTongJiShi_CHART_ITEM_INDEX = 0;
        // 系统即时状态绘图数据缓存
        private List<SeriesPoint> XiTongJiShi_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();      // 经度
        private List<SeriesPoint> XiTongJiShi_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();       // 纬度
        private List<SeriesPoint> XiTongJiShi_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();       // 海拔高度

        private List<SeriesPoint> XiTongJiShi_SuDu_DongXiang_Buffer = new List<SeriesPoint>();      // 东向速度
        private List<SeriesPoint> XiTongJiShi_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();       // 北向速度
        private List<SeriesPoint> XiTongJiShi_SuDu_TianXiang_Buffer = new List<SeriesPoint>();      // 天向速度

        private List<SeriesPoint> XiTongJiShi_JiaoSuDu_Wx_Buffer = new List<SeriesPoint>();         // Wx角速度
        private List<SeriesPoint> XiTongJiShi_JiaoSuDu_Wy_Buffer = new List<SeriesPoint>();         // Wy角速度
        private List<SeriesPoint> XiTongJiShi_JiaoSuDu_Wz_Buffer = new List<SeriesPoint>();         // Wz角速度

        private List<SeriesPoint> XiTongJiShi_GuoZai_ZhouXiang_Buffer = new List<SeriesPoint>();    // 轴向过载
        private List<SeriesPoint> XiTongJiShi_GuoZai_FaXiang_Buffer = new List<SeriesPoint>();      // 法向过载
        private List<SeriesPoint> XiTongJiShi_GuoZai_CeXiang_Buffer = new List<SeriesPoint>();      // 侧向过载

        // 状态数据缓存
        public SYSTEMImmediate_STATUS sObject_XTJS;

        // 是否收到数据
        bool bRecvStatusData = false;

        //--------------------------------------------------------------//
        public delegate void setStatusOnOffLine(uint statusType, bool bOn);
        // 用来接收父窗体方法的委托变量
        public setStatusOnOffLine testFunDelegate;
        public uint statusType;
        //--------------------------------------------------------------//

        public XiTongJiShiSubForm()
        {
            InitializeComponent();
            // 离线定时器初始即启动
            timerOfflineXiTongJiShiStatus.Start();

            // 初始清空数据
            GenericFunction.reSetAllTextEdit(this);
        }

        private void showXiTongJiShiTimeStatus(ref SYSTEMImmediate_STATUS sObject)
        {
            // 故障标志位
            byte guZhangBiaoZhi = sObject.guZhangBiaoZhi;
            // bit0 陀螺x故障标志（0：正常；1：故障）
            XTJS_XTuoLuoGuZhang.Text = (guZhangBiaoZhi & 0x1) == 0 ? "正常" : "故障";
            // bit1 陀螺y故障标志（0：正常；1：故障）
            XTJS_YTuoLuoGuZhang.Text = (guZhangBiaoZhi >> 1 & 0x1) == 0 ? "正常" : "故障";
            // bit2 陀螺z故障标志（0：正常；1：故障）
            XTJS_ZTuoLuoGuZhang.Text = (guZhangBiaoZhi >> 2 & 0x1) == 0 ? "正常" : "故障";
            // bit3 RS422故障标志（0：正常；1：故障）
            XTJS_RS422GuZhang.Text = (guZhangBiaoZhi >> 3 & 0x1) == 0 ? "正常" : "故障";
            // bit4 1553B故障标志（0：正常；1：故障）
            XTJS_1553BGuZhang.Text = (guZhangBiaoZhi >> 4 & 0x1) == 0 ? "正常" : "故障";

            // X陀螺温度
            // Y陀螺温度
            // Z陀螺温度
            XTJS_XTuoLuoWenDu.Text = sObject.tuoLuoWenDu_X.ToString();
            XTJS_YTuoLuoWenDu.Text = sObject.tuoLuoWenDu_Y.ToString();
            XTJS_ZTuoLuoWenDu.Text = sObject.tuoLuoWenDu_Z.ToString();

            // GPS SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
            byte tempValue = sObject.GPS_SV;
            XTJS_GPSSVKeYong.Text = ((byte)(tempValue & 0xF)).ToString();
            XTJS_GPSCanYuDingWei.Text = ((byte)(tempValue >> 4 & 0xF)).ToString();

            // GPS定位模式
            byte GPSDingWeiMoShi = sObject.GPSDingWeiMoShi;
            string tempValueSTR = "";
            // bit0 (1:采用GPS定位 0:没有采用GPS定位)
            tempValueSTR = (GPSDingWeiMoShi >> 0 & 0x01) == 1 ? "采用GPS定位" : "没有采用GPS定位";
            XTJS_GPSDingWeiZhuangTai_GPS.Text = tempValueSTR;
            // bit1 (1:采用BD2定位 0:没有采用BD2定位)
            tempValueSTR = (GPSDingWeiMoShi >> 1 & 0x01) == 1 ? "采用BD2定位" : "没有采用BD2定位";
            XTJS_GPSDingWeiZhuangTai_BD2.Text = tempValueSTR;
            // bit2 1：采用GLONASS定位 0：没有采用GLONASS定位
            tempValueSTR = (GPSDingWeiMoShi >> 2 & 0x01) == 1 ? "采用GLONASS定位" : "没有采用GLONASS定位";
            XTJS_GPSDingWeiZhuangTai_GLONASS.Text = tempValueSTR;
            // bit3 0:没有DGNSS可用 1：DGNSS可用
            tempValueSTR = (GPSDingWeiMoShi >> 3 & 0x01) == 1 ? "DGNSS可用" : "没有DGNSS可用";
            XTJS_GPSDingWeiZhuangTai_DGNSS.Text = tempValueSTR;
            // bit4 bit5 (00:No Fix 01:2DFix 11:3D Fix)
            tempValue = (byte)(GPSDingWeiMoShi >> 4 & 0x03);
            tempValueSTR = tempValue == 0 ? "No Fix" : (tempValue == 1 ? "2DFix" : (tempValue == 3 ? "3D Fix" : ""));
            XTJS_GPSDingWeiZhuangTai_Fix.Text = tempValueSTR;
            // bit6 0:GNSS修正无效 1：GNSS修正有效
            tempValueSTR = (GPSDingWeiMoShi >> 6 & 0x01) == 1 ? "GNSS修正有效" : "GNSS修正无效";
            XTJS_GPSDingWeiZhuangTai_GNSSXiuZheng.Text = tempValueSTR;
            // bit7 0:BD2修正无效 1：BD2修正有效
            tempValueSTR = (GPSDingWeiMoShi >> 7 & 0x01) == 1 ? "BD2修正有效" : "BD2修正无效";
            XTJS_GPSDingWeiZhuangTai_BD2XiuZheng.Text = tempValueSTR;
            // DHManSu_GPSDingWeiZhuangTai.Text = tempValueSTR;

            // PDOP 当量0.01
            XTJS_PDOP.Text = ((double)(sObject.PDOP)).ToString();
            // HDOP 当量0.01
            XTJS_HDOP.Text = ((double)(sObject.HDOP)).ToString();
            // VDOP 当量0.01
            XTJS_VDOP.Text = ((double)(sObject.VDOP)).ToString();

            // GPS时间 单位s,UTC秒部
            XTJS_GPSTime.Text = ((double)(sObject.GPSTime * 0.1)).ToString();

            // sObject.jingDu;              // 经度（组合结果）当量：1e-7
            XTJS_JingDu.Text = ((double)(sObject.jingDu * Math.Pow(10, -7))).ToString();
            // sObject.weiDu;               // 纬度（组合结果）当量：1e-7
            XTJS_WeiDu.Text = ((double)(sObject.weiDu * Math.Pow(10, -7))).ToString();
            // sObject.haiBaGaoDu;          // 海拔高度（组合结果）当量：1e-2
            XTJS_GaoDu.Text = ((double)(sObject.haiBaGaoDu * Math.Pow(10, -2))).ToString();

            //sObject.dongXiangSuDu;        // 东向速度（组合结果）当量：1e-2
            XTJS_DongXiangSuDu.Text = ((double)(sObject.dongXiangSuDu * Math.Pow(10, -2))).ToString();
            //sObject.beiXiangSuDu;         // 北向速度（组合结果）当量：1e-2
            XTJS_BeiXiangSuDu.Text = ((double)(sObject.beiXiangSuDu * Math.Pow(10, -2))).ToString();
            //sObject.tianXiangSuDu;        // 天向速度（组合结果）当量：1e-2
            XTJS_TianXiangSuDu.Text = ((double)(sObject.tianXiangSuDu * Math.Pow(10, -2))).ToString();

            // 轴向过载
            // 法向过载
            // 侧向过载
            XTJS_GuoZhai_ZhouXiang.Text = sObject.zhouXiangGuoZai.ToString();
            XTJS_GuoZhai_FaXiang.Text = sObject.faXiangGuoZai.ToString();
            XTJS_GuoZhai_CeXiang.Text = sObject.ceXiangGuoZai.ToString();

            // Wx角速度
            // Wy角速度
            // Wz角速度
            XTJS_JiaoSuDu_X.Text = sObject.WxJiaoSuDu.ToString();
            XTJS_JiaoSuDu_Y.Text = sObject.WyJiaoSuDu.ToString();
            XTJS_JiaoSuDu_Z.Text = sObject.WzJiaoSuDu.ToString();
        }

        // 添加坐标点集
        public void AddXiTongJiShiZuoBiao(Int32 jingDu, Int32 weiDu, Int32 gaoDu)
        {
            XiTongJiShi_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, jingDu * Math.Pow(10, -7)));
            XiTongJiShi_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, weiDu * Math.Pow(10, -7)));
            XiTongJiShi_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, gaoDu * Math.Pow(10, -2)));
        }

        // 添加速度点集
        public void AddXiTongJiShiSuDu(Int32 dongXiangSuDu, Int32 beiXiangSuDu, Int32 tianXiangSuDu)
        {
            XiTongJiShi_SuDu_DongXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, dongXiangSuDu * Math.Pow(10, -2)));
            XiTongJiShi_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, beiXiangSuDu * Math.Pow(10, -2)));
            XiTongJiShi_SuDu_TianXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, tianXiangSuDu * Math.Pow(10, -2)));
        }

        // 添加角速度点集
        public void AddXiTongJiShiJiaoSuDu(float Wx, float Wy, float Wz)
        {
            XiTongJiShi_JiaoSuDu_Wx_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, Wx));
            XiTongJiShi_JiaoSuDu_Wy_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, Wy));
            XiTongJiShi_JiaoSuDu_Wz_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, Wz));
        }

        // 添加过载点集
        public void AddXiTongJiShiGuoZai(float zhouXiang, float faXiang, float ceXiang)
        {
            XiTongJiShi_GuoZai_ZhouXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, zhouXiang));
            XiTongJiShi_GuoZai_FaXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, faXiang));
            XiTongJiShi_GuoZai_CeXiang_Buffer.Add(new SeriesPoint(XiTongJiShi_CHART_ITEM_INDEX, ceXiang));
        }

        // 绘图定时器，每隔100ms刷新绘图区域
        private void timerUpdateChart_Tick(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------------//
            // 导航数据（慢速）
            //---------------------------------------//
            // 坐标
            chart_XTJS_ZuoBiao_JingDu.Series["经度"].Points.AddRange(XiTongJiShi_ZuoBiao_JingDu_Buffer.ToArray());
            chart_XTJS_ZuoBiao_WeiDu.Series["纬度"].Points.AddRange(XiTongJiShi_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_XTJS_ZuoBiao_GaoDu.Series["海拔高度"].Points.AddRange(XiTongJiShi_ZuoBiao_GaoDu_Buffer.ToArray());
            XiTongJiShi_ZuoBiao_JingDu_Buffer.Clear();
            XiTongJiShi_ZuoBiao_WeiDu_Buffer.Clear();
            XiTongJiShi_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_XTJS_SuDu_Dong.Series["东向速度"].Points.AddRange(XiTongJiShi_SuDu_DongXiang_Buffer.ToArray());
            chart_XTJS_SuDu_Bei.Series["北向速度"].Points.AddRange(XiTongJiShi_SuDu_BeiXiang_Buffer.ToArray());
            chart_XTJS_SuDu_Tian.Series["天向速度"].Points.AddRange(XiTongJiShi_SuDu_TianXiang_Buffer.ToArray());
            XiTongJiShi_SuDu_DongXiang_Buffer.Clear();
            XiTongJiShi_SuDu_BeiXiang_Buffer.Clear();
            XiTongJiShi_SuDu_TianXiang_Buffer.Clear();

            //---------------------------------------//
            // 角速度
            chart_XTJS_JiaoSuDu_Wx.Series["X轴角速度"].Points.AddRange(XiTongJiShi_JiaoSuDu_Wx_Buffer.ToArray());
            chart_XTJS_JiaoSuDu_Wy.Series["Y轴角速度"].Points.AddRange(XiTongJiShi_JiaoSuDu_Wy_Buffer.ToArray());
            chart_XTJS_JiaoSuDu_Wz.Series["Z轴角速度"].Points.AddRange(XiTongJiShi_JiaoSuDu_Wz_Buffer.ToArray());
            XiTongJiShi_JiaoSuDu_Wx_Buffer.Clear();
            XiTongJiShi_JiaoSuDu_Wy_Buffer.Clear();
            XiTongJiShi_JiaoSuDu_Wz_Buffer.Clear();

            //---------------------------------------//
            // 过载
            chart_XTJS_GuoZai_ZhouXiang.Series["轴向过载"].Points.AddRange(XiTongJiShi_GuoZai_ZhouXiang_Buffer.ToArray());
            chart_XTJS_GuoZai_FaXiang.Series["法向过载"].Points.AddRange(XiTongJiShi_GuoZai_FaXiang_Buffer.ToArray());
            chart_XTJS_GuoZai_CeXiang.Series["侧向过载"].Points.AddRange(XiTongJiShi_GuoZai_CeXiang_Buffer.ToArray());
            XiTongJiShi_GuoZai_ZhouXiang_Buffer.Clear();
            XiTongJiShi_GuoZai_FaXiang_Buffer.Clear();
            XiTongJiShi_GuoZai_CeXiang_Buffer.Clear();
        }

        public void clearAllChart()
        {
            XiTongJiShi_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_XTJS_ZuoBiao_JingDu.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_ZuoBiao_JingDu.Update();
            foreach (Series series in chart_XTJS_ZuoBiao_WeiDu.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_ZuoBiao_WeiDu.Update();
            foreach (Series series in chart_XTJS_ZuoBiao_GaoDu.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_ZuoBiao_GaoDu.Update();
            foreach (Series series in chart_XTJS_SuDu_Dong.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_SuDu_Dong.Update();
            foreach (Series series in chart_XTJS_SuDu_Bei.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_SuDu_Bei.Update();
            foreach (Series series in chart_XTJS_SuDu_Tian.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_SuDu_Tian.Update();

            // 角速度
            foreach (Series series in chart_XTJS_JiaoSuDu_Wx.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_JiaoSuDu_Wx.Update();
            foreach (Series series in chart_XTJS_JiaoSuDu_Wy.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_JiaoSuDu_Wy.Update();
            foreach (Series series in chart_XTJS_JiaoSuDu_Wz.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_JiaoSuDu_Wz.Update();

            // 过载
            foreach (Series series in chart_XTJS_GuoZai_ZhouXiang.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_GuoZai_ZhouXiang.Update();
            foreach (Series series in chart_XTJS_GuoZai_FaXiang.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_GuoZai_FaXiang.Update();
            foreach (Series series in chart_XTJS_GuoZai_CeXiang.Series)
            {
                series.Points.Clear();
            }
            chart_XTJS_GuoZai_CeXiang.Update();
        }

        private void timerUpdateXiTongJiShiStatus_Tick(object sender, EventArgs e)
        {
            // 是否收到数据
            if (bRecvStatusData)
            {
                // 填充实时数据
                showXiTongJiShiTimeStatus(ref sObject_XTJS);

                // 更改状态背景颜色
                GenericFunction.changeAllTextEditColor(this);

                testFunDelegate(statusType, true);
            }
        }

        public void setUpdateTimerStatus(bool bOpen)
        {
            if (bOpen)
            {
                timerUpdateXiTongJiShiStatus.Start();
            }
            else
            {
                timerUpdateXiTongJiShiStatus.Stop();
            }
        }

        public void startTimerUpdateChart(bool start)
        {
            if (!start)
            {
                timerUpdateChart.Stop();
            }
            else
            {
                timerUpdateChart.Start();
            }
        }

        public void SetXiTongJiShiStatus(ref SYSTEMImmediate_STATUS sObject)
        {
            sObject_XTJS = sObject;

            // 重新启动离线定时器
            timerOfflineXiTongJiShiStatus.Stop();
            timerOfflineXiTongJiShiStatus.Start();

            // 是否收到数据
            bRecvStatusData = true;
        }

        public void setCHARTITEMINDEXAdd()
        {
            XiTongJiShi_CHART_ITEM_INDEX++;
        }

        private void timerOfflineXiTongJiShiStatus_Tick(object sender, EventArgs e)
        {
            // 清空数据
            // TODO 这里不需要清空最后一帧数据显示
            // GenericFunction.reSetAllTextEdit(this);

            // 是否收到数据
            bRecvStatusData = false;
            testFunDelegate(statusType, false);
        }

        public void initUI()
        {
            // 初始清空数据
            GenericFunction.reSetAllTextEdit(this);
        }
    }
}
