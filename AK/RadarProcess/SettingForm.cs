using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadarProcess
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            LoadConfig();
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
            editLocMaxX.Text = Config.GetInstance().locMaxX.ToString();
            editLocMaxY.Text = Config.GetInstance().locMaxY.ToString();
            editLocMaxZ.Text = Config.GetInstance().locMaxZ.ToString();
            editLocMinX.Text = Config.GetInstance().locMinX.ToString();
            editLocMinY.Text = Config.GetInstance().locMinY.ToString();
            editLocMinZ.Text = Config.GetInstance().locMinZ.ToString();
            editSpeedMaxX.Text = Config.GetInstance().speedMaxX.ToString();
            editSpeedMaxY.Text = Config.GetInstance().speedMaxY.ToString();
            editSpeedMaxZ.Text = Config.GetInstance().speedMaxZ.ToString();
            editSpeedMinX.Text = Config.GetInstance().speedMinX.ToString();
            editSpeedMinY.Text = Config.GetInstance().speedMinY.ToString();
            editSpeedMinZ.Text = Config.GetInstance().speedMinZ.ToString();
            editMultiCastIp.Text = Config.GetInstance().strMultiCastIpAddr;
            editPort.Text = Config.GetInstance().port.ToString();
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
                Config.GetInstance().locMaxX = double.Parse(editLocMaxX.Text);
                Config.GetInstance().locMaxY = double.Parse(editLocMaxY.Text);
                Config.GetInstance().locMaxZ = double.Parse(editLocMaxZ.Text);
                Config.GetInstance().locMinX = double.Parse(editLocMinX.Text);
                Config.GetInstance().locMinY = double.Parse(editLocMinY.Text);
                Config.GetInstance().locMinZ = double.Parse(editLocMinZ.Text);
                Config.GetInstance().speedMaxX = double.Parse(editSpeedMaxX.Text);
                Config.GetInstance().speedMaxY = double.Parse(editSpeedMaxY.Text);
                Config.GetInstance().speedMaxZ = double.Parse(editSpeedMaxZ.Text);
                Config.GetInstance().speedMinX = double.Parse(editSpeedMinX.Text);
                Config.GetInstance().speedMinY = double.Parse(editSpeedMinY.Text);
                Config.GetInstance().speedMinZ = double.Parse(editSpeedMinZ.Text);
                Config.GetInstance().strMultiCastIpAddr = editMultiCastIp.Text;
                Config.GetInstance().port = UInt16.Parse(editPort.Text);
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
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
