using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCP104
{
    public class ASDUClass
    {
        /// <summary>
        /// 数据点类型枚举
        /// </summary>
        public enum FunType
        {
            //在监视方向的过程信息
            /// <summary>
            /// 单遥信息
            /// </summary>
            Single_point = 1,
            /// <summary>
            /// 双点信息
            /// </summary>
            Double_point = 3,
            /// <summary>
            /// 步位置信息
            /// </summary>
            BuSit = 5,
            /// <summary>
            /// 规一化测量值
            /// </summary>
            BackMValue = 9,
            /// <summary>
            /// 标度化测量值
            /// </summary>
            BDMValue = 11,  //0x0b
            /// <summary>
            /// 短浮点型测量值,带品质描述
            /// </summary>
            FloatMValue = 13,  //0x0d
            /// <summary>
            /// 累积值
            /// </summary>
            EnergyValue = 15, //0x0f
            /// <summary>
            /// 带时标单点信息
            /// </summary>
            Time_Single_point = 30,
            /// <summary>
            /// 带时标双点信息
            /// </summary>
            Time_Double_point = 31,
            /// <summary>
            /// 带时标步位置信息
            /// </summary>
            Time_BuSit = 32,
            /// <summary>
            /// 带时标步位置信息
            /// </summary>
            Time_Bu32Bits = 33,
            /// <summary>
            /// 带时标规一化测量值
            /// </summary>
            Time_BackMValue = 34,
            /// <summary>
            /// 带时标标度化测量值
            /// </summary>
            Time_BDMValue = 35,
            /// <summary>
            /// 带时标短浮点型测量值,带品质描述
            /// </summary>
            Time_FloatMValue = 36,
            /// <summary>
            /// 带时标累积值,电量数值遥脉
            /// </summary>
            Time_EnergyValue = 37,
            /// <summary>
            /// 带时标继电器保护装置事件
            /// </summary>
            Time_RelayEvent = 38,
            /// <summary>
            /// 带时标继电器保护装置成组启动事件
            /// </summary>
            Time_RelayStartEvents = 39,
            /// <summary>
            /// 带时标继电器保护装置成组出口信息
            /// </summary>
            Time_RelayOutputInfos = 40,

            //在控制方向的过程信息            
            /// <summary>
            ///  单命令C_SC_NA_1
            /// </summary>
            SingleCMD = 45,
            /// <summary>
            ///  双命令C_DC_NA_1
            /// </summary>
            DoubleCMD = 46,
            /// <summary>
            ///  升降命令C_RC_NA_1
            /// </summary>
            UpOrDownCMD = 47,
            /// <summary>
            ///  设点命令，规一化值 C_SE_NA_1
            /// </summary>
            SetBackCMD = 48,
            /// <summary>
            ///  设点命令，标度化值 C_SE_NB_1
            /// </summary>
            SetBDCMD = 49,
            /// <summary>
            ///  设点命令，短浮点数 C_SE_NC_1
            /// </summary>
            SetFloatCMD = 50,
            /// <summary>
            ///  32比特串C_BO_NA_1
            /// </summary>
            Set32Bits = 51,
            //52..57  保留

            //在控制方向的过程信息，带时标的ASDU
            /// <summary>
            ///  带时标CP56Time2a的单命令 C_SC_NA_1
            /// </summary>
            Time_SingleCMD = 58,
            /// <summary>
            ///  带时标CP56Time2a的双命令 C_DC_NA_1
            /// </summary>
            Time_DoubleCMD = 59,
            /// <summary>
            ///  带时标CP56Time2a的升降命令C_RC_NA_1
            /// </summary>
            Time_UpOrDownCMD = 60,
            /// <summary>
            ///  带时标CP56Time2a的设点命令，规一化值 C_SE_TA_1
            /// </summary>
            Time_SetBackCMD = 61,
            /// <summary>
            ///  带时标CP56Time2a的设点命令，标度化值 C_SE_TB_1
            /// </summary>
            Time_SetBDCMD = 62,
            /// <summary>
            ///  带时标CP56Time2a的设点命令，短浮点数 C_SE_TC_1
            /// </summary>
            Time_SetFloatCMD = 63,
            /// <summary>
            ///  带时标CP56Time2a的32比特串 C_BO_NA_1
            /// </summary>
            Time_Set32Bits = 64,

            //在监视方向的系统信息            
            /// <summary>
            /// 初始化结束 M_EI_NA_1
            /// </summary>
            InitOver = 70,

            //在控制方向的系统信息            
            /// <summary>
            /// 总召C_IC_NA_1
            /// </summary>
            CalAll = 100,
            /// <summary>
            ///  电能脉冲召唤命令C_CI_NA_1
            /// </summary>
            CalEnergyPulse = 101,
            /// <summary>
            ///  读命令 C_RD_NA_1
            /// </summary>
            ReadCMD = 102,
            /// <summary>
            ///  时钟同步命令 C_CS_NA_1
            /// </summary>
            ClockConfirm = 103,
            /// <summary>
            ///  复位进程命令 C_RP_NA_1
            /// </summary>
            ResetProcess = 105,
            /// <summary>
            ///  带时标CP56Time2a的测试命令 C_TS_NA_1
            /// </summary>
            Time_Test = 107,
            //108..109 保留       
     
            //在控制方向的参数            
            //类型标识  UI8[1..8]110..119            
            /// <summary>
            ///  测量值参数，规一化值 P_ME_NA_1
            /// </summary>
            MValueBackPara = 110,
            /// <summary>
            ///  测量值参数，标度化值 P_ME_NB_1
            /// </summary>
            MValueBDPara = 111,
            /// <summary>
            ///  测量值参数，短浮点数 P_ME_NC_1
            /// </summary>
            MValueFloatPara = 112,
            /// <summary>
            ///  参数激活P_AC_NA_1
            /// </summary>
            MValueStartPara = 113,
            //114..119  保留

            //文件传输            
            /// <summary>
            ///  文件已准备好F_FR_NA_1
            /// </summary>
            FileReady = 120,
            /// <summary>
            ///  节已准备好F_SR_NA_1
            /// </summary>
            PareReady = 121,
            /// <summary>
            ///  召唤目录，选择文件，召唤文件，召唤节 F_SC_NA_1
            /// </summary>
            CalMenu = 122,
            /// <summary>
            ///  最后的节，最后的段 F_LS_NA_1
            /// </summary>
            LastPara = 123,
            /// <summary>
            ///  确认文件，确认节 F_AF_NA_1
            /// </summary>
            ConfirmFile = 124,
            /// <summary>
            ///  段 F_SG_NA_1
            /// </summary>
            Para = 125,
            /// <summary>
            ///  目录 F_DR_NA_1
            /// </summary>
            Menu = 126,
            //127  保留
            /*
            //在监视方向的过程信息
            Single_point = 1,//单点信息
            Double_point = 3,//双点信息
            BuSit = 5,//步位置信息
            BackMValue = 9,//规一化测量值
            BDMValue = 11,//标度化测量值
            FloatMValue = 13,//短浮点型测量值
            AddValue = 15,//累积值
            Time_Single_point = 30,//带时标单点信息
            Time_Double_point = 31,//带时标双点信息
            Time_BuSit = 32,//带时标步位置信息
            Time_Bu32Bits = 33,//带时标步位置信息
            Time_BackMValue = 34,//带时标规一化测量值
            Time_BDMValue = 35,//带时标标度化测量值
            Time_FloatMValue = 36,//带时标短浮点型测量值
            Time_AddValue = 37,//带时标累积值
            Time_RelayEvent = 38,//带时标继电器保护装置事件
            Time_RelayStartEvents = 39,//带时标继电器保护装置成组启动事件
            Time_RelayOutputInfos = 40,//带时标继电器保护装置成组出口信息

            //在控制方向的过程信息
            SingleCMD = 45, // 单命令C_SC_NA_1
            DoubleCMD = 46, // 双命令C_DC_NA_1
            UpOrDownCMD = 47, // 升降命令C_RC_NA_1
            SetBackCMD = 48, // 设点命令，规一化值 C_SE_NA_1
            SetBDCMD = 49, // 设点命令，标度化值 C_SE_NB_1
            SetFloatCMD = 50, // 设点命令，短浮点数 C_SE_NC_1
            Set32Bits = 51, // 32比特串C_BO_NA_1
            //52..57  保留
            //在控制方向的过程信息，带时标的ASDU
            Time_SingleCMD = 58, // 带时标CP56Time2a的单命令 C_SC_NA_1
            Time_DoubleCMD = 59, // 带时标CP56Time2a的双命令 C_DC_NA_1
            Time_UpOrDownCMD = 60, // 带时标CP56Time2a的升降命令C_RC_NA_1
            Time_SetBackCMD = 61, // 带时标CP56Time2a的设点命令，规一化值 C_SE_TA_1
            Time_SetBDCMD = 62, // 带时标CP56Time2a的设点命令，标度化值 C_SE_TB_1
            Time_SetFloatCMD = 63, // 带时标CP56Time2a的设点命令，短浮点数 C_SE_TC_1
            Time_Set32Bits = 64, // 带时标CP56Time2a的32比特串 C_BO_NA_1

            //在监视方向的系统信息
            InitOver = 70,//初始化结束 M_EI_NA_1

            //在控制方向的系统信息
            CalAll = 100,//总召C_IC_NA_1
            CalEnergyPulse = 101, // 电能脉冲召唤命令C_CI_NA_1
            ReadCMD = 102, // 读命令 C_RD_NA_1
            ClockConfirm = 103, // 时钟同步命令 C_CS_NA_1
            ResetProcess = 105, // 复位进程命令 C_RP_NA_1
            Time_Test = 107, // 带时标CP56Time2a的测试命令 C_TS_NA_1
            //108..109 保留

            //在控制方向的参数
            //类型标识  UI8[1..8]110..119
            MValueBackPara = 110, // 测量值参数，规一化值 P_ME_NA_1
            MValueBDPara = 111, // 测量值参数，标度化值 P_ME_NB_1
            MValueFloatPara = 112, // 测量值参数，短浮点数 P_ME_NC_1
            MValueStartPara = 113, // 参数激活P_AC_NA_1
            //114..119  保留

            //文件传输
            FileReady = 120, // 文件已准备好F_FR_NA_1
            PareReady = 121, // 节已准备好F_SR_NA_1
            CalMenu = 122, // 召唤目录，选择文件，召唤文件，召唤节 F_SC_NA_1
            LastPara = 123, // 最后的节，最后的段 F_LS_NA_1
            ConfirmFile = 124, // 确认文件，确认节 F_AF_NA_1
            Para = 125, // 段 F_SG_NA_1
            Menu = 126, // 目录 F_DR_NA_1
            //127  保留
             * */
        }
        /// <summary>
        /// 传输原因枚举
        /// </summary>
        public enum TransRes
        {
            /// <summary>
            /// 未定义
            /// </summary>
            UnDef = 0,//未定义
            /// <summary>
            /// 周期扫描
            /// </summary>
            Rate = 1,//周期扫描
            /// <summary>
            /// 背景扫描
            /// </summary>
            BackGroundScan = 2,//背景扫描
            /// <summary>
            /// 突发
            /// </summary>
            AutoSend = 3,//突发
            /// <summary>
            /// 初始化
            /// </summary>
            Init = 4,//初始化
            /// <summary>
            /// 请求或被请求
            /// </summary>
            Request = 5,//请求或被请求
            /// <summary>
            /// 激活
            /// </summary>
            Active = 6,//激活
            /// <summary>
            /// 激活确认
            /// </summary>
            ActiveConfirm = 7,//激活确认
            /// <summary>
            /// 激活停止
            /// </summary>
            ActiveStop = 8,//激活停止
            /// <summary>
            /// 激活停止确认
            /// </summary>
            ActiveStopConfirm = 9,//激活停止确认
            /// <summary>
            /// 激活结束
            /// </summary>
            ActiveEnd = 10,//激活结束
            /// <summary>
            /// 响应总召唤
            /// </summary>
            ResAll = 20,//响应总召唤

            EnergyCall = 37  //电量总召唤
        }

        //byte 类型标识，0
        private FunType type;
        //?? 可变结构体限定词 1
        private byte sqn;
        //U16 传输原因 2，3(0)
        private byte transRes;
        /// <summary>
        /// 设置传输原因
        /// </summary>
        /// <param name="yes_no">是否肯定/否定</param>
        /// <param name="isTest">是否测试</param>
        /// <param name="type">传输原因</param>
        /// <returns>设置是否成功</returns>
        public bool SetTransRes(bool yes_no, bool isTest, TransRes type)
        {
            transRes = (byte)(Convert.ToByte(type) | (isTest?(1<<7):0) | (yes_no?(1<<6):0));
            return true;
        }
        /// <summary>
        /// 获得传输原因
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public TransRes GetTransRes(byte data)
        {
            return (TransRes)(data & 0x3f);
        }
        /// <summary>
        /// 是否肯定/否定
        /// </summary>
        /// <param name="data">从本数据获取</param>
        /// <returns>肯定/否定</returns>
        public bool GetYes_No(byte data)
        {
            return (data & 0x80)==1;
        }
        /// <summary>
        /// 是否测试过
        /// </summary>
        /// <param name="data">从本数据获取</param>
        /// <returns>测试/无测试</returns>
        public bool GetIsTest(byte data)
        {
            return (data & 0x40) == 1;
        }

        //U16,低位在前 RTU地址 4，5
        int apduAddr = 1;
        //数据集
        List<DataStruct> data = new List<DataStruct>();
        //数据集
        public List<DataStruct> Data
        {
            get { return data; }
        }
        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="active">激活/行动类型(传输原因)</param>
        /// <param name="funType">功能(数据类型)</param>
        public void Pack(TransRes active, FunType funType)
        {
            switch (active)
            {
                case TransRes.Active:
                    {
                        switch (funType)
                        {
                            case FunType.ClockConfirm:
                                this.sqn = 1;
                                this.SetTransRes(false, false, active);
                                this.type = funType;
                                this.apduAddr = 1;
                                this.data.Add(new DataStruct() { Addr = 0, Time = DateTime.Now });
                                break;
                            case FunType.CalAll:
                                this.sqn = 1;
                                this.SetTransRes(false, false, active);
                                this.type = funType;
                                this.apduAddr = 1;
                                this.data.Add(new DataStruct() { Addr = 0, Data = 0x14 });
                                break;
                            case FunType.CalEnergyPulse:
                                this.sqn = 1;
                                this.SetTransRes(false,false,active);
                                this.type = funType;
                                this.apduAddr = 1;  //0x6401
                                this.data.Add(new DataStruct() { Addr = 0, Data = 0x05 });
                                break;
                        }
                    }
                    break;
                case TransRes.ActiveEnd:
                    break;
                case TransRes.ActiveStop:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 解包协议
        /// </summary>
        /// <param name="buffer">待解包的数组</param>
        /// <param name="startSit">ASDU起始位置</param>
        /// <param name="lenth">长度</param>
        /// <returns></returns>
        public TransRes UnPack(byte[] buffer, int startSit, int lenth)
        {
            if ((lenth + startSit > buffer.Length)||(lenth - startSit < 4))
            {
                return TransRes.UnDef;
            }
            this.type = (FunType)buffer[startSit];
            this.sqn = buffer[startSit + 1];
            this.transRes = buffer[startSit + 2];
            this.apduAddr = buffer[startSit + 4];
            TransRes res = this.GetTransRes(this.transRes);
            switch (res)
            {
                case TransRes.ResAll:
                    GetDataUnpack(buffer, startSit + 6, lenth - 6);
                    break;
                case TransRes.AutoSend:
                    GetDataUnpack(buffer, startSit + 6, lenth - 6);
                    break;
                case TransRes.ActiveEnd:
                    break;
                case TransRes.Active:
                    GetDataUnpack(buffer, startSit + 6, lenth - 6);
                    break;
                case TransRes.ActiveConfirm:
                    GetDataUnpack(buffer, startSit + 6, lenth - 6);
                    break;
                case TransRes.EnergyCall:
                    GetDataUnpack(buffer, startSit + 6, lenth - 6);
                    break;
                default :
                    return TransRes.UnDef;
            }
            return res;
        }
        /// <summary>
        /// 解包数据
        /// </summary>
        /// <param name="buffer">待解包的数组</param>
        /// <param name="startSit">起始位置</param>
        /// <param name="lenth">长度</param>
        private void GetDataUnpack(byte[] buffer, int startSit, int lenth)
        {
            data.Clear();
            switch (this.type)
            {
                case FunType.Single_point:
                    if ((this.sqn & 0x80) == 0x80)
                    {
                        QueGetSinglePoint(buffer, startSit + 3, sqn & 0x7f, GetAddr(buffer,startSit));
                    }
                    else
                    {
                        GetSinglePoint(buffer, startSit, sqn & 0x7f);
                    }
                    break;
                case FunType.FloatMValue:
                    if ((this.sqn & 0x80) == 0x80)
                    {
                        QueGetFloatMValue(buffer, startSit + 3, sqn & 0x7f, GetAddr(buffer, startSit));
                    }
                    else
                    {
                        GetFloatMValue(buffer, startSit, sqn & 0x7f);
                    }
                    break;
                case FunType.EnergyValue:
                    if ((this.sqn & 0x80) == 0x80)
                    {
                        Console.WriteLine("顺序遥脉");
                        QueGetEngValue(buffer, startSit + 3, sqn & 0x7f, GetAddr(buffer, startSit));
                    }
                    else
                    {
                        Console.WriteLine("非顺序遥脉");
                        GetEnergyPulse(buffer, startSit, sqn & 0x7f);
                    }
                    break;
                case FunType.Time_Single_point:
                    if ((this.sqn & 0x80) == 0x80)
                    {
                        QueGetTimeSinglePoint(buffer, startSit + 3, sqn & 0x7f, GetAddr(buffer, startSit));
                    }
                    else
                    {
                        GetTimeSinglePoint(buffer, startSit, sqn & 0x7f);
                    }
                    break;
                case FunType.Time_FloatMValue:
                    if ((this.sqn & 0x80) == 0x80)
                    {
                        QueGetTimeFloatMValue(buffer, startSit + 3, sqn & 0x7f, GetAddr(buffer, startSit));
                    }
                    else
                    {
                        GetTimeFloatMValue(buffer, startSit, sqn & 0x7f);
                    }
                    break;
                case FunType.ClockConfirm:
                    data.Add(new DataStruct() { Addr = 0, Time = GetDateTime(buffer,startSit + 3) });
                    break;
                default:
                    Console.WriteLine("无效动作标识："+ this.type.ToString());
                    break;
            }
        }
        /// <summary>
        /// 顺序解包单点数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        /// <param name="startAddr"></param>
        private void QueGetSinglePoint(byte[] buffer, int startSit, int lenth, int startAddr)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = startAddr + i, Data = buffer[startSit + i] });
            }
        }
        /// <summary>
        /// 顺序解包浮点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        /// <param name="startAddr"></param>
        private void QueGetFloatMValue(byte[] buffer, int startSit, int lenth, int startAddr)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = startAddr + i, Data = BitConverter.ToSingle(buffer, startSit + 5 * i), Quality = (DataStruct.QualityType)buffer[startSit + 5 * i + 4] });
            }
        }
        /// <summary>
        /// 顺序解包遥脉测点值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        /// <param name="startAddr"></param>
        private void QueGetEngValue(byte[] buffer, int startSit, int lenth, int startAddr)
        {
            for (int i = 0; i < lenth; i++)
            {
                //数值需要反转
            //    byte[] tempbyte = new byte[4];
           //     Array.Copy(buffer, startSit + 5 * i, tempbyte,0, 4);
           //     Array.Reverse(tempbyte);
               data.Add(new DataStruct() { Addr = startAddr + i, Data = BitConverter.ToUInt32(buffer, startSit + 5 * i), Quality = (DataStruct.QualityType)buffer[startSit + 5 * i+4] });
         //    data.Add(new DataStruct() { Addr = startAddr + i, Data = BitConverter.ToSingle(buffer, startSit + 5 * i), Quality = (DataStruct.QualityType)buffer[startSit + 5 * i + 4] });
            }
        }

        /// <summary>
        /// 顺序解包含时标的单点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        /// <param name="startAddr"></param>
        private void QueGetTimeSinglePoint(byte[] buffer, int startSit, int lenth, int startAddr)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = startAddr + i, Data = buffer[startSit + 8 * i], Time = GetDateTime(buffer, startSit + 8 * i + 1) });
            }
        }
        /// <summary>
        /// 顺序解包含时标的浮点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        /// <param name="startAddr"></param>
        private void QueGetTimeFloatMValue(byte[] buffer, int startSit, int lenth, int startAddr)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = startAddr + i, Data = BitConverter.ToSingle(buffer, startSit + 12 * i), Time = GetDateTime(buffer, startSit + 12 * i + 4), Quality = (DataStruct.QualityType)buffer[startSit + 12 * i + 11] });
            }
        }
        /// <summary>
        /// 非顺序解包单点数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        private void GetSinglePoint(byte[] buffer, int startSit, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = GetAddr(buffer, startSit + 4 * i), Data = buffer[startSit + 4 * i + 3] });
            }
        }
        /// <summary>
        /// 非顺序解包浮点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        private void GetFloatMValue(byte[] buffer, int startSit, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = GetAddr(buffer, startSit + 8 * i), Data = BitConverter.ToSingle(buffer, startSit + 8 * i + 3), Quality = (DataStruct.QualityType)buffer[startSit + 8 * i + 7] });
            }
        }

        private void GetEnergyPulse(byte[] buffer, int startSit, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = GetAddr(buffer, startSit + 8 * i), Data = BitConverter.ToSingle(buffer, startSit + 8 * i + 3), Quality = (DataStruct.QualityType)buffer[startSit + 8 * i + 7] });
            }
        }
        /// <summary>
        /// 非顺序解包含时标的单点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        private void GetTimeSinglePoint(byte[] buffer, int startSit, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = GetAddr(buffer, startSit + 11 * i), Data = buffer[startSit + 11 * i + 3], Time = GetDateTime(buffer, startSit + 11 * i + 4) });
            }
        }
        /// <summary>
        /// 非顺序解包含时标的浮点测量值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <param name="lenth"></param>
        private void GetTimeFloatMValue(byte[] buffer, int startSit, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                data.Add(new DataStruct() { Addr = GetAddr(buffer, startSit + 14 * i), Data = BitConverter.ToSingle(buffer, startSit + 15 * i + 3), Time = GetDateTime(buffer, startSit + 15 * i + 7), Quality = (DataStruct.QualityType)buffer[startSit + 15 * i + 14] });
            }
        }
        /// <summary>
        /// 解析时间标志
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <returns></returns>
        private DateTime GetDateTime(byte[] buffer, int startSit)
        {
            try
            {
                int ms = buffer[startSit + 1] * 256 + buffer[startSit + 0];
                DateTime datetime = new DateTime
                    (2000 + buffer[startSit + 6], buffer[startSit + 5], buffer[startSit + 4] & 0x1f, buffer[startSit + 3],
                    buffer[startSit + 2], ms / 1000, ms % 1000);
                return datetime;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return new DateTime();
            }
        }
        /// <summary>
        /// 解析数据地址
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startSit"></param>
        /// <returns></returns>
        private int GetAddr(byte[] buffer, int startSit)
        {
            return (buffer[startSit + 2] << 16) + (buffer[startSit + 1] << 8) + buffer[startSit];
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            List<byte> temp = new List<byte>();
            temp.Add((byte)this.type);
            temp.Add(this.sqn);
            temp.Add(this.transRes);
            temp.Add(0);
            byte[] byteAddr = BitConverter.GetBytes(this.apduAddr);
            temp.Add(byteAddr[0]);
            if (byteAddr.Length>1)
            {
                temp.Add(byteAddr[1]);
            }
            else temp.Add(0);
            foreach (var member in data)
            {
                if (member.Addr != null)
                {
                    temp.Add((byte)(member.Addr & 0x000000ff));
                    temp.Add((byte)(member.Addr & 0x0000ff00));
                    temp.Add((byte)(member.Addr & 0x00ff0000));
                }
                if (member.Data != null)
                {
                    if (member.DataLenth == 1)
                    {
                        temp.Add((byte)member.Data);
                    }
                }
                if (member.Time != null)
                {
                    temp.Add((byte)((member.Time.Value.Millisecond + member.Time.Value.Second * 1000) & 0x000000ff));
                    temp.Add((byte)((member.Time.Value.Millisecond + member.Time.Value.Second * 1000) & 0x0000ff00));
                    temp.Add((byte)(member.Time.Value.Minute));
                    temp.Add((byte)(member.Time.Value.Hour));
                    temp.Add((byte)(member.Time.Value.Day + (((int)member.Time.Value.DayOfWeek << 6)&0xe0)));
                    temp.Add((byte)(member.Time.Value.Month));
                    temp.Add((byte)(member.Time.Value.Year % 2000));
                }
                if (member.Quality != null)
                {
                    temp.Add((byte)member.Quality);
                }
            }
            return temp.ToArray();
        }
        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("\r\nASDU:");
            byte[] temp = this.ToArray();
            for (int i = 0; i < temp.Length; i++)
            {
                sb.Append("0x" + temp[i].ToString("X2") + " ");
            }
            return sb.ToString();
        }
    }

    public class DataStruct
    {
        //长度
        private int _dataLenth = 1;
        public int DataLenth
        {
            get { return _dataLenth; }
            set { _dataLenth = value; }
        }
        //地址
        private int _addr;
        public int Addr
        {
            get { return _addr; }
            set { _addr = value; }
        }
        //数据
        private double? _data;
        public double? Data
        {
            get { return _data; }
            set { _data = value; }
        }
        //时间
        private DateTime? _time;
        public DateTime? Time
        {
            get { return _time; }
            set { _time = value; }
        }
        //数据质量
        private QualityType? _quality;
        public QualityType? Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }
        public enum QualityType
        {
            OK = 0,
            UK1 = 1,
            UK2 = 2,
            UK3 = 3,
            UK4 = 4,
            UK5 = 5,
            UK6 = 6,
        }
    }
}
