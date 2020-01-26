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
    public class DataParser
    {
        public DataParser(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }
        private MainForm mainForm;
        private ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private bool isRuning = false;
        Thread thread;
        public void Enqueue(byte[] data)
        {
            queue.Enqueue(data);
        }

        public void Start()
        {
            while (queue.TryDequeue(out byte[] dropBuffer));
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
            while(isRuning)
            {
                byte[] dataBuffer;
                if(queue.TryDequeue(out dataBuffer))
                {
                    ParseData(dataBuffer);
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void ParseData(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);
            BinaryReader br = new BinaryReader(stream);
        }
    }
}
