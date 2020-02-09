using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace RadarProcess
{
    public partial class TestReviewForm : Form
    {
        private long recordId;
        private int CHART_ITEM_INDEX = 0;
        private HistoryData historyData = null;
        public TestReviewForm(long id)
        {
            recordId = id;
            InitializeComponent();
            LoadTestInfo();
            String errMsg;
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                XtraMessageBox.Show("加载配置文件失败:" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (historyData != null)
            {
                pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x - Config.GetInstance().sideLine,
                    historyData.IdeaFallPoint.y + Config.GetInstance().forwardLine, historyData.IdeaFallPoint.y - Config.GetInstance().backwardLine));
                pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x + Config.GetInstance().sideLine,
                    historyData.IdeaFallPoint.y + Config.GetInstance().forwardLine, historyData.IdeaFallPoint.y - Config.GetInstance().backwardLine));
                pointChartControl.Series["理想落点"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x, historyData.IdeaFallPoint.y));
            }
        }

        private void LoadTestInfo()
        {
            using (DataModels.DatabaseDB db = new DataModels.DatabaseDB())
            {
                var temp = from c in db.TestInfos where c.Id == recordId select c;
                DataModels.TestInfo testInfo = null;
                foreach (DataModels.TestInfo info in temp)
                {
                    testInfo = info;
                }
                editTestName.Text = testInfo?.TestName;
                editOperator.Text = testInfo?.Operator;
                editTestDate.Text = testInfo?.Time.ToString("yyyy-MM-dd HH:mm:ss");
                editComment.Text = testInfo?.Comment;
                String strDateFile = @".\Log\" + testInfo?.Time.ToString("yyyyMMddHHmmss") + @"\History.dat";
                
                try
                {
                    using (FileStream fs = new FileStream(strDateFile, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        historyData = (HistoryData)formatter.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("读取历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if(historyData != null)
                {
                    List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();
                    foreach(S_OBJECT obj in historyData.Objects)
                    {
                        positionXBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.X));
                        positionYBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.Y));
                        positionZBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.Z));
                        speedVxBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VX));
                        speedVyBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VY));
                        speedVzBuffer.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VZ));
                        CHART_ITEM_INDEX++;
                    }
                    positionChart.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
                    positionChart.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
                    positionChart.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
                    speedChart.Series["速度X"].Points.AddRange(speedVxBuffer.ToArray());
                    speedChart.Series["速度Y"].Points.AddRange(speedVyBuffer.ToArray());
                    speedChart.Series["速度Z"].Points.AddRange(speedVzBuffer.ToArray());
                    int i = 0;
                    while (historyData.FallPoints.Count > 0)
                    {
                        FallPoint point = historyData.FallPoints[0];
                        pointChartControl.Series[String.Format("落点{0}", i + 1)].Points.Clear();
                        pointChartControl.Series[String.Format("落点{0}", i + 1)].Points.Add(new SeriesPoint(point.x, point.y));
                        i++;
                        historyData.FallPoints.RemoveAt(0);
                    }
                }
            }
        }
    }
}
