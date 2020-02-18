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
                    while(stream.Position <  stream.Length - 1)
                    {
                        S_OBJECT sObject = new S_OBJECT
                        {
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
                            Reserve = br.ReadBytes(5)
                        };
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(S_OBJECT)));
                        Marshal.StructureToPtr(sObject, ptr, true);
                        PostMessage(mainFormHandle, MainForm.WM_RADAR_DATA, 0, ptr);
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

            if((length - Marshal.SizeOf(typeof(PACK_HEAD)) - Marshal.SizeOf(typeof(S_HEAD))) % Marshal.SizeOf(typeof(S_OBJECT)) != 0)
            {
                errMsg = "数据包含不完整的S_OBJECT";
                return false;
            }

            errMsg = String.Empty;
            return true;
        }
    }
}
