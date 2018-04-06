using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusLib;
using ModbusLib.Protocols;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace ModBusTCP
{
    public enum ValueType
    {
        Float,
        Bool
    }
    public class ModbusMaster
    {
        Socket _socket;
        ICommClient _portClient;
        ModbusClient _driver;
        UInt16[] _registerData = new UInt16[65600];
        int _transactionId = 0;
        
        public ModbusMaster(int aSalveID)
        {
            IPAddress IPAddress = IPAddress.Parse(ConfigurationManager.AppSettings["MODBUSSERVERIP"]);
            int TCPPort = Convert.ToInt32(ConfigurationManager.AppSettings["MODBUSSERVERPORT"]);
            byte SlaveId = Byte.Parse(aSalveID.ToString()); //32
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _socket.SendTimeout = 2000;
            _socket.ReceiveTimeout = 2000;
            _socket.Connect(new IPEndPoint(IPAddress, TCPPort));
            _portClient = _socket.GetClient();
            _driver = new ModbusClient(new ModbusTcpCodec())
            {
                Address = SlaveId
            };
            _driver.OutgoingData += DriverOutgoingData;
            _driver.IncommingData += DriverIncommingData;
            Console.WriteLine(String.Format("SlaveID:"+aSalveID.ToString()+"Connected using TCP to {0}", _socket.RemoteEndPoint));
        }
        public float[] GetTagsFloatValue(short aStartAddress, int aTagCount, bool IsReverse)
        {
            float[] TagsFloatValues = new float[aTagCount];
            ushort[] data = new ushort[0];
            int iTagAddress = aStartAddress;
            int iLoop = (int)Math.Ceiling((double)aTagCount / (double)50);
            int iCount = 0; 
            for (int i=0;i<iLoop;i++)
            {
                if (iLoop.Equals(1))
                {
                    iCount = aTagCount;
                }
                else if (iLoop.Equals(i + 1))
                {
                    iCount = aTagCount - i * 50;
                }
                else iCount = 50;
                iTagAddress = aStartAddress + i * 50 * 2;
                data = GetTagsValueMax(ValueType.Float, iTagAddress, iCount);
                float[] arrayData = GetTagsFloatValue(iCount, data, IsReverse);
                Array.Copy(arrayData, 0, TagsFloatValues, i * 50, arrayData.Length);
            }
            return TagsFloatValues;
        }

        public bool[] GetTagsBoolValue(short aStartAddress, int aTagCount)
        {
            bool[] TagsBoolValues = new bool[aTagCount];
            ushort[] data = new ushort[0];
            int iTagAddress = 0;
            int iLoop = (int)Math.Ceiling((double)aTagCount / (double)2000);
            int iCount = 0;
            for (int i = 0; i < iLoop; i++)
            {
                iTagAddress = aStartAddress + i * 2000;
                if (iLoop.Equals(1))
                {
                    iCount = aTagCount;
                }
                else if (iLoop.Equals(i + 1))
                {
                    iCount = aTagCount - i * 2000;
                }
                else iCount = 2000;
                data = GetTagsValueMax(ValueType.Bool, iTagAddress, iCount);
                bool[] arrayData = GetTagsBoolValue(iCount, data);
                Array.Copy(arrayData, 0, TagsBoolValues, i * 2000, arrayData.Length);
            }
            return TagsBoolValues;
        }

        private ushort[] GetTagsValueMax(ValueType aValueType, int aStartAddress, int aTagCount)
        {
            byte function;
            int ModbusCount = 0;
            if (aValueType.Equals(ValueType.Bool))
            {
                function = ModbusCommand.FuncReadCoils;
                ModbusCount = aTagCount;
            }
            else
            {
                function = ModbusCommand.FuncReadMultipleRegisters;
                ModbusCount = aTagCount * 2;
            }
            var data = new ushort[ModbusCount];
            var command = new ModbusCommand(function) { Offset = aStartAddress, Count = ModbusCount, TransId = _transactionId++ };
            var result = _driver.ExecuteGeneric(_portClient, command);
            if (result.Status == CommResponse.Ack)
            {
                command.Data.CopyTo(_registerData, aStartAddress);
                for (int i = 0; i < ModbusCount; i++)
                {
                    var index = aStartAddress + i;
                    if (index >= _registerData.Length)
                    {
                        break;
                    }
                    data[i] = _registerData[index];
                }
                Console.WriteLine(String.Format("Read succeeded: Function code:{0}.", function));
            }
            else
            {
                Console.WriteLine(String.Format("Failed to execute Read: Error code:{0}", result.Status));
            }
            return data;
        }

        private bool[] GetTagsBoolValue(int aTagCount, ushort[] data)
        {
            bool[] STagValue = new bool[aTagCount];
            for (int i = 0; i < aTagCount; i++)
            {
                var bulbNumber = Convert.ToInt16(i);
                var index = bulbNumber / 16;
                var bulbHiByte = (bulbNumber & 0x0008) != 0;
                var shift = bulbNumber & 0x0007;
                ushort shifter = bulbHiByte ? (ushort)0x0001 : (ushort)0x0100;
                var mask = Convert.ToUInt16(shifter << shift);
                STagValue[i] = (mask & data[index]) != 0;
              //  Console.WriteLine("测点" + i.ToString() + "=" + STagValue[i].ToString());
            }
            return STagValue;
        }

        private float[] GetTagsFloatValue(int aTagCount, ushort[] data, bool IsReverse)
        {
            float[] FTagValue = new float[aTagCount];
            for (int i = 0; i < aTagCount; i++)
            {
                if (IsReverse)
                {
                    FTagValue[i] = ushortArryToSingleReverse(data[2 * i], data[2 * i + 1]);
                }
                else
                {
                    FTagValue[i] = ushortArryToSingle(data[2 * i], data[2 * i + 1]);
                }
             //   Console.WriteLine("测点" + i.ToString() + "=" + FTagValue[i].ToString());
            }
            return FTagValue;

        }

        public void Close()
        {
            //断开连接
            if (_socket != null)
            {
                _socket.Close();
                _socket.Dispose();
                _socket = null;
            }
            _portClient = null;
            _driver = null;
        }

        public void DriverIncommingData(byte[] data)
        {
            var hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2} ", b);
            Console.WriteLine(String.Format("RX: {0}", hex));
        }

        public void DriverOutgoingData(byte[] data)
        {
            var hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2} ", b);
            Console.WriteLine(String.Format("TX: {0}", hex));
        }

        /// <summary>
        /// 反转ushort[2]转float
        /// </summary>
        /// <param name="U1"></param>
        /// <param name="U2"></param>
        /// <returns></returns>
        public Single ushortArryToSingleReverse(ushort U1, ushort U2)
        {
            ushort[] arryshort = new ushort[2] { U1, U2 };
            byte[] arrybyte1 = BitConverter.GetBytes(arryshort[0]);
            byte[] arrybyte2 = BitConverter.GetBytes(arryshort[1]);
            byte[] arrybyte3 = new byte[4];
            arrybyte3[0] = arrybyte2[0];
            arrybyte3[1] = arrybyte2[1];
            arrybyte3[2] = arrybyte1[0];
            arrybyte3[3] = arrybyte1[1];
            return BitConverter.ToSingle(arrybyte3, 0);
        }
        /// <summary>
        /// 顺转ushort[2]转float
        /// </summary>
        /// <param name="U1"></param>
        /// <param name="U2"></param>
        /// <returns></returns>
        public Single ushortArryToSingle(ushort U1, ushort U2)
        {
            ushort[] arryshort = new ushort[2] { U1, U2 };
            byte[] arrybyte1 = BitConverter.GetBytes(arryshort[0]);
            byte[] arrybyte2 = BitConverter.GetBytes(arryshort[1]);
            byte[] arrybyte3 = new byte[4];
            arrybyte3[0] = arrybyte1[0];
            arrybyte3[1] = arrybyte1[1];
            arrybyte3[2] = arrybyte2[0];
            arrybyte3[3] = arrybyte2[1];
            return BitConverter.ToSingle(arrybyte3, 0);
        }
    }
}
