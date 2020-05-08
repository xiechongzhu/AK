using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts;
using System.Security.Cryptography.X509Certificates;
using DevExpress.XtraCharts.Native;

namespace RadarProcess
{
    public struct DisplayData
    {
        public int time;
        public double x;
        public double y;
        public double z;
        public double vx;
        public double vy;
        public double vz;
        public double minX;
        public double maxX;
        public double minY;
        public double maxY;
        public double minZ;
        public double maxZ;
        public double minVx;
        public double maxVx;
        public double minVy;
        public double maxVy;
        public double minVz;
        public double maxVz;
        public FallPoint fallPoint;
        public double fallTime;
        public double distance;
    }

    public partial class MyChartControl : DevExpress.XtraEditors.XtraUserControl
    {
        private List<DisplayData> displayDataList = new List<DisplayData>();
        private int MAX_CHART_POINTS;

        public MyChartControl()
        {
            InitializeComponent();
            chartUpateTimer.Tick += chartUpateTimer_Tick;
        }

        public void SetFallPoint(FallPoint fallPoint)
        {
            if (fallPoint != null)
            {
                chartPoints.Series["预测落点"].Points.Clear();
                chartPoints.Series["预测落点"].Points.Add(new SeriesPoint(fallPoint.x, fallPoint.y));
                chartPoints.Series["落点误差"].Points.Clear();
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x - Config.GetInstance().pointError,
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));
                chartPoints.Series["落点误差"].Points.Add(new SeriesPoint(fallPoint.x + Config.GetInstance().pointError,
                    fallPoint.y + Config.GetInstance().pointError, fallPoint.y - Config.GetInstance().pointError));
            }
        }

        public void CheckPosition(double x, double y, double z, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
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

        private void chartUpateTimer_Tick(object sender, EventArgs e)
        {
            if (displayDataList.Count == 0)
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

            foreach (DisplayData displayData in displayDataList)
            {
                positionXBuffer.Add(new SeriesPoint(displayData.time, displayData.x));
                positionMinXBuffer.Add(new SeriesPoint(displayData.time, displayData.minX));
                positionMaxXBuffer.Add(new SeriesPoint(displayData.time, displayData.maxX));
                positionYBuffer.Add(new SeriesPoint(displayData.time, displayData.y));
                positionMinYBuffer.Add(new SeriesPoint(displayData.time, displayData.minY));
                positionMaxYBuffer.Add(new SeriesPoint(displayData.time, displayData.maxY));
                positionZBuffer.Add(new SeriesPoint(displayData.time, displayData.z));
                positionMinZBuffer.Add(new SeriesPoint(displayData.time, displayData.minZ));
                positionMaxZBuffer.Add(new SeriesPoint(displayData.time, displayData.maxZ));
                speedVxBuffer.Add(new SeriesPoint(displayData.time, displayData.vx));
                speedMinVxBuffer.Add(new SeriesPoint(displayData.time, displayData.minVx));
                speedMaxVxBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVx));
                speedVyBuffer.Add(new SeriesPoint(displayData.time, displayData.vy));
                speedMinVyBuffer.Add(new SeriesPoint(displayData.time, displayData.minVy));
                speedMaxVyBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVy));
                speedVzBuffer.Add(new SeriesPoint(displayData.time, displayData.vz));
                speedMinVzBuffer.Add(new SeriesPoint(displayData.time, displayData.minVz));
                speedMaxVzBuffer.Add(new SeriesPoint(displayData.time, displayData.maxVz));

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

        public void CheckSpeed(double vx, double vy, double vz, double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz)
        {
            if (vx > maxVx || vx < minVx)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VX超出范围:" + vx.ToString());
                chartVx.BackColor = Color.Red;
            }
            else
            {
                chartVx.BackColor = Color.White;
            }
            if (vy > maxVy || vy < minVy)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VY超出范围:" + vy.ToString());
                chartVy.BackColor = Color.Red;
            }
            else
            {
                chartVy.BackColor = Color.White;
            }
            if (vz > maxVz || vz < minVz)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_WARN, "速度VZ超出范围:" + vz.ToString());
                chartVz.BackColor = Color.Red;
            }
            else
            {
                chartVz.BackColor = Color.White;
            }
        }

        public void CheckFallPoint(FallPoint fallPoint, double fallTime)
        {
            if (fallPoint.x < -Config.GetInstance().sideLine ||
                fallPoint.x > Config.GetInstance().sideLine ||
                fallPoint.y < Config.GetInstance().backwardLine ||
                fallPoint.y > Config.GetInstance().forwardLine)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_SELF_DESTRUCT, String.Format("落点超出范围:X={0},Y={1}", fallPoint.x, fallPoint.y));
                chartPoints.BackColor = Color.Red;
            }
            else
            {
                chartPoints.BackColor = Color.White;
            }
        }

        public void ClearAllChart()
        {
            ClearChartData(chartX);
            ClearChartData(chartY);
            ClearChartData(chartZ);
            ClearChartData(chartVx);
            ClearChartData(chartVy);
            ClearChartData(chartVz);
            ClearChartData(chartPoints);
        }

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

        public void InitChartPoints(FallPoint ideaPoint)
        {
            ideaPoint = Algorithm.CalcIdeaPointOfFall();
            chartPoints.Series["理想落点"].Points.Add(new SeriesPoint(ideaPoint.x, ideaPoint.y));
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(-Config.GetInstance().sideLine,
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));
            chartPoints.Series["必炸线"].Points.Add(new SeriesPoint(Config.GetInstance().sideLine,
                Config.GetInstance().forwardLine, Config.GetInstance().backwardLine));
        }

        public void InitPositionSpeedMaxMin()
        {
            chartX.BeginInit();
            chartX.Series["位置X上限"].Points.Clear();
            chartX.Series["位置X下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartX.Series["位置X上限"].Points.AddPoint(item.Time, item.MaxX);
                chartX.Series["位置X下限"].Points.AddPoint(item.Time, item.MinX);
            });
            chartX.EndInit();

            chartY.BeginInit();
            chartY.Series["位置Y上限"].Points.Clear();
            chartY.Series["位置Y下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartY.Series["位置Y上限"].Points.AddPoint(item.Time, item.MaxY);
                chartY.Series["位置Y下限"].Points.AddPoint(item.Time, item.MinY);
            });
            chartY.EndInit();

            chartZ.BeginInit();
            chartZ.Series["位置Z上限"].Points.Clear();
            chartZ.Series["位置Z下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartZ.Series["位置Z上限"].Points.AddPoint(item.Time, item.MaxZ);
                chartZ.Series["位置Z下限"].Points.AddPoint(item.Time, item.MinZ);
            });
            chartZ.EndInit();

            chartVx.BeginInit();
            chartVx.Series["速度VX上限"].Points.Clear();
            chartVx.Series["速度VX下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVx.Series["速度VX上限"].Points.AddPoint(item.Time, item.MaxVx);
                chartVx.Series["速度VX下限"].Points.AddPoint(item.Time, item.MinVx);
            });
            chartVx.EndInit();

            chartVy.BeginInit();
            chartVy.Series["速度VY上限"].Points.Clear();
            chartVy.Series["速度VY下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVy.Series["速度VY上限"].Points.AddPoint(item.Time, item.MaxVy);
                chartVy.Series["速度VY下限"].Points.AddPoint(item.Time, item.MinVy);
            });
            chartVy.EndInit();

            chartVz.BeginInit();
            chartVz.Series["速度VZ上限"].Points.Clear();
            chartVz.Series["速度VZ下限"].Points.Clear();
            Config.GetInstance().minMaxValues.ForEach(item =>
            {
                chartVz.Series["速度VZ上限"].Points.AddPoint(item.Time, item.MaxVz);
                chartVz.Series["速度VZ下限"].Points.AddPoint(item.Time, item.MinVz);
            });
            chartVz.EndInit();
        }

        public void DisplayFallTimeAndDistance(double fallTime, double distance)
        {
            chartPoints.Titles[0].Text = String.Format("剩余落地时间:{0:F}s, 射程:{1:F}m", fallTime, distance);
        }

        public void AddDisplayData(int time, double x, double y, double z, double vx, double vy, double vz,
            double minX, double maxX, double minY, double maxY, double minZ, double maxZ,
            double minVx, double maxVx, double minVy, double maxVy, double minVz, double maxVz,
            FallPoint fallPoint, double fallTime, double distance)
        {
            displayDataList.Add(new DisplayData
            {
                time = time,
                x = x,
                y = y,
                z = z,
                vx = vx,
                vy = vy,
                vz = vz,
                minX = minX,
                maxX = maxX,
                minY = minY,
                maxY = maxY,
                minZ = minZ,
                maxZ = maxZ,
                minVx = minVx,
                maxVx = maxVx,
                minVy = minVy,
                maxVy = maxVy,
                minVz = minVz,
                maxVz = maxVz,
                fallPoint = fallPoint,
                fallTime = fallTime,
                distance = distance
            });
        }

        public void ClearData()
        {
            displayDataList.Clear();
        }

        public void SetMaxChartPoint(int maxChartPoint)
        {
            MAX_CHART_POINTS = maxChartPoint;
        }

        public void StartTimer()
        {
            chartUpateTimer.Start();
        }

        public void StopTimer()
        {
            chartUpateTimer.Stop();
        }
    }
}
