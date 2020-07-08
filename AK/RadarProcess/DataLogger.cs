/******************************************************************* 
* @brief : 数据记录类代码 
* @author : 谢崇竹 
* @date : 2020/6/27 22:43 
* @version : ver 1.0 
* @inparam : 
* @outparam : 
*******************************************************************/
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace RadarProcess
{
    /// <summary>
    /// 数据记录类
    /// </summary>
    public class DataLogger
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public enum DataSourceType
        {
            DATA_RADER, //雷达
            DATA_TELEMETRY //遥测
        }

        /// <summary>
        /// 队列
        /// </summary>
        private ConcurrentQueue<Tuple<DataSourceType, byte[]>> queue = new ConcurrentQueue<Tuple<DataSourceType, byte[]>>();
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool isRuning = false;
        /// <summary>
        /// 处理线程
        /// </summary>
        Thread thread;
        /// <summary>
        /// Writer
        /// </summary>
        protected BinaryWriter logRaderWriter, logTelWriter;

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="dataSourceType"></param>
        /// <param name="data"></param>
        public void Enqueue(DataSourceType dataSourceType, byte[] data)
        {
            queue.Enqueue(new Tuple<DataSourceType, byte[]>(dataSourceType, data));
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            while (queue.TryDequeue(out _)); //清空队列
            logRaderWriter?.Close(); //关闭雷达文件
            logRaderWriter = new BinaryWriter(File.Create(TestInfo.GetInstance().strRaderDataFile));
            logTelWriter?.Close(); //关闭遥测文件
            logTelWriter = new BinaryWriter(File.Create(TestInfo.GetInstance().strTelDataFile));
            isRuning = true; //设置为运行状态
            thread = new Thread(new ThreadStart(ThreadFunction));
            thread.Start(); //启动线程
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            isRuning = false; //设置标志位为停止
            thread?.Join(); //等待线程退出
            logRaderWriter?.Close(); //关闭雷达文件写流
            logTelWriter?.Close(); //关闭y遥测文件写流
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        private void ThreadFunction()
        {
            while (isRuning) //if is running
            {
                //数据出队列
                if (queue.TryDequeue(out Tuple<DataSourceType, byte[]>  dataBuffer))
                {
                    //雷达数据
                    if (dataBuffer.Item1 == DataSourceType.DATA_RADER)
                    {
                        LogData(logRaderWriter, dataBuffer.Item2);  //记录数据
                    }
                    else //遥测数据
                    {
                        LogData(logTelWriter, dataBuffer.Item2);    //记录数据
                    }
                }
                else
                {
                    Thread.Sleep(5); //leep 5 ms
                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="streamWriter"></param>
        /// <param name="buffer"></param>
        private void LogData(BinaryWriter streamWriter, byte[] buffer)
        {
            streamWriter.Write(buffer); //保存数据
            streamWriter.Flush(); //立即写入文件
        }
    }
}
