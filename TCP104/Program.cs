using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace TCP104
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                StartMainWindow();
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                ELogger.Error("Main error:" + ex.Message + ex.StackTrace);
            }
        }
        private static void StartMainWindow()
        {
            Stopwatch watch2 = new Stopwatch();
            //检查同名进程数
            ELogger.Info(string.Format("程序{0}启动...", Application.ProductName));
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            ELogger.Info(string.Format("发现进程数{0}", processes.Length));

            //只允许运行1个实例
            if (processes.Length <= 1)
            {
                watch2.Start();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ELogger.Info(string.Format("启动主线程..."));
                watch2.Stop();
                ELogger.Info("记录2耗时：" + watch2.ElapsedMilliseconds); //记录5耗时：11889
                Application.Run(new Form1());
                ELogger.Info(string.Format("主线程结束..."));
            }
            else
            {
                ELogger.Info("不允许多实例运行，退出");
                //单实例，不允许多实例运行
                System.Environment.Exit(1);
            }

        }
        
    }
}
