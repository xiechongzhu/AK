using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YaoCeProcess
{
    public class Logger
    {
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";

        public enum LOG_LEVEL
        {
            LOG_INFO,
            LOG_WARN,
            LOG_ERROR
        }

        protected StreamWriter logWriter;

        private Logger()
        {
            // 创建日志文件夹
            Directory.CreateDirectory(strLogDir);
        }

        public void NewFile()
        {
            logWriter?.Close();

            // 生成日志文件
            dateTime = DateTime.Now;
            String strDate = dateTime.ToString("yyyy_MM_dd_HH_mm_ss");
            strLogFile = strLogDir + @"\" + strDate + @"_Log.txt";
            logWriter = new StreamWriter(strLogFile);
        }

        public void closeFile()
        {
            logWriter?.Close();
        }

        private MainForm mainForm;
        private static Logger __instance = new Logger();
        public DateTime dateTime;
        public String strLogFile;

        public static Logger GetInstance()
        {
            return __instance;
        }

        public void SetMainForm(MainForm _mainForm)
        {
            mainForm = _mainForm;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(LOG_LEVEL level, String msg)
        {
            DateTime dateTime = DateTime.Now;
            mainForm.Log(dateTime, level, msg);
            String strLevel;
            switch (level)
            {
                case LOG_LEVEL.LOG_INFO:
                    strLevel = "信息";
                    break;
                case LOG_LEVEL.LOG_ERROR:
                    strLevel = "错误";
                    break;
                default:
                    return;
            }
            String strLog = String.Format("[{0}][{1}]{2}", dateTime.ToString("G"), strLevel, msg);
            logWriter.WriteLine(strLog);
            logWriter.Flush();
        }
    }
}
