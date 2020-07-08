/******************************************************************* 
* @brief : 协议 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Runtime.InteropServices;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// PACK_HEAD
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PACK_HEAD
    {
        public byte Station; //站点
        public byte Type; //类型
    }

    /// <summary>
    /// S_HEAD
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S_HEAD
    {
        public UInt16  Len;         //长度
        public Int32   Time;        //时间
        public byte    SrcId;       //源
        public byte    SrcType;     //类型
        public byte    CS;          //CS
        public byte    CT;          //CT
        public byte    FF;          //FF
        public byte    Num;         //Num
        public byte    C;           //C
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] S;            //数据
    }

    /// <summary>
    /// S_OBJECT
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct S_OBJECT
    {
        public int time;            //时间
        public UInt16 ObjectId;     //Id
        public double A;            //A; 
        public double E;            //E; 
        public double R;            //R; 
        public double v;            //v; 
        public double X;            //X; 
        public double Y;            //Y; 
        public double Z;            //Z; 
        public double VX;           //VX;
        public double VY;           //VY;
        public double VZ;           //VZ;
        public byte BD;             //BD;
        public byte SS;             //SS;
        public byte VF;             //VF;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserve;      //保留数据
        public double MinX;         //MinX; 
        public double MaxX;         //MaxX; 
        public double MinY;         //MinY; 
        public double MaxY;         //MaxY; 
        public double MinZ;         //MinZ; 
        public double MaxZ;         //MaxZ; 
        public double MinVx;        //MinVx;
        public double MaxVx;        //MaxVx;
        public double MinVy;        //MinVy;
        public double MaxVy;        //MaxVy;
        public double MinVz;        //MinVz;
        public double MaxVz;        //MaxVz;
        public int suit;            //弹头弹体
    }

    /// <summary>
    /// FallPoint
    /// </summary>
    [Serializable]
    public class FallPoint
    {
        public double x; //侧偏
        public double y; //射程
    }
}
