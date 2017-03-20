namespace AermecNamespace
{
    using System;
    using System.IO.Ports;

    [Serializable]
    public class SerialConfig
    {
        public int BaudRate;
        public string ComPort;
        public System.IO.Ports.Parity Parity;
        public System.IO.Ports.StopBits StopBits;

        public SerialConfig()
        {
            this.ComPort = "";
            this.BaudRate = 0x2580;
            this.StopBits = System.IO.Ports.StopBits.Two;
        }

        public SerialConfig(string comPort, int baudRate, System.IO.Ports.StopBits stopBits, System.IO.Ports.Parity parity)
        {
            this.ComPort = "";
            this.BaudRate = 0x2580;
            this.StopBits = System.IO.Ports.StopBits.Two;
            this.ComPort = comPort;
            this.BaudRate = baudRate;
            this.StopBits = stopBits;
            this.Parity = parity;
        }

        public SerialConfig Clone()
        {
            return new SerialConfig { 
                ComPort = this.ComPort,
                BaudRate = this.BaudRate,
                StopBits = this.StopBits,
                Parity = this.Parity
            };
        }
    }
}

