// 
using System; //
// 
using System.Collections.Generic; //
// 
using System.ComponentModel; //
// 
using System.Data; //
// 
using System.Drawing; //
// 
using System.Linq; //
// 
using System.Text; //
// 
using System.Threading.Tasks; //
// 
using System.Windows.Forms; //
// 

// 
/// <summary>
// 
/// YaoCeProcess
// 
/// </summary>
// 
namespace YaoCeProcess
// 
{
// 
    /// <summary>
// 
    /// 文件名:SettingForm/
// 
    /// 文件功能描述:设置网络参数/
// 
    /// 创建人:yangy
// 
    /// 版权所有:Copyright (C) ZGM/
// 
    /// 创建标识:2020.03.12/     
// 
    /// 修改描述:/
// 
    /// </summary>
// 
    public partial class SettingForm : Form
// 
    {
// 
        /// <summary>
// 
        /// SettingForm
// 
        /// </summary>
// 
        public SettingForm()
// 
        {
// 
            InitializeComponent(); //
// 
            this.editPort.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx; //
// 
            // 5000-65535
// 
            this.editPort.Properties.Mask.EditMask = @"5[0-9]{3}|[6-9]{4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]"; //
// 

// 
            LoadConfig(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// LoadConfig
// 
        /// </summary>
// 
        protected void LoadConfig()
// 
        {
// 
            String errMsg; //
// 
            if (!Config.GetInstance().LoadConfigFile(out errMsg))
// 
            {
// 
                MessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 
                return; //
// 
            }
// 
            editMultiCastIp.Text = Config.GetInstance().strMultiCastIpAddr; //
// 
            editPort.Text = Config.GetInstance().port.ToString(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// btnCancel_Click
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void btnCancel_Click(object sender, EventArgs e)
// 
        {
// 
            Close(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// btnOK_Click
// 
        /// </summary>
// 
        /// <param name="sender"></param>
// 
        /// <param name="e"></param>
// 
        private void btnOK_Click(object sender, EventArgs e)
// 
        {
// 
            if(editMultiCastIp.Text.Equals(String.Empty))
// 
            {
// 
                MessageBox.Show("IP地址不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 
                return; //
// 
            }
// 
            try
// 
            {
// 
                Config.GetInstance().strMultiCastIpAddr = editMultiCastIp.Text; //
// 
                Config.GetInstance().port = UInt16.Parse(editPort.Text); //
// 
            }
// 
            catch (Exception ex)
// 
            {
// 
                MessageBox.Show("输入错误," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 

// 
                return; //
// 
            }
// 
            String errMsg; //
// 
            if (!Config.GetInstance().SaveConfig(out errMsg))
// 
            {
// 
                MessageBox.Show("保存配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); //
// 
            }
// 

// 
            DialogResult = DialogResult.OK; //
// 
            Close(); //
// 
        }
// 
    }
// 
}
// 
