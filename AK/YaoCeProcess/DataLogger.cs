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
        // 二进制数据格式存储
        protected FileStream logWriter;
        // 以文本的格式存储
        // protected StreamWriter logWriter;

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
            // 注意修改文件后缀名
            strDataFile = strLogFolder + @"\" + strDate + @"_data.dat";

            // 二进制数据格式存储
            logWriter = new FileStream(strDataFile, FileMode.Append);
            // 文本数据格式存储
            // logWriter = new StreamWriter(strDataFile);

            isRuning = true;
            thread = new Thread(new ThreadStart(ThreadFunction));
            thread.Start();
        }

        public void Stop()
        {
            isRuning = false;
            thread?.Join();
            logWriter?.Close();
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

            //----------------------------------------------------------//
            // 以二进制格式进行数据存储
            // 20200207 直接原数据存储
            logWriter.Write(buffer, 0, buffer.Length);
            logWriter.Flush();
        }
    }
}
