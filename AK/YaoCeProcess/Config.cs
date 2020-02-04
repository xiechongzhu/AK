using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YaoCeProcess
{
    public class Config
    {
        private static String configFile = "./Config.xml";
        private Config() { }
        private static Config __instance = new Config();
        public static Config GetInstance()
        {
            return __instance;
        }

        public String strMultiCastIpAddr;       //组播地址
        public UInt16 port;                     //组播端口

        public bool LoadConfigFile(out String errMsg)
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(configFile);
                XmlSerializer reader = new XmlSerializer(typeof(Config));
                Config __config = (Config)reader.Deserialize(file);
                __instance = __config;
                file.Close();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                file?.Close();
                return false;
            }
            errMsg = String.Empty;
            return true;
        }

        public bool SaveConfig(out String errMsg)
        {
            FileStream file = File.Create(configFile);
            try
            {
                XmlSerializer wirter = new XmlSerializer(typeof(Config));
                wirter.Serialize(file, __instance);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                file.Close();
                return false;
            }
            file.Close();
            errMsg = String.Empty;
            return true;
        }
    }
}
