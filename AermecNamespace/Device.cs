namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    [Serializable]
    public class Device
    {
        private DataLogConfig _dataLogCfg;
        public string Description;
        public string DeviceName;
        public bool Enabled;
        public byte ModBusID;
        [XmlElement(Type=typeof(ReadCoilsCommand))]
        public ArrayList ReadCoilsCommandArray;
        [XmlElement(Type=typeof(ReadInputRegistersCommand))]
        public ArrayList ReadInputRegistersCommandArray;
        [XmlElement(Type=typeof(ReadRegistersCommand))]
        public ArrayList ReadRegistersCommandArray;
        public int ScanRate;
        public int Timeout;

        public Device()
        {
            this.ModBusID = 1;
            this.Timeout = 0x3e8;
            this.ScanRate = 500;
            this.Enabled = true;
        }

        public Device(bool debug)
        {
            this.ModBusID = 1;
            this.Timeout = 0x3e8;
            this.ScanRate = 500;
            this.Enabled = true;
            this.DeviceName = "Device1";
            this.ReadRegistersCommandArray = new ArrayList(0);
            this.ReadInputRegistersCommandArray = new ArrayList(0);
            this.ReadCoilsCommandArray = new ArrayList(0);
            ReadRegistersCommand command = new ReadRegistersCommand(true, 0, 10);
            this.ReadRegistersCommandArray.Add(command);
            ReadInputRegistersCommand command2 = new ReadInputRegistersCommand(true, 0, 10);
            this.ReadInputRegistersCommandArray.Add(command2);
            ReadCoilsCommand command3 = new ReadCoilsCommand(true, 0, 0x9a);
            this.ReadCoilsCommandArray.Add(command3);
        }

        public Device(string name)
        {
            this.ModBusID = 1;
            this.Timeout = 0x3e8;
            this.ScanRate = 500;
            this.Enabled = true;
            this.DeviceName = name;
            this.ReadRegistersCommandArray = new ArrayList(0);
            this.ReadCoilsCommandArray = new ArrayList(0);
            this.DataLogCfg = new DataLogConfig(0, 0);
        }

        public Device Clone()
        {
            int num;
            Device device = new Device {
                ModBusID = this.ModBusID,
                Timeout = this.Timeout,
                ScanRate = this.ScanRate,
                DeviceName = this.DeviceName,
                Enabled = this.Enabled,
                Description = this.Description,
                DataLogCfg = this.DataLogCfg.Clone(),
                ReadCoilsCommandArray = new ArrayList(0)
            };
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                device.ReadCoilsCommandArray.Add(((ReadCoilsCommand) this.ReadCoilsCommandArray[num]).Clone());
            }
            device.ReadRegistersCommandArray = new ArrayList(0);
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                {
                    device.ReadRegistersCommandArray.Add(((ReadRegistersCommand) this.ReadRegistersCommandArray[num]).Clone());
                }
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                {
                    device.ReadRegistersCommandArray.Add(((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num]).Clone());
                }
            }
            return device;
        }

        public void DeleteReadCommand(int index)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    this.ReadRegistersCommandArray.RemoveAt(num);
                }
                num2++;
            }
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    this.ReadCoilsCommandArray.RemoveAt(num);
                }
                num2++;
            }
        }

        public void EnableReadCommand(int index, bool enable)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                    {
                        ((ReadRegistersCommand) this.ReadRegistersCommandArray[num]).EnableCommnand = enable;
                    }
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                    {
                        ((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num]).EnableCommnand = enable;
                    }
                }
                num2++;
            }
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    ((ReadCoilsCommand) this.ReadCoilsCommandArray[num]).EnableCommnand = enable;
                }
                num2++;
            }
        }

        public int[] GetCoilsAddress()
        {
            ReadCoilsCommand command;
            int num;
            int index = 0;
            int num4 = 0;
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                command = (ReadCoilsCommand) this.ReadCoilsCommandArray[num];
                if (command.EnableCommnand)
                {
                    num4 += command.Size;
                }
            }
            int[] numArray = new int[num4];
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                command = (ReadCoilsCommand) this.ReadCoilsCommandArray[num];
                if (command.EnableCommnand)
                {
                    for (int i = 0; i < command.Size; i++)
                    {
                        numArray[index] = command.StartAddres + i;
                        index++;
                    }
                }
            }
            return numArray;
        }

        public object GetReadCommand(int index)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    return this.ReadRegistersCommandArray[num];
                }
                num2++;
            }
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    return this.ReadCoilsCommandArray[num];
                }
                num2++;
            }
            return null;
        }

        public int[] GetRegistersAddress()
        {
            ReadRegistersCommand command;
            ReadInputRegistersCommand command2;
            int num;
            int index = 0;
            int num4 = 0;
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                {
                    command = (ReadRegistersCommand) this.ReadRegistersCommandArray[num];
                    if (command.EnableCommnand)
                    {
                        num4 += command.Size;
                    }
                }
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                {
                    command2 = (ReadInputRegistersCommand) this.ReadRegistersCommandArray[num];
                    if (command2.EnableCommnand)
                    {
                        num4 += command2.Size;
                    }
                }
            }
            int[] numArray = new int[num4];
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                int num2;
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                {
                    command = (ReadRegistersCommand) this.ReadRegistersCommandArray[num];
                    if (command.EnableCommnand)
                    {
                        num2 = 0;
                        while (num2 < command.Size)
                        {
                            numArray[index] = command.StartAddres + num2;
                            index++;
                            num2++;
                        }
                    }
                }
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                {
                    command2 = (ReadInputRegistersCommand) this.ReadRegistersCommandArray[num];
                    if (command2.EnableCommnand)
                    {
                        for (num2 = 0; num2 < command2.Size; num2++)
                        {
                            numArray[index] = command2.StartAddres + num2;
                            index++;
                        }
                    }
                }
            }
            return numArray;
        }

        public int GetTotalCommandSent()
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                num2 += ((ReadCoilsCommand) this.ReadCoilsCommandArray[num]).TotalCommandSent;
            }
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                {
                    num2 += ((ReadRegistersCommand) this.ReadRegistersCommandArray[num]).TotalCommandSent;
                }
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                {
                    num2 += ((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num]).TotalCommandSent;
                }
            }
            return num2;
        }

        public int GetTotalErrors()
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                num2 += ((ReadCoilsCommand) this.ReadCoilsCommandArray[num]).TotalErrors;
            }
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                {
                    num2 += ((ReadRegistersCommand) this.ReadRegistersCommandArray[num]).TotalErrors;
                }
                if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                {
                    num2 += ((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num]).TotalErrors;
                }
            }
            return num2;
        }

        public void SetReadCommand(int index, bool enable, ushort startAdd, ushort lenght, string description)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < this.ReadRegistersCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                    {
                        ReadRegistersCommand command = (ReadRegistersCommand) this.ReadRegistersCommandArray[num];
                        command.EnableCommnand = enable;
                        command.StartAddres = startAdd;
                        command.Size = lenght;
                        command.Description = description;
                    }
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                    {
                        ReadInputRegistersCommand command2 = (ReadInputRegistersCommand) this.ReadRegistersCommandArray[num];
                        command2.EnableCommnand = enable;
                        command2.StartAddres = startAdd;
                        command2.Size = lenght;
                        command2.Description = description;
                    }
                }
                num2++;
            }
            for (num = 0; num < this.ReadCoilsCommandArray.Count; num++)
            {
                if (index == num2)
                {
                    ReadCoilsCommand command3 = (ReadCoilsCommand) this.ReadCoilsCommandArray[num];
                    command3.EnableCommnand = enable;
                    command3.StartAddres = startAdd;
                    command3.Size = lenght;
                    command3.Description = description;
                }
                num2++;
            }
        }

        public void SortCommandByAddress()
        {
            int num;
            int num2;
            object obj2;
            ushort startAddres = 0;
            ushort num4 = 0;
            for (num2 = 0; num2 < (this.ReadRegistersCommandArray.Count - 1); num2++)
            {
                num = 0;
                while (num < (this.ReadRegistersCommandArray.Count - 1))
                {
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadRegistersCommand))
                    {
                        startAddres = ((ReadRegistersCommand) this.ReadRegistersCommandArray[num]).StartAddres;
                    }
                    if (this.ReadRegistersCommandArray[num].GetType() == typeof(ReadInputRegistersCommand))
                    {
                        startAddres = ((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num]).StartAddres;
                    }
                    if (this.ReadRegistersCommandArray[num + 1].GetType() == typeof(ReadRegistersCommand))
                    {
                        num4 = ((ReadRegistersCommand) this.ReadRegistersCommandArray[num + 1]).StartAddres;
                    }
                    if (this.ReadRegistersCommandArray[num + 1].GetType() == typeof(ReadInputRegistersCommand))
                    {
                        num4 = ((ReadInputRegistersCommand) this.ReadRegistersCommandArray[num + 1]).StartAddres;
                    }
                    if (startAddres > num4)
                    {
                        obj2 = this.ReadRegistersCommandArray[num];
                        this.ReadRegistersCommandArray[num] = this.ReadRegistersCommandArray[num + 1];
                        this.ReadRegistersCommandArray[num + 1] = obj2;
                    }
                    num++;
                }
            }
            for (num2 = 0; num2 < (this.ReadCoilsCommandArray.Count - 1); num2++)
            {
                for (num = 0; num < (this.ReadCoilsCommandArray.Count - 1); num++)
                {
                    if (((ReadCoilsCommand) this.ReadCoilsCommandArray[num]).StartAddres > ((ReadCoilsCommand) this.ReadCoilsCommandArray[num + 1]).StartAddres)
                    {
                        obj2 = this.ReadCoilsCommandArray[num];
                        this.ReadCoilsCommandArray[num] = this.ReadCoilsCommandArray[num + 1];
                        this.ReadCoilsCommandArray[num + 1] = obj2;
                    }
                }
            }
            obj2 = null;
        }

        public DataLogConfig DataLogCfg
        {
            get
            {
                return this._dataLogCfg;
            }
            set
            {
                this._dataLogCfg = value;
            }
        }

        [Serializable]
        public class ReadCoilsCommand
        {
            public string Description;
            public bool EnableCommnand;
            public ushort Size;
            public ushort StartAddres;
            private int totalCommandSent;
            private int totalErrors;

            public ReadCoilsCommand()
            {
                this.EnableCommnand = true;
                this.Description = "";
            }

            public ReadCoilsCommand(bool enable, ushort startAddres, ushort size)
            {
                this.EnableCommnand = true;
                this.Description = "";
                this.EnableCommnand = enable;
                this.StartAddres = startAddres;
                this.Size = size;
            }

            public Device.ReadCoilsCommand Clone()
            {
                return new Device.ReadCoilsCommand(this.EnableCommnand, this.StartAddres, this.Size) { Description = this.Description };
            }

            [XmlIgnore]
            public int TotalCommandSent
            {
                get
                {
                    return this.totalCommandSent;
                }
                set
                {
                    this.totalCommandSent = value;
                }
            }

            [XmlIgnore]
            public int TotalErrors
            {
                get
                {
                    return this.totalErrors;
                }
                set
                {
                    this.totalErrors = value;
                }
            }
        }

        [Serializable]
        public class ReadInputRegistersCommand
        {
            public string Description;
            public bool EnableCommnand;
            public ushort Size;
            public ushort StartAddres;
            private int totalCommandSent;
            private int totalErrors;

            public ReadInputRegistersCommand()
            {
                this.EnableCommnand = true;
                this.Description = "";
            }

            public ReadInputRegistersCommand(bool enable, ushort startAddres, ushort size)
            {
                this.EnableCommnand = true;
                this.Description = "";
                this.EnableCommnand = enable;
                this.StartAddres = startAddres;
                this.Size = size;
            }

            public Device.ReadInputRegistersCommand Clone()
            {
                return new Device.ReadInputRegistersCommand(this.EnableCommnand, this.StartAddres, this.Size) { Description = this.Description };
            }

            [XmlIgnore]
            public int TotalCommandSent
            {
                get
                {
                    return this.totalCommandSent;
                }
                set
                {
                    this.totalCommandSent = value;
                }
            }

            [XmlIgnore]
            public int TotalErrors
            {
                get
                {
                    return this.totalErrors;
                }
                set
                {
                    this.totalErrors = value;
                }
            }
        }

        [Serializable]
        public class ReadRegistersCommand
        {
            public bool Ascii;
            public string Description;
            public bool EnableCommnand;
            public ushort Size;
            public ushort StartAddres;
            private int totalCommandSent;
            private int totalErrors;

            public ReadRegistersCommand()
            {
                this.EnableCommnand = true;
                this.Description = "";
            }

            public ReadRegistersCommand(bool enable, ushort startAddres, ushort size)
            {
                this.EnableCommnand = true;
                this.Description = "";
                this.EnableCommnand = enable;
                this.StartAddres = startAddres;
                this.Size = size;
            }

            public ReadRegistersCommand(bool enable, ushort startAddres, ushort size, bool ascii)
            {
                this.EnableCommnand = true;
                this.Description = "";
                this.EnableCommnand = enable;
                this.StartAddres = startAddres;
                this.Size = size;
                this.Ascii = ascii;
            }

            public Device.ReadRegistersCommand Clone()
            {
                return new Device.ReadRegistersCommand(this.EnableCommnand, this.StartAddres, this.Size) { 
                    Description = this.Description,
                    Ascii = this.Ascii
                };
            }

            [XmlIgnore]
            public int TotalCommandSent
            {
                get
                {
                    return this.totalCommandSent;
                }
                set
                {
                    this.totalCommandSent = value;
                }
            }

            [XmlIgnore]
            public int TotalErrors
            {
                get
                {
                    return this.totalErrors;
                }
                set
                {
                    this.totalErrors = value;
                }
            }
        }
    }
}

