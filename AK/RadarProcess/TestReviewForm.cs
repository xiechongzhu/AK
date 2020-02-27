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
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x - historyData.SideLine,
                    historyData.IdeaFallPoint.y + historyData.ForwardLine, historyData.IdeaFallPoint.y - historyData.BackwardLine));
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x + historyData.SideLine,
                    historyData.IdeaFallPoint.y + historyData.ForwardLine, historyData.IdeaFallPoint.y - historyData.BackwardLine));
                chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x, historyData.IdeaFallPoint.y));
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
                    if (positionXBuffer.Count > 0)
                    {
                        chartX.BeginInit();
                        chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
                        chartX.Series["位置X上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(positionXBuffer[0].Argument, historyData.LocMaxX),
                            new SeriesPoint(positionXBuffer[positionXBuffer.Count-1].Argument, historyData.LocMaxX) });
                        chartX.Series["位置X下限"].Points.AddRange(new SeriesPoint[] {
                         new SeriesPoint(positionXBuffer[0].Argument, historyData.LocMinX),
                            new SeriesPoint(positionXBuffer[positionXBuffer.Count-1].Argument, historyData.LocMinX) });
                        chartX.EndInit();
                    }
                    if (positionYBuffer.Count > 0)
                    {
                        chartY.BeginInit();
                        chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
                        chartY.Series["位置Y上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(positionYBuffer[0].Argument, historyData.LocMaxY),
                            new SeriesPoint(positionYBuffer[positionYBuffer.Count-1].Argument, historyData.LocMaxY) });
                        chartY.Series["位置Y下限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(positionYBuffer[0].Argument, historyData.LocMinY),
                            new SeriesPoint(positionYBuffer[positionYBuffer.Count-1].Argument, historyData.LocMinY) });
                        chartY.EndInit();
                    }
                    if (positionZBuffer.Count > 0)
                    {
                        chartZ.BeginInit();
                        chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
                        chartZ.Series["位置Z上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(positionZBuffer[0].Argument, historyData.LocMaxZ),
                            new SeriesPoint(positionZBuffer[positionZBuffer.Count-1].Argument, historyData.LocMaxZ) });
                        chartZ.Series["位置Z下限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(positionZBuffer[0].Argument, historyData.LocMinZ),
                            new SeriesPoint(positionZBuffer[positionZBuffer.Count-1].Argument, historyData.LocMinZ) });
                        chartZ.EndInit();
                    }
                    if (speedVxBuffer.Count > 0)
                    {
                        chartVx.BeginInit();
                        chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());
                        chartVx.Series["速度VX上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVxBuffer[0].Argument, historyData.SpeedMaxX),
                            new SeriesPoint(speedVxBuffer[speedVxBuffer.Count-1].Argument, historyData.SpeedMaxX) });
                        chartVx.Series["速度VX下限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVxBuffer[0].Argument, historyData.SpeedMinX),
                            new SeriesPoint(speedVxBuffer[speedVxBuffer.Count-1].Argument, historyData.SpeedMinX) });
                        chartVx.EndInit();
                    }
                    if (speedVyBuffer.Count > 0)
                    {
                        chartVy.BeginInit();
                        chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());
                        chartVy.Series["速度VY上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVyBuffer[0].Argument, historyData.SpeedMaxY),
                            new SeriesPoint(speedVyBuffer[speedVyBuffer.Count-1].Argument, historyData.SpeedMaxY) });
                        chartVy.Series["速度VY下限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVyBuffer[0].Argument, historyData.SpeedMinY),
                            new SeriesPoint(speedVyBuffer[speedVyBuffer.Count-1].Argument, historyData.SpeedMinY) });
                        chartVy.EndInit();
                    }
                    if (speedVzBuffer.Count > 0)
                    {
                        chartVz.BeginInit();
                        chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());
                        chartVz.Series["速度VZ上限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVzBuffer[0].Argument, historyData.SpeedMaxZ),
                            new SeriesPoint(speedVzBuffer[speedVzBuffer.Count-1].Argument, historyData.SpeedMaxZ) });
                        chartVz.Series["速度VZ下限"].Points.AddRange(new SeriesPoint[] {
                            new SeriesPoint(speedVzBuffer[0].Argument, historyData.SpeedMinZ),
                            new SeriesPoint(speedVzBuffer[speedVzBuffer.Count-1].Argument, historyData.SpeedMinZ) });
                        chartVz.EndInit();
                    }
                    int i = 0;
                    chartPoints.BeginInit();
                    while (historyData.FallPoints.Count > 0)
                    {
                        FallPoint point = historyData.FallPoints[0];
                        chartPoints.Series[String.Format("落点{0}", i + 1)].Points.Clear();
                        chartPoints.Series[String.Format("落点{0}", i + 1)].Points.Add(new SeriesPoint(point.x, point.y));
                        i++;
                        historyData.FallPoints.RemoveAt(0);
                    }
                    chartPoints.EndInit();
                }
            }
        }

        private void btnViewConfig_Click(object sender, EventArgs e)
        {
            if(null == historyData)
            {
                XtraMessageBox.Show("读取配置失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SettingForm settingForm = new SettingForm();
            settingForm.SetViwMode();
            settingForm.SetParams(historyData.LongitudeInit, historyData.LatitudeInit, historyData.HeightInit,
                historyData.AzimuthInit, historyData.PlacementHeight, historyData.Flightshot, historyData.ForwardLine,
                historyData.BackwardLine, historyData.SideLine, historyData.LocMaxX, historyData.LocMaxY, historyData.LocMaxZ,
                historyData.LocMinX, historyData.LocMinY, historyData.LocMinZ, historyData.SpeedMaxX, historyData.SpeedMaxY, historyData.SpeedMaxZ,
                historyData.SpeedMinX, historyData.SpeedMinY, historyData.SpeedMinZ, historyData.StrMultiCastIpAddr, historyData.Port);
            settingForm.ShowDialog();
        }
    }
}
