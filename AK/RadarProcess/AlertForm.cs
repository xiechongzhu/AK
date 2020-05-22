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

        public void SetMode(int suit, int mode)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (mode == 0)
            {
                Location = new Point((screenWidth - Width) / 2, (screenHeight - Height) / 2);
            }
            else
            {
                if(suit == 1)
                {
                    Location = new Point((screenWidth - Width) / 2, screenHeight / 2- Height - 10);
                }
                else
                {
                    Location = new Point((screenWidth - Width) / 2, screenHeight / 2 + 10);
                }
            }
        }

        public void SetAlert(FallPoint fallPoint, double fallTime, int suit)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("建议启动自毁\n");
            if(fallPoint.x < -Config.GetInstance().sideLine)
            {
                sb.AppendFormat("[套{0}]当前X超出左侧侧向必炸线{1:F}m\n", suit, Math.Abs(fallPoint.x + Config.GetInstance().sideLine));
            }
            if(fallPoint.x > + Config.GetInstance().sideLine)
            {
                sb.AppendFormat("[套{0}]当前X超出右侧侧向必炸线{1:F}m\n", suit, Math.Abs(fallPoint.x - Config.GetInstance().sideLine));
            }
            if(fallPoint.y < Config.GetInstance().backwardLine)
            {
                sb.AppendFormat("[套{0}]当前Y超出后向必炸线{1:F}m\n", suit, Math.Abs(Config.GetInstance().backwardLine - fallPoint.y));
            }
            if(fallPoint.y >  Config.GetInstance().forwardLine)
            {
                sb.AppendFormat("[套{0}]当前Y超出前向必炸线{1:F}m\n", suit, Math.Abs(fallPoint.y - Config.GetInstance().forwardLine));
            }
            sb.AppendFormat("[套{0}]剩余落地时间:{1:F}s", suit, fallTime);
            AlertLabel.Text = sb.ToString();
        }
    }
}
