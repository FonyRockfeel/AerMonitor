namespace AermecNamespace
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Net.Sockets;
    using System.Threading;
    using System.Diagnostics;
    using System.Net;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Runtime;
    public class ModbusInsideTCP
    {
        private byte[] bufferRx;

        ModbusIpTransport _transport;

        // Поток чтения и записи
        NetworkStream stream;

        private string hostName;
        private int port;

        public int ResponseTimeout = 0x3e8;
        private int timeOf4Byte;
        private Thread waitReply;

        public ModbusInsideTCP(string newHostName, int newPort)
        {            
            hostName = newHostName;
            port = newPort;
        }

        public void Connect()
        {
            var tcpAdapter = new TcpClientAdapter(new TcpClient(hostName, port));           
            _transport = new ModbusIpTransport(tcpAdapter);
        }

        public void Disconnect()
        {
            _transport.Dispose();
        }

        public byte[] QueryAndWaitResponse(byte[] command)
        {
            try
            {
                return _transport.RequestAndGetResponse(command);
            }
            catch
            {
                return new byte[8];//in exception case method returns array of 8 bytes
            }
        }

        //public byte[] WaitRx()
        //{
        //    int index = 0;
        //    int num2 = 0;
        //    byte[] buffer = null;
        //    try
        //    {
        //        //this.timeOf4Byte = (int)((((1.0 / ((double)this.comPort.BaudRate)) * 11.0) * 1000.0) * 4.0);
                
        //        // this.comPort.ReadTimeout = this.ResponseTimeout;
        //        this.tcpClient.ReceiveTimeout = this.ResponseTimeout;

        //        num2 = this.ResponseTimeout / 10;
        //        for (index = 0; index < num2; index++)
        //        { 

        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    try
        //    {
        //        index = 0;
        //        while (index < this.bufferRx.Length)
        //        {
        //            //this.bufferRx[index] = (byte)this.comPort.ReadByte();
        //            index++;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    if (index > 0)
        //    {
        //        buffer = new byte[index];
        //        for (index = 0; index < buffer.Length; index++)
        //        {
        //            buffer[index] = this.bufferRx[index];
        //        }
        //    }
        //    return buffer;
        //}
    }

    internal class TcpClientAdapter
    {
        private TcpClient _tcpClient;

        public TcpClientAdapter(TcpClient tcpClient)
        {
            Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");

            _tcpClient = tcpClient;
        }

        public int InfiniteTimeout
        {
            get { return Timeout.Infinite; }
        }

        public int ReadTimeout
        {
            get { return _tcpClient.GetStream().ReadTimeout; }
            set { _tcpClient.GetStream().ReadTimeout = value; }
        }

        public int WriteTimeout
        {
            get { return _tcpClient.GetStream().WriteTimeout; }
            set { _tcpClient.GetStream().WriteTimeout = value; }
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            _tcpClient.GetStream().Write(buffer, offset, size);
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            return _tcpClient.GetStream().Read(buffer, offset, size);
        }

        public void DiscardInBuffer()
        {
            _tcpClient.GetStream().Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tcpClient.GetStream().Close();
                _tcpClient.Close();
            }
        }
    }
    internal class ModbusIpTransport 
    {
        private static readonly object _transactionIdLock = new object();
        private ushort _transactionId;
        TcpClientAdapter _adapter;
        private int _retries=5;
        ModbusIpMessage _requestMessage;
        ModbusIpMessage _responseMessage;
        private int WaitToRetryMilliseconds;

        internal ModbusIpTransport(TcpClientAdapter adapter)           
        {
            _adapter = adapter;
            Debug.Assert(_adapter != null, "Argument streamResource cannot be null.");
        }
        public void Dispose()
        {
            _adapter.Dispose();
        }

        internal static byte[] ReadRequestResponse(TcpClientAdapter adapter)
        {
            // read header
            var mbapHeader = new byte[6];
            int numBytesRead = 0;

            while (numBytesRead != 6)
            {
                int bRead = adapter.Read(mbapHeader, numBytesRead, 6 - numBytesRead);

                if (bRead == 0)
                {
                    throw new IOException("Read resulted in 0 bytes returned.");
                }

                numBytesRead += bRead;
            }

            Debug.WriteLine(String.Format("MBAP header: {0}", string.Join(", ", mbapHeader)));
            var frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(mbapHeader, 4));
            Debug.WriteLine(String.Format("{0} bytes in PDU.", frameLength));

            // read message
            var messageFrame = new byte[frameLength];
            numBytesRead = 0;

            while (numBytesRead != frameLength)
            {
                int bRead = adapter.Read(messageFrame, numBytesRead, frameLength - numBytesRead);

                if (bRead == 0)
                {
                    throw new IOException("Read resulted in 0 bytes returned.");
                }

                numBytesRead += bRead;
            }

            Debug.WriteLine(String.Format("PDU: {0}", frameLength));
            var frame = mbapHeader.Concat(messageFrame).ToArray();
            Debug.WriteLine(String.Format("RX: {0}", string.Join(", ", frame)));

            //_responseFrame = frame;

            return frame;
        }

        internal static byte[] GetMbapHeader(ModbusIpMessage message)
        {
            byte[] transactionId = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)message.TransactionId));
            byte[] length = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(message.ProtocolDataUnit.Length))); //+1

            var stream = new MemoryStream(7);
            stream.Write(transactionId, 0, transactionId.Length);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.Write(length, 0, length.Length);
            //stream.WriteByte(message.SlaveAddress);

            return stream.ToArray();
        }

        /// <summary>
        ///     Create a new transaction ID.
        /// </summary>
        internal virtual ushort GetNewTransactionId()
        {
            lock (_transactionIdLock)
            {
                _transactionId = _transactionId == ushort.MaxValue ? (ushort)1 : ++_transactionId;
            }

            return _transactionId;
        }       

        internal byte[] BuildMessageFrame(ModbusIpMessage message)
        {
            byte[] header = GetMbapHeader(message);
            byte[] pdu = message.ProtocolDataUnit;
            MemoryStream messageBody = new MemoryStream(header.Length + pdu.Length);

            messageBody.Write(header, 0, header.Length);
            messageBody.Write(pdu, 0, pdu.Length);

            var bytes = messageBody.ToArray();            
            return bytes;
        }

        internal void Write(ModbusIpMessage message)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message);
            Debug.WriteLine(String.Format("TX: {0}", string.Join(", ", frame)));
            _adapter.Write(frame, 0, frame.Length);
        }
               

        internal ModbusIpMessage ReadResponse()
        {            
            var fullFrame = ReadRequestResponse(_adapter);
            byte[] mbapHeader = fullFrame.Slice(0, 6).ToArray();
            byte[] messageFrame = fullFrame.Slice(6, fullFrame.Length - 6).ToArray();

            var response = new ModbusIpMessage(messageFrame[0],messageFrame, (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(mbapHeader, 0)));            
            return response;
        }

        //internal void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        //{
        //    if (request.TransactionId != response.TransactionId)
        //    {
        //        string msg = String.Format("Response was not of expected transaction ID. Expected {0}, received {1}.",
        //            request.TransactionId, response.TransactionId);
        //        throw new IOException(msg);
        //    }
        //}

        //internal bool OnShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        //{
        //    if (request.TransactionId > response.TransactionId && request.TransactionId - response.TransactionId < RetryOnOldResponseThreshold)
        //    {
        //        // This response was from a previous request
        //        return true;
        //    }

        //    return base.OnShouldRetryResponse(request, response);
        //}

        internal byte[] RequestAndGetResponse(byte[] command)
        {
            _requestMessage = new ModbusIpMessage(command[0], command, 0);
            int attempt = 1;
            bool success = false;            
            do
            {
                try
                {
                    Write(_requestMessage);
                    bool readAgain=false;
                    do
                    {
                        readAgain = false;
                        _responseMessage = ReadResponse();
                        //TODO may add validation or exception check, that changes 'readAgain' and forces retries
                    }
                    while (readAgain);                   
                    ValidateResponse(_requestMessage, _responseMessage);
                    success = true;
                }
                
                catch (Exception e)
                {
                    if (e is FormatException ||
                        e is NotImplementedException ||
                        e is TimeoutException ||
                        e is IOException)
                    {
                        Debug.WriteLine(String.Format("{0}, {1} retries remaining - {2}", e.GetType().Name,
                            _retries - attempt + 1, e));

                        if (attempt++ > _retries)
                        {
                            Debug.WriteLine("No retries, device not respond!");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Unexpected exception!");
                    }
                }
            }
            while (!success);
            return _responseMessage.ProtocolDataUnit;
        }
        private static void Sleep(int millisecondsTimeout)
        {
            //Task.Delay(millisecondsTimeout).Wait();
        }
        internal void ValidateResponse(ModbusIpMessage request, ModbusIpMessage response)
        {
            if (request.SlaveAddress != response.SlaveAddress)
            {
                string msg = String.Format(
                    "Response slave address does not match request. Expected {0}, received {1}.", response.SlaveAddress,
                    request.SlaveAddress);
                throw new IOException(msg);
            }
            if (request.TransactionId != response.TransactionId)
            {
                string msg = String.Format("Response was not of expected transaction ID. Expected {0}, received {1}.",
                    request.TransactionId, response.TransactionId);
                throw new IOException(msg);
            }
        }
    }
    internal class ModbusIpMessage
    {
        public byte[] ProtocolDataUnit { get; }
        public ushort TransactionId { get; set; }
        public byte SlaveAddress { get; set; }
        public ModbusIpMessage(byte slaveId,byte[] pdu, ushort tId)
        {
            SlaveAddress = slaveId;
            ProtocolDataUnit = pdu;
            TransactionId = tId;
        }
    }
    internal static class SequenceUtility
    {
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var enumerable = source as T[] ?? source.ToArray();
            int num = enumerable.Count();

            if (startIndex < 0 || num < startIndex)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (size < 0 || startIndex + size > num)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            return enumerable.Skip(startIndex).Take(size);
        }
    }
}
