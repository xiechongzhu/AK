// 
using System; //
// 
using System.Collections.Generic; //
// 
using System.Linq; //
// 
using System.Threading.Tasks; //
// 
using System.Windows.Forms; //
// 

// 
/// <summary>
// 
/// YaoCeProcess
// 
/// </summary>
// 
namespace YaoCeProcess
// 
{
// 
    /// <summary>
// 
    /// 文件名:Program/
// 
    /// 文件功能描述:应用程序的主入口点/
// 
    /// 创建人:yangy
// 
    /// 版权所有:Copyright (C) ZGM/
// 
    /// 创建标识:2020.03.12/     
// 
    /// 修改描述:/
// 
    /// </summary>
// 
    static class Program
// 
    {
// 
        /// <summary>
// 
        /// 应用程序的主入口点。
// 
        /// </summary>
// 
        [STAThread]
// 
        static void Main()
// 
        {
// 
            Application.EnableVisualStyles(); //
// 
            Application.SetCompatibleTextRenderingDefault(false); //
// 
            Application.Run(new MainForm()); //
// 
        }
// 
    }
// 
}
// 
