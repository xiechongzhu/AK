﻿using DataModels;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Linq;
using LinqToDB;

namespace RadarProcess
{
    public partial class MainForm : Form
    {
        private UdpClient udpClient;
        private DataParser dataParser;
        private DataLogger dataLogger = new DataLogger();
        public MainForm()
        {
            dataParser = new DataParser(this);
            InitializeComponent();
            btnStop.Enabled = false;
            Logger.GetInstance().SetMainForm(this);
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            if(DialogResult.OK == settingForm.ShowDialog())
            {

            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TestInfoForm testInfoForm = new TestInfoForm();
            if(testInfoForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            TestInfo.GetInstance().New();
            testInfoForm.GetTestInfo(out TestInfo.GetInstance().strTestName, out TestInfo.GetInstance().strOperator, out TestInfo.GetInstance().strComment);
            Logger.GetInstance().NewFile();
            String errMsg;
            if(!Config.GetInstance().LoadConfigFile(out errMsg))
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加载配置文件失败," + errMsg);
                MessageBox.Show("加载配置文件失败," + errMsg, "错误");
                return;
            }
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加载配置文件成功");
            try
            { 
                udpClient = new UdpClient(Config.GetInstance().port);
                udpClient.JoinMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr));
                dataParser.Start();
                dataLogger.Start();
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "加入组播组成功");
            }
            catch(Exception ex)
            {
                Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_ERROR, "加入组播组失败，" + ex.Message);
                MessageBox.Show("加入组播组失败，" + ex.Message);
                udpClient.Close();
                dataParser.Stop();
                dataLogger.Stop();
                return;
            }
            udpClient.BeginReceive(EndReceive, null);
            btnSetting.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
        }

        private void EndReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] recvBuffer = udpClient?.EndReceive(ar, ref endPoint);
                dataParser.Enqueue(recvBuffer);
                dataLogger.Enqueue(recvBuffer);
                udpClient.BeginReceive(EndReceive, null);
            }
            catch(Exception)
            { }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            udpClient?.DropMulticastGroup(IPAddress.Parse(Config.GetInstance().strMultiCastIpAddr));
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "退出组播组成功");
            udpClient?.Close();
            Logger.GetInstance().Log(Logger.LOG_LEVEL.LOG_INFO, "关闭套接字成功");
            dataParser.Stop();
            dataLogger.Stop();
            SaveTestInfo();
            btnSetting.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
        }

        delegate void LogDelegate(DateTime time, Logger.LOG_LEVEL level, String msg);
        public void Log(DateTime time, Logger.LOG_LEVEL level, String msg)
        {
            if (InvokeRequired)
            {
                Invoke(new LogDelegate(Log), time, level, msg);
            }
            else
            {
                ListViewItem item = new ListViewItem
                {
                    Text = time.ToString("G")
                };
                String strLevel;
                switch (level)
                {
                    case Logger.LOG_LEVEL.LOG_INFO:
                        strLevel = "信息";
                        item.BackColor = Color.White;
                        break;
                    case Logger.LOG_LEVEL.LOG_WARN:
                        strLevel = "警告";
                        item.BackColor = Color.Yellow;
                        break;
                    case Logger.LOG_LEVEL.LOG_ERROR:
                        strLevel = "错误";
                        item.BackColor = Color.Red;
                        break;
                    default:
                        return;
                }
                item.SubItems.Add(strLevel);
                item.SubItems.Add(msg);
                LogListView.Items.Add(item);
                LogListView.EnsureVisible(LogListView.Items.Count - 1);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnStop.Enabled)
            {
                btnStop_Click(sender, e);
            }
        }

        private void SaveTestInfo()
        {
            try
            {
                var db = new DatabaseDB();
                db.Insert<DataModels.TestInfo>(new DataModels.TestInfo
                {

                    TestName = TestInfo.GetInstance().strTestName,
                    Operator = TestInfo.GetInstance().strOperator,
                    Comment = TestInfo.GetInstance().strComment,
                    Time = TestInfo.GetInstance().dateTime
                }) ;
                db.CommitTransaction();
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存试验信息失败:" + ex.Message);
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm();
            historyForm.ShowDialog();
        }
    }
}