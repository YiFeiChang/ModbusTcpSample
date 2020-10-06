using NModbus;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ModbusTcpMasterDeviceSimulator
{
    class Program
    {
        //使用.net core 3.1架構開發 - https://docs.microsoft.com/zh-tw/dotnet/core/introduction
        //使用NModbus開源程式碼實作Modbus通訊 - https://github.com/NModbus/NModbus
        static void Main(string[] args)
        {
            ModbusFactory modbusFactory = new ModbusFactory();
            TcpClient tcpClient = new TcpClient();
            //測試裝置IP為192.168.50.2。PLC設置為動態IP，由路由器綁定MAC位址
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("192.168.50.2"), 502));
            IModbusMaster modbusMaster = modbusFactory.CreateMaster(tcpClient);
            modbusMaster.Transport.ReadTimeout = 1000;//讀取超時，讀取時超過1000ms發出異常
            modbusMaster.Transport.WriteTimeout = 1000;//寫入超時，寫入時超過1000ms發出異常
            modbusMaster.Transport.Retries = 3;//重試3次，讀取或寫入失敗時，會重試次數

            while (true)
            {
                //D區單一位址寫入
                modbusMaster.WriteSingleRegister(
                    slaveAddress: byte.MaxValue,//從站位址，台達PLC實測可使用任意數值，不宜大於255
                    registerAddress: 4096 + 0,//D0的Modbus位址，台達PLC從4096開始
                    value: (ushort)DateTime.Now.Hour//寫入ushort數值
                    );
                modbusMaster.WriteSingleRegister(
                    slaveAddress: byte.MinValue,
                    registerAddress: 4096 + 1,//D1的Modbus位址
                    value: (ushort)DateTime.Now.Minute
                    );
                modbusMaster.WriteSingleRegister(
                    slaveAddress: 100,//從站位址，台達PLC實測可使用任意數值，不宜大於255
                    registerAddress: 4096 + 2,//D2的Modbus位址，台達PLC從4096開始
                    value: (ushort)DateTime.Now.Second
                    );
                //D區多位址寫入
                modbusMaster.WriteMultipleRegisters(
                    slaveAddress: 1,
                    startAddress: 4096 + 10,//起始位址在D10
                    data: new ushort[] { (ushort)DateTime.Now.Hour, (ushort)DateTime.Now.Minute, (ushort)DateTime.Now.Second }
                    );
                //D區讀取
                ushort[] holdingRegisters = modbusMaster.ReadHoldingRegisters(
                    slaveAddress: 1,
                    startAddress: 4096 + 100,//起始位址在D100
                    numberOfPoints: 100//讀取長度，大於1，不宜過大
                    );
                Console.Clear();
                Console.WriteLine("PLC D100數值：" + holdingRegisters[0].ToString());
                Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
            }
        }
    }
}