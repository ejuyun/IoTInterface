using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Configuration;
using System.IO;
using System.Text;

namespace TCP104
{
    public partial class Form1 : Form
    {
        //测试数据源，用来显示
        DataTable table = new DataTable();
        DataTable table2 = new DataTable();
        //TCP连接
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //发送队列
        Queue<APDUClass> SendList = new Queue<APDUClass>();
        //等待语句柄。挂起后台线程时阻塞使用
        EventWaitHandle waitHandel = new EventWaitHandle(false, EventResetMode.AutoReset);
        //定时器扫描周期
        int scaneRate = 30000;
        //定时器
        System.Timers.Timer tm;
        //允许读标志。关闭后台线程时同步信号使用
        bool ReadEnable = false;
        //发计数。程序中使用，实际应用中暂未起作用
        short sr = 0;
        //收计数。程序中使用，实际应用中暂未起作用
        short nr = 0;
        //建立数据索引，查找已初始化的数据更新
        Dictionary<int, DataRow> find = new Dictionary<int, DataRow>();
        //读取TAGLIST.CSV文件，建议取数测点信息索引
        Dictionary<int, TagInfo> lstTagInfo = new Dictionary<int, TagInfo>();

        /// <summary>
        /// 104服务器IP
        /// </summary>
        string TCP104SERVER = ConfigurationManager.AppSettings["TCP104SERVER"];
        /// <summary>
        /// 转发至服务端IP
        /// </summary>
        string SENDTCPIP = ConfigurationManager.AppSettings["SENDTCPIP"];
        /// <summary>
        /// 状态监控发送
        /// </summary>
        Socket UDPPushClient;
        EndPoint Remote;
        /// <summary>
        /// 转发至服务端PORT
        /// </summary>
        int SENDTCPPORT = Convert.ToInt32(ConfigurationManager.AppSettings["SENDTCPPORT"]);

        TcpClientEx sendTCPClient;

        string sJsonRT = "{{\"dt\":\"{0}\",\"rt\": [{{\"tagname\":\"{1}\",\"value\":{2}}}]}}";

        Dictionary<string, double?> dicTag = new Dictionary<string, double?>();

        int PauseTime = Convert.ToInt32(ConfigurationManager.AppSettings["PauseTime"]);

