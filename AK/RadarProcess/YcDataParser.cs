﻿using Algorithm;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RadarProcess
{
    class YcDataParser
    {
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int Msg, int wParam, IntPtr lParam);

        public YcDataParser(DataParser radarParser, MainForm mainForm)
        {
            RadarDataParser = radarParser;
            this.mainForm = mainForm;
            mainFormHandle = mainForm.Handle;
            Reset();
        }

        public void Reset()
        {
            FlyTime = 0;
            pos = 0;
        }

        private int pos;
        private Algorithm algorithm = new Algorithm();
        private int FlyTime;
        private int FlyStartTime;
        private DataParser RadarDataParser;
        private MainForm mainForm;
        private IntPtr mainFormHandle;
        // 每一个UDP帧固定长度651
        private const int UDPLENGTH = 651;
        // 每一个状态长帧结尾的校验
        private const int CRCLENGTH = 2;
        //------------------------------------------------------------------------------------//
        //---------------缓存的CAN长帧数据---------------//
        const byte frameType_systemStatus_1 = 0x15;       // 系统判据状态
        const byte frameType_systemStatus_2 = 0x16;       // 系统判据状态
        const byte frameType_HuiLuJianCe = 0x16;          // 回路检测反馈状态
        const byte frameType_daoHangKuaiSu_Ti = 0x21;     // 导航快速（弹体）
        const byte frameType_daoHangKuaiSu_Tou = 0x31;    // 导航快速（弹头）
        const byte frameType_daoHangManSu_Ti = 0x25;      // 导航慢速（弹体）
        const byte frameType_daoHangManSu_Tou = 0x35;     // 导航慢速（弹头）
        const byte frameType_XiTongJiShi_Ti = 0x26;       // 系统状态即时反馈（弹体）   帧总长11  数据段总长度64 帧类型0x0B
        const byte frameType_XiTongJiShi_Tou = 0x36;      // 系统状态即时反馈（弹头）

        // 系统判决状态15
        bool bRecvHeader_XiTong15 = false;
        byte[] statusBuffer_XiTong15 = null;     // 状态数据
        byte totalCountCan_XiTong15 = 0;         // 帧总长度
        byte frameLength_XiTong15 = 0;           // 数据段总长度
        UInt16 frameNO_XiTong15 = 0;             // 帧编号

        // 系统判决状态16
        bool bRecvHeader_XiTong16 = false;
        byte[] statusBuffer_XiTong16 = null;     // 状态数据
        byte totalCountCan_XiTong16 = 0;         // 帧总长度
        byte frameLength_XiTong16 = 0;           // 数据段总长度
        UInt16 frameNO_XiTong16 = 0;             // 帧编号

        // 回路检测反馈数据16
        bool bRecvHeader_HuiLuJianCe16 = false;
        byte[] statusBuffer_HuiLuJianCe16 = null;     // 状态数据
        byte totalCountCan_HuiLuJianCe16 = 0;         // 帧总长度
        byte frameLength_HuiLuJianCe16 = 0;           // 数据段总长度
        UInt16 frameNO_HuiLuJianCe16 = 0;             // 帧编号

        // 当前帧类型（针对Id为0x16时，需要用到帧类型来区分）
        // 系统判决状态 0x15->0x01
        // 系统判决状态查询反馈 0x16->0x05
        // 回路检测反馈数据 0x16->0x06
        byte curFrameType = 0;
        const byte frameType_XTPJZT = 0x01;
        const byte frameType_XTPJFK = 0x05;
        const byte frameType_HLJCFK = 0x06;

        // 导航快速 弹体
        bool bRecvHeader_DHK21 = false;
        byte[] statusBuffer_DHK21 = null;        // 状态数据
        byte totalCountCan_DHK21 = 0;            // 帧总长度
        byte frameLength_DHK21 = 0;              // 数据段总长度
        UInt16 frameNO_DHK21 = 0;                // 帧编号

        // 导航快速 弹头
        bool bRecvHeader_DHK31 = false;
        byte[] statusBuffer_DHK31 = null;        // 状态数据
        byte totalCountCan_DHK31 = 0;            // 帧总长度
        byte frameLength_DHK31 = 0;              // 数据段总长度
        UInt16 frameNO_DHK31 = 0;                // 帧编号

        // 导航慢速 弹体
        bool bRecvHeader_DHM25 = false;
        byte[] statusBuffer_DHM25 = null;        // 状态数据
        byte totalCountCan_DHM25 = 0;            // 帧总长度
        byte frameLength_DHM25 = 0;              // 数据段总长度
        UInt16 frameNO_DHM25 = 0;                // 帧编号

        // 导航慢速 弹头
        bool bRecvHeader_DHM35 = false;
        byte[] statusBuffer_DHM35 = null;        // 状态数据
        byte totalCountCan_DHM35 = 0;            // 帧总长度
        byte frameLength_DHM35 = 0;              // 数据段总长度
        UInt16 frameNO_DHM35 = 0;                // 帧编号

        // TODO 20200219 新增
        // 系统即时状态反馈 弹体
        bool bRecvHeader_XiTongJiShi26 = false;
        byte[] statusBuffer_XiTongJiShi26 = null;        // 状态数据
        byte totalCountCan_XiTongJiShi26 = 0;            // 帧总长度
        byte frameLength_XiTongJiShi26 = 0;              // 数据段总长度
        UInt16 frameNO_XiTongJiShi26 = 0;                // 帧编号

        // 系统即时状态反馈 弹头
        bool bRecvHeader_XiTongJiShi36 = false;
        byte[] statusBuffer_XiTongJiShi36 = null;        // 状态数据
        byte totalCountCan_XiTongJiShi36 = 0;            // 帧总长度
        byte frameLength_XiTongJiShi36 = 0;              // 数据段总长度
        UInt16 frameNO_XiTongJiShi36 = 0;                // 帧编号
        //------------------------------------------------------------------------------------//

        // UDP 缓存buffer
        byte[] UDPBuffer = null;

        //------------------------------------------------------------------------------------//

        /// <summary>
        /// 报告指定的 System.Byte[] 在此实例中的第一个匹配项的索引。
        /// </summary>
        /// <param name="srcBytes">被执行查找的 System.Byte[]。</param>
        /// <param name="searchBytes">要查找的 System.Byte[]。</param>
        /// <returns>如果找到该字节数组，则为 searchBytes 的索引位置；如果未找到该字节数组，则为 -1。如果 searchBytes 为 null 或者长度为0，则返回值为 -1。</returns>
        internal int IndexOf(byte[] srcBytes, byte[] searchBytes)
        {
            if (srcBytes == null) { return -1; }
            if (searchBytes == null) { return -1; }
            if (srcBytes.Length == 0) { return -1; }
            if (searchBytes.Length == 0) { return -1; }
            if (srcBytes.Length < searchBytes.Length) { return -1; }
            for (int i = 0; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] == searchBytes[0])
                {
                    if (searchBytes.Length == 1) { return i; }
                    bool flag = true;
                    for (int j = 1; j < searchBytes.Length; j++)
                    {
                        if (srcBytes[i + j] != searchBytes[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag) { return i; }
                }
            }
            return -1;
        }

        public void ParseDatas(byte[] buffer)
        {
            if (UDPBuffer == null)
            {
                UDPBuffer = new byte[0];
            }

            // 帧头标识
            byte[] searchBytes = new byte[4];
            searchBytes[0] = 0xAA;
            searchBytes[1] = 0x00;
            searchBytes[2] = 0x55;
            searchBytes[3] = 0x77;

            //--------------------------------------------------------------------------------//

            // 拼接上一次剩余的包
            UDPBuffer = UDPBuffer.Concat(buffer).ToArray();

            //--------------------------------------------------------------------------------//

            while (true)
            {
                int findPos = IndexOf(UDPBuffer, searchBytes);
                if (findPos == -1)
                {
                    break;
                }
                // 判断数据长度是否足够
                if (UDPBuffer.Length < findPos + UDPLENGTH)
                {
                    break;
                }
                byte[] findBuffer = UDPBuffer.Skip(findPos).Take(UDPLENGTH).ToArray();
                // 二次判断，判断数据帧中是否存在标识，若存在则直接定义为当前帧不完整，第二帧数据补充进来了
                byte[] findDataBuffer = findBuffer.Skip(searchBytes.Length).Take(UDPLENGTH - searchBytes.Length).ToArray();
                int findPos2 = IndexOf(findDataBuffer, searchBytes);
                if (findPos2 != -1)
                {
                    int skipLength = findPos + searchBytes.Length + findPos2;
                    UDPBuffer = UDPBuffer.Skip(skipLength).Take(UDPBuffer.Length - skipLength).ToArray();
                    continue;
                }

                // 进行数据解析
                handleData(findBuffer);

                // 正常的数据偏移
                int skipLengthNormal = findPos + UDPLENGTH;
                UDPBuffer = UDPBuffer.Skip(skipLengthNormal).Take(UDPBuffer.Length - skipLengthNormal).ToArray();
            }
        }

        private void handleData(byte[] buffer)
        {
            //--------------------------------------------------------------------------------//

            if (buffer.Length < UDPLENGTH)
            {
                return;
            }
            //--------------------------------------------------------------------------------//
            // TODO 针对粘包的情况进行处理（几个UDP包粘在了一起）
            int alreadRead = 0;
            while (true)
            {
                if (buffer.Length - alreadRead >= UDPLENGTH)
                {
                    byte[] subBuffer = buffer.Skip(alreadRead).Take(UDPLENGTH).ToArray();
                    alreadRead += subBuffer.Length;
                    ParseData(subBuffer);
                }
                else
                {
                    break;
                }
            }
            //--------------------------------------------------------------------------------//
        }

        private void ParseData(byte[] buffer)
        {
            //--------------------------------------------------------------------------------//

            String errMsg;
            UInt16 dataLength;
            if (!CheckPacket(buffer, out errMsg, out dataLength))
            {
                // TODO 20200218 错误数据太多，影响界面刷新，卡顿
                // Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "数据包错误:" + errMsg);
                return;
            }
            // 如果dataLength长度等于0，直接不进行下面数据的处理
            if (dataLength == 0)
                return;

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
                        UInt16 frameInfo = (UInt16)(((UInt16)packHead.frameInfo1 << 8) + packHead.frameInfo2);
                        // 3bit占位 8bit 帧id(仲裁场) 1bitRTR(0) 4bit数据场(数据长度)
                        byte canDataId = (byte)(frameInfo >> 5 & 0xFF);

                        // 偏移
                        dataReadPos += (UInt16)Marshal.SizeOf(typeof(CANHead));

                        // 当前CAN帧的数据长度
                        int canLen = (int)frameInfo & 0xF;
                        // 一帧CAN数据最多只有八个字节，往后的数据不进行处理，直接丢弃
                        if (canLen > 8)
                            return;
                        // 读取can数据
                        // TODO 这里剩下的数据长度不包括校验的两个字节，如果想用校验，需要再读取两个字节的校验值
                        // TODO 20200217 更改为直接读取8个字节
                        byte[] canData = br.ReadBytes(/*canLen*/8);
                        // 偏移
                        // dataReadPos += (UInt16)canLen;
                        // 这里直接默认偏移8个字节，数据不足会进行填充
                        dataReadPos += 8;
                        // if (8 - canLen > 0)
                        // {
                        //     byte[] unUseData = br.ReadBytes(8 - canLen);
                        // }

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
                            // TODO 20200219 新增系统即时反馈状态
                            case frameType_XiTongJiShi_Ti:
                            case frameType_XiTongJiShi_Tou:
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
            if (length > UDPLENGTH)
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
                errMsg = "数据包不完整";
                return false;
            }

            errMsg = String.Empty;
            return true;
        }

        private void HandleCanDataPinJie(
            byte canDataId,
            ref byte[] statusBuffer,
            ref byte totalCountCan,
            ref byte frameLength,
            ref bool bRecvHeader,
            ref UInt16 frameNO,
            byte[] buffer,
            byte frameType = 0x00)
        {
            // 子帧序号
            byte xuHao = buffer[0];

            // 数据第一帧
            if (xuHao == 0x00)
            {
                // 设置空 回到原始状态
                statusBuffer = null;
                totalCountCan = buffer[1];  // 帧总长度
                frameLength = buffer[4];    // 数据段总长度
                statusBuffer = new byte[0];
                bRecvHeader = true;
                frameNO = (UInt16)(((UInt16)buffer[3] << 8) + buffer[2]);
            }
            else
            {
                if (!bRecvHeader)
                {
                    return;
                }
                // 中间帧
                if (xuHao != totalCountCan - 1)
                {
                    // 拼接上一次剩余的包(中间帧包括：1字节序号，7字节数据)
                    byte[] canData = new byte[7];
                    Array.Copy(buffer, 1, canData, 0, 7);   // 从buffer的第1个位置开始拷贝7个字节到canData中
                    statusBuffer = statusBuffer.Concat(canData).ToArray();
                }
                // 结束帧
                else
                {
                    // 整个长帧剩余长度
                    int lastDataLen = frameLength - statusBuffer.Length;
                    // CAN结束帧数据域最多不超过7个字节（包括了2Byte的校验，但不包括一个字节的帧序号）
                    if (lastDataLen > 7 || lastDataLen < 0)
                    {
                        bRecvHeader = false;
                        return;
                    }

                    // 拼接上一次剩余的包
                    // TODO +2添加两个字节的校验
                    byte[] canData = new byte[lastDataLen + 2];
                    Array.Copy(buffer, 1, canData, 0, lastDataLen + 2);
                    statusBuffer = statusBuffer.Concat(canData).ToArray();

                    //---------------------------------------------------//
                    // 拼接完成，分类型进行数据的处理
                    ParseStatusData(statusBuffer, canDataId, frameType, frameNO);
                    bRecvHeader = false;
                    //---------------------------------------------------//
                }
            }
        }

        private void ParseCANData(byte[] buffer, byte canDataId)
        {
            // can数据长度（至少大于等于1才能取出数据中的第一个字节：帧序号）
            if (buffer.Length < 1) return;

            //---------------------------------------------------------------//

            PostMessage(mainFormHandle, MainForm.WM_TELEMETRY_DATA_COMMING, 0, IntPtr.Zero);

            switch (canDataId)
            {
                // 系统判据状态
                case frameType_systemStatus_1:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_XiTong15,
                        ref totalCountCan_XiTong15, ref frameLength_XiTong15, ref bRecvHeader_XiTong15,
                        ref frameNO_XiTong15, buffer);
                    break;
                // 系统判据状态 0x16(中间存在两种情况，需要通过帧类型来做进一步的区分)
                case frameType_systemStatus_2:
                    {
                        // 第一子帧，才包含帧类型等信息
                        if (buffer[0] == 0x00 && buffer.Length >= 6)
                        {
                            curFrameType = buffer[5];
                        }
                        if (curFrameType == frameType_XTPJFK)
                        {
                            HandleCanDataPinJie(canDataId, ref statusBuffer_XiTong16,
                        ref totalCountCan_XiTong16, ref frameLength_XiTong16, ref bRecvHeader_XiTong16,
                        ref frameNO_XiTong16, buffer, curFrameType);
                        }
                        else if (curFrameType == frameType_HLJCFK)
                        {
                            HandleCanDataPinJie(canDataId, ref statusBuffer_HuiLuJianCe16,
                        ref totalCountCan_HuiLuJianCe16, ref frameLength_HuiLuJianCe16, ref bRecvHeader_HuiLuJianCe16,
                        ref frameNO_HuiLuJianCe16, buffer, curFrameType);
                        }
                    }
                    break;
                case frameType_daoHangKuaiSu_Ti:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_DHK21,
                        ref totalCountCan_DHK21, ref frameLength_DHK21, ref bRecvHeader_DHK21,
                        ref frameNO_DHK21, buffer);
                    break;
                case frameType_daoHangKuaiSu_Tou:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_DHK31,
                        ref totalCountCan_DHK31, ref frameLength_DHK31, ref bRecvHeader_DHK31,
                        ref frameNO_DHK31, buffer);
                    break;
                case frameType_daoHangManSu_Ti:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_DHM25,
                        ref totalCountCan_DHM25, ref frameLength_DHM25, ref bRecvHeader_DHM25,
                        ref frameNO_DHM25, buffer);
                    break;
                case frameType_daoHangManSu_Tou:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_DHM35,
                        ref totalCountCan_DHM35, ref frameLength_DHM35, ref bRecvHeader_DHM35,
                        ref frameNO_DHM35, buffer);
                    break;
                // TODO 20200219 新增
                case frameType_XiTongJiShi_Ti:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_XiTongJiShi26,
                        ref totalCountCan_XiTongJiShi26, ref frameLength_XiTongJiShi26, ref bRecvHeader_XiTongJiShi26,
                        ref frameNO_XiTongJiShi26, buffer);
                    break;
                case frameType_XiTongJiShi_Tou:
                    HandleCanDataPinJie(canDataId, ref statusBuffer_XiTongJiShi36,
                        ref totalCountCan_XiTongJiShi36, ref frameLength_XiTongJiShi36, ref bRecvHeader_XiTongJiShi36,
                        ref frameNO_XiTongJiShi36, buffer);
                    break;
                default:
                    break;
            }
        }

        private void ParseStatusData(byte[] buffer, byte canId, byte frameType, UInt16 frameNO)
        {
            // TODO 添加CRC16校验待定，此处需要在原数据获取上，添加两个字节
            if (!ParseDataCRC16(buffer))
            {
                return;
            }

            //---------------------------------------//

            switch (canId)
            {
                case frameType_systemStatus_1:                  // 系统判据状态
                    ParseStatusData_SystemStatus(buffer, canId, frameType_XTPJZT, frameNO);
                    break;
                case frameType_systemStatus_2:                  // 系统判据状态
                    if (frameType == frameType_XTPJFK)
                    {
                        ParseStatusData_SystemStatus(buffer, canId, frameType, frameNO);
                    }
                    else if (frameType == frameType_HLJCFK)
                    {
                        ParseStatusData_huiLuJianCe(buffer, canId, frameType, frameNO);
                    }
                    // 重新置为0
                    curFrameType = 0x00;
                    break;
                // TODO 注意导航快速数据需要分别显示在弹头弹体上
                case frameType_daoHangKuaiSu_Ti:                // 导航快速（弹体）
                case frameType_daoHangKuaiSu_Tou:               // 导航快速（弹头）
                    ParseStatusData_daoHangKuaiSu(buffer, canId, frameNO);
                    break;
                // TODO 注意导航慢速数据需要分别显示在弹头弹体上
                case frameType_daoHangManSu_Ti:                 // 导航慢速（弹体）
                case frameType_daoHangManSu_Tou:                // 导航慢速（弹头）
                    ParseStatusData_daoHangManSu(buffer, canId, frameNO);
                    break;
                // TODO 20200219 新增
                case frameType_XiTongJiShi_Ti:                 // 系统状态即时反馈（弹体）
                case frameType_XiTongJiShi_Tou:                // 系统状态即时反馈（弹头）
                    ParseStatusData_XiTongJiShi(buffer, canId, frameNO);
                    break;
                default:
                    break;
            }
        }

        private bool ParseDataCRC16(byte[] buffer)
        {
            return true;
        }

        private void ParseStatusData_SystemStatus(byte[] buffer, byte canId, byte frameType, UInt16 frameNO)
        {
            if (buffer.Length < Marshal.SizeOf(typeof(SYSTEMPARSE_STATUS)) + CRCLENGTH)
            {
                return;
            }
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
                    if (sObject.feiXingZongShiJian != 0)
                    {
                        if (Config.GetInstance().source == 0)
                        {
                            RadarDataParser.SetYcFlyTime((int)(sObject.feiXingZongShiJian * 1000));
                        }
                        else
                        {
                            if (FlyTime == 0)
                            {
                                FlyTime = (int)(sObject.feiXingZongShiJian * 1000);
                                FlyStartTime = (int)(sObject.GNSSTime) - FlyTime;
                                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, String.Format("遥测起飞时间:{0}", FlyStartTime));
                            }
                        }
                    }
                }
            }
        }

        private void ParseStatusData_daoHangKuaiSu(byte[] buffer, byte canId, UInt16 frameNO)
        {
            if (buffer.Length < Marshal.SizeOf(typeof(DAOHANGSHUJU_KuaiSu)) + CRCLENGTH)
            {
                return;
            }
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
                    if((canId == frameType_daoHangKuaiSu_Tou || canId == frameType_daoHangKuaiSu_Ti) && FlyTime != 0 && sObject.GNSSTime != 0)
                    {
                        try
                        {
                            algorithm.CalcResultYc(sObject.weiDu * Math.Pow(10, -7), sObject.jingDu * Math.Pow(10, -7),
                                    sObject.haiBaGaoDu * Math.Pow(10, -2), sObject.dongXiangSuDu * Math.Pow(10, -2), sObject.beiXiangSuDu * Math.Pow(10, -2),
                                    sObject.tianXiangSuDu * Math.Pow(10, -2), mainForm.constLaunchFsx, Config.GetInstance().placementHeight,
                                    out FallPoint fallPoint, out double fallTime, out double distance, out double x, out double y, out double z,
                                    out double vx, out double vy, out double vz);
                            S_OBJECT obj = new S_OBJECT
                            {
                                time = (int)(sObject.GNSSTime) - FlyStartTime,
                                X = x,
                                Y = y,
                                Z = z,
                                VX = vx,
                                VY = vy,
                                VZ = vz
                            };
                            GetMinMax(ref pos, obj.time, out MinMaxValue minMaxValue);
                            obj.MinX = minMaxValue.MinX;
                            obj.MaxX = minMaxValue.MaxX;
                            obj.MinY = minMaxValue.MinY;
                            obj.MaxY = minMaxValue.MaxY;
                            obj.MinZ = minMaxValue.MinZ;
                            obj.MaxZ = minMaxValue.MaxZ;
                            obj.MinVx = minMaxValue.MinVx;
                            obj.MaxVx = minMaxValue.MaxVx;
                            obj.MinVy = minMaxValue.MinVy;
                            obj.MaxVy = minMaxValue.MaxVy;
                            obj.MinVz = minMaxValue.MinVz;
                            obj.MaxVz = minMaxValue.MaxVz;

                            if (canId == frameType_daoHangKuaiSu_Tou)
                            {
                                //弹头
                                obj.suit = 1;
                                YcMessage msg = new YcMessage
                                {
                                    sObject = obj,
                                    fallPoint = fallPoint,
                                    fallTime = fallTime,
                                    distance = distance
                                };
                                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<YcMessage>());
                                Marshal.StructureToPtr(msg, ptr, true);
                                PostMessage(mainFormHandle, MainForm.WM_YC_I, 0, ptr);
                            }
                            else
                            {
                                //弹体
                                obj.suit = 2;
                                YcMessage msg = new YcMessage
                                {
                                    sObject = obj,
                                    fallPoint = fallPoint,
                                    fallTime = fallTime,
                                    distance = distance
                                };
                                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<YcMessage>());
                                Marshal.StructureToPtr(msg, ptr, true);
                                PostMessage(mainFormHandle, MainForm.WM_YC_II, 0, ptr);
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        private void GetMinMax(ref int pos, int time, out MinMaxValue minMaxValue)
        {
            if (Config.GetInstance().minMaxValues == null || Config.GetInstance().minMaxValues.Count == 0)
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
            for (; pos < Config.GetInstance().minMaxValues.Count; ++pos)
            {
                if (time < Config.GetInstance().minMaxValues[pos].Time)
                {
                    break;
                }
            }
            pos -= 1;
            if (pos >= Config.GetInstance().minMaxValues.Count)
            {
                pos = Config.GetInstance().minMaxValues.Count - 1;
            }
            if (pos < 0)
            {
                pos = 0;
            }
            minMaxValue = Config.GetInstance().minMaxValues[pos];
        }

        private void ParseStatusData_daoHangManSu(byte[] buffer, byte canId, UInt16 frameNO)
        {
            if (buffer.Length < Marshal.SizeOf(typeof(DAOHANGSHUJU_ManSu)) + CRCLENGTH)
            {
                return;
            }
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

                        tuoLuoWenDu_X = br.ReadByte(),                      // X陀螺温度
                        tuoLuoWenDu_Y = br.ReadByte(),                      // Y陀螺温度
                        tuoLuoWenDu_Z = br.ReadByte(),                      // Z陀螺温度

                        jiaJiWenDu_X = br.ReadByte(),                       // X加计温度
                        jiaJiWenDu_Y = br.ReadByte(),                       // Y加计温度
                        jiaJiWenDu_Z = br.ReadByte(),                       // Z加计温度

                        dianYaZhi_zheng5V = br.ReadSByte(),                  // +5V电压值     当量0.05
                        dianYaZhi_fu5V = br.ReadSByte(),                     // -5V电压值     当量0.05

                        dianYaZhi_zheng15V = br.ReadSByte(),                 // +15V电压值    当量0.02
                        dianYaZhi_fu15V = br.ReadSByte(),                    // -15V电压值    当量0.02

                        tuoLuoDianYaZhi_X_zheng5V = br.ReadSByte(),          // X陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_X_fu5V = br.ReadSByte(),             // X陀螺-5V电压值     当量0.05

                        tuoLuoDianYaZhi_Y_zheng5V = br.ReadSByte(),          // Y陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_Y_fu5V = br.ReadSByte(),             // Y陀螺-5V电压值     当量0.05

                        tuoLuoDianYaZhi_Z_zheng5V = br.ReadSByte(),          // Z陀螺+5V电压值     当量0.05
                        tuoLuoDianYaZhi_Z_fu5V = br.ReadSByte(),             // Z陀螺-5V电压值     当量0.05

                        yuTuoLuoTongXingCuoWuJiShu_X = br.ReadByte(),       // 与X陀螺通信错误计数（一直循环计数）
                        yuTuoLuoTongXingCuoWuJiShu_Y = br.ReadByte(),       // 与Y陀螺通信错误计数（一直循环计数）
                        yuTuoLuoTongXingCuoWuJiShu_Z = br.ReadByte(),       // 与Z陀螺通信错误计数（一直循环计数）
                        yuGPSJieShouJiTongXingCuoWuJiShu = br.ReadByte(),   // 与GPS接收机通信错误计数（一直循环计数）

                        IMUJinRuZhongDuanCiShu = br.ReadByte(),             // IMU进入中断次数（每800次+1 循环计数）
                        GPSZhongDuanCiShu = br.ReadByte(),                  // GPS中断次数（每10次+1 循环计数）

                        biaoZhiWei1 = br.ReadByte(),                        // 标志位1
                        biaoZhiWei2 = br.ReadByte(),                        // 标志位2
                    };
                }
            }
        }

        private void ParseStatusData_huiLuJianCe(byte[] buffer, byte canId, byte frameType, UInt16 frameNO)
        {
            if (buffer.Length < Marshal.SizeOf(typeof(HUILUJIANCE_STATUS)) + CRCLENGTH)
            {
                return;
            }
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    HUILUJIANCE_STATUS sObject = new HUILUJIANCE_STATUS
                    {
                        shuChu1HuiLuDianZu = br.ReadSingle(),    // 电机驱动输出1回路电阻
                        reserve1 = br.ReadUInt32(),              // 保留1
                        shuChu2HuiLuDianZu = br.ReadSingle(),    // 电机驱动输出2回路电阻
                        reserve2 = br.ReadUInt32(),              // 保留2
                        QBDH1AHuiLuDianZu = br.ReadSingle(),     // 起爆点火1A回路电阻
                        QBDH1BHuiLuDianZu = br.ReadSingle(),     // 起爆点火1B回路电阻
                        QBDH2AHuiLuDianZu = br.ReadSingle(),     // 起爆点火2A回路电阻
                        QBDH2BHuiLuDianZu = br.ReadSingle()      // 起爆点火2B回路电阻
                    };
                }
            }
        }

        private void ParseStatusData_XiTongJiShi(byte[] buffer, byte canId, UInt16 frameNO)
        {
            if (buffer.Length < Marshal.SizeOf(typeof(SYSTEMImmediate_STATUS)) + CRCLENGTH)
            {
                return;
            }
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    SYSTEMImmediate_STATUS sObject = new SYSTEMImmediate_STATUS
                    {
                        guZhangBiaoZhi = br.ReadByte(),         // 故障标志位

                        tuoLuoWenDu_X = br.ReadByte(),          // X陀螺温度
                        tuoLuoWenDu_Y = br.ReadByte(),          // Y陀螺温度
                        tuoLuoWenDu_Z = br.ReadByte(),          // Z陀螺温度

                        GPS_SV = br.ReadByte(),                 // GPS SV可用/参与定位数（低4位为可用数，高4位为参与定位数）
                        GPSDingWeiMoShi = br.ReadByte(),        // GPS定位模式

                        PDOP = br.ReadUInt16(),                 // PDOP 
                        HDOP = br.ReadUInt16(),                 // HDOP 
                        VDOP = br.ReadUInt16(),                 // VDOP 

                        GPSTime = br.ReadUInt32(),              // GPS时间 单位s,UTC秒，当量：0.1

                        jingDu = br.ReadInt32(),                // 经度           当量：1e-7
                        weiDu = br.ReadInt32(),                 // 纬度           当量：1e-7
                        haiBaGaoDu = br.ReadInt32(),            // 海拔高度       当量：1e-2

                        dongXiangSuDu = br.ReadInt32(),         // 东向速度       当量：1e-2
                        beiXiangSuDu = br.ReadInt32(),          // 北向速度       当量：1e-2
                        tianXiangSuDu = br.ReadInt32(),         // 天向速度       当量：1e-2

                        zhouXiangGuoZai = br.ReadSingle(),      // 轴向过载
                        faXiangGuoZai = br.ReadSingle(),        // 法向过载
                        ceXiangGuoZai = br.ReadSingle(),        // 侧向过载

                        WxJiaoSuDu = br.ReadSingle(),           // Wx角速度
                        WyJiaoSuDu = br.ReadSingle(),           // Wy角速度
                        WzJiaoSuDu = br.ReadSingle(),           // Wz角速度
                    };
                }
            }
        }
    }
}
