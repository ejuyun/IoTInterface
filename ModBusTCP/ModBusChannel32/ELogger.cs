using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace ModBusTCP
{
    public class ELogger
    {
        private static Logger _logger;

        public static Logger logger
        {
            get
            {
                if (_logger == null)
                    _logger = LogManager.GetCurrentClassLogger();
                return _logger;
            }
        }

        public static void Trace(string msg)
        {
            logger.Trace(msg);
        }

        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        public static void Warn(string msg)
        {
            logger.Warn(msg);
        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        public static void Fatal(string msg)
        {
            logger.Fatal(msg);
        }
    }
}
