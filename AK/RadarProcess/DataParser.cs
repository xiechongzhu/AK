/******************************************************************* 
* @brief : 雷达数据处理类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// namespace
/// </summary>
namespace RadarProcess
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    public class DataParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int Msg, int wParam, IntPtr lParam);
        /// <summary>
        /// 数据解析实例
        /// </summary>
        private YcDataParser YcDataParser;
        /// <summary>
        /// 是否开始获取T0
        /// </summary>
        private bool isStartGetT0;
        /// <summary>
        /// T0延迟
        /// </summary>
        private int T0Delay;
        /// <summary>
        /// T0时间
        /// </summary>
        private int T0;
        /// <summary>
        /// 遥测飞行时间
        /// </summary>
        private int telemetryFlyTime;
        /// <summary>
        /// 收到雷达T0时间
        /// </summary>
        private DateTime recvRaderT0;
        /// <summary>
        /// 收到遥测飞行时间
        /// </summary>
        private DateTime recvTelemetryFlyTime;
        /// <summary>
        /// 数据搜索位置
        /// </summary>
        private int pos;
        /// <summary>
        /// 打印日志标志
        /// </summary>
        private bool isPrintRaderLog = false;
        /// <summary>
        /// 弹头数据
        /// </summary>
        private List<S_OBJECT> sObjectListSuit1 = new List<S_OBJECT>();
        /// <summary>
        /// 弹体数据
        /// </summary>
        private List<S_OBJECT> sObjectListSuit2 = new List<S_OBJECT>();
        /// <summary>
        /// 主窗口句柄
        /// </summary>
        private IntPtr mainFormHandle;

        /// <summary>
        /// 数据源
        /// </summary>
        public enum DataSourceType
        { 
            DATA_RADER,
            DATA_TELEMETRY
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="mainForm"></param>
        public DataParser(MainForm mainForm)
        {
            this.mainForm = mainForm;
            mainFormHandle = mainForm.Handle;
            YcDataParser = new YcDataParser(this, mainForm);
        }

        /// <summary>
        /// 设置遥测飞行时间
        /// </summary>
        /// <param name="time"></param>
        public void SetYcFlyTime(int time)
        {
            //如果未设置，就设置
            if (telemetryFlyTime == -1 && time >= 0)
            {
                telemetryFlyTime = time;
                recvTelemetryFlyTime = DateTime.Now; //赋值为当前时间
                //打印日志
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到遥测总飞行时间：{0}", telemetryFlyTime));
            }
        }

        /// <summary>
        /// 主窗口
        /// </summary>
        private readonly MainForm mainForm;
        /// <summary>
        /// 数据队列
        /// </summary>
        private ConcurrentQueue<KeyValuePair<DataSourceType, byte[]>> queue = new ConcurrentQueue<KeyValuePair<DataSourceType, byte[]>>();
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool isRuning = false;
        /// <summary>
        /// 处理线程
        /// </summary>
        Thread thread;
        /// <summary>
        /// 数据入队列
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        public void Enqueue(DataSourceType dataType, byte[] data)
        {
            queue.Enqueue(new KeyValuePair<DataSourceType, byte[]>(dataType, data));
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            isPrintRaderLog = false;
            sObjectListSuit1.Clear();
            sObjectListSuit2.Clear();
            T0 = -1;
            telemetryFlyTime = -1;
            recvRaderT0 = recvTelemetryFlyTime = DateTime.MinValue;
            pos = 0;
            isStartGetT0 = false;
            YcDataParser.Reset();
            while (queue.TryDequeue(out KeyValuePair<DataSourceType, byte[]> dropBuffer));
            isRuning = true; //设置运行标志
            thread = new Thread(new ThreadStart(ThreadFunction)); //新建线程
            thread.Start(); //启动线程
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            isRuning = false; //设置运行标志
            thread?.Join(); //停止线程
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        private void ThreadFunction()
        {
            while(isRuning)
            {
                //取数据
                if(queue.TryDequeue(out KeyValuePair<DataSourceType, byte[]>  dataBuffer))
                {
                    switch(dataBuffer.Key)
                    {
                        case DataSourceType.DATA_RADER: //雷达
                            ParseRadarData(dataBuffer.Value); //数据解析
                            break;
                        case DataSourceType.DATA_TELEMETRY: //遥测
                            YcDataParser.ParseDatas(dataBuffer.Value); //数据解析
                            break;
                    }
                    
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// 解析雷达数据
        /// </summary>
        /// <param name="buffer"></param>
        private void ParseRadarData(byte[] buffer)
        {
            String errMsg; //错误消息
            //数据检测
            if(!CheckPacket(buffer, out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "数据包错误:" + errMsg);
                return;
            }
            PostMessage(mainFormHandle, MainForm.WM_RADAR_DATA_COMMING, 0, IntPtr.Zero); //发送WM_RADAR_DATA_COMMING
            if (Config.GetInstance().source != 0) //如果数据源不是雷达
            {
                return;
            }
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    //解析数据
                    PACK_HEAD packHead = new PACK_HEAD
                    {
                        Station = br.ReadByte(),
                        Type = br.ReadByte()
                    };
                    //如果stationId不为空且和数据包里的Station不匹配，就丢弃
                    if (!Config.GetInstance().stationId.Equals(String.Empty) && packHead.Station != int.Parse(Config.GetInstance().stationId))
                    {
                        return;
                    }
                    
                    //雷达T0数据
                    if (packHead.Type == 0x20)
                    {
                        S_HEAD sHead = new S_HEAD
                        {
                            Len = br.ReadUInt16(),
                            Time = br.ReadInt32(),
                            SrcId = br.ReadByte(),
                            SrcType = br.ReadByte(),
                            CS = br.ReadByte(),
                            CT = br.ReadByte(),
                            FF = br.ReadByte(),
                            Num = br.ReadByte(),
                            C = br.ReadByte(),
                            S = br.ReadBytes(1)
                        };

                        //如果数据源为雷达
                        if (Config.GetInstance().source == 0)
                        {
                            if (isPrintRaderLog == false) //打印日志
                            {
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到第一帧雷测数据,Type={0}，Time={1}ms",
                                    packHead.Type, sHead.Time));
                                isPrintRaderLog = true;
                            }
                            //判断是否取得起飞时间
                            if (T0 == -1)
                            {
                                //如果已经开始获取T0，并且数据包时间大于T0延迟
                                if (isStartGetT0 && sHead.Time >= T0Delay)
                                {
                                    T0 = sHead.Time - T0Delay;
                                    isStartGetT0 = false;
                                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("手动计算T0结果:{0}", T0));
                                    //发送WM_T0
                                    PostMessage(mainFormHandle, MainForm.WM_T0, 0, IntPtr.Zero);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        //循环读取数据
                        while (stream.Position <= stream.Length - 90)
                        {
                            S_OBJECT sObject = new S_OBJECT
                            {
                                time = sHead.Time - T0,      //time
                                ObjectId = br.ReadUInt16(),  //ObjectId
                                A = br.ReadDouble(),         //A
                                E = br.ReadDouble(),         //E
                                R = br.ReadDouble(),         //R
                                v = br.ReadDouble(),         //v
                                X = br.ReadDouble(),         //X
                                Y = br.ReadDouble(),         //Y
                                Z = br.ReadDouble(),         //Z
                                VX = br.ReadDouble(),        //VX
                                VY = br.ReadDouble(),        //VY
                                VZ = br.ReadDouble(),        //VZ
                                BD = br.ReadByte(),          //BD
                                SS = br.ReadByte(),          //SS
                                VF = br.ReadByte(),          //VF
                                Reserve = br.ReadBytes(5),   //Reserve
                            };
                            //获取上下限
                            GetMinMax(ref pos, sObject.time, out MinMaxValue minMaxValue);
                            sObject.MinX = minMaxValue.MinX;        //MinX 
                            sObject.MaxX = minMaxValue.MaxX;        //MaxX 
                            sObject.MinY = minMaxValue.MinY;        //MinY 
                            sObject.MaxY = minMaxValue.MaxY;        //MaxY 
                            sObject.MinZ = minMaxValue.MinZ;        //MinZ 
                            sObject.MaxZ = minMaxValue.MaxZ;        //MaxZ 
                            sObject.MinVx = minMaxValue.MinVx;      //MinVx
                            sObject.MaxVx = minMaxValue.MaxVx;      //MaxVx
                            sObject.MinVy = minMaxValue.MinVy;      //MinVy
                            sObject.MaxVy = minMaxValue.MaxVy;      //MaxVy
                            sObject.MinVz = minMaxValue.MinVz;      //MinVz
                            sObject.MaxVz = minMaxValue.MaxVz;      //MaxVz
                            sObject.suit = 1;

                            sObjectListSuit1.Add(sObject);
                            if(sObjectListSuit1.Count > 25)
                            {
                                sObjectListSuit1.RemoveAt(0);
                            }
                            if(sObjectListSuit1.Count == 25)
                            {
                                double[] timeArray = new double[25];
                                double[] xArray = new double[25];
                                double[] yArray = new double[25];
                                double[] zArray = new double[25];
                                for(int i = 0; i < 25; ++i)
                                {
                                    timeArray[i] = sObjectListSuit1[i].time;
                                    xArray[i] = sObjectListSuit1[i].X;
                                    yArray[i] = sObjectListSuit1[i].Y;
                                    zArray[i] = sObjectListSuit1[i].Z;
                                }
                                S_OBJECT obj = sObjectListSuit1[12];
                                obj.VX = Algorithm.GetSpeed(timeArray, xArray, obj.time);      //计算VX
                                obj.VY = Algorithm.GetSpeed(timeArray, yArray, obj.time);      //计算VY
                                obj.VZ = Algorithm.GetSpeed(timeArray, zArray, obj.time);      //计算VZ
                                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(S_OBJECT)));
                                Marshal.StructureToPtr(obj, ptr, true);
                                PostMessage(mainFormHandle, MainForm.WM_RADAR_DATA, 0, ptr);
                            }
                        }
                    }
                    else if(packHead.Type == 0x50)
                    {
                        S_HEAD sHead = new S_HEAD
                        {
                            Len = br.ReadUInt16(),
                            Time = br.ReadInt32(),
                            SrcId = br.ReadByte(),
                            SrcType = br.ReadByte(),
                            CS = br.ReadByte(),
                            CT = br.ReadByte(),
                            FF = br.ReadByte(),
                            Num = br.ReadByte(),
                            C = br.ReadByte(),
                            S = br.ReadBytes(1)
                        };
                        if (Config.GetInstance().source == 0)
                        {
                            if (isPrintRaderLog == false)
                            {
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到第一帧雷测数据,Type={0}，Time={1}ms",
                                    packHead.Type, sHead.Time));
                                isPrintRaderLog = true;
                            }
                            recvRaderT0 = DateTime.Now;
                            if (T0 == -1 && telemetryFlyTime != -1 && recvRaderT0 != DateTime.MinValue && recvTelemetryFlyTime != DateTime.MinValue
                                && recvRaderT0 >= recvTelemetryFlyTime)
                            {
                                if (sHead.Time > (int)(recvRaderT0 - recvTelemetryFlyTime).TotalMilliseconds - telemetryFlyTime)
                                {
                                    T0 = sHead.Time - (int)(recvRaderT0 - recvTelemetryFlyTime).TotalMilliseconds - telemetryFlyTime;
                                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到雷测T0帧，T0={0}ms", T0));
                                    PostMessage(mainFormHandle, MainForm.WM_T0, 0, IntPtr.Zero);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// CheckPacket
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool CheckPacket(byte[] buffer, out String errMsg)
        {
            int length = buffer.Length;
            if(length < Marshal.SizeOf(typeof(PACK_HEAD)) + Marshal.SizeOf(typeof(S_HEAD)))
            {
                errMsg = "数据长度小于报文头";
                return false;
            }

            if((length - Marshal.SizeOf(typeof(PACK_HEAD)) - Marshal.SizeOf(typeof(S_HEAD))) % 90 != 0)
            {
                errMsg = "数据长度错误";
                return false;
            }

            errMsg = String.Empty;
            return true;
        }

        /// <summary>
        /// GetMinMax
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="time"></param>
        /// <param name="minMaxValue"></param>
        private void GetMinMax(ref int pos, int time, out MinMaxValue minMaxValue)
        {
            if(Config.GetInstance().minMaxValues== null || Config.GetInstance().minMaxValues.Count == 0)
            {
                minMaxValue = new MinMaxValue
                {
                    MinX = 0,
                    MaxX = 0,
                    MinY = 0,
                    MaxY = 0,
                    MinZ = 0,
                    MaxZ = 0,
                    MinVx = 0,
                    MaxVx = 0,
                    MinVy = 0,
                    MaxVy = 0,
                    MinVz = 0,
                    MaxVz = 0
                };
                return;
            }
            for(; pos < Config.GetInstance().minMaxValues.Count; ++pos)
            {
                if(time < Config.GetInstance().minMaxValues[pos].Time)
                {
                    break;
                }
            }
            pos -= 1;
            if(pos >= Config.GetInstance().minMaxValues.Count)
            {
                pos = Config.GetInstance().minMaxValues.Count - 1;
            }
            if(pos < 0)
            {
                pos = 0;
            }
            minMaxValue = Config.GetInstance().minMaxValues[pos];
        }

        /// <summary>
        /// StartGetT0
        /// </summary>
        /// <param name="delay"></param>
        public void StartGetT0(int delay)
        {
            if (Config.GetInstance().source == 0)
            {
                T0Delay = delay;
                isStartGetT0 = true;
            }
        }
    }
}
