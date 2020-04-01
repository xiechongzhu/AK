using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    public class TestInfo
    {
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";

        private TestInfo() { }

        private static TestInfo __instance = new TestInfo();

        public static TestInfo GetInstance()
        {
            return __instance;
        }

        public void New()
        {
            dateTime = DateTime.Now;
            String strDate = dateTime.ToString("yyyyMMddHHmmss");
            strLogFolder = strLogDir + @"\" + strDate;
            strLogFile = strLogFolder + @"\Log.txt";
            strRaderDataFile = strLogFolder + @"\雷测数据.bin";
            strTelDataFile = strLogFolder + @"\遥测数据.bin";
            strHistoryFile = strLogFolder + @"\History.dat";
            Directory.CreateDirectory(strLogFolder);
        }

        public DateTime dateTime;
        public String strLogFolder;
        public String strLogFile;
        public String strRaderDataFile;
        public String strTelDataFile;
        public String strHistoryFile;
        public String strTestName;
        public String strOperator;
        public String strComment;
    }
}
