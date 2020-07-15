using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Microsoft.Win32;

namespace DataSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        private UdpClient UdpSocket;
        private MMTimer Timer;
        private BinaryReader FileReader;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenPortClick(object sender, RoutedEventArgs e)
        {
            if (UdpSocket == null)
            {
                try
                {
                    UdpSocket = new UdpClient();
                    EditAddress.IsEnabled = false;
                    EditPort.IsEnabled = false;
                    BtnOpenPort.Content = "关闭";
                    EditFileName.IsEnabled = true;
                    BtnSelectFile.IsEnabled = true;
                    EditPacketSize.IsEnabled = true;
                    EditInterval.IsEnabled = true;
                    BtnSend.IsEnabled = true;
                    BtnPause.IsEnabled = true;
                    BtnStop.IsEnabled = true;
                    EditProgress.IsEnabled = true;
                    BtnJump.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    UdpSocket?.Close();
                    UdpSocket = null;
                    DXMessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                UdpSocket?.Close();
                UdpSocket = null;
                EditAddress.IsEnabled = true;
                EditPort.IsEnabled = true;
                BtnOpenPort.Content = "打开";
                EditFileName.IsEnabled = false;
                BtnSelectFile.IsEnabled = false;
                EditPacketSize.IsEnabled = false;
                EditInterval.IsEnabled = false;
                BtnSend.IsEnabled = false;
                BtnPause.IsEnabled = false;
                BtnStop.IsEnabled = false;
                EditProgress.IsEnabled = false;
                BtnJump.IsEnabled = false;
            }
        }

        private void BtnSelectFileClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(true == openFileDialog.ShowDialog())
            {
                EditFileName.Text = openFileDialog.FileName;
            }
        }

        private void BtnSendClicked(object sender, RoutedEventArgs e)
        {
            Timer?.Stop();
            FileReader?.Close();
            Timer = new MMTimer();
            Timer.Timer += Timer_Timer;
            try
            {
                FileReader = new BinaryReader(File.OpenRead(EditFileName.Text));
            }
            catch(Exception ex)
            {
                DXMessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Timer.Start(10, true);
        }

        private void Timer_Timer(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    byte[] buffer = FileReader.ReadBytes((int)EditPacketSize.Value);
                    UdpSocket.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Parse(EditAddress.Text), (int)EditPort.Value));
                    ProgressBar.Value = 100.0F * FileReader.BaseStream.Position / FileReader.BaseStream.Length;
                    ProgressBar.Content = String.Format("{0:F}%", ProgressBar.Value);
                }
                catch (Exception ex)
                {
                    DXMessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Timer.Stop();
                    return;
                }

                if (FileReader.BaseStream.Position >= FileReader.BaseStream.Length)
                {
                    Timer.Stop();
                    DXMessageBox.Show("发送完毕", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
        }

        private void BtnParseClicked(object sender, RoutedEventArgs e)
        {
            if (Timer != null)
            {
                if (Timer.IsEnabled == true)
                {
                    Timer.Stop();
                    BtnPause.Content = "继续";
                }
                else
                {
                    Timer.Start(10, true);
                    BtnPause.Content = "暂停";
                }
            }
        }

        private void BtnStopClicked(object sender, RoutedEventArgs e)
        {
            Timer?.Stop();
        }

        private void BtnJumpClicked(object sender, RoutedEventArgs e)
        {
            FileReader?.BaseStream.Seek((int)(FileReader.BaseStream.Length * (double)EditProgress.Value / 100), SeekOrigin.Begin);
        }
    }
}
