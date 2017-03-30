namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO.Ports;
    using System.Threading;
    using System.Windows.Forms;
    public class SupervisorCOM : Supervisor
    {        
        private SerialCom rs485;

        //public event EventHandler DataAllDevicesUpdate;

        //public event EventHandler<DeviceEventArgs> DataDeviceUpdate;

        //public event EventHandler<DeviceErrorEventArgs> DeviceReadCoilsError;

        //public event EventHandler<DeviceErrorEventArgs> DeviceReadRegisterError;

        //public event EventHandler LogStopped;

        public SupervisorCOM()
        {
            this.exitedLoop = true;
            this.deviceDB = new BmsProject();
            this.ModbusMasterArrayList = new ArrayList();
        }

        public SupervisorCOM(BmsProject bms, SerialPort serialcom) : base(bms)
        {            
            if (serialcom == null)
            {
                throw new Exception("Supervisor initialization fail. serialcom == null");
            }
            this.SetSerialPort(serialcom);
        }

        public override Supervisor Clone()
        {
            SupervisorCOM supervisor = new SupervisorCOM
            {
                rs485 = this.rs485,                
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
                    ModbusMaster master = (ModbusMaster)this.ModbusMasterArrayList[i];
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
                                    this.rs485.ResponseTimeout = device.ScanRate;
                                    command.TotalCommandSent++;
                                    
                                    buffer = this.rs485.QueryAndWaitResponse(master.ReadHoldingRegisters(command.StartAddres, command.Size));
                                    
                                    /** Test **/
                                    String array = "";
                                    for (int ir = 0; ir < command.Size; ir++)
                                    {
                                        array = array + buffer[ir].ToString() + " ";                                        
                                    }
                                    MessageBox.Show("начало" + command.StartAddres.ToString() + "\n" + "размер" + command.Size.ToString() + "\nbuffer = " + array);
                                    /** /Test **/

                                    code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_HOLDING_REGISTERS, command.Size);
                                    if (code != ModbusMaster.AnswerCode.OK)
                                    {
                                        command.TotalErrors++;                                        
                                        Raise_DeviceReadRegisterError(this, new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code));
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
                                    this.rs485.ResponseTimeout = device.ScanRate;
                                    command2.TotalCommandSent++;
                                    buffer = this.rs485.QueryAndWaitResponse(master.ReadInputRegisters(command2.StartAddres, command2.Size));
                                    code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_INPUT_REGISTERS, command2.Size);
                                    if (code != ModbusMaster.AnswerCode.OK)
                                    {
                                        command2.TotalErrors++;                                        
                                        Raise_DeviceReadRegisterError(this, new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code));
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
                                this.rs485.ResponseTimeout = device.ScanRate;
                                command3.TotalCommandSent++;
                                buffer = this.rs485.QueryAndWaitResponse(master.ReadCoils(command3.StartAddres, command3.Size));
                                code = master.Answer(buffer, ModbusMaster.ModBusCommand.READ_COILS, command3.Size);
                                if (code != ModbusMaster.AnswerCode.OK)
                                {
                                    command3.TotalErrors++;                                    
                                    Raise_DeviceReadCoilsError(this, new DeviceErrorEventArgs(device.DeviceName, device.ModBusID, commandIndex, code));
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
                        Raise_DataDeviceUpdate(this, new DeviceEventArgs(device.ModBusID));
                    }
                }                
                Raise_DataAllDevicesUpdate(this);
            }
            this.rs485.ClosePort();
            this.exitedLoop = true;            
            Raise_LogStopped(this);
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

        public void SetSerialPort(SerialPort serialcom)
        {
            if (serialcom != null)
            {
                if (this.deviceDB.BmsSerialConfig.ComPort == null)
                {
                    throw new Exception("COM Port not selected! Select it from property of BMS");
                }
                serialcom.PortName = this.deviceDB.BmsSerialConfig.ComPort;
                serialcom.Parity = this.deviceDB.BmsSerialConfig.Parity;
                serialcom.BaudRate = this.deviceDB.BmsSerialConfig.BaudRate;
                serialcom.StopBits = this.deviceDB.BmsSerialConfig.StopBits;
                this.rs485 = new SerialCom(serialcom);
            }
        }

        public override void StartModbusMasterPool()
        {
            if (!this.IsRunning())
            {
                this.rs485.OpenPort();
                this.comLoop = new Thread(new ThreadStart(this.CommunicationControlBrain));
                this.loopCommunication = true;
                this.exitedLoop = false;
                this.comLoop.Start();
            }
        }

        public void StopModbusMasterPool()
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

