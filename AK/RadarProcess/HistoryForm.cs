using System;
using System.Data;
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
            using(DataModels.DatabaseDB db = new DataModels.DatabaseDB())
            {
                var temp = from c in db.TestInfos select c;
                foreach(DataModels.TestInfo info in temp)
                {
                    dt.Rows.Add(info.TestName, info.Operator, info.Time.ToString("yyyy-MM-dd HH:mm:ss"), info.Comment, info.Id);
                }
            }

            gridControl.DataSource = dt;
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hInfo = gridView.CalcHitInfo(gridControl.PointToClient(MousePosition));
            if (hInfo.InRow)
            {
                int selectedRow = gridView.GetSelectedRows()[0];
                long id = (long)gridView.GetRowCellValue(selectedRow, "Id");
                TestReviewForm testReviewForm = new TestReviewForm(id);
                testReviewForm.ShowDialog();
            }
        }
    }
}
