using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RadarProcess
{
    [Serializable]
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
        public double flightshot;           //理论射程
        public double forwardLine;          //预示落点射程前向必炸线
        public double backwardLine;         //当前点发射系位置x后向必炸线
        public double sideLine;             //侧向必炸线
        public List<MinMaxValue> minMaxValues;
        public String strRadarMultiCastIpAddr;       //雷测组播地址
        public UInt16 radarPort;                     //雷测组播端口
        public String strTelemetryMultiCastIpAddr;  //遥测组播地址
        public UInt16 telemetryPort;                //遥测组播端口
        public int stationId;                   //雷达站编号
        public bool manualT0;
        public int delayT0;

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

    [Serializable]
    public class MinMaxValue
    {
        public int Time;
        public double MinX;
        public double MaxX;
        public double MinY;
        public double MaxY;
        public double MinZ;
        public double MaxZ;
        public double MinVx;
        public double MaxVx;
        public double MinVy;
        public double MaxVy;
        public double MinVz;
        public double MaxVz;
    }
}
