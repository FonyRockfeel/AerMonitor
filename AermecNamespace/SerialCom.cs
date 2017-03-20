namespace AermecNamespace
{
    using System;
    using System.IO.Ports;
    using System.Net.Sockets;
    using System.Threading;

    public class SerialCom
    {
        private byte[] bufferRx;
        private SerialPort comPort;
        private TcpClient tcpClient;
        public int ResponseTimeout = 0x3e8;
        private int timeOf4Byte;
        private Thread waitReply;

        public SerialCom(SerialPort serialCom)
        {
            if (serialCom == null)
            {
                throw new NullReferenceException("SerialCom = Null");
            }
            this.comPort = serialCom;
            this.bufferRx = new byte[300];
        }

        public void ClosePort()
        {
            this.comPort.Close();
        }

        public void OpenPort()
        {
            this.comPort.Open();
        }

        public byte[] QueryAndWaitResponse(byte[] command)
        {
            try
            {
                this.comPort.DiscardInBuffer();
                this.comPort.Write(command, 0, command.Length);
                //this.tcpClient.Client.Send()
            }
            catch (Exception)
            {
            }
            return this.WaitRx();
        }

        public byte[] WaitRx()
        {
            int index = 0;
            int num2 = 0;
            byte[] buffer = null;
            try
            {
                this.timeOf4Byte = (int) ((((1.0 / ((double) this.comPort.BaudRate)) * 11.0) * 1000.0) * 4.0);
                this.comPort.ReadTimeout = this.ResponseTimeout;
                num2 = this.ResponseTimeout / 10;
                for (index = 0; index < num2; index++)
                {
                }
            }
            catch (Exception)
            {
            }
            try
            {
                index = 0;
                while (index < this.bufferRx.Length)
                {
                    this.bufferRx[index] = (byte) this.comPort.ReadByte();
                    index++;
                }
            }
            catch (Exception)
            {
            }
            if (index > 0)
            {
                buffer = new byte[index];
                for (index = 0; index < buffer.Length; index++)
                {
                    buffer[index] = this.bufferRx[index];
                }
            }
            return buffer;
        }
    }
}

