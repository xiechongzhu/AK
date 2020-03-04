using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 43)]
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
    }

    [Serializable]
    public class FallPoint
    {
        public double x;
        public double y;
    }
}
