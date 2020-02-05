using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                timer.Interval = int.Parse(editInterval.Text);
                timer.Start();
                btnSendLoop.Text = "停止";
                btnSendOne.Enabled = false;
                isLoopSend = true;
            }
            else
            {
                timer.Stop();
                btnSendLoop.Text = "循环发送";
                btnSendOne.Enabled = true;
                isLoopSend = false;
            }
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
                catch(Exception ex)
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

        private byte[] BuildPacket(double x, double y, double z, double vx, double vy, double vz)
        {
            PACK_HEAD packHeader = new PACK_HEAD()
            {
                Station = 0,
                Type = 0
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
            Random random = new Random();
            byte[] sendBuffer = BuildPacket(random.NextDouble() * 1000, random.NextDouble() * 1000, random.NextDouble() * 1000,
                random.NextDouble() * 1000, random.NextDouble() * 1000, random.NextDouble() * 1000);
            udpClient?.Send(sendBuffer, sendBuffer.Length, editIp.Text, int.Parse(editPort.Text));
        }
    }
}
