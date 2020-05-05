using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YaoCeProcess
{
    public class GenericFunction
    {
        public static void reSetAllTextEdit(Form form)
        {
            // 初始化清空显示数据
            foreach (Control control in form.Controls)
            {
                txtClear(control);
            }
        }

        public static void reSetAllTextEdit(XtraTabPage page)
        {
            // 初始化清空显示数据
            foreach (Control control in page.Controls)
            {
                txtClear(control);
            }
        }

        public static void txtClear(Control ctls)
        {
            if (ctls is TextEdit)
            {
                TextEdit t = (TextEdit)ctls;
                t.Text = "— —";
                t.BackColor = Color.FromArgb(30, 255, 255, 255); // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
            }
            else if (ctls.HasChildren)
            {
                foreach (Control ctl in ctls.Controls)
                {
                    txtClear(ctl);
                }
            }
        }

        public static void changeAllTextEditColor(Form form)
        {
            // 更改控件背景颜色
            foreach (Control control in form.Controls)
            {
                textEditColorChange(control);
            }
        }

        public static void changeAllTextEditColor(XtraTabPage page)
        {
            // 更改控件背景颜色
            foreach (Control control in page.Controls)
            {
                textEditColorChange(control);
            }
        }

        public static void textEditColorChange(Control ctls)
        {
            if (ctls is TextEdit)
            {
                TextEdit t = (TextEdit)ctls;
                if (t.Text.Contains("正常") || t.Text.Contains("有效"))
                {
                    t.BackColor = Color.FromArgb(30, 0, 255, 0);     // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
                }
                else if (t.Text.Contains("异常") || t.Text.Contains("无效"))
                {
                    t.BackColor = Color.FromArgb(30, 255, 0, 0);     // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
                }
                else
                {
                    t.BackColor = Color.FromArgb(30, 255, 255, 255); // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
                }
            }
            else if (ctls.HasChildren)
            {
                foreach (Control ctl in ctls.Controls)
                {
                    textEditColorChange(ctl);
                }
            }
        }
    }
}
