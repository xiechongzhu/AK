using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    [Serializable]
    public class HistoryData
    {
        private List<S_OBJECT> listSObject = new List<S_OBJECT>();
        private FallPoint ideaFallPoint = new FallPoint();
        private FallPoint fallPoint = new FallPoint();
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
        private int maxPointCount;

        public void Clear()
        {
            listSObject.Clear();
        }

        public void AddObject(S_OBJECT sObject)
        {
            listSObject.Add(sObject);
        }

        public void AddFallPoint(FallPoint fallPoint)
        {
            this.fallPoint = fallPoint;
        }

        public List<S_OBJECT> Objects
        {
            get { return listSObject; }
        }

        public FallPoint FallPoint
        {
            get { return fallPoint; }
        }

        public FallPoint IdeaFallPoint
        {
            get { return ideaFallPoint; }
            set { ideaFallPoint = value; }
        }

        public double ForwardLine { get => forwardLine; set => forwardLine = value; }
        public double BackwardLine { get => backwardLine; set => backwardLine = value; }
        public double SideLine { get => sideLine; set => sideLine = value; }
        public double LongitudeInit { get => longitudeInit; set => longitudeInit = value; }
        public double LatitudeInit { get => latitudeInit; set => latitudeInit = value; }
        public double HeightInit { get => heightInit; set => heightInit = value; }
        public double AzimuthInit { get => azimuthInit; set => azimuthInit = value; }
        public double PlacementHeight { get => placementHeight; set => placementHeight = value; }
        public double Flightshot { get => flightshot; set => flightshot = value; }
        public string StrMultiCastIpAddr { get => strMultiCastIpAddr; set => strMultiCastIpAddr = value; }
        public ushort Port { get => port; set => port = value; }
        public String StationId { get => stationId; set => stationId = value; }
        public double SpeedError { get => speedError; set => speedError = value; }
        public double PointError { get => pointError; set => pointError = value; }
        public int MaxPointCount { get => maxPointCount; set => maxPointCount = value; }
    }
}
