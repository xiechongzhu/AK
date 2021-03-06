﻿// 
// 
// 
using DevExpress.XtraEditors; //
// 
// 
// 
using DevExpress.XtraTab; //
// 
// 
// 
using System; //
// 
// 
// 
using System.Collections.Generic; //
// 
// 
// 
using System.Drawing; //
// 
// 
// 
using System.Linq; //
// 
// 
// 
using System.Text; //
// 
// 
// 
using System.Threading.Tasks; //
// 
// 
// 
using System.Windows.Forms; //
// 
// 
// 

// 
// 
// 
/// <summary>
// 
// 
// 
/// YaoCeProcess
// 
// 
// 
/// </summary>
// 
// 
// 
namespace YaoCeProcess
// 
// 
// 
{
// 
// 
// 
    /// <summary>
// 
// 
// 
    /// 文件名:GenericFunction/
// 
// 
// 
    /// 文件功能描述:工具类函数接口/
// 
// 
// 
    /// 创建人:yangy
// 
// 
// 
    /// 版权所有:Copyright (C) ZGM/
// 
// 
// 
    /// 创建标识:2020.03.12/     
// 
// 
// 
    /// 修改描述:/
// 
// 
// 
    /// </summary>
// 
// 
// 
    public class GenericFunction
// 
// 
// 
    {
// 
// 
// 
        /// <summary>
// 
// 
// 
        /// reSetAllTextEdit
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="form"></param>
// 
// 
// 
        public static void reSetAllTextEdit(Form form)
// 
// 
// 
        {
// 
// 
// 
            // 初始化清空显示数据
// 
// 
// 
            foreach (Control control in form.Controls)
// 
// 
// 
            {
// 
// 
// 
                txtClear(control); //
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 

// 
// 
// 
        /// <summary>
// 
// 
// 
        /// reSetAllTextEdit
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="page"></param>
// 
// 
// 
        public static void reSetAllTextEdit(XtraTabPage page)
// 
// 
// 
        {
// 
// 
// 
            // 初始化清空显示数据
// 
// 
// 
            foreach (Control control in page.Controls)
// 
// 
// 
            {
// 
// 
// 
                txtClear(control); //
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 

// 
// 
// 
        /// <summary>
// 
// 
// 
        /// txtClear
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="ctls"></param>
// 
// 
// 
        public static void txtClear(Control ctls)
// 
// 
// 
        {
// 
// 
// 
            if (ctls is TextEdit)
// 
// 
// 
            {
// 
// 
// 
                TextEdit t = (TextEdit)ctls; //
// 
// 
// 
                t.Text = "— —"; //
// 
// 
// 
                t.BackColor = Color.FromArgb(30, 255, 255, 255); // // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
// 
// 
// 
            }
// 
// 
// 
            else if (ctls.HasChildren)
// 
// 
// 
            {
// 
// 
// 
                foreach (Control ctl in ctls.Controls)
// 
// 
// 
                {
// 
// 
// 
                    txtClear(ctl); //
// 
// 
// 
                }
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 

// 
// 
// 
        /// <summary>
// 
// 
// 
        /// changeAllTextEditColor
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="form"></param>
// 
// 
// 
        public static void changeAllTextEditColor(Form form)
// 
// 
// 
        {
// 
// 
// 
            // 更改控件背景颜色
// 
// 
// 
            foreach (Control control in form.Controls)
// 
// 
// 
            {
// 
// 
// 
                textEditColorChange(control); //
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 

// 
// 
// 
        /// <summary>
// 
// 
// 
        /// changeAllTextEditColor
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="page"></param>
// 
// 
// 
        public static void changeAllTextEditColor(XtraTabPage page)
// 
// 
// 
        {
// 
// 
// 
            // 更改控件背景颜色
// 
// 
// 
            foreach (Control control in page.Controls)
// 
// 
// 
            {
// 
// 
// 
                textEditColorChange(control); //
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 

// 
// 
// 
        /// <summary>
// 
// 
// 
        /// textEditColorChange
// 
// 
// 
        /// </summary>
// 
// 
// 
        /// <param name="ctls"></param>
// 
// 
// 
        public static void textEditColorChange(Control ctls)
// 
// 
// 
        {
// 
// 
// 
            if (ctls is TextEdit)
// 
// 
// 
            {
// 
// 
// 
                TextEdit t = (TextEdit)ctls; //
// 
// 
// 
                if (t.Text.Contains("正常") || t.Text.Contains("有效"))
// 
// 
// 
                {
// 
// 
// 
                    t.BackColor = Color.FromArgb(30, 0, 255, 0); //     // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
// 
// 
// 
                }
// 
// 
// 
                else if (t.Text.Contains("异常") || t.Text.Contains("无效"))
// 
// 
// 
                {
// 
// 
// 
                    t.BackColor = Color.FromArgb(30, 255, 0, 0); //     // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
// 
// 
// 
                }
// 
// 
// 
                else
// 
// 
// 
                {
// 
// 
// 
                    t.BackColor = Color.FromArgb(30, 255, 255, 255); // // 第1个参数为透明度(alpha)参数,其后为红,绿和蓝
// 
// 
// 
                }
// 
// 
// 
            }
// 
// 
// 
            else if (ctls.HasChildren)
// 
// 
// 
            {
// 
// 
// 
                foreach (Control ctl in ctls.Controls)
// 
// 
// 
                {
// 
// 
// 
                    textEditColorChange(ctl); //
// 
// 
// 
                }
// 
// 
// 
            }
// 
// 
// 
        }
// 
// 
// 
    }
// 
// 
// 
}
// 
// 
// 
