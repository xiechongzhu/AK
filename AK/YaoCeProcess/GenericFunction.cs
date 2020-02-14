using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
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
            }
            else if (ctls.HasChildren)
            {
                foreach (Control ctl in ctls.Controls)
                {
                    txtClear(ctl);
                }
            }
        }
    }
}
