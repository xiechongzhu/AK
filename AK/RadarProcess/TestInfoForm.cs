﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadarProcess
{
    public partial class TestInfoForm : Form
    {
        public TestInfoForm()
        {
            InitializeComponent();
            cbxSource.SelectedIndex = Config.GetInstance().source;
        }

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void GetTestInfo(out String testName, out String testOperator, out String testComment)
        {
            testName = editTestName.Text.Trim();
            testOperator = editOperator.Text.Trim();
            testComment = editComment.Text.Trim();
        }
    }
}
