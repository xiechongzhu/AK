using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
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
            LoadTestInfo();
            InitGMap();
        }
        private void InitGMap()
        {
            gMapControl.MapProvider = GoogleChinaMapProvider.Instance;
            gMapControl.MinZoom = 1;
            gMapControl.MaxZoom = 13;
            gMapControl.Zoom = 9;
            gMapControl.ShowCenter = false;
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
                    foreach(S_OBJECT obj in objectList)
                    {
                        positionChart.Series["位置X"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.X));
                        positionChart.Series["位置Y"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.Y));
                        positionChart.Series["位置Z"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.Z));
                        speedChart.Series["速度X"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VX));
                        speedChart.Series["速度Y"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VY));
                        speedChart.Series["速度Z"].Points.Add(new SeriesPoint(CHART_ITEM_INDEX, obj.VZ));
                        CHART_ITEM_INDEX++;
                    }
                }
            }
        }
    }
}
