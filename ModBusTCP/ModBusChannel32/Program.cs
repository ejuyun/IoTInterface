using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
namespace ModBusTCP
{
    public struct TagInfo
    {
        public int ADDRNO;  //设备取数地址编号
        public string TAGNAME;  //测点编码
        public string TAGMC;   //测点名称
        public string TAGLX;  //测点类型：A-模拟量,S-开关量
    }

    public class Program
    {
        private static int PauseTime = Convert.ToInt32(ConfigurationManager.AppSettings["PauseTime"]);
        static void Main(string[] args)
        {
            StartMainWindow();
            try
            {
                float[] arryFloat;
                bool[] arryBool;
                string sSendMessage = string.Empty;
                string sTagName = string.Empty;
                TagInfo ATagInfo;
                string sJsonRT = "{{\"dt\":\"{0}\",\"rt\": [{{\"tagname\":\"{1}\",\"value\":{2}}}]}}";
                TcpClientEx sendTCPClient;
                //读取TAGLIST.CSV文件，建议取数测点信息索引
                Dictionary<int, TagInfo> lstTagInfo_32_FLOAT = new Dictionary<int, TagInfo>();
                Dictionary<int, TagInfo> lstTagInfo_32_BOOL = new Dictionary<int, TagInfo>();

                lstTagInfo_32_FLOAT = ReadCSV("TagInfo_32_FLOAT.csv");
                lstTagInfo_32_BOOL = ReadCSV("TagInfo_32_BOOL.csv");

                sendTCPClient = new TcpClientEx(ConfigurationManager.AppSettings["SENDSERVERIP"], Convert.ToInt32(ConfigurationManager.AppSettings["SENDSERVERPORT"]));
                Console.WriteLine("已启动");
                ELogger.Info("已启动");

                ModbusMaster aModbusClient32 = new ModbusMaster(32);

                while (true)
                {
                    int iIndex = 0;
                    try
                    {
                        //设备地址32的float测点数量224，从地址1开始，每个测点占2个地址
                        //取第一段
                        arryFloat = aModbusClient32.GetTagsFloatValue(1, 224, true);
                        for (int i = 0; i < arryFloat.Length; i++)
                        {
                            iIndex = i + 1;
                            if (lstTagInfo_32_FLOAT.TryGetValue(iIndex, out ATagInfo))
                            {
                                sSendMessage = string.Format(sJsonRT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ATagInfo.TAGNAME, arryFloat[i].ToString());
                                if (sendTCPClient.IsConnection)
                                {
                                    sendTCPClient.SendMessage(sSendMessage);
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + sSendMessage);
                                    //ELogger.Trace("发送：" + sSendMessage);
                                }
                            }
                        }
                        arryFloat = new float[0];

                        Thread.Sleep(1000);

                        //设备地址32的bool测点数量255，从地址1开始
                        arryBool = aModbusClient32.GetTagsBoolValue(1, 255);
                        for (int i = 0; i < arryBool.Length; i++)
                        {
                            iIndex = i + 1;
                            if (lstTagInfo_32_BOOL.TryGetValue(iIndex, out ATagInfo))
                            {
                                sSendMessage = string.Format(sJsonRT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ATagInfo.TAGNAME, arryBool[i].ToString());
                                if (sendTCPClient.IsConnection)
                                {
                                    sendTCPClient.SendMessage(sSendMessage);
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + sSendMessage);
                                    // ELogger.Info("发送：" + sSendMessage);
                                }
                            }
                        }
                        arryBool = new bool[0];
                        ELogger.Info("已取数一周期");
                        Thread.Sleep(PauseTime);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("while error:" + ex.Message + ex.StackTrace);
                        ELogger.Error("while error:" + ex.Message + ex.StackTrace);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message + ex.StackTrace);
                ELogger.Error("error:" + ex.Message + ex.StackTrace);
            }
        }

        public static Dictionary<int, TagInfo> ReadCSV(string FileName)
        {
            Dictionary<int, TagInfo> lstTagInfo = new Dictionary<int, TagInfo>();
            StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\" + FileName);
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
            ELogger.Info("ModBusChannel32初始化" + FileName + "文件TAGLIST个数:" + i.ToString());
            return lstTagInfo;
        }
        private static void StartMainWindow()
        {
            Stopwatch watch2 = new Stopwatch();
            //检查同名进程数
            ELogger.Info(string.Format("ModBusChannel32程序{0}启动...", Application.ProductName));
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            ELogger.Info(string.Format("ModBusChannel32发现进程数{0}", processes.Length));

            //只允许运行1个实例
            if (processes.Length <= 1)
            {
                watch2.Start();
                ELogger.Info(string.Format("ModBusChannel32启动主线程..."));
                watch2.Stop();
                ELogger.Info("ModBusChannel32记录2耗时：" + watch2.ElapsedMilliseconds); //记录5耗时：11889
                ELogger.Info(string.Format("ModBusChannel32主线程结束..."));
            }
            else
            {
                ELogger.Info("ModBusChannel32不允许多实例运行，退出");
                //单实例，不允许多实例运行
                System.Environment.Exit(1);
            }
        }
    }
}
