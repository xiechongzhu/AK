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
        XmlDictionary<int, MinMaxValue> minMaxValues;
        private double locMaxX;              //位置x分量上限
        private double locMinX;              //位置x分量下限
        private double locMaxY;              //位置y分量上限
        private double locMinY;              //位置y分量下限
        private double locMaxZ;              //位置z分量上限
        private double locMinZ;              //位置z分量下限
        private double speedMaxX;              //速度x分量上限
        private double speedMinX;              //速度x分量下限
        private double speedMaxY;              //速度y分量上限
        private double speedMinY;              //速度y分量下限
        private double speedMaxZ;              //速度z分量上限
        private double speedMinZ;              //速度z分量下限
        private String strMultiCastIpAddr;       //组播地址
        private UInt16 port;                     //组播端口
        private int stationId;                  //雷达站编号

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

        public double LocMaxX { get => locMaxX; set => locMaxX = value; }
        public double LocMinX { get => locMinX; set => locMinX = value; }
        public double LocMaxY { get => locMaxY; set => locMaxY = value; }
        public double LocMinY { get => locMinY; set => locMinY = value; }
        public double LocMaxZ { get => locMaxZ; set => locMaxZ = value; }
        public double LocMinZ { get => locMinZ; set => locMinZ = value; }
        public double SpeedMaxX { get => speedMaxX; set => speedMaxX = value; }
        public double SpeedMinX { get => speedMinX; set => speedMinX = value; }
        public double SpeedMaxY { get => speedMaxY; set => speedMaxY = value; }
        public double SpeedMinY { get => speedMinY; set => speedMinY = value; }
        public double SpeedMaxZ { get => speedMaxZ; set => speedMaxZ = value; }
        public double SpeedMinZ { get => speedMinZ; set => speedMinZ = value; }
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
        public int StationId { get => stationId; set => stationId = value; }
        public XmlDictionary<int, MinMaxValue> MinMaxValues { get => minMaxValues; set => minMaxValues = value; }
    }
}
