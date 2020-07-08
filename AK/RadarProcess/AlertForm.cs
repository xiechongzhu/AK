/******************************************************************* 
* @brief : 告警窗口类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RadarProcess
{
    /// <summary>
    /// 告警窗口类实现
    /// </summary>
    public partial class AlertForm : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <returns></returns>
        public AlertForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置模式
        /// suit 套数
        /// mode 模式
        /// </summary>
        /// <returns>void</returns>
        public void SetMode(int suit, int mode)
        {
            //获取窗口宽高
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;    //宽度
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;  //高度

            //如果时雷达数据
            if (mode == 0)
            {
                Location = new Point((screenWidth - Width) / 2, (screenHeight - Height) / 2);
            }
            //遥测数据
            else
            {
                //弹头
                if(suit == 1)
                {
                    Location = new Point((screenWidth - Width) / 2, screenHeight / 2- Height - 10);
                }
                //弹体
                else
                {
                    Location = new Point((screenWidth - Width) / 2, screenHeight / 2 + 10);
                }
            }
        }

        /// <summary>
        /// 显示告警
        /// suit 套数
        /// mode 模式
        /// </summary>
        /// <returns>void</returns>
        public void SetAlert(FallPoint fallPoint, double fallTime, int suit)
        {
            //字符串构造
            StringBuilder sb = new StringBuilder();
            sb.Append("建议启动自毁\n");
            //超出左侧必炸线
            if(fallPoint.x < -Config.GetInstance().sideLine)
            {
                sb.AppendFormat("[{0}]当前X超出左侧侧向必炸线{1:F}m\n", suit == 1 ? "弹头" : "弹体", Math.Abs(fallPoint.x + Config.GetInstance().sideLine));
            }
            //超出右侧必炸线
            if(fallPoint.x > + Config.GetInstance().sideLine)
            {
                sb.AppendFormat("[{0}]当前X超出右侧侧向必炸线{1:F}m\n", suit == 1 ? "弹头" : "弹体", Math.Abs(fallPoint.x - Config.GetInstance().sideLine));
            }
            //超出后向必炸线
            if(fallPoint.y < Config.GetInstance().backwardLine)
            {
                sb.AppendFormat("[{0}]当前Y超出后向必炸线{1:F}m\n", suit == 1 ? "弹头" : "弹体", Math.Abs(Config.GetInstance().backwardLine - fallPoint.y));
            }
            //超出前向必炸线
            if(fallPoint.y >  Config.GetInstance().forwardLine)
            {
                sb.AppendFormat("[{0}]当前Y超出前向必炸线{1:F}m\n", suit == 1 ? "弹头" : "弹体", Math.Abs(fallPoint.y - Config.GetInstance().forwardLine));
            }
            sb.AppendFormat("[{0}]剩余落地时间:{1:F}s", suit == 1 ? "弹头" : "弹体", fallTime);
            //显示告警
            AlertLabel.Text = sb.ToString();
        }
    }
}
