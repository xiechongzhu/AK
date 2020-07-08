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
/// namespace RadarProcess
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// YcMessage
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct YcMessage
    {
        public S_OBJECT sObject;    //S_OBJECT
        public FallPoint fallPoint; //落点
        public double fallTime;     //剩余飞行时间
        public double distance;     //距离
    }
}