using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace RadarProcess
{
    public class DataParser
    {
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int Msg, int wParam, IntPtr lParam);
        private YcDataParser YcDataParser;
        private bool isStartGetT0;
        private int T0Delay;
        private int T0;
        private int telemetryFlyTime;
        private DateTime recvRaderT0;
        private DateTime recvTelemetryFlyTime;
        private int pos;
        private bool isPrintRaderLog = false;
        List<S_OBJECT> sObjectListSuit1 = new List<S_OBJECT>();
        List<S_OBJECT> sObjectListSuit2 = new List<S_OBJECT>();

        public enum DataSourceType
        { 
            DATA_RADER,
            DATA_TELEMETRY
        }

        public DataParser(MainForm mainForm)
        {
            this.mainForm = mainForm;
            YcDataParser = new YcDataParser(this, mainForm);
        }

        public void SetYcFlyTime(int time)
        {
            if (telemetryFlyTime == -1)
            {
                telemetryFlyTime = time;
                recvTelemetryFlyTime = DateTime.Now;
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到遥测总飞行时间：{0}", telemetryFlyTime));
            }
        }

        private MainForm mainForm;
        private ConcurrentQueue<KeyValuePair<DataSourceType, byte[]>> queue = new ConcurrentQueue<KeyValuePair<DataSourceType, byte[]>>();
        private bool isRuning = false;
        Thread thread;
        public void Enqueue(DataSourceType dataType, byte[] data)
        {
            queue.Enqueue(new KeyValuePair<DataSourceType, byte[]>(dataType, data));
        }

        public void Start()
        {
            sObjectListSuit1.Clear();
            sObjectListSuit2.Clear();
            T0 = -1;
            telemetryFlyTime = -1;
            recvRaderT0 = recvTelemetryFlyTime = DateTime.MinValue;
            pos = 0;
            isStartGetT0 = false;
            YcDataParser.Reset();
            while (queue.TryDequeue(out KeyValuePair<DataSourceType, byte[]> dropBuffer));
            isRuning = true;
            thread = new Thread(new ThreadStart(ThreadFunction));
            thread.Start();
        }

        public void Stop()
        {
            isRuning = false;
            thread?.Join();
        }

        private void ThreadFunction()
        {
            while(isRuning)
            {
                if(queue.TryDequeue(out KeyValuePair<DataSourceType, byte[]>  dataBuffer))
                {
                    switch(dataBuffer.Key)
                    {
                        case DataSourceType.DATA_RADER:
                            ParseRadarData(dataBuffer.Value);
                            break;
                        case DataSourceType.DATA_TELEMETRY:
                            YcDataParser.ParseDatas(dataBuffer.Value);
                            break;
                    }
                    
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void ParseRadarData(byte[] buffer)
        {
            String errMsg;
            if(!CheckPacket(buffer, out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "数据包错误:" + errMsg);
                return;
            }
            PostMessage(mainForm.Handle, MainForm.WM_RADAR_DATA_COMMING, 0, IntPtr.Zero);
            if(Config.GetInstance().source != 0)
            {
                return;
            }
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    PACK_HEAD packHead = new PACK_HEAD
                    {
                        Station = br.ReadByte(),
                        Type = br.ReadByte()
                    };
                    if(!Config.GetInstance().stationId.Equals(String.Empty) && packHead.Station != int.Parse(Config.GetInstance().stationId))
                    {
                        return;
                    }
                    
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
                        if (Config.GetInstance().source == 0)
                        {
                            if (isPrintRaderLog == false)
                            {
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到第一帧雷测数据,Type={0}，Time={1}ms",
                                    packHead.Type, sHead.Time));
                                isPrintRaderLog = true;
                            }
                            if (T0 == -1)
                            {
                                if (isStartGetT0)
                                {
                                    T0 = sHead.Time - T0Delay;
                                    isStartGetT0 = false;
                                    Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("手动计算T0结果:{0}", T0));
                                    PostMessage(mainForm.Handle, MainForm.WM_T0, 0, IntPtr.Zero);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        while (stream.Position <= stream.Length - 90)
                        {
                            S_OBJECT sObject = new S_OBJECT
                            {
                                time = sHead.Time - T0,
                                ObjectId = br.ReadUInt16(),
                                A = br.ReadDouble(),
                                E = br.ReadDouble(),
                                R = br.ReadDouble(),
                                v = br.ReadDouble(),
                                X = br.ReadDouble(),
                                Y = br.ReadDouble(),
                                Z = br.ReadDouble(),
                                VX = br.ReadDouble(),
                                VY = br.ReadDouble(),
                                VZ = br.ReadDouble(),
                                BD = br.ReadByte(),
                                SS = br.ReadByte(),
                                VF = br.ReadByte(),
                                Reserve = br.ReadBytes(5),
                            };
                            GetMinMax(ref pos, sObject.time, out MinMaxValue minMaxValue);
                            sObject.MinX = minMaxValue.MinX;
                            sObject.MaxX = minMaxValue.MaxX;
                            sObject.MinY = minMaxValue.MinY;
                            sObject.MaxY = minMaxValue.MaxY;
                            sObject.MinZ = minMaxValue.MinZ;
                            sObject.MaxZ = minMaxValue.MaxZ;
                            sObject.MinVx = minMaxValue.MinVx;
                            sObject.MaxVx = minMaxValue.MaxVx;
                            sObject.MinVy = minMaxValue.MinVy;
                            sObject.MaxVy = minMaxValue.MaxVy;
                            sObject.MinVz = minMaxValue.MinVz;
                            sObject.MaxVz = minMaxValue.MaxVz;
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
                                obj.VX = Algorithm.GetSpeed(timeArray, xArray, obj.time);
                                obj.VY = Algorithm.GetSpeed(timeArray, yArray, obj.time);
                                obj.VZ = Algorithm.GetSpeed(timeArray, zArray, obj.time);
                                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(S_OBJECT)));
                                Marshal.StructureToPtr(obj, ptr, true);
                                PostMessage(mainForm.Handle, MainForm.WM_RADAR_DATA, 0, ptr);
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
                                T0 = sHead.Time - (int)(recvRaderT0 - recvTelemetryFlyTime).TotalMilliseconds - telemetryFlyTime;
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到雷测T0帧，T0={0}ms", T0));
                                PostMessage(mainForm.Handle, MainForm.WM_T0, 0, IntPtr.Zero);
                            }
                        }
                    }
                }
            }
        }

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
