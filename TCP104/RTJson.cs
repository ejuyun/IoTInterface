using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCP104
{
    public struct TagInfo
    {
        public int ADDRNO;  //104设备取数地址编号
        public string TAGNAME;  //测点编码
        public string TAGMC;   //测点名称
        public string TAGLX;  //测点类型：A-模拟量,S-开关量
    }

    public class RtItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string tagname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double value { get; set; }

    }

    public class RTJson
    {
        /// <summary>
        /// 
        /// </summary>
        public string dt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RtItem> rt { get; set; }

    }
}
