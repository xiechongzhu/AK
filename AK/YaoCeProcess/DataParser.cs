using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaoCeProcess
{
    public class DataParser
    {
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int Msg, int wParam, IntPtr lParam);

        public DataParser(IntPtr mainFormHandle)
        {
            this.mainFormHandle = mainFormHandle;
        }

        //------------------------------------------------------------------------------------//
        private IntPtr mainFormHandle;
        private ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private bool isRuning = false;
        Thread thread;
        //------------------------------------------------------------------------------------//
        // 缓存的CAN长帧数据
        byte[] statusBuffer = null;     // 状态数据
        byte   totalCountCan = 0;       // 帧总长度
        byte   frameLength   = 0;       // 数据段总长度
        byte   frameType     = 0;       // 帧类型
        //------------------------------------------------------------------------------------//

        public void Enqueue(byte[] data)
        {
            queue.Enqueue(data);
        }

        public void Start()
        {
            while (queue.TryDequeue(out byte[] dropBuffer)) ;
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
            while (isRuning)
            {
                byte[] dataBuffer;
                if (queue.TryDequeue(out dataBuffer))
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
            UInt16 dataLength;
            if (!CheckPacket(buffer, out errMsg, out dataLength))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "数据包错误:" + errMsg);
                return;
            }
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                // 位置偏移到CAN数据域
                stream.Seek(Marshal.SizeOf(typeof(UDPHead)), 0);
                // 解析CAN数据帧
                using (BinaryReader br = new BinaryReader(stream))
                {
                    UInt16 dataReadPos = 0;
                    while (stream.Position < stream.Length - 1 && dataReadPos < dataLength)
                    {
                        // 解析CAN帧头
                        CANHead packHead = new CANHead
                        {
                            frameInfo = br.ReadByte()
                        };
                        // 偏移
                        dataReadPos += (UInt16)Marshal.SizeOf(typeof(CANHead));

                        // 当前CAN帧的数据长度
                        int canLen = (int)packHead.frameInfo & 0xF;
                        // 读取can数据
                        byte[] canData = br.ReadBytes(canLen);
                        // 偏移
                        dataReadPos += (UInt16)canLen;

                        // 将数据放入CAN数据处理模块，进行长帧的拼包工作
                        ParseCANData(canData);
                    }

                }
            }
        }

        private bool CheckPacket(byte[] buffer, out String errMsg, out UInt16 dataLength)
        {
            // 默认初始化
            dataLength = 0;

            int length = buffer.Length;
            if (length > 651)
            {
                errMsg = "数据不是合法数据";
                return false;
            }

            if (length < Marshal.SizeOf(typeof(UDPHead)))
            {
                errMsg = "数据长度小于UDP报文头";
                return false;
            }

            // 判断帧头信息
            if (!(buffer[0] == 0xAA && buffer[1] == 0x00 && buffer[2] == 0x55 && buffer[3] == 0x77))
            {
                errMsg = "数据帧头错误";
                return false;
            }

            // 数据长度判断
            dataLength = (UInt16)(((UInt16)buffer[5] << 8) + buffer[4]);
            if ((length - Marshal.SizeOf(typeof(UDPHead))) < dataLength)
            {
                errMsg = "数据包含不完整";
                return false;
            }

            errMsg = String.Empty;
            return true;
        }

        private void ParseCANData(byte[] buffer)
        {
            // can数据长度
            int length = buffer.Length;
            if (length < 1)
            {
                return;
            }

            // 子帧序号
            byte xuHao = buffer[0];

            // 数据第一帧
            if (xuHao == 0x00)
            {
                // 设置空 回到原始状态
                statusBuffer    = null;    

                totalCountCan   = buffer[1];
                frameLength     = buffer[4];
                frameType       = buffer[5];
            }
            else
            {
                // 中间帧
                if (xuHao != totalCountCan - 1)
                {
                    // 拼接上一次剩余的包
                    byte[] canData = new byte[7];
                    Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                    statusBuffer = statusBuffer.Concat(canData).ToArray();
                }
                // 结束帧
                else
                {
                    int lastDataLen = frameLength - statusBuffer.Length;

                    // 拼接上一次剩余的包
                    byte[] canData = new byte[lastDataLen];
                    Array.Copy(buffer, 1, canData, 0, lastDataLen); 
                    statusBuffer = statusBuffer.Concat(canData).ToArray();

                    //---------------------------------------------------//
                    // 拼接完成，分类型进行数据的处理
                    ParseStatusData(statusBuffer, frameType);
                }
            }
        }

        private void ParseStatusData(byte[] buffer, byte frameType)
        {
            const byte frameType_systemStatus = 0x05;
            const byte frameType_daoHangKuaiSu = 0x01;
            const byte frameType_daoHangManSu = 0x02;

            switch(frameType)
            {
                case frameType_systemStatus:
                    ParseStatusData_SystemStatus(buffer);
                    break;
                case frameType_daoHangKuaiSu:
                    ParseStatusData_daoHangKuaiSu(buffer);
                    break;
                case frameType_daoHangManSu:
                    ParseStatusData_daoHangKuaiSu(buffer);
                    break;
                default:
                    break;
            }
        }

        private void ParseStatusData_SystemStatus(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    SYSTEMPARSE_STATUS sObject = new SYSTEMPARSE_STATUS
                    {
                        //ObjectId = br.ReadUInt16(),
                        //A = br.ReadDouble(),
                        //E = br.ReadDouble(),
                        //R = br.ReadDouble(),
                        //v = br.ReadDouble(),
                        //X = br.ReadDouble(),
                        //Y = br.ReadDouble(),
                        //Z = br.ReadDouble(),
                        //VX = br.ReadDouble(),
                        //VY = br.ReadDouble(),
                        //VZ = br.ReadDouble(),
                        //BD = br.ReadByte(),
                        //SS = br.ReadByte(),
                        //VF = br.ReadByte(),
                        //Reserve = br.ReadBytes(5)
                    };
                    // 向界面传递数据
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SYSTEMPARSE_STATUS)));
                    Marshal.StructureToPtr(sObject, ptr, true);
                    PostMessage(mainFormHandle, MainForm.WM_YAOCE_SystemStatus_DATA, 0, ptr);
                }
            }
        }

        private void ParseStatusData_daoHangKuaiSu(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    DAOHANGSHUJU_KuaiSu sObject = new DAOHANGSHUJU_KuaiSu
                    {
                        //ObjectId = br.ReadUInt16(),
                        //A = br.ReadDouble(),
                        //E = br.ReadDouble(),
                        //R = br.ReadDouble(),
                        //v = br.ReadDouble(),
                        //X = br.ReadDouble(),
                        //Y = br.ReadDouble(),
                        //Z = br.ReadDouble(),
                        //VX = br.ReadDouble(),
                        //VY = br.ReadDouble(),
                        //VZ = br.ReadDouble(),
                        //BD = br.ReadByte(),
                        //SS = br.ReadByte(),
                        //VF = br.ReadByte(),
                        //Reserve = br.ReadBytes(5)
                    };
                    // 向界面传递数据
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DAOHANGSHUJU_KuaiSu)));
                    Marshal.StructureToPtr(sObject, ptr, true);
                    PostMessage(mainFormHandle, MainForm.WM_YAOCE_daoHangKuaiSu_DATA, 0, ptr);
                }
            }
        }

        private void ParseStatusData_daoHangManSu(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    DAOHANGSHUJU_ManSu sObject = new DAOHANGSHUJU_ManSu
                    {
                        //ObjectId = br.ReadUInt16(),
                        //A = br.ReadDouble(),
                        //E = br.ReadDouble(),
                        //R = br.ReadDouble(),
                        //v = br.ReadDouble(),
                        //X = br.ReadDouble(),
                        //Y = br.ReadDouble(),
                        //Z = br.ReadDouble(),
                        //VX = br.ReadDouble(),
                        //VY = br.ReadDouble(),
                        //VZ = br.ReadDouble(),
                        //BD = br.ReadByte(),
                        //SS = br.ReadByte(),
                        //VF = br.ReadByte(),
                        //Reserve = br.ReadBytes(5)
                    };
                    // 向界面传递数据
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DAOHANGSHUJU_ManSu)));
                    Marshal.StructureToPtr(sObject, ptr, true);
                    PostMessage(mainFormHandle, MainForm.WM_YAOCE_daoHangManSu_DATA, 0, ptr);
                }
            }
        }
    }
}
