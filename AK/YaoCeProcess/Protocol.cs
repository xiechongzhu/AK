using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YaoCeProcess
{
    // UDP包头
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct UDPHead
    { 
        public byte         header1;            // 固定0xAA
        public byte         header2;            // 固定0x00
        public byte         header3;            // 固定0x55
        public byte         header4;            // 固定0x77

        public UInt16       dataLength;         // 数据长度L
        // 后面直接跟<=645个字节数据
    }

    //----------------------------------------------------------------------------//
    // CAN数据帧协议头
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CANHead 
    {
        public byte         frameInfo;          // 包含了该帧内的数据长度(bit0-bit3)
        // 后面直接跟数据段第1-8个字节
    }

    // 第一子帧帧结构
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CANFirst
    { 
        public byte         xuHao;              // 子帧序号 第一帧固定为0x00
        public byte         zhenZongChang;      // 帧总长度
        public UInt16       zhenBianHao;        // 帧编号（0-65535循环计数）??使用待定
        public byte         shuJuDuanZongChangDu;   // 数据段总长度
        public byte         zhenLeiXing;        // 帧类型
    }

    // 中间帧帧结构
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CANCenter
    {
        public byte         xuHao;              // 子帧序号 依次递增
        // 后面直接跟数据段7个字节
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[]       data;
    }

    // 结束帧帧结构
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CANEnd
    {
        public byte         xuHao;              // 子帧序号 依次递增
        // 后面直接跟数据段1-5个字节数据
        // 跟两个字节的CRC16校验结果
    }

    //----------------------------------------------------------------------------//

    // 系统判据状态数据
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SYSTEMPARSE_STATUS
    {
        public double       jingDu;             // 经度
        public double       weiDu;              // 纬度
        public float        haiBaGaoDu;         // 海拔高度

        public float        dongXiangSuDu;      // 东向速度
        public float        beiXiangSuDu;       // 北向速度
        public float        tianXiangSuDu;      // 天向速度

        public float        WxJiaoSuDu;         // Wx角速度
        public float        WyJiaoSuDu;         // Wy角速度
        public float        WzJiaoSuDu;         // Wz角速度

        public float        zhouXiangGuoZai;    // 轴向过载
        public float        GNSSTime;           // GNSS时间

        public float        curFaSheXi_X;       // 当前发射系X
        public float        curFaSheXi_Y;       // 当前发射系Y
        public float        curFaSheXi_Z;       // 当前发射系Z

        public float        yuShiLuoDianSheCheng;   // 预示落点射程
        public float        yuShiLuoDianZ;      // 预示落点Z
        public double       feiXingZongShiJian; // 飞行总时间

        public byte         ceLueJieDuan;       // 策略阶段(0-准备 1-起飞 2-一级 3-二级 4-结束)
        public byte         danTouZhuangTai;    // 弹头状态(0-状态异常 1-产品遥测上电正常 2-初始化正常 3-一级保险解除
                                                // 4-二级保险解除 5-收到保险解除信号 6-三级保险解除 7-充电 8-起爆)

        public byte         daoHangTip1;        // 导航状态提示1
                                                // bit0 导航数据选择结果（0：数据不可用 1：数据可用）
                                                // bit1 陀螺数据融合结果（0：所有数据不可用 1：数据可用）
                                                // bit2 bit3 数据未更新标志（00：均无数据; 01：1号输入无数据，2号输入有数据;
                                                //                           10：1号输入有数据，2号输入无数据; 11：均有数据）
                                                // bit4 bit5 时间间隔异常标志（00：时间间隔均正常; 01：1号时间间隔异常，2号时间间隔正常； 10：1号时间间隔正常，2号时间间隔异常； 00：时间间隔均不正常）
                                                // bit6 弹头组合无效标志（1表示无效）
                                                // bit7 弹体组合无效标志（1表示无效）

        public byte         daoHangTip2;        // 导航状态提示2
                                                // bit0 bit1 1号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit2 bit3 1号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit4 bit5 1号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit6 bit7 1号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）

        public byte         daoHangTip3;        // 导航状态提示3
                                                // bit0 bit1 1号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit2 bit3 1号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit4 bit5 2号数据经度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit6 bit7 2号数据纬度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）

        public byte         daoHangTip4;        // 导航状态提示4
                                                // bit0 bit1 2号数据高度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit2 bit3 2号数据东向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit4 bit5 2号数据北向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）
                                                // bit6 bit7 2号数据天向速度标志（00：不是野值；01：无数据；10：数据用于初始化；11：是野值）

        public byte         sysyemStatusTip;    // 系统状态指示
                                                // bit0 功率输出闭合（1有效）
                                                // bit1 解保指令发出（1有效）
                                                // bit2 自毁指令发出（1有效）
                                                // bit3 复位信号（1有效）
                                                // bit4 对外供电（1有效）
                                                // bit5 模拟自毁指令1（1有效）
                                                // bit6 模拟自毁指令2（1有效）
                                                // bit7 回路检测 ?? 待定

        public byte         chuDianZhuangTai;   // 触点状态指示
                                                // bit0 起飞分离脱插信号（0有效）
                                                // bit1 一级分离脱插信号（0有效）
                                                // bit2 安控接收机预令（1有效）
                                                // bit3 安控接收机动令（1有效）
                                                // bit4 一级自毁工作状态A（1有效）
                                                // bit5 一级自毁工作状态B（1有效）
                                                // bit6 二级自毁工作状态A（1有效）
                                                // bit7 二级自毁工作状态B（1有效）

        public byte         jueCePanJueJieGuo1; // 策略判决结果1
                                                // bit0 总飞行时间（1：有效）
                                                // bit1 侧向（1：有效）
                                                // bit2 Wx角速度（1：有效）
                                                // bit3 Wy角速度（1：有效）
                                                // bit4 Wz角速度（1：有效）
                                                // bit5 后向（1：有效）
                                                // bit6 坠落（1：有效）
                                                // bit7 分离时间（1：有效）

        public byte         jueCePanJueJieGuo2; // 策略判决结果2
                                                // bit0 控制区下限（1：有效）
                                                // bit1 控制区上限（1：有效）

        public byte         shuChuKaiGuanStatus1; // 输出开关状态1
                                                  // bit0 弹头保险（1：闭合）
                                                  // bit1 弹头起爆（1：闭合）
                                                  // bit2 一级保险1（1：闭合）
                                                  // bit3 一级保险2（1：闭合）
                                                  // bit4 一级起爆1（1：闭合）
                                                  // bit5 一级起爆2（1：闭合）
                                                  // bit6 二级保险1（1：闭合）
                                                  // bit7 二级保险2（1：闭合）

        public byte         shuChuKaiGuanStatus2; // 输出开关状态2
                                                  // bit0 二级起爆1（1：闭合）
                                                  // bit1 二级起爆2（1：闭合）
                                                  // bit2 bit3 参试状态（00：测试1，数据输出状态；01：测试2，低压输出状态；10：保留状态；11：正式实验）
    }

    // 导航数据(快速)
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DAOHANGSHUJU_KuaiSu
    {

    }

    // 导航数据(慢速)
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct DAOHANGSHUJU_ManSu
    {

    }
}
