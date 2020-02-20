using DevExpress.XtraEditors;
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
    public partial class LoadDataForm : Form
    {
        // 向主界面传输动作控制
        public delegate void setOffLineFilePlayStatus(int action, int param1 = 0);
        public setOffLineFilePlayStatus setPlayStatus;

        // 加载的文件名称
        public string loadFileName;
        // 是否正在加载
        public bool bLoadFileing = false;

        public LoadDataForm()
        {
            InitializeComponent();
            setBtnsEnable(true);
            setProgressBarValue(0, 100, 0);
        }

        // 窗口事件
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            // 捕捉关闭窗体消息(用户点击关闭窗体控制按钮) 
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                if (!bLoadFileing)
                {
                    this.Hide();
                    return;
                }
                else
                {
                    XtraMessageBox.Show("正在加载离线文件，不能关闭窗口!", "错误", MessageBoxButtons.OK);
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // 是否可以选择多个文件
            dialog.Multiselect = false;
            dialog.Title = "请选择文件夹";
            dialog.Filter = "数据文件(*.dat,*.bin)|*.dat;*.bin";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = dialog.FileName;
                if (filePath == "")
                {
                    return;
                }
                edit_LoadFileName.Text = filePath;
                LoadFileToolTip.SetToolTip(edit_LoadFileName, filePath);
                // 加载的文件名称
                loadFileName = filePath;
            }
        }

        public string getLoadFileName()
        {
            return loadFileName;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // 如果文件存在
            if (!System.IO.File.Exists(loadFileName)) {
                XtraMessageBox.Show("请先加载文件!", "错误", MessageBoxButtons.OK);
                return;
            }
            setPlayStatus(MainForm.E_LOADFILE_START);
            // 是否正在加载
            bLoadFileing = true;
            setBtnsEnable(false);

            // 更新进度
            setProgressBarValue(0, 100, 0);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            setPlayStatus(MainForm.E_LOADFILE_STOP);
            // 是否正在加载
            bLoadFileing = false;
            setBtnsEnable(true);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            // 是否正在加载
            if (bLoadFileing)
            {
                setPlayStatus(MainForm.E_LOADFILE_PAUSE);
                bLoadFileing = false;
                btnPause.Text = "启动";
            }
            else
            {
                setPlayStatus(MainForm.E_LOADFILE_CONTINUE);
                bLoadFileing = true;
                btnPause.Text = "暂停";
            }
        }

        private void setBtnsEnable(bool bEnable)
        {
            btnOpenFile.Enabled = bEnable;
            btnStart.Enabled = bEnable;
            btnStop.Enabled = !bEnable;
            btnPause.Enabled = !bEnable;

            btnSkip.Enabled = !bEnable;
            spinEdit_Progress.Enabled = !bEnable;
        }

        public void loadFileFinish()
        {
            setBtnsEnable(true);
            // 是否正在加载
            bLoadFileing = false;
        }

        public void setProgressBarValue(double minValue, double maxValue, double curValue)
        {
            progressBar1.Minimum = (int)minValue;
            progressBar1.Maximum = (int)maxValue;
            progressBar1.Value = (int)curValue;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            return;
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            int progress = (int)spinEdit_Progress.Value;
            setPlayStatus(MainForm.E_LOADFILE_SKIPPROGRAM, progress);
        }
    }
}
