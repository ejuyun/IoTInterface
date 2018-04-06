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
                ushort[] data;
                float[] arryFloat;
                bool[] arryBool;
                string sSendMessage = string.Empty;
                string sTagName = string.Empty;
                TagInfo ATagInfo;
                string sJsonRT = "{{\"dt\":\"{0}\",\"rt\": [{{\"tagname\":\"{1}\",\"value\":{2}}}]}}";
                TcpClientEx sendTCPClient;
                //读取TAGLIST.CSV文件，建议取数测点信息索引
                Dictionary<int, TagInfo> lstTagInfo_1_FLOAT = new Dictionary<int, TagInfo>();
                Dictionary<int, TagInfo> lstTagInfo_1_BOOL = new Dictionary<int, TagInfo>();

                lstTagInfo_1_FLOAT = ReadCSV("TagInfo_1_FLOAT.csv");
                lstTagInfo_1_BOOL = ReadCSV("TagInfo_1_BOOL.csv");

                sendTCPClient = new TcpClientEx(ConfigurationManager.AppSettings["SENDSERVERIP"], Convert.ToInt32(ConfigurationManager.AppSettings["SENDSERVERPORT"]));
                Console.WriteLine("已启动");
                ELogger.Info("已启动");

                ModbusMaster aModbusClient1 = new ModbusMaster(1);

                while (true)
                {
                    int iIndex = 0;
                    try
                    {
                        //设备地址1的float测点数量450
                        arryFloat = aModbusClient1.GetTagsFloatValue(1, 450, false);
                        for (int i = 0; i < arryFloat.Length; i++)
                        {
                            iIndex = i + 1;
                            if (lstTagInfo_1_FLOAT.TryGetValue(i + 1, out ATagInfo))
                            {
                                sSendMessage = string.Format(sJsonRT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ATagInfo.TAGNAME, arryFloat[i].ToString());
                                if (sendTCPClient.IsConnection)
                                {
                                    sendTCPClient.SendMessage(sSendMessage);
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + sSendMessage);
                                    // ELogger.Info("发送：" + sSendMessage);
                                }
                            }
                        }
                        arryFloat = new float[0];
                        Thread.Sleep(1000);

                        //设备地址1的bool测点数量50
                        arryBool = aModbusClient1.GetTagsBoolValue(1, 50);
                        for (int i = 0; i < arryBool.Length; i++)
                        {
                            if (lstTagInfo_1_BOOL.TryGetValue(i + 1, out ATagInfo))
                            {
                                sSendMessage = string.Format(sJsonRT, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ATagInfo.TAGNAME, arryBool[i].ToString());
                                if (sendTCPClient.IsConnection)
                                {
                                    sendTCPClient.SendMessage(sSendMessage);
                                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + sSendMessage);
                                    //   ELogger.Info("发送：" + sSendMessage);
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
            ELogger.Info("ModBusChannel1初始化" + FileName + "文件TAGLIST个数:" + i.ToString());
            return lstTagInfo;
        }
        private static void StartMainWindow()
        {
            Stopwatch watch2 = new Stopwatch();
            //检查同名进程数
            ELogger.Info(string.Format("ModBusChannel1程序{0}启动...", Application.ProductName));
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            ELogger.Info(string.Format("ModBusChannel1发现进程数{0}", processes.Length));

            //只允许运行1个实例
            if (processes.Length <= 1)
            {
                watch2.Start();
                ELogger.Info(string.Format("ModBusChannel1启动主线程..."));
                watch2.Stop();
                ELogger.Info("ModBusChannel1记录2耗时：" + watch2.ElapsedMilliseconds); //记录5耗时：11889
                ELogger.Info(string.Format("ModBusChannel1主线程结束..."));
            }
            else
            {
                ELogger.Info("ModBusChannel1不允许多实例运行，退出");
                //单实例，不允许多实例运行
                System.Environment.Exit(1);
            }
        }
    }
}
