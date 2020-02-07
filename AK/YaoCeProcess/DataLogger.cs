using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaoCeProcess
{
    public class DataLogger
    {
        private static String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";

        private ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private bool isRuning = false;
        Thread thread;
        protected StreamWriter logWriter;

        public String strLogFolder;
        public String strDataFile;

        public DataLogger()
        {
            strLogFolder = strLogDir + @"\Bitstream";
            Directory.CreateDirectory(strLogFolder);
        }

        public void Enqueue(byte[] data)
        {
            queue.Enqueue(data);
        }

        public void Start()
        {
            while (queue.TryDequeue(out byte[] dropBuffer)) ;
            logWriter?.Close();

            // 生成码流日志文件名称
            DateTime dateTime = DateTime.Now;
            String strDate = dateTime.ToString("yyyy_MM_dd_HH_mm_ss");
            strDataFile = strLogFolder + @"\" + strDate + @"_Data.dat";

            logWriter = new StreamWriter(strDataFile);
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
                    LogData(dataBuffer);
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void LogData(byte[] buffer)
        {
            /*
            // 按文本格式存储
            StringBuilder sb = new StringBuilder(buffer.Length * 3);
            foreach (byte b in buffer)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0') + " ");
            }
            String strData = sb.ToString().ToUpper();
            logWriter.WriteLine(strData);
            logWriter.Flush();
            */
            // 20200207 直接原数据存储
            logWriter.WriteLine(buffer);
            logWriter.Flush();
        }
    }
}
