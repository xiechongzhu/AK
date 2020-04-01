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
        protected BinaryWriter logRaderWriter, logTelWriter;

        public void Enqueue(DataSourceType dataSourceType, byte[] data)
        {
            queue.Enqueue(new Tuple<DataSourceType, byte[]>(dataSourceType, data));
        }

        public void Start()
        {
            while (queue.TryDequeue(out _));
            logRaderWriter?.Close();
            logRaderWriter = new BinaryWriter(File.Create(TestInfo.GetInstance().strRaderDataFile));
            logTelWriter?.Close();
            logTelWriter = new BinaryWriter(File.Create(TestInfo.GetInstance().strTelDataFile));
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

        private void LogData(BinaryWriter streamWriter, byte[] buffer)
        {
            streamWriter.Write(buffer);
            streamWriter.Flush();
        }
    }
}
