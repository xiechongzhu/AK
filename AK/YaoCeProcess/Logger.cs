﻿// 
using System; //
// 
using System.Collections.Generic; //
// 
using System.IO; //
// 
using System.Linq; //
// 
using System.Runtime.CompilerServices; //
// 
using System.Text; //
// 
using System.Threading.Tasks; //
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
    /// 文件名:Logger/
// 
    /// 文件功能描述:日志记录/
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
    public class Logger
// 
    {
// 
        /// <summary>
// 
        /// strLogDir
// 
        /// </summary>
// 
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log"; //
// 

// 
        /// <summary>
// 
        /// LOG_LEVEL
// 
        /// </summary>
// 
        public enum LOG_LEVEL
// 
        {
// 
            LOG_INFO,
// 
            LOG_WARN,
// 
            LOG_ERROR
// 
        }
// 

// 
        /// <summary>
// 
        /// logWriter
// 
        /// </summary>
// 
        protected StreamWriter logWriter; //
// 

// 
        /// <summary>
// 
        /// Logger
// 
        /// </summary>
// 
        private Logger()
// 
        {
// 
            // 创建日志文件夹
// 
            Directory.CreateDirectory(strLogDir); //
// 
        }
// 

// 
        /// <summary>
// 
        /// NewFile
// 
        /// </summary>
// 
        public void NewFile()
// 
        {
// 
            logWriter?.Close(); //
// 

// 
            // 生成日志文件
// 
            dateTime = DateTime.Now; //
// 
            String strDate = dateTime.ToString("yyyy_MM_dd_HH_mm_ss"); //
// 
            strLogFile = strLogDir + @"\" + strDate + @"_Log.txt"; //
// 
            logWriter = new StreamWriter(strLogFile); //
// 
        }
// 

// 
        /// <summary>
// 
        /// closeFile
// 
        /// </summary>
// 
        public void closeFile()
// 
        {
// 
            logWriter?.Close(); //
// 
        }
// 

// 
        /// <summary>
// 
        /// mainForm
// 
        /// </summary>
// 
        private MainForm mainForm; //
// 

// 
        /// <summary>
// 
        /// __instance
// 
        /// </summary>
// 
        private static Logger __instance = new Logger(); //
// 

// 
        /// <summary>
// 
        /// dateTime
// 
        /// </summary>
// 
        public DateTime dateTime; //
// 

// 
        /// <summary>
// 
        /// strLogFile
// 
        /// </summary>
// 
        public String strLogFile; //
// 

// 
        /// <summary>
// 
        /// GetInstance
// 
        /// </summary>
// 
        /// <returns></returns>
// 
        public static Logger GetInstance()
// 
        {
// 
            return __instance; //
// 
        }
// 

// 
        /// <summary>
// 
        /// SetMainForm
// 
        /// </summary>
// 
        /// <param name="_mainForm"></param>
// 
        public void SetMainForm(MainForm _mainForm)
// 
        {
// 
            mainForm = _mainForm; //
// 
        }
// 

// 
        /// <summary>
// 
        /// Log
// 
        /// </summary>
// 
        /// <param name="level"></param>
// 
        /// <param name="msg"></param>
// 
        [MethodImpl(MethodImplOptions.Synchronized)]
// 
        public void Log(LOG_LEVEL level, String msg)
// 
        {
// 
            DateTime dateTime = DateTime.Now; //
// 
            mainForm.Log(dateTime, level, msg); //
// 
            String strLevel; //
// 
            switch (level)
// 
            {
// 
                case LOG_LEVEL.LOG_INFO:
// 
                    strLevel = "信息"; //
// 
                    break; //
// 
                case LOG_LEVEL.LOG_ERROR:
// 
                    strLevel = "错误"; //
// 
                    break; //
// 
                default:
// 
                    return; //
// 
            }
// 
            String strLog = String.Format("[{0}][{1}]{2}", dateTime.ToString("G"), strLevel, msg); //
// 
            logWriter.WriteLine(strLog); //
// 
            logWriter.Flush(); //
// 
        }
// 
    }
// 
}
// 
