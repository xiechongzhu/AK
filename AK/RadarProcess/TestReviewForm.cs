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
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(- historyData.SideLine,
                    historyData.ForwardLine, historyData.BackwardLine));
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(historyData.SideLine,
                    historyData.ForwardLine, historyData.BackwardLine));
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

                if (historyData == null || historyData.Objects == null || historyData.Objects.Count == 0)
                {
                    return;
                }


                List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMinXBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMaxXBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMinYBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMaxYBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMinZBuffer = new List<SeriesPoint>();
                List<SeriesPoint> positionMaxZBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMinVxBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMaxVxBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMinVyBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMaxVyBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMinVzBuffer = new List<SeriesPoint>();
                List<SeriesPoint> speedMaxVzBuffer = new List<SeriesPoint>();
                foreach (S_OBJECT obj in historyData.Objects)
                {
                    positionXBuffer.Add(new SeriesPoint(obj.time, obj.X));
                    positionMinXBuffer.Add(new SeriesPoint(obj.time, obj.MinX));
                    positionMaxXBuffer.Add(new SeriesPoint(obj.time, obj.MaxX));
                    positionYBuffer.Add(new SeriesPoint(obj.time, obj.Y));
                    positionMinYBuffer.Add(new SeriesPoint(obj.time, obj.MinY));
                    positionMaxYBuffer.Add(new SeriesPoint(obj.time, obj.MaxY));
                    positionZBuffer.Add(new SeriesPoint(obj.time, obj.Z));
                    positionMinZBuffer.Add(new SeriesPoint(obj.time, obj.MinZ));
                    positionMaxZBuffer.Add(new SeriesPoint(obj.time, obj.MaxZ));
                    speedVxBuffer.Add(new SeriesPoint(obj.time, obj.VX));
                    speedMinVxBuffer.Add(new SeriesPoint(obj.time, obj.MinVx));
                    speedMaxVxBuffer.Add(new SeriesPoint(obj.time, obj.MaxVx));
                    speedVyBuffer.Add(new SeriesPoint(obj.time, obj.VY));
                    speedMinVyBuffer.Add(new SeriesPoint(obj.time, obj.MinVy));
                    speedMaxVyBuffer.Add(new SeriesPoint(obj.time, obj.MaxVy));
                    speedVzBuffer.Add(new SeriesPoint(obj.time, obj.VZ));
                    speedMinVzBuffer.Add(new SeriesPoint(obj.time, obj.MinVz));
                    speedMaxVzBuffer.Add(new SeriesPoint(obj.time, obj.MaxVz));
                }

                chartX.BeginInit();
                chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
                chartX.Series["位置X上限"].Points.AddRange(positionMaxXBuffer.ToArray());
                chartX.Series["位置X下限"].Points.AddRange(positionMinXBuffer.ToArray());
                chartX.EndInit();

                chartY.BeginInit();
                chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
                chartY.Series["位置Y上限"].Points.AddRange(positionMaxYBuffer.ToArray());
                chartY.Series["位置Y下限"].Points.AddRange(positionMinYBuffer.ToArray());
                chartY.EndInit();

                chartZ.BeginInit();
                chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
                chartZ.Series["位置Z上限"].Points.AddRange(positionMaxZBuffer.ToArray());
                chartZ.Series["位置Z下限"].Points.AddRange(positionMinZBuffer.ToArray());
                chartZ.EndInit();

                chartVx.BeginInit();
                chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());
                chartVx.Series["速度VX上限"].Points.AddRange(speedMaxVxBuffer.ToArray());
                chartVx.Series["速度VX下限"].Points.AddRange(speedMinVxBuffer.ToArray());
                chartVx.EndInit();

                chartVy.BeginInit();
                chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());
                chartVy.Series["速度VY上限"].Points.AddRange(speedMaxVyBuffer.ToArray());
                chartVy.Series["速度VY下限"].Points.AddRange(speedMinVyBuffer.ToArray());
                chartVy.EndInit();

                chartVz.BeginInit();
                chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());
                chartVz.Series["速度VZ上限"].Points.AddRange(speedMaxVzBuffer.ToArray());
                chartVz.Series["速度VZ下限"].Points.AddRange(speedMinVzBuffer.ToArray());
                chartVz.EndInit();

                chartPoints.BeginInit();
                FallPoint point = historyData.FallPoint;
                chartPoints.Series["预示落点"].Points.Clear();
                chartPoints.Series["预示落点"].Points.Add(new SeriesPoint(point.x, point.y));
                chartPoints.EndInit();
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
                historyData.BackwardLine, historyData.SideLine, historyData.StrMultiCastIpAddr, historyData.Port, historyData.StationId);
            settingForm.ShowDialog();
        }
    }
}