        object objlock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            table.Columns.Add("方向");
            table.Columns.Add("协议层数据");
            table.Columns.Add("数据层数据");
            table.Columns.Add("时间");
            table.Columns.Add("SR");
            table.Columns.Add("NR");
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[0].FillWeight = 10;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].FillWeight = 20;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].FillWeight = 60;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].FillWeight = 10;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].FillWeight = 5;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[5].FillWeight = 5;

            table2.Columns.Add("地址");
            table2.Columns.Add("值");
            table2.Columns.Add("时间");
            table2.PrimaryKey = new DataColumn[] { table2.Columns[0]};
            dataGridView2.DataSource = table2;
            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[0].FillWeight = 10;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[1].FillWeight = 20;
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[2].FillWeight = 60;

            ReadCSV();
            if (!ReadEnable)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                waitHandel.Set();
            }
            ELogger.Info("启动成功");
        }

        private void ReadCSV()
        {
            StreamReader sr = new StreamReader(Application.StartupPath + @"\TAGLIST.CSV");
            String line;
            int i = 0;
            //越过第一行标题
            sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                TagInfo aTagInfo = new TagInfo();
                string[] strarr = line.Split(',');
                aTagInfo.ADDRNO = Convert.ToInt32(strarr[0]);
                aTagInfo.TAGNAME = strarr[1];
                aTagInfo.TAGMC = strarr[2];
                aTagInfo.TAGLX = strarr[3];
                lstTagInfo.Add(aTagInfo.ADDRNO, aTagInfo);
                i++;
            }
            sr.Close();
            ELogger.Info("1214:初始化TAGLIST个数:" + i.ToString());
        }
        /// <summary>
        /// 后台线程启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                UDPPushClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint send = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9020);
                Remote = (EndPoint)(send);

                APDUClass myBuffer = new APDUClass(new APCIClassUFormat(APCIClassUFormat.UFormatType.StartSet), null);
                tm = new System.Timers.Timer();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPAddress.Parse(textBox1.Text), 2404);
                socket.SendTimeout = 500;
                socket.ReceiveTimeout = 500;

                sendTCPClient = new TcpClientEx(SENDTCPIP, SENDTCPPORT);

                SendList.Enqueue(new APDUClass(new APCIClassUFormat(APCIClassUFormat.UFormatType.StartSet), null));
                sr = 0;
                nr = 0;
                ASDUClass clockBuffer = new ASDUClass();
                clockBuffer.Pack(ASDUClass.TransRes.Active, ASDUClass.FunType.ClockConfirm);
                ASDUClass calBuffer = new ASDUClass();
                calBuffer.Pack(ASDUClass.TransRes.Active, ASDUClass.FunType.CalAll);

                ASDUClass ymBuffer = new ASDUClass();
                ymBuffer.Pack(ASDUClass.TransRes.Active, ASDUClass.FunType.CalEnergyPulse);

                //       SendList.Enqueue(new APDUClass(new APCIClassIFormat(++sr, nr), clockBuffer));
                SendList.Enqueue(new APDUClass(new APCIClassIFormat(++sr, nr), calBuffer));
                SendList.Enqueue(new APDUClass(new APCIClassIFormat(++sr, nr), ymBuffer));
                //SendList.Enqueue(new APDUClass(new APCIClassUFormat(APCIClassUFormat.UFormatType.StopSet), null));

                ReadEnable = true;
                ReceiveOnce();
                tm.AutoReset = true;
                tm.Interval = scaneRate;
                tm.Elapsed += new System.Timers.ElapsedEventHandler(tm_Elapsed);
                tm.Start();

                new Thread(t =>
                {
                    while (true)
                    {
                        foreach (var item in dicTag)
                        {
                            var sSendMessage = string.Format(sJsonRT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.Key, item.Value);

                            if (sendTCPClient.IsConnection)
                            {
                                sendTCPClient.SendMessage(sSendMessage);
                                UDPPushClient.SendTo(EncodeUDP(sSendMessage), Remote);
                                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + sSendMessage);
                                ELogger.Info("发送：" + sSendMessage);
                            }
                        }
                        Thread.Sleep(PauseTime);
                    }
                }).Start();
                waitHandel.WaitOne();
            }
            catch (Exception ex)
            {
                ELogger.Error("backgroundWorker1_DoWork error：" + ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 后台线程提交数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage == -1)
                {
                    APDUClass temp = new APDUClass(e.UserState as byte[]);
                    APCIClass.UISFormat dataFormat = temp.GetApciType();
                    if (temp != null)
                    {
                        table.Rows.Add("TX", temp.ApciToString(), temp.AsduToString(), DateTime.Now, temp.GetSR(), temp.GetNR());
                    }
                }
                else if (e.ProgressPercentage > 0)
                {
                    byte[] receive = new byte[e.ProgressPercentage];
                    Array.Copy(e.UserState as byte[], receive, e.ProgressPercentage);
                    if (receive.Length > 6 && receive[8] == 0x14 && receive[6] == 0x01)
                    {
                        System.Diagnostics.Debug.WriteLine("??");
                    }

                    APDUClass temp = new APDUClass(receive);

                    APCIClass.UISFormat dataFormat = temp.GetApciType();
                    if (dataFormat == APCIClass.UISFormat.I)
                    {
                        //if (nr > short.MaxValue)
                        //{
                        //    nr = 0;
                        //}
                        SendList.Enqueue(new APDUClass(new APCIClassSFormat(++nr), null));
                        //sr++;
                    }
                    else if (dataFormat == APCIClass.UISFormat.S)
                    {
                        //if (nr > short.MaxValue)
                        //{
                        //    nr = 0;
                        //}
                        SendList.Enqueue(new APDUClass(new APCIClassSFormat(++nr), null));
                        //sr++;
                    }

                    if (temp != null)
                    {
                        string sSendMessage = string.Empty;
                        string sTagName = string.Empty;
                        TagInfo ATagInfo;
                        table.Rows.Add("RX", temp.ApciToString(), temp.AsduToString(), DateTime.Now, temp.GetSR(), temp.GetNR());
                        if ((temp.Res.Equals(ASDUClass.TransRes.ResAll)) || (temp.Res.Equals(ASDUClass.TransRes.EnergyCall)))
                        {
                            var datas = temp.GetData();
                            foreach (var data in datas)
                            {
                                ELogger.Trace("接收：" + data.Addr.ToString() + "=" + data.Data.ToString());

                                //      find.Add(data.Addr,table2.Rows.Add(data.Addr,data.Data,data.Time));

                                //向网闸外侧服务发送实时数据
                                if (lstTagInfo.TryGetValue(data.Addr, out ATagInfo))
                                {
                                    //listTgas.Add(ATagInfo);
                                    lock (objlock)
                                    {
                                        if (dicTag.ContainsKey(ATagInfo.TAGNAME))
                                        {
                                            dicTag.Remove(ATagInfo.TAGNAME);
                                            dicTag.Add(ATagInfo.TAGNAME, data.Data);
                                        }
                                        else
                                        {
                                            dicTag.Add(ATagInfo.TAGNAME, data.Data);
                                        }
                                    }
                                }
                            }
                        }
                        else if (temp.Res == ASDUClass.TransRes.AutoSend)
                        {
                            var datas = temp.GetData();
                            if (datas==null)
                            {
                                return;
                            }
                            foreach (var data in datas)
                            {
                                ELogger.Trace("接收：" + data.Addr.ToString() + "=" + data.Data.ToString());
                                try
                                {
                                    //向网闸外侧服务发送实时数据
                                    if (lstTagInfo.TryGetValue(data.Addr, out ATagInfo))
                                    {
                                        //listTgas.Add(ATagInfo);
                                        lock (objlock)
                                        {
                                            if (dicTag.ContainsKey(ATagInfo.TAGNAME))
                                            {
                                                dicTag.Remove(ATagInfo.TAGNAME);
                                                dicTag.Add(ATagInfo.TAGNAME, data.Data);
                                            }
                                            else
                                            {
                                                dicTag.Add(ATagInfo.TAGNAME, data.Data);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":backgroundWorker1_ProgressChanged send error：" + ex.Message + ex.StackTrace);
                                    ELogger.Error("backgroundWorker1_ProgressChanged send error：" + ex.Message+ex.StackTrace);
                                }
                            }
                        }
                    }
                }
                if (table.Rows.Count > 20)
                {
                    table.Rows.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                ELogger.Error("backgroundWorker1_ProgressChanged error：" + ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 后台线程完成工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ReadEnable = false;
                tm.Stop();
                tm.Dispose();
                System.Threading.Thread.Sleep(1000);
                socket.Close();
                sendTCPClient.Close();
                UDPPushClient.Close();
            }
            catch(Exception ex)
            {
                ELogger.Error("backgroundWorker1_RunWorkerCompleted error：" + ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 定时发送的定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ASDUClass ymBuffer = new ASDUClass();
                ymBuffer.Pack(ASDUClass.TransRes.Active, ASDUClass.FunType.CalEnergyPulse);
                SendList.Enqueue(new APDUClass(new APCIClassIFormat(++sr, nr), ymBuffer));
                if (SendList.Count > 0)
                {
                    byte[] SendBuffer = SendList.Dequeue().ToArray();
                    backgroundWorker1.ReportProgress(-1, SendBuffer);
                    socket.Send(SendBuffer);
                }
            }
            catch (Exception ex)
            {
                ELogger.Error("tm_Elapsed error：" + ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 接收数据的函数
        /// </summary>
        private void ReceiveOnce()
        {
            byte[] ReceiveBuffer = new byte[1024];
            socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, Received, ReceiveBuffer);
        }
        /// <summary>
        /// 异步接收触发函数
        /// </summary>
        /// <param name="ar"></param>
        private void Received(IAsyncResult ar)
        {
            try
            {
                if (ReadEnable)
                {
                    int lenth = socket.EndReceive(ar);
                    backgroundWorker1.ReportProgress(lenth, ar.AsyncState as byte[]);
                    ReceiveOnce();
                }
            }
            catch (Exception ex)
            {
                ELogger.Error("Received error：" + ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// 表格添加行后的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
            catch (Exception ex)
            {
                ELogger.Error("Received error：" + ex.Message + ex.StackTrace);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private byte[] EncodeUDP(string sMessage)
        {
            byte[] data = Encoding.UTF8.GetBytes(sMessage.ToString().Trim());
            return data;
        }
    }
}
