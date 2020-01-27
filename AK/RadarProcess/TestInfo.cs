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
            strLogFile = strLogFolder + @"\" + "Log.txt";
            strDataFile = strLogFolder + @"\" + "Data.txt";
            Directory.CreateDirectory(strLogFolder);
        }

        public DateTime dateTime;
        public String strLogFolder;
        public String strLogFile;
        public String strDataFile;
        public String strTestName;
        public String strOperator;
        public String strComment;
    }
}
