// 
using System; //
// 
using System.Collections.Generic; //
// 
using System.IO; //
// 
using System.Linq; //
// 
using System.Text; //
// 
using System.Threading.Tasks; //
// 
using System.Xml.Serialization; //
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
    /// 文件名:Config/
// 
    /// 文件功能描述:参数配置/
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
    public class Config
// 
    {
// 
        /// <summary>
// 
        /// configFile
// 
        /// </summary>
// 
        private static String configFile = "./Config.xml"; //
// 

// 
        /// <summary>
// 
        /// Config
// 
        /// </summary>
// 
        private Config() { }
// 

// 
        /// <summary>
// 
        /// __instance
// 
        /// </summary>
// 
        private static Config __instance = new Config(); //
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
        public static Config GetInstance()
// 
        {
// 
            return __instance; //
// 
        }
// 

// 
        public String strMultiCastIpAddr; //       //组播地址
// 
        public UInt16 port; //                     //组播端口
// 

// 
        /// <summary>
// 
        /// LoadConfigFile
// 
        /// </summary>
// 
        /// <param name="errMsg"></param>
// 
        /// <returns></returns>
// 
        public bool LoadConfigFile(out String errMsg)
// 
        {
// 
            StreamReader file = null; //
// 
            try
// 
            {
// 
                file = new StreamReader(configFile); //
// 
                XmlSerializer reader = new XmlSerializer(typeof(Config)); //
// 
                Config __config = (Config)reader.Deserialize(file); //
// 
                __instance = __config; //
// 
                file.Close(); //
// 
            }
// 
            catch (Exception ex)
// 
            {
// 
                errMsg = ex.Message; //
// 
                file?.Close(); //
// 
                return false; //
// 
            }
// 
            errMsg = String.Empty; //
// 
            return true; //
// 
        }
// 

// 
        /// <summary>
// 
        /// SaveConfig
// 
        /// </summary>
// 
        /// <param name="errMsg"></param>
// 
        /// <returns></returns>
// 
        public bool SaveConfig(out String errMsg)
// 
        {
// 
            FileStream file = File.Create(configFile); //
// 
            try
// 
            {
// 
                XmlSerializer wirter = new XmlSerializer(typeof(Config)); //
// 
                wirter.Serialize(file, __instance); //
// 
            }
// 
            catch (Exception ex)
// 
            {
// 
                errMsg = ex.Message; //
// 
                file.Close(); //
// 
                return false; //
// 
            }
// 
            file.Close(); //
// 
            errMsg = String.Empty; //
// 
            return true; //
// 
        }
// 

// 
        //---------------------------------------------------------------//
// 

// 
        /// <summary>
// 
        /// SaveConfigComment
// 
        /// </summary>
// 
        /// <param name="errMsg"></param>
// 
        /// <returns></returns>
// 
        /*
// 
        public bool SaveConfigComment(out String errMsg)
// 
        {
// 
            FileStream file = File.Create(configFile); //
// 
            try
// 
            {
// 
                XmlSerializer wirter = new XmlSerializer(typeof(Config)); //
// 
                wirter.Serialize(file, __instance); //
// 
            }
// 
            catch (Exception ex)
// 
            {
// 
                errMsg = ex.Message; //
// 
                file.Close(); //
// 
                return false; //
// 
            }
// 
            file.Close(); //
// 
            errMsg = String.Empty; //
// 
            return true; //
// 
        }
// 
        */
// 
    }
// 
}
// 
