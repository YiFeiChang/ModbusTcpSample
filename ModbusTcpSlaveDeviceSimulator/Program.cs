using NModbus;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ModbusTcpSlaveDeviceSimulator
{
    class Program
    {
        //使用.net core 3.1架構開發 - https://docs.microsoft.com/zh-tw/dotnet/core/introduction
        //使用NModbus開源程式碼實作Modbus通訊 - https://github.com/NModbus/NModbus
        static void Main(string[] args)
        {
            ModbusFactory modbusFactory = new ModbusFactory();
            TcpListener tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, 502));
            IModbusSlaveNetwork modbusSlaveNetwork = modbusFactory.CreateSlaveNetwork(tcpListener);
            IModbusSlave modbusSlave = modbusFactory.CreateSlave(1);
            modbusSlaveNetwork.AddSlave(modbusSlave);
            modbusSlaveNetwork.ListenAsync();
            Process.GetCurrentProcess().WaitForExit();
        }
    }
}