using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    public class Logger
    {
        public enum LOG_LEVEL
        {
            LOG_INFO,
            LOG_WARN,
            LOG_ERROR,
            LOG_SELF_DESTRUCT
        }

        protected StreamWriter logWriter;

        private Logger()
        {
        }

        public void NewFile()
        {
            logWriter?.Close();
            logWriter = new StreamWriter(TestInfo.GetInstance().strLogFile);
        }

        public void Close()
        {
            logWriter?.Close();
        }

        private MainForm mainForm;
        private static Logger __instance = new Logger();
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
                case LOG_LEVEL.LOG_WARN:
                    strLevel = "告警";
                    break;
                case LOG_LEVEL.LOG_ERROR:
                    strLevel = "错误";
                    break;
                case LOG_LEVEL.LOG_SELF_DESTRUCT:
                    strLevel = "自毁";
                    break;
                default:
                    return;
            }
            String strLog = String.Format("[{0}][{1}]{2}", dateTime.ToString("G"), strLevel, msg);
            try
            {
                logWriter.WriteLine(strLog);
                logWriter.Flush();
            }
            catch (Exception)
            { }
        }
    }
}
