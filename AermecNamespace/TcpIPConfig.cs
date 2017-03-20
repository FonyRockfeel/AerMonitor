namespace AermecNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class TcpIPConfig
    {
        public string IPaddress;
        public int Port;
        public int BaudRate;
        public System.IO.Ports.Parity Parity;
        public System.IO.Ports.StopBits StopBits;

        public TcpIPConfig()
        {
            this.IPaddress = "";
            this.Port = 0;
            this.BaudRate = 0x2580;
            this.StopBits = System.IO.Ports.StopBits.Two;
        }

        public TcpIPConfig(string IPaddress, int Port, int baudRate, System.IO.Ports.StopBits stopBits, System.IO.Ports.Parity parity)
        {
            this.IPaddress = IPaddress;
            this.Port = Port; 
            this.BaudRate = baudRate;
            this.StopBits = stopBits;
            this.Parity = parity;
        }

        public TcpIPConfig Clone()
        {
            return new TcpIPConfig
            {
                IPaddress = this.IPaddress,
                Port = this.Port,
                BaudRate = this.BaudRate,
                StopBits = this.StopBits,
                Parity = this.Parity
            };
        }

    }
}
