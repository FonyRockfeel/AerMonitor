namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO.Ports;
    using System.Threading;
    using System.Windows.Forms;
    public class Supervisor
    {
        protected Thread comLoop;
        public BmsProject deviceDB;
        protected bool exitedLoop;
        protected bool loopCommunication;
        protected ArrayList ModbusMasterArrayList;       

        public event EventHandler DataAllDevicesUpdate;

        public event EventHandler<DeviceEventArgs> DataDeviceUpdate;

        public event EventHandler<DeviceErrorEventArgs> DeviceReadCoilsError;

        public event EventHandler<DeviceErrorEventArgs> DeviceReadRegisterError;

        public event EventHandler LogStopped;

        #region Events Invocators
        protected void Raise_DataAllDevicesUpdate(object sender)
        {
            EventHandler handler = DataAllDevicesUpdate;
            if (handler != null) handler(sender, null);

        }
        protected void Raise_DataDeviceUpdate(object sender,DeviceEventArgs args)
        {
            EventHandler<DeviceEventArgs> handler = DataDeviceUpdate;
            if (handler != null) handler(sender, args);
        }
        protected void Raise_DeviceReadCoilsError(object sender,DeviceErrorEventArgs args)
        {
            EventHandler<DeviceErrorEventArgs> handler = DeviceReadCoilsError;
            if (handler != null) handler(sender, args);
        }
        protected void Raise_DeviceReadRegisterError(object sender,DeviceErrorEventArgs args)
        {
            EventHandler<DeviceErrorEventArgs> handler = DeviceReadRegisterError;
            if (handler != null) handler(sender, args);
        }
        protected void Raise_LogStopped(object sender)
        {
            EventHandler handler = LogStopped;
            if (handler != null) handler(sender, null);
        }

        #endregion


        public Supervisor()
        {
            this.exitedLoop = true;
            this.deviceDB = new BmsProject();
            this.ModbusMasterArrayList = new ArrayList();
        }

        public Supervisor(BmsProject bms)
        {
            this.exitedLoop = true;
            if (bms == null)
            {
                throw new Exception("Supervisor initialization fail. bms == null");
            }            
            this.deviceDB = bms;
        }

        public virtual Supervisor Clone()
        {
            Supervisor supervisor = new Supervisor {               
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
                supervisor.ModbusMasterArrayList.Add(((ModbusMaster) this.ModbusMasterArrayList[i]).Clone());
            }
            return supervisor;
        }

        public virtual void Close()
        {
        }

        public virtual void CommunicationControlBrain()
        {
        }

        public virtual decimal GetCoilFromModbusId(int id, int address, bool gain)
        {
            decimal num = 0M;
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster) this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    Device device = (Device) this.deviceDB.DeviceArrayList[i];
                    num = ((ModbusMaster) this.ModbusMasterArrayList[i]).Coils[address];
                    if (gain)
                    {
                        num *= device.DataLogCfg.Coils[address].Gain;
                    }
                    return num;
                }
            }
            return num;
        }

        public virtual short[] GetCoilsFromDevice(int index)
        {
            short[] numArray = null;
            int num2 = 0;
            if (index < this.ModbusMasterArrayList.Count)
            {
                numArray = new short[((Device) this.deviceDB.DeviceArrayList[index]).DataLogCfg.Coils.Length];
                for (int i = 0; i < numArray.Length; i++)
                {
                    Device device = (Device) this.deviceDB.DeviceArrayList[index];
                    if (device.DataLogCfg.Coils[i].Enable)
                    {
                        numArray[num2] = ((ModbusMaster) this.ModbusMasterArrayList[index]).Coils[device.DataLogCfg.Coils[i].Address];
                        num2++;
                    }
                }
            }
            return numArray;
        }

        public virtual short[] GetCoilsFromModbusID(byte id)
        {
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster) this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    return (short[]) ((ModbusMaster) this.ModbusMasterArrayList[i]).Coils.Clone();
                }
            }
            return null;
        }

        public virtual decimal GetRegisterFromModbusId(int id, int address, bool gain)
        {
            decimal num = 0M;
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster) this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    Device device = (Device) this.deviceDB.DeviceArrayList[i];
                    num = ((ModbusMaster) this.ModbusMasterArrayList[i]).Registers[address];
                    if (gain)
                    {
                        num *= device.DataLogCfg.GetRegisterFromAddress(address).Gain;
                    }
                    return num;
                }
            }
            return num;
        }

        public virtual short[] GetRegistersFromDevice(int index)
        {
            short[] numArray = null;
            int num2 = 0;
            if (index < this.ModbusMasterArrayList.Count)
            {
                numArray = new short[((Device) this.deviceDB.DeviceArrayList[index]).DataLogCfg.Registers.Length];
                Device device = (Device) this.deviceDB.DeviceArrayList[index];
                for (int i = 0; i < numArray.Length; i++)
                {
                    if (device.DataLogCfg.Registers[i].Enable)
                    {
                        numArray[num2] = ((ModbusMaster) this.ModbusMasterArrayList[index]).Registers[device.DataLogCfg.Registers[i].Address];
                        num2++;
                    }
                }
            }
            return numArray;
        }

        public virtual short[] GetRegistersFromModbusID(byte id)
        {
            for (int i = 0; i < this.ModbusMasterArrayList.Count; i++)
            {
                if (((ModbusMaster) this.ModbusMasterArrayList[i]).DeviceID == id)
                {
                    return (short[]) ((ModbusMaster) this.ModbusMasterArrayList[i]).Registers.Clone();
                }
            }
            return null;
        }

        public virtual bool IsRunning()
        {
            return !this.exitedLoop;
        }

        public virtual ModbusMaster ModbusFromDeviceIndex(int indexDevice)
        {
            return (ModbusMaster) this.ModbusMasterArrayList[indexDevice];
        }        

        public virtual void StartModbusMasterPool()
        {            
        }

        public virtual void StopModbusMasterPool()
        {
            this.loopCommunication = false;
        }

        protected virtual void UpdateModbusMasterArray()
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

        public class DeviceErrorEventArgs : EventArgs
        {
            public int CommandIndex;
            public ModbusMaster.AnswerCode ErrorCode;
            public byte ModbusID;
            public string Name;

            public DeviceErrorEventArgs(string name, byte modbusid, int commandIndex, ModbusMaster.AnswerCode errorcode)
            {
                this.ErrorCode = errorcode;
                this.ModbusID = modbusid;
                this.CommandIndex = commandIndex;
                this.Name = name;
            }
        }

        public class DeviceEventArgs : EventArgs
        {
            public byte modbusID;

            public DeviceEventArgs(byte modbusid)
            {
                this.modbusID = modbusid;
            }
        }
    }
}

