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
        private List<FallPoint> listFallPoints = new List<FallPoint>();
        private double locMaxX, locMinX, locMaxY, locMinY, locMaxZ, locMinZ;
        private double speedMaxX, speedMinX, speedMaxY, speedMinY, speedMaxZ, speedMinZ;
        private double forwardLine, backwardLine, sideLine;

        public void Clear()
        {
            listSObject.Clear();
            listFallPoints.Clear();
        }

        public void AddObject(S_OBJECT sObject)
        {
            listSObject.Add(sObject);
        }

        public void AddFallPoint(FallPoint fallPoint)
        {
            if(listFallPoints.Count == 5)
            {
                listFallPoints.RemoveAt(0);
            }
            listFallPoints.Add(fallPoint);
        }

        public List<S_OBJECT> Objects
        {
            get { return listSObject; }
        }

        public List<FallPoint> FallPoints
        {
            get { return listFallPoints; }
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
    }
}
