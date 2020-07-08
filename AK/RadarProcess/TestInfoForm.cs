/******************************************************************* 
* @brief : 历史数据窗口 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

/// <summary>
/// namespace RadarProcess
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// class TestInfoForm
    /// </summary>
    public partial class TestInfoForm : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TestInfoForm()
        {
            InitializeComponent();
            cbxSource.SelectedIndex = Config.GetInstance().source;
        }

        /// <summary>
        /// btnOK_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if(editTestName.Text.Trim().Length == 0 || editOperator.Text.Trim().Length == 0)
            {
                XtraMessageBox.Show("试验名称和操作人员不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Config.GetInstance().source = cbxSource.SelectedIndex;
            Config.GetInstance().SaveConfig(out _);
            Close();
        }

        /// <summary>
        /// btnCancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// GetTestInfo
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testOperator"></param>
        /// <param name="testComment"></param>
        public void GetTestInfo(out String testName, out String testOperator, out String testComment)
        {
            testName = editTestName.Text.Trim();
            testOperator = editOperator.Text.Trim();
            testComment = editComment.Text.Trim();
        }
    }
}
