/******************************************************************* 
* @brief : 日志类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.IO;
using System.Runtime.CompilerServices;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// LOG_LEVEL
        /// </summary>
        public enum LOG_LEVEL
        {
            LOG_INFO,          //信息
            LOG_WARN,          //告警
            LOG_ERROR,         //错误
            LOG_SELF_DESTRUCT  //自毁
        }

        /// <summary>
        /// logWriter
        /// </summary>
        protected StreamWriter logWriter;

        /// <summary>
        /// 构造函数
        /// </summary>
        private Logger()
        {
        }

        /// <summary>
        /// 生成新文件
        /// </summary>
        public void NewFile()
        {
            logWriter?.Close(); //关闭流
            logWriter = new StreamWriter(TestInfo.GetInstance().strLogFile); //新建流
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            logWriter?.Close(); //关闭流
        }

        /// <summary>
        /// 程序主窗口
        /// </summary>
        private MainForm mainForm;
        /// <summary>
        /// 单例
        /// </summary>
        private static Logger __instance = new Logger();
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static Logger GetInstance()
        {
            return __instance;  //返回单例
        }

        /// <summary>
        /// 设置主窗口
        /// </summary>
        /// <param name="_mainForm"></param>
        public void SetMainForm(MainForm _mainForm)
        {
            mainForm = _mainForm; //设置主窗口
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(LOG_LEVEL level, String msg)
        {
            DateTime dateTime = DateTime.Now; //获取当前时间
            mainForm.Log(dateTime, level, msg); //主窗口显示日志
            String strLevel;
            switch (level)
            {
                case LOG_LEVEL.LOG_INFO:    //信息
                    strLevel = "信息";
                    break;
                case LOG_LEVEL.LOG_WARN:    //告警
                    strLevel = "告警";
                    break;
                case LOG_LEVEL.LOG_ERROR:   //错误
                    strLevel = "错误";
                    break;
                case LOG_LEVEL.LOG_SELF_DESTRUCT:   //自毁
                    strLevel = "自毁";
                    break;
                default:
                    return;
            }
            //格式化
            String strLog = String.Format("[{0}][{1}]{2}", dateTime.ToString("G"), strLevel, msg);
            try
            {
                logWriter.WriteLine(strLog); //写日志
                logWriter.Flush(); //写入文件
            }
            catch (Exception) //异常
            { }
        }
    }
}
