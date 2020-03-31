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
        public enum DataSourceType
        {
            DATA_RADER,
            DATA_TELEMETRY
        }

        private ConcurrentQueue<Tuple<DataSourceType, byte[]>> queue = new ConcurrentQueue<Tuple<DataSourceType, byte[]>>();
        private bool isRuning = false;
        Thread thread;
        protected StreamWriter logRaderWriter, logTelWriter;

        public void Enqueue(DataSourceType dataSourceType, byte[] data)
        {
            queue.Enqueue(new Tuple<DataSourceType, byte[]>(dataSourceType, data));
        }

        public void Start()
        {
            while (queue.TryDequeue(out _));
            logRaderWriter?.Close();
            logRaderWriter = new StreamWriter(TestInfo.GetInstance().strRaderDataFile);
            logTelWriter?.Close();
            logTelWriter = new StreamWriter(TestInfo.GetInstance().strTelDataFile);
            isRuning = true;
            thread = new Thread(new ThreadStart(ThreadFunction));
            thread.Start();
        }

        public void Stop()
        {
            isRuning = false;
            thread?.Join();
            logRaderWriter?.Close();
            logTelWriter?.Close();
        }

        private void ThreadFunction()
        {
            while (isRuning)
            {
                if (queue.TryDequeue(out Tuple<DataSourceType, byte[]>  dataBuffer))
                {
                    if (dataBuffer.Item1 == DataSourceType.DATA_RADER)
                    {
                        LogData(logRaderWriter, dataBuffer.Item2);
                    }
                    else
                    {
                        LogData(logTelWriter, dataBuffer.Item2);
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void LogData(StreamWriter streamWriter, byte[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 3 + 30);
            sb.Append(String.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")));
            foreach(byte b in buffer)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0') + " ");
            }
            String strData = sb.ToString().ToUpper();
            streamWriter.WriteLine(strData);
            streamWriter.Flush();
        }
    }
}
