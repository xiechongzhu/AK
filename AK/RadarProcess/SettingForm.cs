using DevExpress.XtraEditors;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadarProcess
{
    public partial class SettingForm : Form
    {
        private List<MinMaxValue> minMaxValues = new List<MinMaxValue>();
        public SettingForm()
        {
            InitializeComponent();
            LoadConfig();
        }

        public void SetViwMode()
        {
            foreach(Control control in Controls)
            {
                if(control is TextEdit)
                {
                    ((TextEdit)control).ReadOnly = true;
                }
            }
            layoutControlItem2.HideToCustomization();
            layoutControlItem4.HideToCustomization();
            layoutControlItem5.HideToCustomization();
        }

        protected void LoadConfig()
        {
            String errMsg;
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                MessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            editLongitudeInit.Text = Config.GetInstance().longitudeInit.ToString();
            editLatitudeInit.Text = Config.GetInstance().latitudeInit.ToString();
            editHeightInit.Text = Config.GetInstance().heightInit.ToString();
            editAzimuthInit.Text = Config.GetInstance().azimuthInit.ToString();
            editPlacementHeight.Text = Config.GetInstance().placementHeight.ToString();
            editFligtShot.Text = Config.GetInstance().flightshot.ToString();
            editForwardLine.Text = Config.GetInstance().forwardLine.ToString();
            editBackLine.Text = Config.GetInstance().backwardLine.ToString();
            editSideLine.Text = Config.GetInstance().sideLine.ToString();
            editRadarMultiCastIp.Text = Config.GetInstance().strRadarMultiCastIpAddr;
            editRadarPort.Text = Config.GetInstance().radarPort.ToString();
            editTelemetryMultiCastIp.Text = Config.GetInstance().strTelemetryMultiCastIpAddr;
            editTelemetryPort.Text = Config.GetInstance().telemetryPort.ToString();
            editStation.Text = Config.GetInstance().stationId.ToString();
            minMaxValues = Config.GetInstance().minMaxValues;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Config.GetInstance().longitudeInit = double.Parse(editLongitudeInit.Text);
                Config.GetInstance().latitudeInit = double.Parse(editLatitudeInit.Text);
                Config.GetInstance().heightInit = double.Parse(editHeightInit.Text);
                Config.GetInstance().azimuthInit = double.Parse(editAzimuthInit.Text);
                Config.GetInstance().placementHeight = double.Parse(editPlacementHeight.Text);
                Config.GetInstance().flightshot = double.Parse(editFligtShot.Text);
                Config.GetInstance().forwardLine = double.Parse(editForwardLine.Text);
                Config.GetInstance().backwardLine = double.Parse(editBackLine.Text);
                Config.GetInstance().sideLine = double.Parse(editSideLine.Text);
                Config.GetInstance().strRadarMultiCastIpAddr = editRadarMultiCastIp.Text;
                Config.GetInstance().radarPort = UInt16.Parse(editRadarPort.Text);
                Config.GetInstance().strTelemetryMultiCastIpAddr = editTelemetryMultiCastIp.Text;
                Config.GetInstance().telemetryPort = UInt16.Parse(editTelemetryPort.Text);
                Config.GetInstance().stationId = int.Parse(editStation.Text);
                Config.GetInstance().minMaxValues = minMaxValues;
            }
            catch(Exception ex)
            {
                MessageBox.Show("输入错误," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            String errMsg;
            if(!Config.GetInstance().SaveConfig(out errMsg))
            {
                MessageBox.Show("保存配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void SetParams(double longitudeInit, double latitudeInit, double heightInit, double azimuthInit,
            double placementHeight, double flightshot, double forwardLine, double backwardLine, double sideLine,
            String strMultiCastIpAddr, UInt16 port, int stationId)
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
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "参数模板文件|*.xlsx";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(@"resource/参数模板.xlsx", saveFileDialog.FileName, true);
                }
                catch(Exception ex)
                {
                    XtraMessageBox.Show("下载模板参数文件失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "参数模板文件|*.xlsx";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadConfigExcelFile(openFileDialog.FileName);
            }
        }

        private void LoadConfigExcelFile(String fileName)
        {
            minMaxValues.Clear();
            try
            {
                using (Stream fileStream = File.Open(fileName, FileMode.Open))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    editLongitudeInit.Text = sheet.GetRow(0).GetCell(1).ToString();
                    editLatitudeInit.Text = sheet.GetRow(1).GetCell(1).ToString();
                    editHeightInit.Text = sheet.GetRow(2).GetCell(1).ToString();
                    editAzimuthInit.Text = sheet.GetRow(3).GetCell(1).ToString();
                    editPlacementHeight.Text = sheet.GetRow(4).GetCell(1).ToString();
                    editFligtShot.Text = sheet.GetRow(5).GetCell(1).ToString();
                    editForwardLine.Text = sheet.GetRow(6).GetCell(1).ToString();
                    editBackLine.Text = sheet.GetRow(7).GetCell(1).ToString();
                    editSideLine.Text = sheet.GetRow(8).GetCell(1).ToString();
                    editStation.Text = sheet.GetRow(9).GetCell(1).ToString();

                    int startTime = 0;
                    sheet = workbook.GetSheetAt(1);
                    for(int i = 1; i <= sheet.LastRowNum; ++i)
                    {
                        int time = (int)sheet.GetRow(i).GetCell(0).NumericCellValue;
                        MinMaxValue minMaxValue = new MinMaxValue
                        {
                            Time = (int)sheet.GetRow(i).GetCell(0).NumericCellValue,
                            MinX = sheet.GetRow(i).GetCell(1).NumericCellValue,
                            MaxX = sheet.GetRow(i).GetCell(2).NumericCellValue,
                            MinY = sheet.GetRow(i).GetCell(3).NumericCellValue,
                            MaxY = sheet.GetRow(i).GetCell(4).NumericCellValue,
                            MinZ = sheet.GetRow(i).GetCell(5).NumericCellValue,
                            MaxZ = sheet.GetRow(i).GetCell(6).NumericCellValue,
                            MinVx = sheet.GetRow(i).GetCell(7).NumericCellValue,
                            MaxVx = sheet.GetRow(i).GetCell(8).NumericCellValue,
                            MinVy = sheet.GetRow(i).GetCell(9).NumericCellValue,
                            MaxVy = sheet.GetRow(i).GetCell(10).NumericCellValue,
                            MinVz = sheet.GetRow(i).GetCell(11).NumericCellValue,
                            MaxVz = sheet.GetRow(i).GetCell(12).NumericCellValue
                        };
                        if(minMaxValue.Time < startTime)
                        {
                            throw new Exception(String.Format("第{0}行时间数据不正确", i + 1));
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
    }
}
