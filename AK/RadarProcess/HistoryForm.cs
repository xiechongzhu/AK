using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RadarProcess
{
    public partial class HistoryForm : Form
    {
        private DataTable dt = new DataTable();

        public HistoryForm()
        {
            InitializeComponent();
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            dt.Columns.Add("TestName", typeof(String));
            dt.Columns.Add("Operator", typeof(String));
            dt.Columns.Add("Time", typeof(String));
            dt.Columns.Add("Comment", typeof(String));
            dt.Columns.Add("Id", typeof(long));
            using (DataModels.DatabaseDB db = new DataModels.DatabaseDB())
            {
                var temp = from c in db.TestInfos select c;
                foreach (DataModels.TestInfo info in temp)
                {
                    dt.Rows.Add(info.TestName, info.Operator, info.Time.ToString("yyyy-MM-dd HH:mm:ss"), info.Comment, info.Id);
                }
            }

            gridControl.DataSource = dt;
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            GridHitInfo hInfo = gridView.CalcHitInfo(gridControl.PointToClient(MousePosition));
            if (hInfo.InRow)
            {
                int selectedRow = gridView.GetSelectedRows()[0];
                long id = (long)gridView.GetRowCellValue(selectedRow, "Id");
                TestReviewForm testReviewForm = new TestReviewForm(id);
                testReviewForm.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
                XtraMessageBox.Show("请至少选中一条记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DialogResult.Yes != XtraMessageBox.Show("确定要删除选中的记录吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }
            List<long> ids = new List<long>();
            foreach (int selectedRow in selectedRows)
            {
                long id = (long)gridView.GetRowCellValue(selectedRow, "Id");
                ids.Add(id);
                String date = (String)gridView.GetRowCellValue(selectedRow, "Time");
                DelectDir(String.Format("./Log/{0}", DateTime.Parse(date).ToString("yyyyMMddHHmmss")));
            }
            using (DataModels.DatabaseDB db = new DataModels.DatabaseDB())
            {
                db.TestInfos.Delete(item => ids.Contains(item.Id));
                db.CommitTransaction();
            }

            gridView.DeleteSelectedRows();
        }

        protected void DelectDir(string srcPath)
        {
            try
            {
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(srcPath);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(srcPath, System.IO.FileAttributes.Normal);

                //判断文件夹是否还存在
                if (Directory.Exists(srcPath))
                {
                    foreach (string f in Directory.GetFileSystemEntries(srcPath))
                    {
                        if (File.Exists(f))
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                            Console.WriteLine(f);
                        }
                        else
                        {
                            //循环递归删除子文件夹
                            DelectDir(f);
                        }
                    }

                    //删除空文件夹
                    Directory.Delete(srcPath);
                }

            }
            catch (Exception) // 异常处理
            {
                
            }
        }
    }
}
