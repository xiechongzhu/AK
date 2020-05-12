using System;
using System.Runtime.InteropServices;
using DevExpress.Xpo;

namespace RadarProcess
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct YcMessage
    {
        public S_OBJECT sObject;
        public FallPoint fallPoint;
        public double fallTime;
        public double distance;
    }
}