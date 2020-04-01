using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadarSimulator
{
    public partial class mainForm : Form
    {
        private UdpClient udpClient = null;
        private bool isLoopSend = false;
        private List<byte[]> binaryBufferList = new List<byte[]>();

        // 创建文件对象
        FileStream fileStream = null;
        // 创建工作簿对象
        XSSFWorkbook workbook = null;
        // 读取工作簿第一张表(此处参数可为下标,也可为表名)
        ISheet sheet = null;
        // 读取当前行数(从1行开始读取，第0行为标题)
        int curReadLine = 1;

        public mainForm()
        {
            InitializeComponent();
        }

        private void btnSendOne_Click(object sender, EventArgs e)
        {
            byte[] sendBuffer = BuildPacket(double.Parse(editX.Text), double.Parse(editY.Text), double.Parse(editZ.Text),
                double.Parse(editVx.Text), double.Parse(editVy.Text), double.Parse(editVz.Text));
            udpClient?.Send(sendBuffer, sendBuffer.Length, editIp.Text, int.Parse(editPort.Text));
        }

        private void btnSendLoop_Click(object sender, EventArgs e)
        {
            if (!isLoopSend)
            {
                //-------------------------------------------//
                // 选择文件
                OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框              
                if (InitialDialog(openFileDialog, "Open"))
                {
                    try
                    {
                        using (Stream stream = openFileDialog.OpenFile())
                        {
                            string FileName = ((System.IO.FileStream)stream).Name;
                            // 执行相关文件操作
                            //---------------------------------------------------//
                            //创建文件对象
                            fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                            // 创建工作簿对象
                            workbook = new XSSFWorkbook(fileStream);
                            // 读取工作簿第一张表(此处参数可为下标,也可为表名)
                            sheet = workbook.GetSheetAt(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    timerExcel.Interval = int.Parse(editInterval.Text);
                    timerExcel.Start();
                    btnSendLoop.Text = "停止";
                    btnSendOne.Enabled = false;
                    btnSendRadarFile.Enabled = false;
                    isLoopSend = true;
                }
            }
            else
            {
                timerExcel.Stop();
                btnSendLoop.Text = "发送Excel文件";
                btnSendOne.Enabled = true;
                btnSendRadarFile.Enabled = true;
                isLoopSend = false;
            }
            curReadLine = 1;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (udpClient == null)
            {
                try
                {
                    udpClient = new UdpClient();
                    btnOpen.Text = "关闭";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开端口失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                udpClient.Close();
                udpClient = null;
                btnOpen.Text = "打开";
            }
        }

        private byte[] BuildT0()
        {
            PACK_HEAD packHeader = new PACK_HEAD()
            {
                Station = 0x21,
                Type = 0x50
            };
            S_HEAD sHead = new S_HEAD()
            {
                Len = (ushort)(Marshal.SizeOf(typeof(S_HEAD))),
                Time = (int)DateTime.Now.TimeOfDay.TotalMilliseconds,
            };
            int len = Marshal.SizeOf(typeof(PACK_HEAD)) + sHead.Len;
            byte[] buffer = new byte[len];
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(StructToBytes(packHeader));
            binaryWriter.Write(StructToBytes(sHead));
            return buffer;
        }

        private byte[] BuildPacket(double x, double y, double z, double vx, double vy, double vz)
        {
            PACK_HEAD packHeader = new PACK_HEAD()
            {
                Station = 0x21,
                Type = 0x01
            };
            S_HEAD sHead = new S_HEAD()
            {
                Len = (ushort)(Marshal.SizeOf(typeof(S_HEAD)) + Marshal.SizeOf(typeof(S_OBJECT))),
                Time = (int)DateTime.Now.TimeOfDay.TotalMilliseconds,
            };
            S_OBJECT sObject = new S_OBJECT
            {
                X = x,
                Y = y,
                Z = z,
                VX = vx,
                VY = vy,
                VZ = vz
            };
            int len = Marshal.SizeOf(typeof(PACK_HEAD)) + sHead.Len;
            byte[] buffer = new byte[len];
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(StructToBytes(packHeader));
            binaryWriter.Write(StructToBytes(sHead));
            binaryWriter.Write(StructToBytes(sObject));
            return buffer;
        }

        private byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sheet == null) return;
            // 新建当前工作表行数据
            IRow row;
            if (curReadLine <= sheet.LastRowNum)
            {
                // row读入第i行数据
                row = sheet.GetRow(curReadLine++);
                // 获取每一列的数据,并转换为对应的数据类型.
                double x = System.Convert.ToDouble(row.GetCell(2).ToString());
                double y = System.Convert.ToDouble(row.GetCell(3).ToString());
                double z = System.Convert.ToDouble(row.GetCell(4).ToString());

                double Vx = System.Convert.ToDouble(row.GetCell(5).ToString());
                double Vy = System.Convert.ToDouble(row.GetCell(6).ToString());
                double Vz = System.Convert.ToDouble(row.GetCell(7).ToString());
                // 发送数据
                byte[] sendBuffer = BuildPacket(x , y, z,
                Vx, Vy, Vz);
                udpClient?.Send(sendBuffer, sendBuffer.Length, editIp.Text, int.Parse(editPort.Text));
            }
            else
            {
                timerExcel.Stop();
                btnSendLoop.Text = "发送Excel文件";
                btnSendOne.Enabled = true;
                btnSendRadarFile.Enabled = true;
                isLoopSend = false;
            }
        }

        //统一对话框
        private bool InitialDialog(FileDialog fileDialog, string title)
        {              // 初始化路径
            fileDialog.Filter = "excel files (*.xls,*.xlsx)|*.xls;*.xlsx";   // 过滤选项设置，文本文件，所有文件。
            fileDialog.FilterIndex = 1;                                      // 当前使用第二个过滤字符串
            fileDialog.RestoreDirectory = true;                              // 对话框关闭时恢复原目录
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 1; i <= fileDialog.FileName.Length; i++)
                {
                    if (fileDialog.FileName.Substring(fileDialog.FileName.Length - i, 1).Equals(@"\"))
                    {
                        return true;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnSendT0_Click(object sender, EventArgs e)
        {
            byte[] sendBuffer = BuildT0();
            udpClient?.Send(sendBuffer, sendBuffer.Length, editIp.Text, int.Parse(editPort.Text));
        }

        private void btnSendRadarFile_Click(object sender, EventArgs e)
        {
            binaryBufferList.Clear();
            if (!isLoopSend)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "二进制文件|*.bin";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    String fileName = openFileDialog.FileName;
                    using (FileStream fileStream = File.Open(fileName, FileMode.Open))
                    {
                        BinaryReader reader = new BinaryReader(fileStream);
                        while(reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            byte[] packHead = reader.ReadBytes(Marshal.SizeOf(typeof(PACK_HEAD)));
                            byte[] s_head = reader.ReadBytes(Marshal.SizeOf(typeof(S_HEAD)));
                            int size = Marshal.SizeOf(typeof(S_HEAD));
                            IntPtr buffer = Marshal.AllocHGlobal(size);
                            Marshal.Copy(s_head, 0, buffer, size);
                            S_HEAD sHead = (S_HEAD)Marshal.PtrToStructure(buffer, typeof(S_HEAD));
                            Marshal.FreeHGlobal(buffer);
                            int bodyLen = sHead.Len;
                            byte[] sObjects = reader.ReadBytes(bodyLen - Marshal.SizeOf(typeof(S_HEAD)));
                            binaryBufferList.Add(packHead.Concat(s_head).Concat(sObjects).ToArray());
                        }
                    }
                    btnSendRadarFile.Text = "停止";
                    isLoopSend = true;
                    timerBin.Interval = int.Parse(editInterval.Text);
                    timerBin.Start();
                    btnSendOne.Enabled = false;
                    btnSendLoop.Enabled = false;
                }
            }
            else
            {
                timerBin.Stop();
                btnSendRadarFile .Text= "发送二进制雷测文件";
                isLoopSend = false;
                btnSendOne.Enabled = true;
                btnSendLoop.Enabled = true;
            }
        }

        private void timerBin_Tick(object sender, EventArgs e)
        {
            if(binaryBufferList.Count > 0)
            {
                udpClient?.Send(binaryBufferList[0], binaryBufferList[0].Length, editIp.Text, int.Parse(editPort.Text));
                binaryBufferList.RemoveAt(0);
            }
            else
            {
                timerBin.Stop();
                btnSendRadarFile.Text = "发送二进制雷测文件";
                isLoopSend = false;
                btnSendOne.Enabled = true;
                btnSendLoop.Enabled = true;
            }
        }
    }
}
