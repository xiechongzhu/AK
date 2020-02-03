using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RadarProcess
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

        public double longitudeInit;        //初始位置经度
        public double latitudeInit;         //初始位置纬度
        public double heightInit;           //初始位置高度
        public double azimuthInit;          //初始位置方位角
        public double placementHeight;      //落点附近高度
        public double forwardLine;          //预示落点射程前向必炸线
        public double backwardLine;         //当前点发射系位置x后向必炸线
        public double sideLine;             //侧向必炸线
        public double locMaxX;              //位置x分量上限
        public double locMinX;              //位置x分量下限
        public double locMaxY;              //位置y分量上限
        public double locMinY;              //位置y分量下限
        public double locMaxZ;              //位置z分量上限
        public double locMinZ;              //位置z分量下限
        public double speedMaxX;              //速度x分量上限
        public double speedMinX;              //速度x分量下限
        public double speedMaxY;              //速度y分量上限
        public double speedMinY;              //速度y分量下限
        public double speedMaxZ;              //速度z分量上限
        public double speedMinZ;              //速度z分量下限
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
