using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ModBusTCP
{
    public class TcpClientEx
    {

        private TcpClient client;
        private bool isConnection = false;
        private string hostip;//TCP服务器
        private int port;//端口
        Thread checkStateThread; //检查网络状态线程

        public TcpClientEx(string hostip, int port)
        {
            this.hostip = hostip;
            this.port = port;

            client = new TcpClient();
            try
            {
                client.Connect(hostip, port);
                if (client.Connected)
                {
                    IsConnection = true;
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "网闸外侧连接成功");
                    ELogger.Info("网闸外侧连接成功");
                }
            }
            catch (Exception ex)
            {
                IsConnection = false;
            }
            checkStateThread = new Thread(new ThreadStart(checkState));
            checkStateThread.IsBackground = true;
            checkStateThread.Start();
            
        }
        /// <summary>
        /// 获取或者设置网络连接状态
        /// </summary>
        public bool IsConnection
        {
            get { return isConnection; }
            set { isConnection = value; }
        }
        private void checkState()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (client.Connected == false)
                {
                    try
                    {
                        client.Close();
                        client = new TcpClient();
                        client.Connect(hostip, port);
                        IsConnection = true;
                    }
                    catch
                    {
                        IsConnection = false;
                        ELogger.Info("网闸外侧连接失败");
                    }
                }

            }
        }
        public void SendMessage(string strMessage)
        {
            try
            {
                byte[] bytesArray = Encoding.UTF8.GetBytes(strMessage + "\n");
                NetworkStream networkStream = client.GetStream();
                networkStream.Write(bytesArray, 0, bytesArray.Length);
            }
            catch
            {
                IsConnection = false;
            }
        }
        public void Close()
        {
            client.Close();
        }
    }
}