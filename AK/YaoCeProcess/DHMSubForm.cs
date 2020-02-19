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
    public partial class DHMSubForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量
        //--------//
        // 创建曲线X轴索引值
        private int DHManSu_CHART_ITEM_INDEX = 0;
        // 导航慢速状态绘图数据缓存
        private List<SeriesPoint> DHManSu_ZuoBiao_JingDu_Buffer = new List<SeriesPoint>();      // 经度
        private List<SeriesPoint> DHManSu_ZuoBiao_WeiDu_Buffer = new List<SeriesPoint>();       // 纬度
        private List<SeriesPoint> DHManSu_ZuoBiao_GaoDu_Buffer = new List<SeriesPoint>();       // 海拔高度

        private List<SeriesPoint> DHManSu_SuDu_DongXiang_Buffer = new List<SeriesPoint>();      // 东向速度
        private List<SeriesPoint> DHManSu_SuDu_BeiXiang_Buffer = new List<SeriesPoint>();       // 北向速度
        private List<SeriesPoint> DHManSu_SuDu_TianXiang_Buffer = new List<SeriesPoint>();      // 天向速度

        // 状态数据缓存
        public DAOHANGSHUJU_ManSu sObject_DHM;

        // 是否收到数据
        bool bRecvStatusData = false;

        //--------------------------------------------------------------//
        public delegate void setStatusOnOffLine(uint statusType, bool bOn);
        // 用来接收父窗体方法的委托变量
        public setStatusOnOffLine testFunDelegate;
        public uint statusType;
        //--------------------------------------------------------------//

        public DHMSubForm()
        {
            InitializeComponent();
            // 离线定时器初始即启动
            timerOfflineDHMStatus.Start();

            // 初始清空数据
            GenericFunction.reSetAllTextEdit(this);
        }

        ~DHMSubForm()
        {
            timerOfflineDHMStatus.Stop();
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
        public DateTime ConvertIntDatetime(uint utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds((double)utc);
            startTime = startTime.AddHours(8);  // 转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        private void showDHManSuTimeStatus(ref DAOHANGSHUJU_ManSu sObject)
        {
            // GPS时间 单位s,UTC秒部
            DHManSu_GPSTime.Text = /*ConvertIntDatetime(sObject.GPSTime).ToString()*/sObject.GPSTime.ToString();
            // GPS定位模式
            byte GPSDingWeiMoShi = sObject.GPSDingWeiMoShi;
            string tempValueSTR = "";

            // bit0 (1:采用GPS定位 0:没有采用GPS定位)
            tempValueSTR = (GPSDingWeiMoShi >> 0 & 0x01) == 1 ? "采用GPS定位" : "没有采用GPS定位";
            DHManSu_GPSDingWeiZhuangTai_GPS.Text = tempValueSTR;
            // bit1 (1:采用BD2定位 0:没有采用BD2定位)
            tempValueSTR = (GPSDingWeiMoShi >> 1 & 0x01) == 1 ? "采用BD2定位" : "没有采用BD2定位";
            DHManSu_GPSDingWeiZhuangTai_BD2.Text = tempValueSTR;
            // bit2 1：采用GLONASS定位 0：没有采用GLONASS定位
            tempValueSTR = (GPSDingWeiMoShi >> 2 & 0x01) == 1 ? "采用GLONASS定位" : "没有采用GLONASS定位";
            DHManSu_GPSDingWeiZhuangTai_GLONASS.Text = tempValueSTR;
            // bit3 0:没有DGNSS可用 1：DGNSS可用
            tempValueSTR = (GPSDingWeiMoShi >> 3 & 0x01) == 1 ? "DGNSS可用" : "没有DGNSS可用";
            DHManSu_GPSDingWeiZhuangTai_DGNSS.Text = tempValueSTR;
            // bit4 bit5 (00:No Fix 01:2DFix 11:3D Fix)
            byte tempValue = (byte)(GPSDingWeiMoShi >> 4 & 0x03);
            tempValueSTR = tempValue == 0 ? "No Fix" : (tempValue == 1 ? "2DFix" : (tempValue == 3 ? "3D Fix" : ""));
            DHManSu_GPSDingWeiZhuangTai_Fix.Text = tempValueSTR;
            // bit6 0:GNSS修正无效 1：GNSS修正有效
            tempValueSTR = (GPSDingWeiMoShi >> 6 & 0x01) == 1 ? "GNSS修正有效" : "GNSS修正无效";
            DHManSu_GPSDingWeiZhuangTai_GNSSXiuZheng.Text = tempValueSTR;
            // bit7 0:BD2修正无效 1：BD2修正有效
            tempValueSTR = (GPSDingWeiMoShi >> 7 & 0x01) == 1 ? "BD2修正有效" : "BD2修正无效";
            DHManSu_GPSDingWeiZhuangTai_BD2XiuZheng.Text = tempValueSTR;
            // DHManSu_GPSDingWeiZhuangTai.Text = tempValueSTR;


            // GPS SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
            tempValue = sObject.GPS_SV;
            DHManSu_GPSSVKeYong.Text = ((byte)(tempValue & 0xF)).ToString();
            DHManSu_GPSCanYuDingWei.Text = ((byte)(tempValue >> 4 & 0xF)).ToString();
            // BD2 SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
            tempValue = sObject.BD2_SV;
            DHManSu_BD2KeYong.Text = ((byte)(tempValue & 0xF)).ToString();
            DHManSu_BD2CanYuDingWei.Text = ((byte)(tempValue >> 4 & 0xF)).ToString();

            // sObject.jingDu;              // 经度（组合结果）当量：1e-7
            DHManSu_JingDu.Text = ((double)(sObject.jingDu * Math.Pow(10, -7))).ToString();
            // sObject.weiDu;               // 纬度（组合结果）当量：1e-7
            DHManSu_WeiDu.Text = ((double)(sObject.weiDu * Math.Pow(10, -7))).ToString();
            // sObject.haiBaGaoDu;          // 海拔高度（组合结果）当量：1e-2
            DHManSu_GaoDu.Text = ((double)(sObject.haiBaGaoDu * Math.Pow(10, -2))).ToString();

            //sObject.dongXiangSuDu;        // 东向速度（组合结果）当量：1e-2
            DHManSu_DongXiangSuDu.Text = ((double)(sObject.dongXiangSuDu * Math.Pow(10, -2))).ToString();
            //sObject.beiXiangSuDu;         // 北向速度（组合结果）当量：1e-2
            DHManSu_BeiXiangSuDu.Text = ((double)(sObject.beiXiangSuDu * Math.Pow(10, -2))).ToString();
            //sObject.tianXiangSuDu;        // 天向速度（组合结果）当量：1e-2
            DHManSu_TianXiangSuDu.Text = ((double)(sObject.tianXiangSuDu * Math.Pow(10, -2))).ToString();

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
        public void AddDHManSuZuoBiao(Int32 jingDu, Int32 weiDu, Int32 gaoDu)
        {
            DHManSu_ZuoBiao_JingDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, jingDu * Math.Pow(10, -7)));
            DHManSu_ZuoBiao_WeiDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, weiDu * Math.Pow(10, -7)));
            DHManSu_ZuoBiao_GaoDu_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, gaoDu * Math.Pow(10, -2)));
        }

        // 添加速度点集
        public void AddDHManSuSuDu(Int32 dongXiangSuDu, Int32 beiXiangSuDu, Int32 tianXiangSuDu)
        {
            DHManSu_SuDu_DongXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, dongXiangSuDu * Math.Pow(10, -2)));
            DHManSu_SuDu_BeiXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, beiXiangSuDu * Math.Pow(10, -2)));
            DHManSu_SuDu_TianXiang_Buffer.Add(new SeriesPoint(DHManSu_CHART_ITEM_INDEX, tianXiangSuDu * Math.Pow(10, -2)));
        }


        // 绘图定时器，每隔100ms刷新绘图区域
        private void timerUpdateChart_Tick(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------------//
            // 导航数据（慢速）
            //---------------------------------------//
            // 坐标
            chart_DHManSu_ZuoBiao_JingDu.Series["经度"].Points.AddRange(DHManSu_ZuoBiao_JingDu_Buffer.ToArray());
            chart_DHManSu_ZuoBiao_WeiDu.Series["纬度"].Points.AddRange(DHManSu_ZuoBiao_WeiDu_Buffer.ToArray());
            chart_DHManSu_ZuoBiao_GaoDu.Series["海拔高度"].Points.AddRange(DHManSu_ZuoBiao_GaoDu_Buffer.ToArray());
            DHManSu_ZuoBiao_JingDu_Buffer.Clear();
            DHManSu_ZuoBiao_WeiDu_Buffer.Clear();
            DHManSu_ZuoBiao_GaoDu_Buffer.Clear();

            //---------------------------------------//
            // 速度
            chart_DHManSu_SuDu_Dong.Series["东向速度"].Points.AddRange(DHManSu_SuDu_DongXiang_Buffer.ToArray());
            chart_DHManSu_SuDu_Bei.Series["北向速度"].Points.AddRange(DHManSu_SuDu_BeiXiang_Buffer.ToArray());
            chart_DHManSu_SuDu_Tian.Series["天向速度"].Points.AddRange(DHManSu_SuDu_TianXiang_Buffer.ToArray());
            DHManSu_SuDu_DongXiang_Buffer.Clear();
            DHManSu_SuDu_BeiXiang_Buffer.Clear();
            DHManSu_SuDu_TianXiang_Buffer.Clear();
        }

        public void clearAllChart()
        {
            DHManSu_CHART_ITEM_INDEX = 0;
            foreach (Series series in chart_DHManSu_ZuoBiao_JingDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_ZuoBiao_JingDu.Update();
            foreach (Series series in chart_DHManSu_ZuoBiao_WeiDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_ZuoBiao_WeiDu.Update();
            foreach (Series series in chart_DHManSu_ZuoBiao_GaoDu.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_ZuoBiao_GaoDu.Update();
            foreach (Series series in chart_DHManSu_SuDu_Dong.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_SuDu_Dong.Update();
            foreach (Series series in chart_DHManSu_SuDu_Bei.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_SuDu_Bei.Update();
            foreach (Series series in chart_DHManSu_SuDu_Tian.Series)
            {
                series.Points.Clear();
            }
            chart_DHManSu_SuDu_Tian.Update();
        }

        private void timerUpdateDHMStatus_Tick(object sender, EventArgs e)
        {
            // 是否收到数据
            if (bRecvStatusData)
            {
                // 填充实时数据
                showDHManSuTimeStatus(ref sObject_DHM);
                testFunDelegate(statusType, true);
            }
        }

        public void setUpdateTimerStatus(bool bOpen)
        {
            if (bOpen)
            {
                timerUpdateDHMStatus.Start();
            }
            else
            {
                timerUpdateDHMStatus.Stop();
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

        public void SetDHMStatus(ref DAOHANGSHUJU_ManSu sObject)
        {
            sObject_DHM = sObject;

            // 重新启动离线定时器
            timerOfflineDHMStatus.Stop();
            timerOfflineDHMStatus.Start();

            // 是否收到数据
            bRecvStatusData = true;
        }

        public void setCHARTITEMINDEXAdd()
        {
            DHManSu_CHART_ITEM_INDEX++;
        }

        private void timerOfflineDHMStatus_Tick(object sender, EventArgs e)
        {
            // 清空数据
            // TODO 这里不需要清空最后一帧数据显示
            // GenericFunction.reSetAllTextEdit(this);

            // 是否收到数据
            bRecvStatusData = false;
            testFunDelegate(statusType, false);
        }
    }
}
