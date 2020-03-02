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
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
        }

        public void SetAlert(FallPoint ideaPoint, FallPoint fallPoint, double fallTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("建议启动自毁\n");
            if(fallPoint.x < ideaPoint.x - Config.GetInstance().sideLine)
            {
                sb.AppendFormat("当前X超出左侧侧向必炸线{0:F}m\n", Math.Abs(fallPoint.x - (ideaPoint.x - Config.GetInstance().sideLine)));
            }
            if(fallPoint.x > ideaPoint.x + Config.GetInstance().sideLine)
            {
                sb.AppendFormat("当前X超出右侧侧向必炸线{0:F}m\n", Math.Abs(fallPoint.x - (ideaPoint.x + Config.GetInstance().sideLine)));
            }
            if(fallPoint.y < ideaPoint.y - Config.GetInstance().backwardLine)
            {
                sb.AppendFormat("当前Y超出后向必炸线{0:F}m\n", Math.Abs(fallPoint.y - (ideaPoint.y - Config.GetInstance().backwardLine)));
            }
            if(fallPoint.y > ideaPoint.y + Config.GetInstance().forwardLine)
            {
                sb.AppendFormat("当前Y超出前向必炸线{0:F}m\n", Math.Abs(fallPoint.y - (ideaPoint.y + Config.GetInstance().forwardLine)));
            }
            sb.AppendFormat("剩余落地时间:{0:F}s",fallTime);
            AlertLabel.Text = sb.ToString();
        }
    }
}
