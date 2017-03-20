namespace AermecNamespace
{
    using System;    
    using System.Threading;
    using EasyModbus;

    class TcpIP
    {
        private byte[] bufferRx;
        private ModbusClient modbusTCPClient;
        public int ResponseTimeout = 0x3e8;
        private int timeOf4Byte;
        private Thread waitReply;

        public TcpIP(ModbusClient modbusTCPClient)
        {
            if (modbusTCPClient == null)
            {
                throw new NullReferenceException("modbusTCPClient = Null");
            }
            this.modbusTCPClient = modbusTCPClient;
            this.bufferRx = new byte[300];
        }

        public void ClosePort()
        {
            this.modbusTCPClient.Disconnect();
        }

        public void OpenPort()
        {
            this.modbusTCPClient.Connect();
        }       

    }
}
