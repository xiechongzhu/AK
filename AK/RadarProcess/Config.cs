/******************************************************************* 
* @brief : 配置类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RadarProcess
{
    /// <summary>
    /// 配置类
    /// </summary>
    [Serializable]
    public class Config
    {
        //配置文件
        private static readonly String configFile = "./Config.xml";
        /// <summary>
        /// 构造函数
        /// </summary>
        private Config() { }
        //单例
        private static Config __instance = new Config();
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static Config GetInstance()
        {
            return __instance;  //返回单例
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
        public String stationId;                   //雷达站编号
        public int delayT0;                     //T0延迟
        public double speedError;               //速度误差
        public double pointError;               //落点误差
        public int maxPointCount;               //最大显示点数
        public int source;                      //数据源

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool LoadConfigFile(out String errMsg)
        {
            StreamReader file = null;   //读流
            try
            {
                file = new StreamReader(configFile);    //创建流
                XmlSerializer reader = new XmlSerializer(typeof(Config));
                Config __config = (Config)reader.Deserialize(file); //反序列化
                __instance = __config;
                file.Close(); //关闭文件
            }
            catch (Exception ex)    //异常
            {
                errMsg = ex.Message;    //异常消息
                file?.Close(); //关闭文件
                return false;
            }
            errMsg = String.Empty;
            return true;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
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
                //关闭文件
                errMsg = ex.Message;
                file.Close();
                return false;
            }
            //关闭文件
            file.Close();
            errMsg = String.Empty;
            return true;
        }
    }

    /// <summary>
    /// 上下限
    /// </summary>
    [Serializable]
    public class MinMaxValue
    {
        /// <summary>
        /// 时间
        /// </summary>
        public int Time;
        /// <summary>
        /// X最小值
        /// </summary>
        public double MinX;
        /// <summary>
        /// X最大值
        /// </summary>
        public double MaxX;
        /// <summary>
        /// Y最小值
        /// </summary>
        public double MinY;
        /// <summary>
        /// Y最大值
        /// </summary>
        public double MaxY;
        /// <summary>
        /// Z最小值
        /// </summary>
        public double MinZ;
        /// <summary>
        /// Z最大值
        /// </summary>
        public double MaxZ;
        /// <summary>
        /// Vx最小值
        /// </summary>
        public double MinVx;
        /// <summary>
        /// Vx最大值
        /// </summary>
        public double MaxVx;
        /// <summary>
        /// Vy最小值
        /// </summary>
        public double MinVy;
        /// <summary>
        /// Vy最大值
        /// </summary>
        public double MaxVy;
        /// <summary>
        /// Vz最小值
        /// </summary>
        public double MinVz;
        /// <summary>
        /// Vz最大值
        /// </summary>
        public double MaxVz;
    }
}
