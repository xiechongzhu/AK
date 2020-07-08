/******************************************************************* 
* @brief : 历史事件查看窗口 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

/// <summary>
/// namespace RadarProcess
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// class TestReviewForm
    /// </summary>
    public partial class TestReviewForm : Form
    {
        /// <summary>
        /// recordId
        /// </summary>
        private long recordId;
        /// <summary>
        /// historyData
        /// </summary>
        private HistoryData historyData = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public TestReviewForm(long id)
        {
            recordId = id;
            InitializeComponent();
            LoadTestInfo();
            String errMsg;
            //加载配置文件
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                XtraMessageBox.Show("加载配置文件失败:" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (historyData != null)
            {
                //绘制曲线
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(- historyData.SideLine,
                    historyData.ForwardLine, historyData.BackwardLine));
                chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(historyData.SideLine,
                    historyData.ForwardLine, historyData.BackwardLine));
                chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(historyData.IdeaFallPoint.x, historyData.IdeaFallPoint.y));
            }
        }

        /// <summary>
        /// LoadTestInfo
        /// </summary>
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

                List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();     //位置X
                List<SeriesPoint> positionMinXBuffer = new List<SeriesPoint>();  //位置X最小值
                List<SeriesPoint> positionMaxXBuffer = new List<SeriesPoint>();  //位置X最大值
                List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();     //位置Y
                List<SeriesPoint> positionMinYBuffer = new List<SeriesPoint>();  //位置Y最小值
                List<SeriesPoint> positionMaxYBuffer = new List<SeriesPoint>();  //位置Y最大值
                List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();     //位置Z
                List<SeriesPoint> positionMinZBuffer = new List<SeriesPoint>();  //位置Z最小值
                List<SeriesPoint> positionMaxZBuffer = new List<SeriesPoint>();  //位置Z最大值
                List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();       //速度Vx
                List<SeriesPoint> speedMinVxBuffer = new List<SeriesPoint>();    //速度Vx最小值
                List<SeriesPoint> speedMaxVxBuffer = new List<SeriesPoint>();    //速度Vx最大值
                List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();       //速度Vy
                List<SeriesPoint> speedMinVyBuffer = new List<SeriesPoint>();    //速度Vy最小值
                List<SeriesPoint> speedMaxVyBuffer = new List<SeriesPoint>();    //速度Vy最大值
                List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();       //速度Vz
                List<SeriesPoint> speedMinVzBuffer = new List<SeriesPoint>();    //速度Vz最小值
                List<SeriesPoint> speedMaxVzBuffer = new List<SeriesPoint>();    //速度Vz最大值
                foreach (S_OBJECT obj in historyData.Objects)
                {
                    positionXBuffer.Add(new SeriesPoint(obj.time, obj.X));            // 位置X
                    positionMinXBuffer.Add(new SeriesPoint(obj.time, obj.MinX));      // 位置X最小值
                    positionMaxXBuffer.Add(new SeriesPoint(obj.time, obj.MaxX));      // 位置X最大值
                    positionYBuffer.Add(new SeriesPoint(obj.time, obj.Y));            // 位置Y
                    positionMinYBuffer.Add(new SeriesPoint(obj.time, obj.MinY));      // 位置Y最小值
                    positionMaxYBuffer.Add(new SeriesPoint(obj.time, obj.MaxY));      // 位置Y最大值
                    positionZBuffer.Add(new SeriesPoint(obj.time, obj.Z));            // 位置Z
                    positionMinZBuffer.Add(new SeriesPoint(obj.time, obj.MinZ));      // 位置Z最小值
                    positionMaxZBuffer.Add(new SeriesPoint(obj.time, obj.MaxZ));      // 位置Z最大值
                    speedVxBuffer.Add(new SeriesPoint(obj.time, obj.VX));             // 速度Vx
                    speedMinVxBuffer.Add(new SeriesPoint(obj.time, obj.MinVx));       // 速度Vx最小值
                    speedMaxVxBuffer.Add(new SeriesPoint(obj.time, obj.MaxVx));       // 速度Vx最大值
                    speedVyBuffer.Add(new SeriesPoint(obj.time, obj.VY));             // 速度Vy
                    speedMinVyBuffer.Add(new SeriesPoint(obj.time, obj.MinVy));       // 速度Vy最小值
                    speedMaxVyBuffer.Add(new SeriesPoint(obj.time, obj.MaxVy));       // 速度Vy最大值
                    speedVzBuffer.Add(new SeriesPoint(obj.time, obj.VZ));             // 速度Vz
                    speedMinVzBuffer.Add(new SeriesPoint(obj.time, obj.MinVz));       // 速度Vz最小值
                    speedMaxVzBuffer.Add(new SeriesPoint(obj.time, obj.MaxVz));       // 速度Vz最大值
                }

                chartX.BeginInit();
                chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());         //位置X
                chartX.Series["位置X上限"].Points.AddRange(positionMaxXBuffer.ToArray());  //位置X上限
                chartX.Series["位置X下限"].Points.AddRange(positionMinXBuffer.ToArray());  //位置X下限
                chartX.EndInit();                                                          //
                                                                                           //
                chartY.BeginInit();                                                        //
                chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());         //位置Y
                chartY.Series["位置Y上限"].Points.AddRange(positionMaxYBuffer.ToArray());  //位置Y上限
                chartY.Series["位置Y下限"].Points.AddRange(positionMinYBuffer.ToArray());  //位置Y下限
                chartY.EndInit();                                                          //
                                                                                           //
                chartZ.BeginInit();                                                        //
                chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());         //位置Z
                chartZ.Series["位置Z上限"].Points.AddRange(positionMaxZBuffer.ToArray());  //位置Z上限
                chartZ.Series["位置Z下限"].Points.AddRange(positionMinZBuffer.ToArray());  //位置Z下限
                chartZ.EndInit();                                                          //
                                                                                           //
                chartVx.BeginInit();                                                       //
                chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());         //速度VX
                chartVx.Series["速度VX上限"].Points.AddRange(speedMaxVxBuffer.ToArray());  //速度VX上限
                chartVx.Series["速度VX上限"].Points.AddRange(speedMinVxBuffer.ToArray());  //速度VX上限
                chartVx.EndInit();                                                         //
                                                                                           //
                chartVy.BeginInit();                                                       //
                chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());         //速度VY
                chartVy.Series["速度VY上限"].Points.AddRange(speedMaxVyBuffer.ToArray());  //速度VY上限
                chartVy.Series["速度VY下限"].Points.AddRange(speedMinVyBuffer.ToArray());  //速度VY下限
                chartVy.EndInit();                                                         //
                                                                                           //
                chartVz.BeginInit();                                                       //
                chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());         //速度VZ
                chartVz.Series["速度VZ上限"].Points.AddRange(speedMaxVzBuffer.ToArray());  //速度VZ上限
                chartVz.Series["速度VZ下限"].Points.AddRange(speedMinVzBuffer.ToArray());  //速度VZ下限
                chartVz.EndInit();

                chartPoints.BeginInit();
                FallPoint point = historyData.FallPoint;
                chartPoints.Series["预示落点"].Points.Clear();                                                 //预示落点
                chartPoints.Series["预示落点"].Points.Add(new SeriesPoint(point.x, point.y));                  //预示落点
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(point.x - historyData.PointError,    //落点误差
                    point.y + historyData.PointError, point.y - historyData.PointError));                      //
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(point.x + historyData.PointError,    //落点误差
                    point.y + historyData.PointError, point.y - historyData.PointError));                      //
                chartPoints.EndInit();
            }
        }

        /// <summary>
        /// btnViewConfig_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewConfig_Click(object sender, EventArgs e)
        {
            if(null == historyData)
            {
                XtraMessageBox.Show("读取配置失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //创建窗口
            SettingForm settingForm = new SettingForm();
            //设置模式
            settingForm.SetViwMode();
            //设置参数
            settingForm.SetParams(historyData.LongitudeInit, historyData.LatitudeInit, historyData.HeightInit,
                historyData.AzimuthInit, historyData.PlacementHeight, historyData.Flightshot, historyData.ForwardLine,
                historyData.BackwardLine, historyData.SideLine, historyData.StrMultiCastIpAddr, historyData.Port, historyData.StationId,
                historyData.SpeedError, historyData.PointError, historyData.MaxPointCount);
            settingForm.ShowDialog();
        }
    }
}
