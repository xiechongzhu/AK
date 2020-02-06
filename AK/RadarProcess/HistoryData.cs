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
    }
}
