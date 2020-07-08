/******************************************************************* 
* @brief : 曲线图代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraCharts;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// DisplayData
    /// </summary>
    public struct DisplayData
    {
        public int time;               //事件，毫秒
        public double x;               //位置X
        public double y;               //位置Y
        public double z;               //位置Z
        public double vx;              //速度Vx
        public double vy;              //速度Vy
        public double vz;              //速度Vz
        public double minX;            //X最小值
        public double maxX;            //X最大值
        public double minY;            //Y最小值
        public double maxY;            //Y最大值
        public double minZ;            //Z最小值
        public double maxZ;            //Z最大值
        public double minVx;           //Vx最小值
        public double maxVx;           //Vx最大值
        public double minVy;           //Vy最小值
        public double maxVy;           //Vy最大值
        public double minVz;           //Vz最小值
        public double maxVz;           //Vz最大值
        public FallPoint fallPoint;    //落点
        public double fallTime;        //剩余飞行时间
        public double distance;        //射程
    }

    /// <summary>
    /// MyChartControl
    /// </summary>
    public partial class MyChartControl : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// displayDataList
        /// </summary>
        private List<DisplayData> displayDataList = new List<DisplayData>();
        /// <summary>
        /// MAX_CHART_POINTS
        /// </summary>
        private int MAX_CHART_POINTS;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyChartControl()
        {
            InitializeComponent();
            chartUpateTimer.Tick += chartUpateTimer_Tick;
        }

        /// <summary>
        /// 设置落点
        /// </summary>
        /// <param name="fallPoint"></param>
        public void SetFallPoint(FallPoint fallPoint)
        {
            if (fallPoint != null)
            {
                chartPoints.Series["预测落点"].Points.Clear();                                                                //预测落点
                chartPoints.Series["预测落点"].Points.Add(new SeriesPoint(fallPoint.x, fallPoint.y));                         //预测落点
                chartPoints.Series["落点误差"].Points.Clear();                                                                //落点误差
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x - Config.GetInstance().pointError,      //落点误差
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));           //
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x + Config.GetInstance().pointError,      //落点误差
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));           //
            }
        }

        /// <summary>
        /// 检测位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        public void CheckPosition(double x, double y, double z, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            //设置背景色
            if (x > maxX || x < minX)
            {
                chartX.BackColor = Color.Red;
            }
            else
            {
                chartX.BackColor = Color.White;
            }
            if (y > maxY || y < minY)
            {
                chartY.BackColor = Color.Red;
            }
            else
            {
                chartY.BackColor = Color.White;
            }
            if (z > maxZ || z < minZ)
            {
                chartZ.BackColor = Color.Red;
            }
            else
            {
                chartZ.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 曲线图更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chartUpateTimer_Tick(object sender, EventArgs e)
        {
            if (displayDataList.Count == 0)
            {
                return;
            }

            List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();        //positionXBuffer
            List<SeriesPoint> positionMinXBuffer = new List<SeriesPoint>();     //positionMinXBuffer
            List<SeriesPoint> positionMaxXBuffer = new List<SeriesPoint>();     //positionMaxXBuffer
            List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();        //positionYBuffer
            List<SeriesPoint> positionMinYBuffer = new List<SeriesPoint>();     //positionMinYBuffer
            List<SeriesPoint> positionMaxYBuffer = new List<SeriesPoint>();     //positionMaxYBuffer
            List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();        //positionZBuffer
            List<SeriesPoint> positionMinZBuffer = new List<SeriesPoint>();     //positionMinZBuffer
            List<SeriesPoint> positionMaxZBuffer = new List<SeriesPoint>();     //positionMaxZBuffer
            List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();          //speedVxBuffer
            List<SeriesPoint> speedMinVxBuffer = new List<SeriesPoint>();       //speedMinVxBuffer
            List<SeriesPoint> speedMaxVxBuffer = new List<SeriesPoint>();       //speedMaxVxBuffer
            List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();          //speedVyBuffer
            List<SeriesPoint> speedMinVyBuffer = new List<SeriesPoint>();       //speedMinVyBuffer
            List<SeriesPoint> speedMaxVyBuffer = new List<SeriesPoint>();       //speedMaxVyBuffer
            List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();          //speedVzBuffer
            List<SeriesPoint> speedMinVzBuffer = new List<SeriesPoint>();       //speedMinVzBuffer
            List<SeriesPoint> speedMaxVzBuffer = new List<SeriesPoint>();       //speedMaxVzBuffer

            foreach (DisplayData displayData in displayDataList)
            {
                positionXBuffer.Add(new SeriesPoint(displayData.time, displayData.x));         //添加X
                positionMinXBuffer.Add(new SeriesPoint(displayData.time, displayData.minX));   //添加MinX
                positionMaxXBuffer.Add(new SeriesPoint(displayData.time, displayData.maxX));   //添加MaxX
                positionYBuffer.Add(new SeriesPoint(displayData.time, displayData.y));         //添加Y
                positionMinYBuffer.Add(new SeriesPoint(displayData.time, displayData.minY));   //添加MinY
                positionMaxYBuffer.Add(new SeriesPoint(displayData.time, displayData.maxY));   //添加MaxY
                positionZBuffer.Add(new SeriesPoint(displayData.time, displayData.z));         //添加Z
                positionMinZBuffer.Add(new SeriesPoint(displayData.time, displayData.minZ));   //添加MinZ
                positionMaxZBuffer.Add(new SeriesPoint(displayData.time, displayData.maxZ));   //添加MaxZ
                speedVxBuffer.Add(new SeriesPoint(displayData.time, displayData.vx));          //添加Vx
                speedMinVxBuffer.Add(new SeriesPoint(displayData.time, displayData.minVx));    //添加MinVx
                speedMaxVxBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVx));    //添加MaxVx
                speedVyBuffer.Add(new SeriesPoint(displayData.time, displayData.vy));          //添加Vy
                speedMinVyBuffer.Add(new SeriesPoint(displayData.time, displayData.minVy));    //添加MinVy
                speedMaxVyBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVy));    //添加MaxVy
                speedVzBuffer.Add(new SeriesPoint(displayData.time, displayData.vz));          //添加Vz
                speedMinVzBuffer.Add(new SeriesPoint(displayData.time, displayData.minVz));    //添加MinVz
                speedMaxVzBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVz));    //添加MaxVz

                SetFallPoint(displayData.fallPoint);
            }

            DisplayFallTimeAndDistance(displayDataList[displayDataList.Count - 1].fallTime, displayDataList[displayDataList.Count - 1].distance);

            chartX.BeginInit();
            chartX.Series["位置X"].Points.AddRange(positionXBuffer.ToArray());
            if (chartX.Series["位置X"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X"].Points.RemoveRange(0, chartX.Series["位置X"].Points.Count - MAX_CHART_POINTS);
            }
            chartX.Series["位置X上限"].Points.AddRange(positionMaxXBuffer.ToArray());
            if (chartX.Series["位置X上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X上限"].Points.RemoveRange(0, chartX.Series["位置X上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartX.Series["位置X下限"].Points.AddRange(positionMinXBuffer.ToArray());
            if (chartX.Series["位置X下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartX.Series["位置X下限"].Points.RemoveRange(0, chartX.Series["位置X下限"].Points.Count - MAX_CHART_POINTS);
            }
            double distanceHigh = displayDataList[displayDataList.Count - 1].maxX - displayDataList[displayDataList.Count - 1].x;
            double distanceLow = displayDataList[displayDataList.Count - 1].x - displayDataList[displayDataList.Count - 1].minX;
            chartX.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartX.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].x);
            chartX.EndInit();

            chartY.BeginInit();
            chartY.Series["位置Y"].Points.AddRange(positionYBuffer.ToArray());
            if (chartY.Series["位置Y"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y"].Points.RemoveRange(0, chartY.Series["位置Y"].Points.Count - MAX_CHART_POINTS);
            }
            chartY.Series["位置Y上限"].Points.AddRange(positionMaxYBuffer.ToArray());
            if (chartY.Series["位置Y上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y上限"].Points.RemoveRange(0, chartY.Series["位置Y上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartY.Series["位置Y下限"].Points.AddRange(positionMinYBuffer.ToArray());
            if (chartY.Series["位置Y下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartY.Series["位置Y下限"].Points.RemoveRange(0, chartY.Series["位置Y下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxY - displayDataList[displayDataList.Count - 1].y;
            distanceLow = displayDataList[displayDataList.Count - 1].y - displayDataList[displayDataList.Count - 1].minY;
            chartY.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartY.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].y);
            chartY.EndInit();

            chartZ.BeginInit();
            chartZ.Series["位置Z"].Points.AddRange(positionZBuffer.ToArray());
            if (chartZ.Series["位置Z"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z"].Points.RemoveRange(0, chartZ.Series["位置Z"].Points.Count - MAX_CHART_POINTS);
            }
            chartZ.Series["位置Z上限"].Points.AddRange(positionMaxZBuffer.ToArray());
            if (chartZ.Series["位置Z上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z上限"].Points.RemoveRange(0, chartZ.Series["位置Z上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartZ.Series["位置Z下限"].Points.AddRange(positionMinZBuffer.ToArray());
            if (chartZ.Series["位置Z下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartZ.Series["位置Z下限"].Points.RemoveRange(0, chartZ.Series["位置Z下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxZ - displayDataList[displayDataList.Count - 1].z;
            distanceLow = displayDataList[displayDataList.Count - 1].z - displayDataList[displayDataList.Count - 1].minZ;
            chartZ.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartZ.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].z);
            chartZ.EndInit();

            chartVx.BeginInit();
            chartVx.Series["速度VX"].Points.AddRange(speedVxBuffer.ToArray());
            if (chartVx.Series["速度VX"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX"].Points.RemoveRange(0, chartVx.Series["速度VX"].Points.Count - MAX_CHART_POINTS);
            }
            chartVx.Series["速度VX上限"].Points.AddRange(speedMaxVxBuffer.ToArray());
            if (chartVx.Series["速度VX上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX上限"].Points.RemoveRange(0, chartVx.Series["速度VX上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVx.Series["速度VX下限"].Points.AddRange(speedMinVxBuffer.ToArray());
            if (chartVx.Series["速度VX下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVx.Series["速度VX下限"].Points.RemoveRange(0, chartVx.Series["速度VX下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVx - displayDataList[displayDataList.Count - 1].vx;
            distanceLow = displayDataList[displayDataList.Count - 1].vx - displayDataList[displayDataList.Count - 1].minVx;
            chartVx.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVx.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vx);
            chartVx.Series["速度VX误差"].Points.Clear();
            chartVx.Series["速度VX误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time,
                displayDataList[displayDataList.Count - 1].vx + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vx - Config.GetInstance().speedError));
            chartVx.EndInit();

            chartVy.BeginInit();
            chartVy.Series["速度VY"].Points.AddRange(speedVyBuffer.ToArray());
            if (chartVy.Series["速度VY"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY"].Points.RemoveRange(0, chartVy.Series["速度VY"].Points.Count - MAX_CHART_POINTS);
            }
            chartVy.Series["速度VY上限"].Points.AddRange(speedMaxVyBuffer.ToArray());
            if (chartVy.Series["速度VY上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY上限"].Points.RemoveRange(0, chartVy.Series["速度VY上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVy.Series["速度VY下限"].Points.AddRange(speedMinVyBuffer.ToArray());
            if (chartVy.Series["速度VY下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVy.Series["速度VY下限"].Points.RemoveRange(0, chartVy.Series["速度VY下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVy - displayDataList[displayDataList.Count - 1].vy;
            distanceLow = displayDataList[displayDataList.Count - 1].vy - displayDataList[displayDataList.Count - 1].minVy;
            chartVy.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVy.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vy);
            chartVy.Series["速度VY误差"].Points.Clear();
            chartVy.Series["速度VY误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time,
                displayDataList[displayDataList.Count - 1].vy + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vy - Config.GetInstance().speedError));
            chartVy.EndInit();

            chartVz.BeginInit();
            chartVz.Series["速度VZ"].Points.AddRange(speedVzBuffer.ToArray());
            if (chartVz.Series["速度VZ"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ"].Points.RemoveRange(0, chartVz.Series["速度VZ"].Points.Count - MAX_CHART_POINTS);
            }
            chartVz.Series["速度VZ上限"].Points.AddRange(speedMaxVzBuffer.ToArray());
            if (chartVz.Series["速度VZ上限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ上限"].Points.RemoveRange(0, chartVz.Series["速度VZ上限"].Points.Count - MAX_CHART_POINTS);
            }
            chartVz.Series["速度VZ下限"].Points.AddRange(speedMinVzBuffer.ToArray());
            if (chartVz.Series["速度VZ下限"].Points.Count > MAX_CHART_POINTS)
            {
                chartVz.Series["速度VZ下限"].Points.RemoveRange(0, chartVz.Series["速度VZ下限"].Points.Count - MAX_CHART_POINTS);
            }
            distanceHigh = displayDataList[displayDataList.Count - 1].maxVz - displayDataList[displayDataList.Count - 1].vz;
            distanceLow = displayDataList[displayDataList.Count - 1].vz - displayDataList[displayDataList.Count - 1].minVz;
            chartVz.Titles[0].Text = String.Format("上限差值={0:F},下限差值={1:F}", distanceHigh, distanceLow);
            ((TextAnnotation)chartVz.Annotations[0]).Text = String.Format("{0:F}", displayDataList[displayDataList.Count - 1].vz);
            chartVz.Series["速度VZ误差"].Points.Clear();
            chartVz.Series["速度VZ误差"].Points.Add(new SeriesPoint(displayDataList[displayDataList.Count - 1].time,
                displayDataList[displayDataList.Count - 1].vz + Config.GetInstance().speedError,
                displayDataList[displayDataList.Count - 1].vz - Config.GetInstance().speedError));
            chartVz.EndInit();

            displayDataList.Clear();
        }

        /// <summary>
        /// 检测速度
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        /// <param name="vz"></param>
        /// <param name="minVx"></param>
        /// <param name="maxVx"></param>
        /// <param name="minVy"></param>
        /// <param name="maxVy"></param>
        /// <param name="minVz"></param>
        /// <param name="maxVz"></param>
        public void CheckSpeed(double vx, double vy, double vz, double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz)
        {
            //设置背景色
            if (vx > maxVx || vx < minVx)
            {
                chartVx.BackColor = Color.Red;
            }
            else
            {
                chartVx.BackColor = Color.White;
            }
            if (vy > maxVy || vy < minVy)
            {
                chartVy.BackColor = Color.Red;
            }
            else
            {
                chartVy.BackColor = Color.White;
            }
            if (vz > maxVz || vz < minVz)
            {
                chartVz.BackColor = Color.Red;
            }
            else
            {
                chartVz.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 检测落点
        /// </summary>
        /// <param name="fallPoint"></param>
        /// <param name="fallTime"></param>
        public void CheckFallPoint(FallPoint fallPoint, double fallTime)
        {
            //范围检测
            if (fallPoint.x < -Config.GetInstance().sideLine ||
                fallPoint.x > Config.GetInstance().sideLine ||
                fallPoint.y < Config.GetInstance().backwardLine ||
                fallPoint.y > Config.GetInstance().forwardLine)
            {
                chartPoints.BackColor = Color.Red;
            }
            else
            {
                chartPoints.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 清除显示
        /// </summary>
        public void ClearAllChart()
        {
            ClearChartData(chartX);         //清空chartX
            ClearChartData(chartY);         //清空chartY
            ClearChartData(chartZ);         //清空chartZ
            ClearChartData(chartVx);        //清空chartVx
            ClearChartData(chartVy);        //清空chartVy
            ClearChartData(chartVz);        //清空chartVz
            ClearChartData(chartPoints);    //清空chartPoints
        }

        /// <summary>
        /// 清除显示
        /// </summary>
        /// <param name="chartControl"></param>
        private void ClearChartData(ChartControl chartControl)
        {
            foreach (Series series in chartControl.Series)
            {
                series.Points.Clear();
            }
            foreach (ChartTitle title in chartControl.Titles)
            {
                title.Text = String.Empty;
            }
            foreach (TextAnnotation textAnnotation in chartControl.Annotations)
            {
                textAnnotation.Text = String.Empty;
            }
            chartControl.BackColor = Color.White;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ideaPoint"></param>
        public void InitChartPoints(FallPoint ideaPoint)
        {
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(ideaPoint.x, ideaPoint.y));         //理想落点
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(-Config.GetInstance().sideLine,       //必炸线
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));                    //
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(Config.GetInstance().sideLine,        //必炸线
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));                    //
        }

        /// <summary>
        /// 初始化上下限
        /// </summary>
        public void InitPositionSpeedMaxMin()
        {
            chartX.BeginInit();                                                         //
            chartX.Series["位置X上限"].Points.Clear();                                  //
            chartX.Series["位置X下限"].Points.Clear();                                  //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartX.Series["位置X上限"].Points.AddPoint(item.Time, item.MaxX);       //
                chartX.Series["位置X下限"].Points.AddPoint(item.Time, item.MinX);       //
            });                                                                         //
            chartX.EndInit();                                                           //
                                                                                        //
            chartY.BeginInit();                                                         //
            chartY.Series["位置Y上限"].Points.Clear();                                  //
            chartY.Series["位置Y下限"].Points.Clear();                                  //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartY.Series["位置Y上限"].Points.AddPoint(item.Time, item.MaxY);       //
                chartY.Series["位置Y下限"].Points.AddPoint(item.Time, item.MinY);       //
            });                                                                         //
            chartY.EndInit();                                                           //
                                                                                        //
            chartZ.BeginInit();                                                         //
            chartZ.Series["位置Z上限"].Points.Clear();                                  //
            chartZ.Series["位置Z下限"].Points.Clear();                                  //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartZ.Series["位置Z上限"].Points.AddPoint(item.Time, item.MaxZ);       //
                chartZ.Series["位置Z下限"].Points.AddPoint(item.Time, item.MinZ);       //
            });                                                                         //
            chartZ.EndInit();                                                           //
                                                                                        //
            chartVx.BeginInit();                                                        //
            chartVx.Series["速度VX上限"].Points.Clear();                                //
            chartVx.Series["速度VX下限"].Points.Clear();                                //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartVx.Series["速度VX上限"].Points.AddPoint(item.Time, item.MaxVx);    //
                chartVx.Series["速度VX下限"].Points.AddPoint(item.Time, item.MinVx);    //
            });                                                                         //
            chartVx.EndInit();                                                          //
                                                                                        //
            chartVy.BeginInit();                                                        //
            chartVy.Series["速度VY上限"].Points.Clear();                                //
            chartVy.Series["速度VY下限"].Points.Clear();                                //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartVy.Series["速度VY上限"].Points.AddPoint(item.Time, item.MaxVy);    //
                chartVy.Series["速度VY下限"].Points.AddPoint(item.Time, item.MinVy);    //
            });                                                                         //
            chartVy.EndInit();                                                          //
                                                                                        //
            chartVz.BeginInit();                                                        //
            chartVz.Series["速度VZ上限"].Points.Clear();                                //
            chartVz.Series["速度VZ下限"].Points.Clear();                                //
            Config.GetInstance().minMaxValues.ForEach(item =>                           //
            {                                                                           //
                chartVz.Series["速度VZ上限"].Points.AddPoint(item.Time, item.MaxVz);    //
                chartVz.Series["速度VZ下限"].Points.AddPoint(item.Time, item.MinVz);    //
            });                                                                         //
            chartVz.EndInit();                                                          //
        }

        /// <summary>
        /// 显示落点和射程
        /// </summary>
        /// <param name="fallTime"></param>
        /// <param name="distance"></param>
        public void DisplayFallTimeAndDistance(double fallTime, double distance)
        {
            chartPoints.Titles[0].Text = String.Format("剩余落地时间:{0:F}s, 射程:{1:F}m", fallTime, distance);
        }

        /// <summary>
        /// 添加显示数据
        /// </summary>
        /// <param name="time"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        /// <param name="vz"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="minVx"></param>
        /// <param name="maxVx"></param>
        /// <param name="minVy"></param>
        /// <param name="maxVy"></param>
        /// <param name="minVz"></param>
        /// <param name="maxVz"></param>
        /// <param name="fallPoint"></param>
        /// <param name="fallTime"></param>
        /// <param name="distance"></param>
        public void AddDisplayData(int time, double x, double y, double z, double vx, double vy, double vz,
            double minX, double maxX, double minY, double maxY, double minZ, double maxZ,
            double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz,
            FallPoint fallPoint, double fallTime, double distance)
        {
            displayDataList.Add(new DisplayData
            {
                time = time,               //time
                x = x,                     //x
                y = y,                     //y
                z = z,                     //z
                vx = vx,                   //vx
                vy = vy,                   //vy
                vz = vz,                   //vz
                minX = minX,               //minX
                maxX = maxX,               //maxX
                minY = minY,               //minY
                maxY = maxY,               //maxY
                minZ = minZ,               //minZ
                maxZ = maxZ,               //maxZ
                minVx = minVx,             //minVx
                maxVx = maxVx,             //maxVx
                minVy = minVy,             //minVy
                maxVy = maxVy,             //maxVy
                minVz = minVz,             //minVz
                maxVz = maxVz,             //maxVz
                fallPoint = fallPoint,     //fallPoint
                fallTime = fallTime,       //fallTime
                distance = distance        //distance
            });
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            displayDataList.Clear(); // 清除数据
        }

        /// <summary>
        /// 设置最大显示点数
        /// </summary>
        /// <param name="maxChartPoint"></param>
        public void SetMaxChartPoint(int maxChartPoint)
        {
            MAX_CHART_POINTS = maxChartPoint; // 设置最大显示点数
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void StartTimer()
        {
            chartUpateTimer.Start(); //定时器开始
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void StopTimer()
        {
            chartUpateTimer.Stop(); //定时器停止
        }
    }
}
