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
    public partial class FrameInfoSubForm : Form
    {
        //-----------------------------------------------------//
        // 成员变量
        //-----------------------------------------------------//

        // 常量数据
        const byte frameType_systemStatus_1 = 0x15;       // 系统判据状态
        const byte frameType_systemStatus_2 = 0x16;       // 系统判据状态
        const byte frameType_HuiLuJianCe = 0x16;          // 回路检测反馈状态
        const byte frameType_daoHangKuaiSu_Ti = 0x21;     // 导航快速（弹体）
        const byte frameType_daoHangKuaiSu_Tou = 0x31;    // 导航快速（弹头）
        const byte frameType_daoHangManSu_Ti = 0x25;      // 导航慢速（弹体）
        const byte frameType_daoHangManSu_Tou = 0x35;     // 导航慢速（弹头）
        const byte frameType_XiTongJiShi_Ti = 0x26;       // 系统状态即时反馈（弹体）   帧总长11  数据段总长度64 帧类型0x0B
        const byte frameType_XiTongJiShi_Tou = 0x36;      // 系统状态即时反馈（弹头）

        const byte frameType_XTPJZT = 0x01;
        const byte frameType_XTPJFK = 0x05;
        const byte frameType_HLJCFK = 0x06;

        //-----------------------------------------------------//

        // 绘图数据缓存——系统判据0x15
        private List<SeriesPoint> XiTongPanJu15_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——系统判据0x16
        private List<SeriesPoint> XiTongPanJu16_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——回路检测
        private List<SeriesPoint> HuiLuJianCe_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——导航快速（弹体）
        private List<SeriesPoint> DHK_Ti_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——导航快速（弹头）
        private List<SeriesPoint> DHK_Tou_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——导航慢速（弹体）
        private List<SeriesPoint> DHM_Ti_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——导航慢速（弹头）
        private List<SeriesPoint> DHM_Tou_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——系统即时（弹体）
        private List<SeriesPoint> XTJS_Ti_Buffer = new List<SeriesPoint>();
        // 绘图数据缓存——系统即时（弹体）
        private List<SeriesPoint> XTJS_Tou_Buffer = new List<SeriesPoint>();

        private int XiTongPanJu15_CHART_ITEM_INDEX = 0;
        private int XiTongPanJu16_CHART_ITEM_INDEX = 0;
        private int HuiLuJianCe_CHART_ITEM_INDEX = 0;
        private int DHK_Ti_CHART_ITEM_INDEX = 0;
        private int DHK_Tou_CHART_ITEM_INDEX = 0;
        private int DHM_Ti_CHART_ITEM_INDEX = 0;
        private int DHM_Tou_CHART_ITEM_INDEX = 0;
        private int XTJS_Ti_CHART_ITEM_INDEX = 0;
        private int XTJS_Tou_CHART_ITEM_INDEX = 0;
        //-----------------------------------------------------//

        public FrameInfoSubForm()
        {
            InitializeComponent();
        }

        public void addFrameInfo(ref FRAME_PROPERTY sObject)
        {
            switch (sObject.CanId)
            {
                // 系统判据状态
                case frameType_systemStatus_1:
                    {
                        XiTongPanJu15_Buffer.Add(new SeriesPoint(XiTongPanJu15_CHART_ITEM_INDEX++, sObject.frameNo));
                    }
                    break;
                // 系统判据状态 0x16(中间存在两种情况，需要通过帧类型来做进一步的区分)
                case frameType_systemStatus_2:
                    {
                        if (sObject.frameType == frameType_XTPJFK)
                        {
                            XiTongPanJu16_Buffer.Add(new SeriesPoint(XiTongPanJu16_CHART_ITEM_INDEX++, sObject.frameNo));
                        }
                        else if (sObject.frameType == frameType_HLJCFK)
                        {
                            HuiLuJianCe_Buffer.Add(new SeriesPoint(HuiLuJianCe_CHART_ITEM_INDEX++, sObject.frameNo));
                        }
                    }
                    break;
                case frameType_daoHangKuaiSu_Ti:
                    DHK_Ti_Buffer.Add(new SeriesPoint(DHK_Ti_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                case frameType_daoHangKuaiSu_Tou:
                    DHK_Tou_Buffer.Add(new SeriesPoint(DHK_Tou_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                case frameType_daoHangManSu_Ti:
                    DHM_Ti_Buffer.Add(new SeriesPoint(DHM_Ti_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                case frameType_daoHangManSu_Tou:
                    DHM_Tou_Buffer.Add(new SeriesPoint(DHM_Tou_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                case frameType_XiTongJiShi_Ti:
                    XTJS_Ti_Buffer.Add(new SeriesPoint(XTJS_Ti_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                case frameType_XiTongJiShi_Tou:
                    XTJS_Tou_Buffer.Add(new SeriesPoint(XTJS_Tou_CHART_ITEM_INDEX++, sObject.frameNo));
                    break;
                default:
                    break;
            }
        }

        private void timerChartUpdate_Tick(object sender, EventArgs e)
        {
            chart_XiTongPanJu15.Series["系统判据状态"].Points.AddRange(XiTongPanJu15_Buffer.ToArray());
            XiTongPanJu15_Buffer.Clear();

            chart_XiTongPanJu16.Series["系统判据状态查询"].Points.AddRange(XiTongPanJu16_Buffer.ToArray());
            XiTongPanJu16_Buffer.Clear();

            chart_HuiLuJianCe.Series["回路检测数据"].Points.AddRange(HuiLuJianCe_Buffer.ToArray());
            HuiLuJianCe_Buffer.Clear();

            chart_DHK_Ti.Series["导航快速(弹体)"].Points.AddRange(DHK_Ti_Buffer.ToArray());
            DHK_Ti_Buffer.Clear();

            chart_DHK_Tou.Series["导航快速(弹头)"].Points.AddRange(DHK_Tou_Buffer.ToArray());
            DHK_Tou_Buffer.Clear();

            chart_DHM_Ti.Series["导航慢速(弹体)"].Points.AddRange(DHM_Ti_Buffer.ToArray());
            DHM_Ti_Buffer.Clear();

            chart_DHM_Tou.Series["导航慢速(弹头)"].Points.AddRange(DHM_Tou_Buffer.ToArray());
            DHM_Tou_Buffer.Clear();

            chart_XTJS_Ti.Series["系统即时(弹体)"].Points.AddRange(XTJS_Ti_Buffer.ToArray());
            XTJS_Ti_Buffer.Clear();

            chart_XTJS_Tou.Series["系统即时(弹头)"].Points.AddRange(XTJS_Tou_Buffer.ToArray());
            XTJS_Tou_Buffer.Clear();
        }

        public void clearAllChart()
        {
            foreach (Series series in chart_XiTongPanJu15.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_XiTongPanJu16.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_HuiLuJianCe.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_DHK_Ti.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_DHK_Tou.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_DHM_Ti.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_DHM_Tou.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_XTJS_Ti.Series)
            {
                series.Points.Clear();
            }

            foreach (Series series in chart_XTJS_Tou.Series)
            {
                series.Points.Clear();
            }

            XiTongPanJu15_CHART_ITEM_INDEX = 0;
            XiTongPanJu16_CHART_ITEM_INDEX = 0;
            HuiLuJianCe_CHART_ITEM_INDEX = 0;
            DHK_Ti_CHART_ITEM_INDEX = 0;
            DHK_Tou_CHART_ITEM_INDEX = 0;
            DHM_Ti_CHART_ITEM_INDEX = 0;
            DHM_Tou_CHART_ITEM_INDEX = 0;
            XTJS_Ti_CHART_ITEM_INDEX = 0;
            XTJS_Tou_CHART_ITEM_INDEX = 0;
        }

        public void startTimerUpdateChart(bool start)
        {
            if (!start)
            {
                timerChartUpdate.Stop();
            }
            else
            {
                timerChartUpdate.Start();
            }
        }
    }
}
