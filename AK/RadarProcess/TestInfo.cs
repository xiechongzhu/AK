/******************************************************************* 
* @brief : 测试数据 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.IO;

/// <summary>
/// namespace RadarProcess
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// class TestInfo
    /// </summary>
    public class TestInfo
    {
        /// <summary>
        /// strLogDir
        /// </summary>
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";

        /// <summary>
        /// 构造函数
        /// </summary>
        private TestInfo() { }

        /// <summary>
        /// 单例
        /// </summary>
        private static TestInfo __instance = new TestInfo();

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static TestInfo GetInstance()
        {
            return __instance;  //返回单例
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        public void New()
        {
            dateTime = DateTime.Now;                                 //获取当前时间
            String strDate = dateTime.ToString("yyyyMMddHHmmss");    //格式化时间
            strLogFolder = strLogDir + @"\" + strDate;               //日志目录
            strLogFile = strLogFolder + @"\Log.txt";                 //日志文件名称
            strRaderDataFile = strLogFolder + @"\雷测数据.bin";      //雷测数据
            strTelDataFile = strLogFolder + @"\遥测数据.bin";        //遥测数据
            strHistoryFile = strLogFolder + @"\History.dat";         //History
            Directory.CreateDirectory(strLogFolder);                 //创建目录
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime dateTime;
        /// <summary>
        /// 
        /// </summary>
        public String strLogFolder;
        /// <summary>
        /// 
        /// </summary>
        public String strLogFile;
        /// <summary>
        /// 
        /// </summary>
        public String strRaderDataFile;
        /// <summary>
        /// 
        /// </summary>
        public String strTelDataFile;
        /// <summary>
        /// 
        /// </summary>
        public String strHistoryFile;
        /// <summary>
        /// 
        /// </summary>
        public String strTestName;
        /// <summary>
        /// 
        /// </summary>
        public String strOperator;
        /// <summary>
        /// 
        /// </summary>
        public String strComment;
    }
}
