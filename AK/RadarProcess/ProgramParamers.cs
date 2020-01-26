using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    public class ProgramParamers
    {
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";

        private ProgramParamers() { }

        private static ProgramParamers __instance = new ProgramParamers();

        public static ProgramParamers GetInstance()
        {
            return __instance;
        }

        public void New()
        {
            String strDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            String strCurrentLogDir = strLogDir + @"\" + strDate;
            strLogFile = strCurrentLogDir + @"\" + "Log.txt";
            strDataFile = strCurrentLogDir + @"\" + "Data.txt";
            Directory.CreateDirectory(strCurrentLogDir);
        }

        public String strLogFile;
        public String strDataFile;
    }
}
