/******************************************************************* 
* @brief : 雷达数据处理类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Collections.Generic;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HistoryData
    {
        private List<S_OBJECT> listSObject = new List<S_OBJECT>(); //数据链表
        private FallPoint ideaFallPoint = new FallPoint(); //理想落点
        private FallPoint fallPoint = new FallPoint(); //预测落点
        private double longitudeInit;        //初始位置经度
        private double latitudeInit;         //初始位置纬度
        private double heightInit;           //初始位置高度
        private double azimuthInit;          //初始位置方位角
        private double placementHeight;      //落点附近高度
        private double flightshot;           //理论射程
        private double forwardLine;          //预示落点射程前向必炸线
        private double backwardLine;         //当前点发射系位置x后向必炸线
        private double sideLine;             //侧向必炸线
        private String strMultiCastIpAddr;       //组播地址
        private UInt16 port;                     //组播端口
        private String stationId;                  //雷达站编号
        private double speedError;              //速度误差
        private double pointError;              //落点误差
        private int maxPointCount;              //最大显示点数

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            listSObject.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sObject"></param>
        public void AddObject(S_OBJECT sObject)
        {
            listSObject.Add(sObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fallPoint"></param>
        public void AddFallPoint(FallPoint fallPoint)
        {
            this.fallPoint = fallPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<S_OBJECT> Objects
        {
            get { return listSObject; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FallPoint FallPoint
        {
            get { return fallPoint; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FallPoint IdeaFallPoint
        {
            get { return ideaFallPoint; }
            set { ideaFallPoint = value; }
        }
        /// <summary>
        /// ForwardLine
        /// </summary>
        public double ForwardLine { get => forwardLine; set => forwardLine = value; }
        /// <summary>
        /// BackwardLine
        /// </summary>
        public double BackwardLine { get => backwardLine; set => backwardLine = value; }
        /// <summary>
        /// SideLine
        /// </summary>
        public double SideLine { get => sideLine; set => sideLine = value; }
        /// <summary>
        /// LongitudeInit
        /// </summary>
        public double LongitudeInit { get => longitudeInit; set => longitudeInit = value; }
        /// <summary>
        /// LatitudeInit
        /// </summary>
        public double LatitudeInit { get => latitudeInit; set => latitudeInit = value; }
        /// <summary>
        /// HeightInit
        /// </summary>
        public double HeightInit { get => heightInit; set => heightInit = value; }
        /// <summary>
        /// AzimuthInit
        /// </summary>
        public double AzimuthInit { get => azimuthInit; set => azimuthInit = value; }
        /// <summary>
        /// PlacementHeight
        /// </summary>
        public double PlacementHeight { get => placementHeight; set => placementHeight = value; }
        /// <summary>
        /// Flightshot
        /// </summary>
        public double Flightshot { get => flightshot; set => flightshot = value; }
        /// <summary>
        /// StrMultiCastIpAddr
        /// </summary>
        public string StrMultiCastIpAddr { get => strMultiCastIpAddr; set => strMultiCastIpAddr = value; }
        /// <summary>
        /// Port
        /// </summary>
        public ushort Port { get => port; set => port = value; }
        /// <summary>
        /// StationId
        /// </summary>
        public String StationId { get => stationId; set => stationId = value; }
        /// <summary>
        /// SpeedError
        /// </summary>
        public double SpeedError { get => speedError; set => speedError = value; }
        /// <summary>
        /// PointError
        /// </summary>
        public double PointError { get => pointError; set => pointError = value; }
        /// <summary>
        /// MaxPointCount
        /// </summary>
        public int MaxPointCount { get => maxPointCount; set => maxPointCount = value; }
    }
}
