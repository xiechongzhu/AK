using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadarProcess
{
    public class DataLogger
    {
        private ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private bool isRuning = false;
        Thread thread;
        protected StreamWriter logWriter;

        public void Enqueue(byte[] data)
        {
            queue.Enqueue(data);
        }

        public void Start()
        {
            while (queue.TryDequeue(out byte[] dropBuffer)) ;
            logWriter?.Close();
            logWriter = new StreamWriter(TestInfo.GetInstance().strDataFile);
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
            StringBuilder sb = new StringBuilder(buffer.Length * 3);
            foreach(byte b in buffer)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0') + " ");
            }
            String strData = sb.ToString().ToUpper();
            logWriter.WriteLine(strData);
            logWriter.Flush();
        }
    }
}
