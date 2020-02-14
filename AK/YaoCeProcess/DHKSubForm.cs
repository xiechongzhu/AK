using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
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
    public partial class DHKSubForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量
        //--------//
        // 创建曲线X轴索引值
        private int DHKuaiSu_CHART_ITEM_INDEX = 0;
        // 导航快速状态绘图数据缓存
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();     // 经度
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();      // 纬度
        private List<SeriesPoint> DHKuaiSu_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();      // 海拔高度

        private List<SeriesPoint> DHKuaiSu_SuDu_DongXiang_Buffer = new List<SeriesPoint>();     // 东向速度
        private List<SeriesPoint> DHKuaiSu_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();      // 北向速度
        private List<SeriesPoint> DHKuaiSu_SuDu_TianXiang_Buffer = new List<SeriesPoint>();     // 天向速度

        // 状态数据缓存
        public DAOHANGSHUJU_KuaiSu sObject_DHK;

        // 是否收到数据
        bool bRecvStatusData = false;

        //--------------------------------------------------------------//

        public DHKSubForm()
        {
            InitializeComponent();
            // 离线定时器初始即启动
            timerOfflineDHKStatus.Start();

            // 初始清空数据
            GenericFunction.reSetAllTextEdit(this);
        }

        ~DHKSubForm()
        {
            timerOfflineDHKStatus.Stop();
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
        public void AddDHKuaiSuZuoBiao(Int32 jingDu, Int32 weiDu, Int32 gaoDu)
        {
            DHKuaiSu_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, jingDu * Math.Pow(10, -7)));
            DHKuaiSu_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, weiDu * Math.Pow(10, -7)));
            DHKuaiSu_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, gaoDu * Math.Pow(10, -2)));
        }

        // 添加速度点集
        public void AddDHKuaiSuSuDu(Int32 dongXiangSuDu, Int32 beiXiangSuDu, Int32 tianXiangSuDu)
        {
            DHKuaiSu_SuDu_DongXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, dongXiangSuDu * Math.Pow(10, -2)));
            DHKuaiSu_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, beiXiangSuDu * Math.Pow(10, -2)));
            DHKuaiSu_SuDu_TianXiang_Buffer.Add(new SeriesPoint(DHKuaiSu_CHART_ITEM_INDEX, tianXiangSuDu * Math.Pow(10, -2)));
        }

        // 绘图定时器，每隔100ms刷新绘图区域
        private void timerUpdateChart_Tick(object sender, EventArgs e)
        {
            // 导航数据（快速）
            //---------------------------------------//
            // 坐标
            chart_DHKuaiSu_ZuoBiao_JingDu.Series["经度"].Points.AddRange(DHKuaiSu_ZuoBiao_JingDu_Buffer.ToArray());
            chart_DHKuaiSu_ZuoBiao_WeiDu.Series["纬度"].Points.AddRange(DHKuaiSu_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_DHKuaiSu_ZuoBiao_GaoDu.Series["海拔高度"].Points.AddRange(DHKuaiSu_ZuoBiao_GaoDu_Buffer.ToArray());
            DHKuaiSu_ZuoBiao_JingDu_Buffer.Clear();
            DHKuaiSu_ZuoBiao_WeiDu_Buffer.Clear();
            DHKuaiSu_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_DHKuaiSu_SuDu_Dong.Series["东向速度"].Points.AddRange(DHKuaiSu_SuDu_DongXiang_Buffer.ToArray());
            chart_DHKuaiSu_SuDu_Bei.Series["北向速度"].Points.AddRange(DHKuaiSu_SuDu_BeiXiang_Buffer.ToArray());
            chart_DHKuaiSu_SuDu_Tian.Series["天向速度"].Points.AddRange(DHKuaiSu_SuDu_TianXiang_Buffer.ToArray());
            DHKuaiSu_SuDu_DongXiang_Buffer.Clear();
            DHKuaiSu_SuDu_BeiXiang_Buffer.Clear();
            DHKuaiSu_SuDu_TianXiang_Buffer.Clear();
        }

        public void clearAllChart()
        {
            // 导航数据（快速）状态曲线
            DHKuaiSu_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_DHKuaiSu_ZuoBiao_JingDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_ZuoBiao_JingDu.Update();
            foreach (Series series in chart_DHKuaiSu_ZuoBiao_WeiDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_ZuoBiao_WeiDu.Update();
            foreach (Series series in chart_DHKuaiSu_ZuoBiao_GaoDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_ZuoBiao_GaoDu.Update();
            foreach (Series series in chart_DHKuaiSu_SuDu_Dong.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_SuDu_Dong.Update();
            foreach (Series series in chart_DHKuaiSu_SuDu_Bei.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_SuDu_Bei.Update();
            foreach (Series series in chart_DHKuaiSu_SuDu_Tian.Series)
            {
                series.Points.Clear();
            }
            chart_DHKuaiSu_SuDu_Tian.Update();
        }

        private void timerUpdateDHKStatus_Tick(object sender, EventArgs e)
        {
            // 是否收到数据
            if (bRecvStatusData)
            {
                // 填充实时数据
                showDHKuaiSuTimeStatus(ref sObject_DHK);
            }
        }

        public void setUpdateTimerStatus(bool bOpen)
        {
            if (bOpen)
            {
                timerUpdateDHKStatus.Start();
            }
            else
            {
                timerUpdateDHKStatus.Stop();
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

        public void SetDHKStatus(ref DAOHANGSHUJU_KuaiSu sObject)
        {
            sObject_DHK = sObject;
            // 重新启动离线定时器
            timerOfflineDHKStatus.Stop();
            timerOfflineDHKStatus.Start();

            // 是否收到数据
            bRecvStatusData = true;
        }

        public void setCHARTITEMINDEXAdd()
        {
            DHKuaiSu_CHART_ITEM_INDEX++;
        }

        private void timerOfflineDHKStatus_Tick(object sender, EventArgs e)
        {
            // 清空数据
            GenericFunction.reSetAllTextEdit(this);

            // 是否收到数据
            bRecvStatusData = false;
        }
    }
}
