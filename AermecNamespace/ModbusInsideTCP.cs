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
            //first remove the crc bytes from pdu to get original ModbusTCP frame
            MemoryStream tmpStream = new MemoryStream();
            tmpStream.Write(command, 0, command.Length - 2);

            _requestMessage = new ModbusIpMessage(command[0], tmpStream.ToArray(), 0);
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
            //adding crc bytes
            tmpStream = new MemoryStream();
            tmpStream.Write(_responseMessage.ProtocolDataUnit, 0, _responseMessage.ProtocolDataUnit.Length);
            var crc = CalculateCrc(_responseMessage.ProtocolDataUnit);
            tmpStream.Write(crc, 0, crc.Length);
            var output = tmpStream.ToArray();
            return output;
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
        public byte[] CalculateCrc(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ushort crc = ushort.MaxValue;

            foreach (byte b in data)
            {
                byte tableIndex = (byte)(crc ^ b);
                crc >>= 8;
                crc ^= CrcTable[tableIndex];
            }

            return BitConverter.GetBytes(crc);
        }
        private readonly ushort[] CrcTable =
        {
            0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
            0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
            0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
            0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
            0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
            0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
            0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
            0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
            0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
            0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
            0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
            0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
            0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
            0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
            0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
            0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
            0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
            0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
            0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
            0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
            0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
            0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
            0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
            0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
            0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
            0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
            0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
            0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
            0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
            0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
            0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
            0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040
        };
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
