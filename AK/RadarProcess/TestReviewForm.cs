﻿using DevExpress.XtraCharts;
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
        int CHART_ITEM_INDEX = 0;
        public TestReviewForm(long id)
        {
            recordId = id;
            InitializeComponent();
            String errMsg;
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                XtraMessageBox.Show("加载配置文件失败:" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(-Config.GetInstance().sideLine, Config.GetInstance().forwardLine, -Config.GetInstance().backwardLine));
            pointChartControl.Series["必炸线"].Points.Add(new SeriesPoint(Config.GetInstance().sideLine, Config.GetInstance().forwardLine, -Config.GetInstance().backwardLine));
            pointChartControl.Series["理想落点"].Points.Add(new SeriesPoint(0, 0));
            LoadTestInfo();
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
                List<S_OBJECT> objectList = null;
                try
                {
                    using (FileStream fs = new FileStream(strDateFile, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        objectList = (List<S_OBJECT>)formatter.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("读取历史数据失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if(objectList != null)
                {
                    List<SeriesPoint> positionXBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> positionYBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> positionZBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVxBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVyBuffer = new List<SeriesPoint>();
                    List<SeriesPoint> speedVzBuffer = new List<SeriesPoint>();
                    foreach(S_OBJECT obj in objectList)
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
                }
            }
        }
    }
}
