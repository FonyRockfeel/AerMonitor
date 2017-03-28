namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO.Ports;
    using System.Threading;   
    using System.Windows.Forms;       
    using System.Net.Sockets;
    

    public class SupervisorTcpIP : Supervisor
    {
        // private ModbusClient modbusTCPClient;

        // Класс который отвечает за получение Modbus обернутого в TCP
        private ModbusInsideTCP modbusInsideTCPClient;

        //test
        private byte[] bufferRx;
        public int ResponseTimeout = 0x3e8;

        public event EventHandler DataAllDevicesUpdate;

        public event EventHandler<DeviceEventArgs> DataDeviceUpdate;

        public event EventHandler<DeviceErrorEventArgs> DeviceReadCoilsError;

        public event EventHandler<DeviceErrorEventArgs> DeviceReadRegisterError;

        public event EventHandler LogStopped;

        public SupervisorTcpIP()
        {
            this.exitedLoop = true;
            this.deviceDB = new BmsProject();
            this.ModbusMasterArrayList = new ArrayList();
        }

        public SupervisorTcpIP(BmsProject bms, TcpIPConfig tcpIPConfig) : base(bms)
        {
            if (tcpIPConfig == null)
            {
                throw new Exception("Supervisor initialization fail. tcpIPConfig == null");
            }
            this.SetTcpIPConfig(tcpIPConfig);
        }

        public override Supervisor Clone()
        {
            SupervisorTcpIP supervisor = new SupervisorTcpIP
            {                
                modbusInsideTCPClient = this.modbusInsideTCPClient,
                DataDeviceUpdate = this.DataDeviceUpdate,
                DeviceReadCoilsError = this.DeviceReadCoilsError,
                DeviceReadRegisterError = this.DeviceReadRegisterError,
                LogStopped = this.LogStopped,
                deviceDB = this.deviceDB.Clone(),
                ModbusMasterArrayList = new ArrayList(0)
            };
            if (this.ModbusMasterArrayList == null)
            {
                this.ModbusMasterArrayList = new ArrayList(0);
            }
            for (int i = 0; i < this.deviceDB.DeviceCount(); i++)
            {
                if (this.ModbusMasterArrayList.Count <= i)
                {
                    this.ModbusMasterArrayList.Add(new ModbusMaster());
                }
                supervisor.ModbusMasterArrayList.Add(((ModbusMaster)this.ModbusMasterArrayList[i]).Clone());
            }
            return supervisor;
        }

        public override void CommunicationControlBrain()
        {
            this.UpdateModbusMasterArray();
            while (this.loopCommunication)
            {                
                for (int i = 0; i < this.deviceDB.DeviceCount(); i++)
                {
                    // коннектор с модбасом
                    ModbusMaster master = (ModbusMaster)this.ModbusMasterArrayList[i];
                    // база данных девайсов которая говорит откуда читать coils и registers
                    Device device = this.deviceDB.GetDevice(i);
                    master.DeviceID = device.ModBusID;

                    if (device.Enabled && this.loopCommunication)
                    {
                        byte[] buffer;
                        ModbusMaster.AnswerCode code;
                        int commandIndex = 0;
                        while (commandIndex < device.ReadRegistersCommandArray.Count)
                        {
                            if (device.ReadRegistersCommandArray[i].GetType() == typeof(Device.ReadRegistersCommand))
                            {
                                Device.ReadRegistersCommand command = (Device.ReadRegistersCommand)device.ReadRegistersCommandArray[commandIndex];
                                if (command.EnableCommnand)
                                {
                                    this.ResponseTimeout = device.ScanRate;
                                    command.TotalCommandSent++;

                                    //buffer = this.rs485.QueryAndWaitResponse(master.ReadHoldingRegisters(command.StartAddres, command.Size));

                                    buffer = this.modbusInsideTCPClient.QueryAndWaitResponse(master.ReadHoldingRegisters(command.StartAddres, command.Size));

                                    /** Test **/
                                    String array = "";
                                    for (int ir = 0; ir < command.Size; ir++)
                                    {
                                        array = array + buffer[ir].ToString() + " ";
                                    }
                                    MessageBox.Show("начало" + command.StartAddres.ToString() + "\n" + "размер" + command.Size.ToString() + "\nbuffer = " + array);
                                    /** /Test **/

                                    code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_HOLDING_REGISTERS, command.Size);
                                    //проглатываем ошибки crc
                                    if (code != ModbusMaster.AnswerCode.OK & code != ModbusMaster.AnswerCode.CRCError)
                                    {
                                        MessageBox.Show(code.ToString());

                                        command.TotalErrors++;
                                        if (this.DeviceReadRegisterError != null)
                                        {
                                            DeviceErrorEventArgs e = new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code);
                                            this.DeviceReadRegisterError(this, e);
                                        }
                                    }
                                    if (!this.loopCommunication)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(1);
                                }
                            }
                            if (device.ReadRegistersCommandArray[i].GetType() == typeof(Device.ReadInputRegistersCommand))
                            {
                                Device.ReadInputRegistersCommand command2 = (Device.ReadInputRegistersCommand)device.ReadRegistersCommandArray[commandIndex];
                                if (command2.EnableCommnand)
                                {
                                    //this.rs485.ResponseTimeout = device.ScanRate;

                                    command2.TotalCommandSent++;
                                                                    
                                    buffer = this.modbusInsideTCPClient.QueryAndWaitResponse(master.ReadHoldingRegisters(command2.StartAddres, command2.Size));

                                    /** Test **/
                                    String array = "";
                                    for (int ir = 0; ir < command2.Size; ir++)
                                    {
                                        array = array + buffer[ir].ToString() + " ";
                                    }
                                    MessageBox.Show("начало" + command2.StartAddres.ToString() + "\n" + "размер" + command2.Size.ToString() + "\nbuffer = " + array);
                                    /** /Test **/
                                    
                                    code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_INPUT_REGISTERS, command2.Size);

                                    //проглатываем ошибки crc
                                    if (code != ModbusMaster.AnswerCode.OK & code != ModbusMaster.AnswerCode.CRCError)
                                    {
                                        command2.TotalErrors++;
                                        if (this.DeviceReadRegisterError != null)
                                        {
                                            DeviceErrorEventArgs args2 = new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code);
                                            this.DeviceReadRegisterError(this, args2);
                                        }
                                    }
                                    if (!this.loopCommunication)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(1);
                                }
                            }
                            commandIndex++;
                        }
                        if (!this.loopCommunication)
                        {
                            break;
                        }
                        for (commandIndex = 0; commandIndex < device.ReadCoilsCommandArray.Count; commandIndex++)
                        {
                            Device.ReadCoilsCommand command3 = (Device.ReadCoilsCommand)device.ReadCoilsCommandArray[commandIndex];
                            if (command3.EnableCommnand)
                            {
                                //this.rs485.ResponseTimeout = device.ScanRate;

                                command3.TotalCommandSent++;
                                
                                buffer = this.modbusInsideTCPClient.QueryAndWaitResponse(master.ReadHoldingRegisters(command3.StartAddres, command3.Size));

                                /** Test **/
                                String array = "";
                                for (int ir = 0; ir < command3.Size; ir++)
                                {
                                    array = array + buffer[ir].ToString() + " ";
                                }
                                MessageBox.Show("начало" + command3.StartAddres.ToString() + "\n" + "размер" + command3.Size.ToString() + "\nbuffer = " + array);
                                /** /Test **/

                                code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_COILS, command3.Size);
                                //проглатываем ошибки crc
                                if (code != ModbusMaster.AnswerCode.OK & code != ModbusMaster.AnswerCode.CRCError)
                                {
                                    command3.TotalErrors++;
                                    if (this.DeviceReadCoilsError != null)
                                    {
                                        DeviceErrorEventArgs args3 = new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code);
                                        this.DeviceReadCoilsError(this, args3);
                                    }
                                }
                                if (!this.loopCommunication)
                                {
                                    break;
                                }
                                Thread.Sleep(1);
                            }
                        }
                        if (!this.loopCommunication)
                        {
                            break;
                        }
                        if (this.DataDeviceUpdate != null)
                        {
                            DeviceEventArgs args4 = new DeviceEventArgs(device.ModBusID);
                            this.DataDeviceUpdate(this, args4);
                        }
                    }
                }
                if (this.loopCommunication && (this.DataAllDevicesUpdate != null))
                {
                    EventArgs args5 = new EventArgs();
                    this.DataAllDevicesUpdate(this, args5);
                }
            }
            this.modbusInsideTCPClient.Disconnect();            
            this.exitedLoop = true;
            if (this.LogStopped != null)
            {
                this.LogStopped(this, new EventArgs());
            }
        }


        public decimal GetCoilFromModbusId(int id, int address, bool gain)
        {
            decimal num = 0M;
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster)this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    Device device = (Device)this.deviceDB.DeviceArrayList[i];
                    num = ((ModbusMaster)this.ModbusMasterArrayList[i]).Coils[address];
                    if (gain)
                    {
                        num *= device.DataLogCfg.Coils[address].Gain;
                    }
                    return num;
                }
            }
            return num;
        }

        public short[] GetCoilsFromDevice(int index)
        {
            short[] numArray = null;
            int num2 = 0;
            if (index < this.ModbusMasterArrayList.Count)
            {
                numArray = new short[((Device)this.deviceDB.DeviceArrayList[index]).DataLogCfg.Coils.Length];
                for (int i = 0; i < numArray.Length; i++)
                {
                    Device device = (Device)this.deviceDB.DeviceArrayList[index];
                    if (device.DataLogCfg.Coils[i].Enable)
                    {
                        numArray[num2] = ((ModbusMaster)this.ModbusMasterArrayList[index]).Coils[device.DataLogCfg.Coils[i].Address];
                        num2++;
                    }
                }
            }
            return numArray;
        }

        public short[] GetCoilsFromModbusID(byte id)
        {
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster)this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    return (short[])((ModbusMaster)this.ModbusMasterArrayList[i]).Coils.Clone();
                }
            }
            return null;
        }

        public decimal GetRegisterFromModbusId(int id, int address, bool gain)
        {
            decimal num = 0M;
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster)this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    Device device = (Device)this.deviceDB.DeviceArrayList[i];
                    num = ((ModbusMaster)this.ModbusMasterArrayList[i]).Registers[address];
                    if (gain)
                    {
                        num *= device.DataLogCfg.GetRegisterFromAddress(address).Gain;
                    }
                    return num;
                }
            }
            return num;
        }

        public short[] GetRegistersFromDevice(int index)
        {
            short[] numArray = null;
            int num2 = 0;
            if (index < this.ModbusMasterArrayList.Count)
            {
                numArray = new short[((Device)this.deviceDB.DeviceArrayList[index]).DataLogCfg.Registers.Length];
                Device device = (Device)this.deviceDB.DeviceArrayList[index];
                for (int i = 0; i < numArray.Length; i++)
                {
                    if (device.DataLogCfg.Registers[i].Enable)
                    {
                        numArray[num2] = ((ModbusMaster)this.ModbusMasterArrayList[index]).Registers[device.DataLogCfg.Registers[i].Address];
                        num2++;
                    }
                }
            }
            return numArray;
        }

        public short[] GetRegistersFromModbusID(byte id)
        {
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster)this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    return (short[])((ModbusMaster)this.ModbusMasterArrayList[i]).Registers.Clone();
                }
            }
            return null;
        }

        public bool IsRunning()
        {
            return !this.exitedLoop;
        }

        public ModbusMaster ModbusFromDeviceIndex(int indexDevice)
        {
            return (ModbusMaster)this.ModbusMasterArrayList[indexDevice];
        }

        public void SetTcpIPConfig(TcpIPConfig tcpIPConfig)
        {
            if (tcpIPConfig != null)
            {
                //if (this.deviceDB.BmsSerialConfig.ComPort == null)
                //{
                //    throw new Exception("COM Port not selected! Select it from property of BMS");
                //}
                //serialcom.PortName = this.deviceDB.BmsSerialConfig.ComPort;
                //serialcom.Parity = this.deviceDB.BmsSerialConfig.Parity;
                //serialcom.BaudRate = this.deviceDB.BmsSerialConfig.BaudRate;
                //serialcom.StopBits = this.deviceDB.BmsSerialConfig.StopBits;

                // Новый код вызова инициализатора TCP клиента
                //this.modbusTCPClient = new ModbusClient(tcpIPConfig.IPaddress, tcpIPConfig.Port);
                this.modbusInsideTCPClient = new ModbusInsideTCP(tcpIPConfig.IPaddress, tcpIPConfig.Port);
                
                //test
                this.bufferRx = new byte[300];
            }
        }

        public override void StartModbusMasterPool()
        {
            if (!this.IsRunning())
            {
                this.modbusInsideTCPClient.Connect();
                this.comLoop = new Thread(new ThreadStart(this.CommunicationControlBrain));
                this.loopCommunication = true;
                this.exitedLoop = false;
                this.comLoop.Start();
            }
        }

        public override void StopModbusMasterPool()
        {
            this.loopCommunication = false;
        }

        private void UpdateModbusMasterArray()
        {
            if (this.ModbusMasterArrayList == null)
            {
                this.ModbusMasterArrayList = new ArrayList();
            }
            if (this.deviceDB.DeviceCount() != this.ModbusMasterArrayList.Count)
            {
                this.ModbusMasterArrayList.Clear();
                for (int i = 0; i < this.deviceDB.DeviceCount(); i++)
                {
                    this.ModbusMasterArrayList.Add(new ModbusMaster());
                }
            }
        }

    }
}

