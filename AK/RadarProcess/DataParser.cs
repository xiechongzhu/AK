using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace RadarProcess
{
    public class DataParser
    {
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int Msg, int wParam, IntPtr lParam);
        private bool isStartGetT0;
        private int T0StartTime;
        private int T0Delay;
        private int T0;
        private int pos;

        public DataParser(IntPtr mainFormHandle)
        {
            this.mainFormHandle = mainFormHandle;
        }
        private IntPtr mainFormHandle;
        private ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private bool isRuning = false;
        Thread thread;
        public void Enqueue(byte[] data)
        {
            queue.Enqueue(data);
        }

        public void Start()
        {
            T0 = -1;
            pos = 0;
            isStartGetT0 = false;
            while (queue.TryDequeue(out byte[] dropBuffer));
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
                byte[] dataBuffer;
                if(queue.TryDequeue(out dataBuffer))
                {
                    ParseData(dataBuffer);
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void ParseData(byte[] buffer)
        {
            PostMessage(mainFormHandle, MainForm.WM_DATA_COMMING, 0, IntPtr.Zero);
            String errMsg;
            if(!CheckPacket(buffer, out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "数据包错误:" + errMsg);
                return;
            }
            using(MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    PACK_HEAD packHead = new PACK_HEAD
                    {
                        Station = br.ReadByte(),
                        Type = br.ReadByte()
                    };
                    if(packHead.Station != 0x20 + Config.GetInstance().stationId)
                    {
                        return;
                    }
                    
                    if (packHead.Type == 0x01)
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
                            S = br.ReadBytes(43)
                        };
                        if(T0 == -1)
                        {
                            if(isStartGetT0 && (int)DateTime.Now.TimeOfDay.TotalMilliseconds - T0StartTime > T0Delay)
                            {
                                T0 = sHead.Time;
                                isStartGetT0 = false;
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("手动计算T0结果:{0}", T0));
                            }
                            else
                            {
                                return;
                            }
                        }
                        while (stream.Position < stream.Length - 1)
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
                            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(S_OBJECT)));
                            Marshal.StructureToPtr(sObject, ptr, true);
                            PostMessage(mainFormHandle, MainForm.WM_RADAR_DATA, 0, ptr);
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
                            S = br.ReadBytes(43)
                        };
                        if(T0 == -1)
                        {
                            T0 = sHead.Time;
                            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("收到T0帧，T0={0}", T0));
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
            if(time < Config.GetInstance().minMaxValues[pos].Time)
            {
                minMaxValue = Config.GetInstance().minMaxValues[pos];
                return;
            }
            int i = pos;
            for(; i < Config.GetInstance().minMaxValues.Count; ++i)
            {
                if(time < Config.GetInstance().minMaxValues[i].Time)
                {
                    break;
                }
            }
            pos = i;
            if(pos >= Config.GetInstance().minMaxValues.Count)
            {
                pos = Config.GetInstance().minMaxValues.Count - 1;
            }
            minMaxValue = Config.GetInstance().minMaxValues[pos];
        }

        public void StartGetT0(int delay)
        {
            T0Delay = delay;
            isStartGetT0 = true;
            T0StartTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }
    }
}
