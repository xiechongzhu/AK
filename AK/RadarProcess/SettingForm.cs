/******************************************************************* 
* @brief : 设置窗口 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using DevExpress.XtraEditors;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// 设置窗口 
    /// </summary>
    public partial class SettingForm : Form
    {
        /// <summary>
        /// minMaxValues
        /// </summary>
        private List<MinMaxValue> minMaxValues = new List<MinMaxValue>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingForm()
        {
            InitializeComponent();  //初始化窗口
            LoadConfig();   //加载配置
        }

        /// <summary>
        /// SetViwMode
        /// </summary>
        public void SetViwMode()
        {
            //初始化
            //遍历控件
            foreach(Control control in Controls)
            {
                if(control is TextEdit) //如果时编辑框
                {
                    ((TextEdit)control).ReadOnly = true;
                }
            }
            layoutControlItem2.HideToCustomization();   //隐藏控件
            layoutControlItem4.HideToCustomization();   //隐藏控件
            layoutControlItem5.HideToCustomization();   //隐藏控件
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        protected void LoadConfig()
        {
            String errMsg; //错误信息
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                MessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //设置控件显示
            editLongitudeInit.Text = Config.GetInstance().longitudeInit.ToString();                //设置控件显示
            editLatitudeInit.Text = Config.GetInstance().latitudeInit.ToString();                  //设置控件显示
            editHeightInit.Text = Config.GetInstance().heightInit.ToString();                      //设置控件显示
            editAzimuthInit.Text = Config.GetInstance().azimuthInit.ToString();                    //设置控件显示
            editPlacementHeight.Text = Config.GetInstance().placementHeight.ToString();            //设置控件显示
            editFligtShot.Text = Config.GetInstance().flightshot.ToString();                       //设置控件显示
            editForwardLine.Text = Config.GetInstance().forwardLine.ToString();                    //设置控件显示
            editBackLine.Text = Config.GetInstance().backwardLine.ToString();                      //设置控件显示
            editSideLine.Text = Config.GetInstance().sideLine.ToString();                          //设置控件显示
            editRadarMultiCastIp.Text = Config.GetInstance().strRadarMultiCastIpAddr;              //设置控件显示
            editRadarPort.Text = Config.GetInstance().radarPort.ToString();                        //设置控件显示
            editTelemetryMultiCastIp.Text = Config.GetInstance().strTelemetryMultiCastIpAddr;      //设置控件显示
            editTelemetryPort.Text = Config.GetInstance().telemetryPort.ToString();                //设置控件显示
            editStation.Text = Config.GetInstance().stationId.ToString();                          //设置控件显示
            editSpeedError.Text = Config.GetInstance().speedError.ToString();                      //设置控件显示
            editPointError.Text = Config.GetInstance().pointError.ToString();                      //设置控件显示
            editPoint.Text = Config.GetInstance().maxPointCount.ToString();                        //设置控件显示
            minMaxValues = Config.GetInstance().minMaxValues;                                      //设置控件显示
        }

        /// <summary>
        /// btnOK_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            String errMsg;
            if(editRadarMultiCastIp.Text.Equals(String.Empty) || editTelemetryMultiCastIp.Text.Equals(String.Empty))
            {
                XtraMessageBox.Show("IP地址不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(!CheckInputParams(out errMsg))
            {
                XtraMessageBox.Show(errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Config.GetInstance().longitudeInit = double.Parse(editLongitudeInit.Text);                //保存配置
                Config.GetInstance().latitudeInit = double.Parse(editLatitudeInit.Text);                  //保存配置
                Config.GetInstance().heightInit = double.Parse(editHeightInit.Text);                      //保存配置
                Config.GetInstance().azimuthInit = double.Parse(editAzimuthInit.Text);                    //保存配置
                Config.GetInstance().placementHeight = double.Parse(editPlacementHeight.Text);            //保存配置
                Config.GetInstance().flightshot = double.Parse(editFligtShot.Text);                       //保存配置
                Config.GetInstance().forwardLine = double.Parse(editForwardLine.Text);                    //保存配置
                Config.GetInstance().backwardLine = double.Parse(editBackLine.Text);                      //保存配置
                Config.GetInstance().sideLine = double.Parse(editSideLine.Text);                          //保存配置
                Config.GetInstance().strRadarMultiCastIpAddr = editRadarMultiCastIp.Text;                 //保存配置
                Config.GetInstance().radarPort = UInt16.Parse(editRadarPort.Text);                        //保存配置
                Config.GetInstance().strTelemetryMultiCastIpAddr = editTelemetryMultiCastIp.Text;         //保存配置
                Config.GetInstance().telemetryPort = UInt16.Parse(editTelemetryPort.Text);                //保存配置
                Config.GetInstance().speedError = double.Parse(editSpeedError.Text);                      //保存配置
                Config.GetInstance().pointError = double.Parse(editPointError.Text);                      //保存配置
                if(editPoint.Text.Equals(String.Empty))                                                   //保存配置
                {
                    Config.GetInstance().maxPointCount = 1000;
                }
                else
                {
                    Config.GetInstance().maxPointCount = int.Parse(editPoint.Text);
                }
                Config.GetInstance().minMaxValues = minMaxValues;
                Config.GetInstance().stationId = editStation.Text;
            }
            catch(Exception ex)
            {
                MessageBox.Show("输入错误," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            if(!Config.GetInstance().SaveConfig(out errMsg))
            {
                MessageBox.Show("保存配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// btnCancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// SetParams
        /// </summary>
        /// <param name="longitudeInit"></param>
        /// <param name="latitudeInit"></param>
        /// <param name="heightInit"></param>
        /// <param name="azimuthInit"></param>
        /// <param name="placementHeight"></param>
        /// <param name="flightshot"></param>
        /// <param name="forwardLine"></param>
        /// <param name="backwardLine"></param>
        /// <param name="sideLine"></param>
        /// <param name="strMultiCastIpAddr"></param>
        /// <param name="port"></param>
        /// <param name="stationId"></param>
        /// <param name="speedError"></param>
        /// <param name="pointError"></param>
        /// <param name="maxPointCount"></param>
        public void SetParams(double longitudeInit, double latitudeInit, double heightInit, double azimuthInit,
            double placementHeight, double flightshot, double forwardLine, double backwardLine, double sideLine,
            String strMultiCastIpAddr, UInt16 port, String stationId, double speedError, double pointError, int maxPointCount)
        {
            editLongitudeInit.Text = longitudeInit.ToString();
            editLatitudeInit.Text = latitudeInit.ToString();
            editHeightInit.Text = heightInit.ToString();
            editAzimuthInit.Text = azimuthInit.ToString();
            editPlacementHeight.Text = placementHeight.ToString();
            editFligtShot.Text = flightshot.ToString();
            editForwardLine.Text = forwardLine.ToString();
            editBackLine.Text = backwardLine.ToString();
            editSideLine.Text = sideLine.ToString();
            editRadarMultiCastIp.Text = strMultiCastIpAddr;
            editRadarPort.Text = port.ToString();
            editStation.Text = stationId.ToString();
            editSpeedError.Text = speedError.ToString();
            editPointError.Text = pointError.ToString();
            editPoint.Text = maxPointCount.ToString();
        }

        /// <summary>
        /// btnDownload_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "参数文件|*.xlsx";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Assembly assembly = Assembly.GetEntryAssembly();
                    ResourceManager resourceManager = new ResourceManager("RadarProcess.Properties.Resources", assembly);
                    byte[] fileByte = resourceManager.GetObject("参数模板") as byte[];
                    using (FileStream fileStream = File.Create(saveFileDialog.FileName))
                    {
                        fileStream.Write(fileByte, 0, fileByte.Length);
                    }
                    XtraMessageBox.Show("下载模板参数文件成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex)
                {
                    XtraMessageBox.Show("下载模板参数文件失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// btnImport_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "参数模板文件|*.xlsx";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadConfigExcelFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// LoadConfigExcelFile
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadConfigExcelFile(String fileName)
        {
            minMaxValues.Clear(); //清除数据
            try
            {
                //读取excel
                using (Stream fileStream = File.Open(fileName, FileMode.Open))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    editLongitudeInit.Text = sheet.GetRow(0).GetCell(1).ToString();     //经度
                    editLatitudeInit.Text = sheet.GetRow(1).GetCell(1).ToString();      //纬度
                    editHeightInit.Text = sheet.GetRow(2).GetCell(1).ToString();        //初始高度
                    editAzimuthInit.Text = sheet.GetRow(3).GetCell(1).ToString();       //发射角度
                    editPlacementHeight.Text = sheet.GetRow(4).GetCell(1).ToString();   //高度
                    editFligtShot.Text = sheet.GetRow(5).GetCell(1).ToString();         //最大射程
                    editForwardLine.Text = sheet.GetRow(6).GetCell(1).ToString();       //前向必炸线
                    editBackLine.Text = sheet.GetRow(7).GetCell(1).ToString();          //后向必炸线
                    editSideLine.Text = sheet.GetRow(8).GetCell(1).ToString();          //侧向必炸线
                    editStation.Text = sheet.GetRow(9).GetCell(1).ToString();           //站点Id
                    editSpeedError.Text = sheet.GetRow(10).GetCell(1).ToString();       //速度错误数据
                    editPointError.Text = sheet.GetRow(11).GetCell(1).ToString();       //落点错误数据

                    int startTime = 0;
                    sheet = workbook.GetSheetAt(1);
                    for(int i = 1; i <= sheet.LastRowNum; ++i)
                    {
                        int time = (int)sheet.GetRow(i).GetCell(0).NumericCellValue;
                        MinMaxValue minMaxValue = new MinMaxValue
                        {
                            Time = time,                                             //时间
                            MinX = sheet.GetRow(i).GetCell(1).NumericCellValue,      //X最小值
                            MaxX = sheet.GetRow(i).GetCell(2).NumericCellValue,      //X最大值
                            MinY = sheet.GetRow(i).GetCell(3).NumericCellValue,      //Y最小值
                            MaxY = sheet.GetRow(i).GetCell(4).NumericCellValue,      //Y最大值
                            MinZ = sheet.GetRow(i).GetCell(5).NumericCellValue,      //Z最小值
                            MaxZ = sheet.GetRow(i).GetCell(6).NumericCellValue,      //Z最大值
                            MinVx = sheet.GetRow(i).GetCell(7).NumericCellValue,     //Vx最小值
                            MaxVx = sheet.GetRow(i).GetCell(8).NumericCellValue,     //Vx最大值
                            MinVy = sheet.GetRow(i).GetCell(9).NumericCellValue,     //Vy最小值
                            MaxVy = sheet.GetRow(i).GetCell(10).NumericCellValue,    //Vy最大值
                            MinVz = sheet.GetRow(i).GetCell(11).NumericCellValue,    //Vz最小值
                            MaxVz = sheet.GetRow(i).GetCell(12).NumericCellValue     //Vz最大值
                        };
                        if(minMaxValue.Time < startTime)
                        {
                            throw new Exception(String.Format("第{0}行数据时间不正确", i));
                        }
                        if (minMaxValue.MinX >= minMaxValue.MaxX)
                        {
                            throw new Exception(String.Format("第{0}行数据位置X下限大于或等于上限", i));
                        }
                        if (minMaxValue.MinY >= minMaxValue.MaxY)
                        {
                            throw new Exception(String.Format("第{0}行数据位置Y下限大于或等于上限", i));
                        }
                        if (minMaxValue.MinZ >= minMaxValue.MaxZ)
                        {
                            throw new Exception(String.Format("第{0}行数据位置Z下限大于或等于上限", i));
                        }
                        if (minMaxValue.MinVx >= minMaxValue.MaxVx)
                        {
                            throw new Exception(String.Format("第{0}行数据速度Vx下限大于或等于上限", i));
                        }
                        if (minMaxValue.MinVy >= minMaxValue.MaxVy)
                        {
                            throw new Exception(String.Format("第{0}行数据速度Vy下限大于或等于上限", i));
                        }
                        if (minMaxValue.MinVz >= minMaxValue.MaxVz)
                        {
                            throw new Exception(String.Format("第{0}行数据速度Vz下限大于或等于上限", i));
                        }
                        startTime = minMaxValue.Time;
                        minMaxValues.Add(minMaxValue);
                    }
                    XtraMessageBox.Show(String.Format("读取分量上下限数据成功，共{0}条记录", minMaxValues.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                minMaxValues.Clear();
                XtraMessageBox.Show("导入参数文件失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        /// <summary>
        /// CheckInputParams
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool CheckInputParams(out String errMsg)
        {
            errMsg = String.Empty;
            double longitudeInit = double.Parse(editLongitudeInit.Text);
            if(longitudeInit < 0 || longitudeInit > 180)
            {
                errMsg = "经度范围:[0,180]";
                return false;
            }

            double latitudeInit = double.Parse(editLatitudeInit.Text);
            if(latitudeInit <=0 || latitudeInit >= 90)
            {
                errMsg = "纬度范围:(0,90)";
                return false;
            }

            double heightInit = double.Parse(editHeightInit.Text);
            if(heightInit < -1000 || heightInit > 10000)
            {
                errMsg = "初始高度范围:[-1000,10000]";
                return false;
            }

            double azimuthInit = double.Parse(editAzimuthInit.Text);
            if(azimuthInit <= -180 || azimuthInit > 180)
            {
                errMsg = "初始方位角范围:(-180,180]";
                return false;
            }

            double placementHeight = double.Parse(editPlacementHeight.Text);
            if(placementHeight < 0)
            {
                errMsg = "落点附近高度不能为负数";
                return false;
            }

            double fligtShot = double.Parse(editFligtShot.Text);
            if(fligtShot < 0)
            {
                errMsg = "理论射程不能为负数";
                return false;
            }

            double forwardLine = double.Parse(editForwardLine.Text);
            double backLine = double.Parse(editBackLine.Text);
            if(forwardLine <= backLine)
            {
                errMsg = "前向必炸线必须大于后向必炸线";
                return false;
            }
            if(fligtShot > forwardLine || fligtShot < backLine)
            {
                errMsg = "理论射程必须在前向必炸线和后向必炸线范围之间";
                return false;
            }

            double sideLine = double.Parse(editSideLine.Text);
            if(sideLine <=0)
            {
                errMsg = "侧向必炸线必须大于0";
                return false;
            }

            return true;
        }
    }
}
