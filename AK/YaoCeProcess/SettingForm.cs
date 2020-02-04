using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YaoCeProcess
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
            if (!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                MessageBox.Show("加载配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            editMultiCastIp.Text = Config.GetInstance().strMultiCastIpAddr;
            editPort.Text = Config.GetInstance().port.ToString();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Config.GetInstance().strMultiCastIpAddr = editMultiCastIp.Text;
                Config.GetInstance().port = UInt16.Parse(editPort.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入错误," + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            String errMsg;
            if (!Config.GetInstance().SaveConfig(out errMsg))
            {
                MessageBox.Show("保存配置文件失败," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
