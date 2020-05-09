using System;
using System.Runtime.InteropServices;

namespace RadarProcess
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACK_HEAD
    {
        public byte Station;
        public byte Type;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S_HEAD
    {
        public UInt16  Len;
        public Int32   Time;
        public byte    SrcId;
        public byte    SrcType;
        public byte    CS;
        public byte    CT;
        public byte    FF;
        public byte    Num;
        public byte    C;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] S;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S_OBJECT
    {
        public int time;
        public UInt16 ObjectId;
        public double A;
        public double E;
        public double R;
        public double v;
        public double X;
        public double Y;
        public double Z;
        public double VX;
        public double VY;
        public double VZ;
        public byte BD;
        public byte SS;
        public byte VF;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserve;
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
        public int suit;
    }

    [Serializable]
    public class FallPoint
    {
        public double x;
        public double y;
    }
}
