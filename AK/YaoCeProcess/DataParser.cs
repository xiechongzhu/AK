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
        //---------------缓存的CAN长帧数据---------------//
        const byte frameType_systemStatus_1 = 0x15;       // 系统判据状态
        const byte frameType_systemStatus_2 = 0x16;       // 系统判据状态
        const byte frameType_daoHangKuaiSu_Ti = 0x21;     // 导航快速（弹体）
        const byte frameType_daoHangKuaiSu_Tou = 0x31;    // 导航快速（弹头）
        const byte frameType_daoHangManSu_Ti = 0x25;      // 导航慢速（弹体）
        const byte frameType_daoHangManSu_Tou = 0x35;     // 导航慢速（弹头）

        // 系统判决状态15
        bool bRecvHeader_XiTong15 = false;
        byte[] statusBuffer_XiTong15 = null;     // 状态数据
        byte totalCountCan_XiTong15 = 0;         // 帧总长度
        byte frameLength_XiTong15 = 0;           // 数据段总长度

        // 系统判决状态16
        bool bRecvHeader_XiTong16 = false;
        byte[] statusBuffer_XiTong16 = null;     // 状态数据
        byte totalCountCan_XiTong16 = 0;         // 帧总长度
        byte frameLength_XiTong16 = 0;           // 数据段总长度

        // 导航快速 弹体
        bool bRecvHeader_DHK21 = false;
        byte[] statusBuffer_DHK21 = null;        // 状态数据
        byte totalCountCan_DHK21 = 0;            // 帧总长度
        byte frameLength_DHK21 = 0;              // 数据段总长度

        // 导航快速 弹头
        bool bRecvHeader_DHK31 = false;
        byte[] statusBuffer_DHK31 = null;        // 状态数据
        byte totalCountCan_DHK31 = 0;            // 帧总长度
        byte frameLength_DHK31 = 0;              // 数据段总长度

        // 导航慢速 弹体
        bool bRecvHeader_DHM25 = false;
        byte[] statusBuffer_DHM25 = null;        // 状态数据
        byte totalCountCan_DHM25 = 0;            // 帧总长度
        byte frameLength_DHM25 = 0;              // 数据段总长度

        // 导航慢速 弹头
        bool bRecvHeader_DHM35 = false;
        byte[] statusBuffer_DHM35 = null;        // 状态数据
        byte totalCountCan_DHM35 = 0;            // 帧总长度
        byte frameLength_DHM35 = 0;              // 数据段总长度
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
                            frameInfo1 = br.ReadByte(),
                            frameInfo2 = br.ReadByte()
                        };
                        // TODO 这里的CAN帧头是大端字节序
                        UInt16 frameInfo = (UInt16)((UInt16)packHead.frameInfo1 << 8 + packHead.frameInfo2);
                        // 3bit占位 8bit 帧id(仲裁场) 1bitRTR(0) 4bit数据场(数据长度)
                        byte canDataId = (byte)(frameInfo >> 5 & 0xFF);

                        // 偏移
                        dataReadPos += (UInt16)Marshal.SizeOf(typeof(CANHead));

                        // 当前CAN帧的数据长度
                        int canLen = (int)frameInfo & 0xF;
                        // 读取can数据
                        byte[] canData = br.ReadBytes(canLen);
                        // 偏移
                        // dataReadPos += (UInt16)canLen;
                        // 这里直接默认偏移8个字节，数据不足会进行填充
                        dataReadPos += 8;
                        byte[] unUseData = br.ReadBytes(8 - canLen);

                        //---------------------------------------------------------------------------------//
                        // 只进行如下状态数据
                        switch (canDataId)
                        {
                            case frameType_systemStatus_1:
                            case frameType_systemStatus_2:
                            case frameType_daoHangKuaiSu_Ti:
                            case frameType_daoHangKuaiSu_Tou:
                            case frameType_daoHangManSu_Ti:
                            case frameType_daoHangManSu_Tou:
                                // 将数据放入CAN数据处理模块，进行长帧的拼包工作
                                ParseCANData(canData, canDataId);
                                break;
                            default:
                                break;
                        }
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
                errMsg = "数据不是合法数据，大于了标准帧长";
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
                errMsg = "数据帧头标识错误";
                return false;
            }

            // 20200212更改
            // 数据长度判断（大端）
            dataLength = (UInt16)(((UInt16)buffer[4] << 8) + buffer[5]);
            if ((length - Marshal.SizeOf(typeof(UDPHead))) < dataLength)
            {
                errMsg = "数据包含不完整";
                return false;
            }

            errMsg = String.Empty;
            return true;
        }

        private void ParseCANData(byte[] buffer, byte canDataId)
        {
            // can数据长度（至少大于等于1才能取出数据中的第一个字节：帧序号）
            if (buffer.Length < 1) return;

            //---------------------------------------------------------------//

            // 子帧序号
            byte xuHao = buffer[0];

            // 数据第一帧
            if (xuHao == 0x00)
            {
                switch (canDataId)
                {
                    case frameType_systemStatus_1:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_XiTong15 = null;
                            totalCountCan_XiTong15 = buffer[1];  // 帧总长度
                            frameLength_XiTong15 = buffer[4];    // 数据段总长度
                            statusBuffer_XiTong15 = new byte[0];
                            bRecvHeader_XiTong15 = true;
                        }
                        break;
                    case frameType_systemStatus_2:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_XiTong16 = null;
                            totalCountCan_XiTong16 = buffer[1];  // 帧总长度
                            frameLength_XiTong16 = buffer[4];    // 数据段总长度
                            statusBuffer_XiTong16 = new byte[0];
                            bRecvHeader_XiTong16 = true;
                        }
                        break;
                    case frameType_daoHangKuaiSu_Ti:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_DHK21 = null;
                            totalCountCan_DHK21 = buffer[1];  // 帧总长度
                            frameLength_DHK21 = buffer[4];    // 数据段总长度
                            statusBuffer_DHK21 = new byte[0];
                            bRecvHeader_DHK21 = true;
                        }
                        break;
                    case frameType_daoHangKuaiSu_Tou:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_DHK31 = null;
                            totalCountCan_DHK31 = buffer[1];  // 帧总长度
                            frameLength_DHK31 = buffer[4];    // 数据段总长度
                            statusBuffer_DHK31 = new byte[0];
                            bRecvHeader_DHK31 = true;
                        }
                        break;
                    case frameType_daoHangManSu_Ti:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_DHM25 = null;
                            totalCountCan_DHM25 = buffer[1];  // 帧总长度
                            frameLength_DHM25 = buffer[4];    // 数据段总长度
                            statusBuffer_DHM25 = new byte[0];
                            bRecvHeader_DHM25 = true;
                        }
                        break;
                    case frameType_daoHangManSu_Tou:
                        {
                            // 设置空 回到原始状态
                            statusBuffer_DHM35 = null;
                            totalCountCan_DHM35 = buffer[1];  // 帧总长度
                            frameLength_DHM35 = buffer[4];    // 数据段总长度
                            statusBuffer_DHM35 = new byte[0];
                            bRecvHeader_DHM35 = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (canDataId)
                {
                    case frameType_systemStatus_1:
                        {
                            if (!bRecvHeader_XiTong15)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_XiTong15 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_XiTong15 = statusBuffer_XiTong15.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_XiTong15 - statusBuffer_XiTong15.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_XiTong15 = statusBuffer_XiTong15.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_XiTong15, frameType_systemStatus_1);
                                bRecvHeader_XiTong15 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    case frameType_systemStatus_2:
                        {
                            if (!bRecvHeader_XiTong16)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_XiTong16 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_XiTong16 = statusBuffer_XiTong16.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_XiTong16 - statusBuffer_XiTong16.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_XiTong16 = statusBuffer_XiTong16.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_XiTong16, frameType_systemStatus_2);
                                bRecvHeader_XiTong16 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    case frameType_daoHangKuaiSu_Ti:
                        {
                            if (!bRecvHeader_DHK21)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_DHK21 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_DHK21 = statusBuffer_DHK21.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_DHK21 - statusBuffer_DHK21.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_DHK21 = statusBuffer_DHK21.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_DHK21, frameType_daoHangKuaiSu_Ti);
                                bRecvHeader_DHK21 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    case frameType_daoHangKuaiSu_Tou:
                        {
                            if (!bRecvHeader_DHK31)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_DHK31 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_DHK31 = statusBuffer_DHK31.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_DHK31 - statusBuffer_DHK31.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_DHK31 = statusBuffer_DHK31.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_DHK31, frameType_daoHangKuaiSu_Tou);
                                bRecvHeader_DHK31 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    case frameType_daoHangManSu_Ti:
                        {
                            if (!bRecvHeader_DHM25)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_DHM25 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_DHM25 = statusBuffer_DHM25.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_DHM25 - statusBuffer_DHM25.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_DHM25 = statusBuffer_DHM25.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_DHM25, frameType_daoHangManSu_Ti);
                                bRecvHeader_DHM25 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    case frameType_daoHangManSu_Tou:
                        {
                            if (!bRecvHeader_DHM35)
                            {
                                return;
                            }
                            // 中间帧
                            if (xuHao != totalCountCan_DHM35 - 1)
                            {
                                // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                                byte[] canData = new byte[7];
                                Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                                statusBuffer_DHM35 = statusBuffer_DHM35.Concat(canData).ToArray();
                            }
                            // 结束帧
                            else
                            {
                                // 整个长帧剩余长度
                                int lastDataLen = frameLength_DHM35 - statusBuffer_DHM35.Length;

                                // 拼接上一次剩余的包
                                byte[] canData = new byte[lastDataLen];
                                Array.Copy(buffer, 1, canData, 0, lastDataLen);
                                statusBuffer_DHM35 = statusBuffer_DHM35.Concat(canData).ToArray();

                                //---------------------------------------------------//
                                // 拼接完成，分类型进行数据的处理
                                ParseStatusData(statusBuffer_DHM35, frameType_daoHangManSu_Tou);
                                bRecvHeader_DHM35 = false;
                                //---------------------------------------------------//
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ParseStatusData(byte[] buffer, byte frameType)
        {
            // TODO 添加CRC16校验待定
            if (!ParseDataCRC16(buffer))
            {
                return;
            }

            //---------------------------------------//

            switch (frameType)
            {
                case frameType_systemStatus_1:                  // 系统判据状态
                case frameType_systemStatus_2:                  // 系统判据状态
                    ParseStatusData_SystemStatus(buffer);
                    break;
                    // TODO 注意导航快速数据需要分别显示在弹头弹体上
                case frameType_daoHangKuaiSu_Ti:                // 导航快速（弹体）
                case frameType_daoHangKuaiSu_Tou:               // 导航快速（弹头）
                    ParseStatusData_daoHangKuaiSu(buffer);
                    break;
                    // TODO 注意导航慢速数据需要分别显示在弹头弹体上
                case frameType_daoHangManSu_Ti:                 // 导航慢速（弹体）
                case frameType_daoHangManSu_Tou:                // 导航慢速（弹头）
                    ParseStatusData_daoHangKuaiSu(buffer);
                    break;
                default:
                    break;
            }
        }

        private bool ParseDataCRC16(byte[] buffer)
        {
            // CRC16
            string crcValueSTR = CRC.ToCRC16(buffer, true);    // bool 是否逆序
            byte[] crcValue = CRC.StringToHexByte(crcValueSTR);
            // 取出最后两字节的校验位
            byte crcHig = buffer[buffer.Length - 1];
            byte crcLow = buffer[buffer.Length - 2];
            if (crcValue.Length >= 2)
            {
                if (crcValue[0] == crcLow && crcValue[0] == crcHig)
                {
                    return true;
                }
                // TODO 测试阶段，不添加添加CRC16校验
                // return false;
            }

            return true;
        }

        private void ParseStatusData_SystemStatus(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    SYSTEMPARSE_STATUS sObject = new SYSTEMPARSE_STATUS
                    {
                        jingDu = br.ReadDouble(),                   // 经度
                        weiDu = br.ReadDouble(),                    // 纬度
                        haiBaGaoDu = br.ReadSingle(),               // 海拔高度

                        dongXiangSuDu = br.ReadSingle(),            // 东向速度
                        beiXiangSuDu = br.ReadSingle(),             // 北向速度
                        tianXiangSuDu = br.ReadSingle(),            // 天向速度

                        WxJiaoSuDu = br.ReadSingle(),               // Wx角速度
                        WyJiaoSuDu = br.ReadSingle(),               // Wy角速度
                        WzJiaoSuDu = br.ReadSingle(),               // Wz角速度

                        zhouXiangGuoZai = br.ReadSingle(),          // 轴向过载
                        GNSSTime = br.ReadSingle(),                 // GNSS时间

                        curFaSheXi_X = br.ReadSingle(),             // 当前发射系X
                        curFaSheXi_Y = br.ReadSingle(),             // 当前发射系Y
                        curFaSheXi_Z = br.ReadSingle(),             // 当前发射系Z

                        yuShiLuoDianSheCheng = br.ReadSingle(),     // 预示落点射程
                        yuShiLuoDianZ = br.ReadSingle(),            // 预示落点Z
                        feiXingZongShiJian = br.ReadDouble(),       // 飞行总时间

                        ceLueJieDuan = br.ReadByte(),               // 策略阶段(0-准备 1-起飞 2-一级 3-二级 4-结束)
                        danTouZhuangTai = br.ReadByte(),            // 弹头状态(0-状态异常 1-产品遥测上电正常 2-初始化正常 3-一级保险解除
                                                                    // 4-二级保险解除 5-收到保险解除信号 6-三级保险解除 7-充电 8-起爆)

                        daoHangTip1 = br.ReadByte(),                // 导航状态提示1
                        daoHangTip2 = br.ReadByte(),                // 导航状态提示2
                        daoHangTip3 = br.ReadByte(),                // 导航状态提示3
                        daoHangTip4 = br.ReadByte(),                // 导航状态提示4
                        sysyemStatusTip = br.ReadByte(),            // 系统状态指示
                        chuDianZhuangTai = br.ReadByte(),           // 触点状态指示
                        jueCePanJueJieGuo1 = br.ReadByte(),         // 策略判决结果1
                        jueCePanJueJieGuo2 = br.ReadByte(),         // 策略判决结果2
                        shuChuKaiGuanStatus1 = br.ReadByte(),       // 输出开关状态1
                        shuChuKaiGuanStatus2 = br.ReadByte(),       // 输出开关状态2

                        // 一次读取n个字节
                        // Reserve = br.ReadBytes(5)
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
                        daoHangXiTongShiJian = br.ReadUInt32(),     // 导航系统时间
                        jingDu = br.ReadInt32(),                    // 经度（组合结果）当量：1e-7
                        weiDu = br.ReadInt32(),                     // 纬度（组合结果）当量：1e-7
                        haiBaGaoDu = br.ReadInt32(),                // 海拔高度（组合结果）当量：1e-7

                        dongXiangSuDu = br.ReadInt32(),             // 东向速度（组合结果）当量：1e-7
                        beiXiangSuDu = br.ReadInt32(),              // 北向速度（组合结果）当量：1e-7
                        tianXiangSuDu = br.ReadInt32(),             // 天向速度（组合结果）当量：1e-7

                        GNSSTime = br.ReadUInt32(),                 // GNSS时间 单位s,UTC秒部
                        fuYangJiao = br.ReadSingle(),               // 俯仰角
                        gunZhuanJiao = br.ReadSingle(),             // 滚转角
                        pianHangJiao = br.ReadSingle(),             // 偏航角

                        // 上5ms速度
                        tuoLuoShuJu_X = br.ReadSingle(),            // 陀螺X数据
                        tuoLuoShuJu_Y = br.ReadSingle(),            // 陀螺Y数据
                        tuoLuoShuJu_Z = br.ReadSingle(),            // 陀螺Z数据

                        // 上5ms加速度
                        jiaSuDuJiShuJu_X = br.ReadSingle(),         // 加速度计X数据
                        jiaSuDuJiShuJu_Y = br.ReadSingle(),         // 加速度计Y数据
                        jiaSuDuJiShuJu_Z = br.ReadSingle(),         // 加速度计Z数据

                        // 本5ms速度
                        tuoLuoShuJu_X2 = br.ReadSingle(),           // 陀螺X数据2
                        tuoLuoShuJu_Y2 = br.ReadSingle(),           // 陀螺Y数据2
                        tuoLuoShuJu_Z2 = br.ReadSingle(),           // 陀螺Z数据2

                        // 本5ms加速度
                        jiaSuDuJiShuJu_X2 = br.ReadSingle(),        // 加速度计X数据2
                        jiaSuDuJiShuJu_Y2 = br.ReadSingle(),        // 加速度计Y数据2
                        jiaSuDuJiShuJu_Z2 = br.ReadSingle(),        // 加速度计Z数据2

                        zhuangTaiBiaoZhiWei = br.ReadByte(),        // 状态标志位
                        tuoLuoGuZhangBiaoZhi = br.ReadByte(),       // 陀螺故障标志
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
                        GPSTime = br.ReadUInt32(),                          // GPS时间 单位s,UTC秒部
                        GPSDingWeiMoShi = br.ReadByte(),                    // GPS定位模式

                        GPS_SV = br.ReadByte(),                             // GPS SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
                        BD2_SV = br.ReadByte(),                             // BD2 SV可用/参与定位数（低4位为可用数，高4位为参与定位数）

                        jingDu = br.ReadInt32(),                            // 经度（GPS测量）当量：1e-7
                        weiDu = br.ReadInt32(),                             // 纬度（GPS测量）当量：1e-7
                        haiBaGaoDu = br.ReadInt32(),                        // 海拔高度（GPS测量）当量：1e-2

                        dongXiangSuDu = br.ReadInt32(),                     // 东向速度（GPS测量）当量：1e-2
                        beiXiangSuDu = br.ReadInt32(),                      // 北向速度（GPS测量）当量：1e-2
                        tianXiangSuDu = br.ReadInt32(),                     // 天向速度（GPS测量）当量：1e-2

                        PDOP = br.ReadUInt16(),                             // PDOP 当量0.01
                        HDOP = br.ReadUInt16(),                             // HDOP 当量0.01
                        VDOP = br.ReadUInt16(),                             // VDOP 当量0.01

                        tuoLuoWenDu_X = br.ReadChar(),                      // X陀螺温度
                        tuoLuoWenDu_Y = br.ReadChar(),                      // Y陀螺温度
                        tuoLuoWenDu_Z = br.ReadChar(),                      // Z陀螺温度

                        jiaJiWenDu_X = br.ReadChar(),                       // X加计温度
                        jiaJiWenDu_Y = br.ReadChar(),                       // Y加计温度
                        jiaJiWenDu_Z = br.ReadChar(),                       // Z加计温度

                        dianYaZhi_zheng5V = br.ReadChar(),                  // +5V电压值     当量0.05
                        dianYaZhi_fu5V = br.ReadChar(),                     // -5V电压值     当量0.05

                        dianYaZhi_zheng15V = br.ReadChar(),                 // +15V电压值    当量0.02
                        dianYaZhi_fu15V = br.ReadChar(),                    // -15V电压值    当量0.02

                        tuoLuoDianYaZhi_X_zheng5V = br.ReadChar(),          // X陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_X_fu5V = br.ReadChar(),             // X陀螺-5V电压值     当量0.05

                        tuoLuoDianYaZhi_Y_zheng5V = br.ReadChar(),          // Y陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_Y_fu5V = br.ReadChar(),             // Y陀螺-5V电压值     当量0.05

                        tuoLuoDianYaZhi_Z_zheng5V = br.ReadChar(),          // Z陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_Z_fu5V = br.ReadChar(),             // Z陀螺-5V电压值     当量0.05

                        yuTuoLuoTongXingCuoWuJiShu_X = br.ReadByte(),       // 与X陀螺通信错误计数（一直循环计数）
                        yuTuoLuoTongXingCuoWuJiShu_Y = br.ReadByte(),       // 与Y陀螺通信错误计数（一直循环计数）
                        yuTuoLuoTongXingCuoWuJiShu_Z = br.ReadByte(),       // 与Z陀螺通信错误计数（一直循环计数）
                        yuGPSJieShouJiTongXingCuoWuJiShu = br.ReadByte(),   // 与GPS接收机通信错误计数（一直循环计数）

                        IMUJinRuZhongDuanCiShu = br.ReadByte(),             // IMU进入中断次数（每800次+1 循环计数）
                        GPSZhongDuanCiShu = br.ReadByte(),                  // GPS中断次数（每10次+1 循环计数）

                        biaoZhiWei1 = br.ReadByte(),                        // 标志位1
                        biaoZhiWei2 = br.ReadByte(),                        // 标志位2
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
