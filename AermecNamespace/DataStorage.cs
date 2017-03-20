namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class DataStorage
    {
        private BmsProject bmsProject;
        [XmlElement(Type=typeof(DataSnapShot))]
        public ArrayList DataSnapShots;

        public DataStorage()
        {
            this.DataSnapShots = new ArrayList(0);
        }

        public DataStorage(BmsProject bms)
        {
            this.bmsProject = bms;
            this.DataSnapShots = new ArrayList(0);
        }

        public void AddSnapshot(Supervisor superv)
        {
            DataSnapShot shot = new DataSnapShot();
            for (int i = 0; i < this.bmsProject.DeviceArrayList.Count; i++)
            {
                if (this.bmsProject.GetDevice(i).Enabled)
                {
                    DataSnapShot.DataDevice device = new DataSnapShot.DataDevice(superv.GetCoilsFromDevice(i), superv.GetRegistersFromDevice(i), this.bmsProject.GetDevice(i).DataLogCfg);
                    shot.DataDevices.Add(device);
                }
            }
            this.DataSnapShots.Add(shot);
        }

        public string ClearCharsNotPermitted(string str)
        {
            while (str.Contains("\""))
            {
                str = str.Replace("\"", "");
            }
            while (str.Contains(";"))
            {
                str = str.Replace(";", "");
            }
            return str;
        }

        public bool ExportDataToCsv(BmsProject nexConfigProject, string filePath)
        {
            TextWriter writer = null;
            bool flag = false;
            int num3 = 0;
            if (nexConfigProject == null)
            {
                throw new Exception("Project null");
            }
            try
            {
                int num;
                int num2;
                writer = new StreamWriter(filePath, false, Encoding.Default);
                this.TotalRow();
                if (this.DataSnapShots.Count > 0)
                {
                    num3 = 0;
                    DataSnapShot shot1 = (DataSnapShot) this.DataSnapShots[num3];
                    for (num = 0; num < nexConfigProject.DeviceCount(); num++)
                    {
                        bool enabled = nexConfigProject.GetDevice(num).Enabled;
                    }
                    writer.Write("TIME;");
                    num = 0;
                    while (num < nexConfigProject.DeviceCount())
                    {
                        if (nexConfigProject.GetDevice(num).Enabled)
                        {
                            DataLogConfig.DataLog[] registers = nexConfigProject.GetDevice(num).DataLogCfg.Registers;
                            num2 = 0;
                            while (num2 < registers.Length)
                            {
                                if (registers[num2].Enable)
                                {
                                    if (nexConfigProject.DeviceCount() > 1)
                                    {
                                        writer.Write(nexConfigProject.GetDevice(num).DeviceName + ": ");
                                    }
                                    if (registers[num2].Name == "")
                                    {
                                        if (registers[num2].Description == "")
                                        {
                                            writer.Write("Address: " + registers[num2].Address + ";");
                                        }
                                        else
                                        {
                                            writer.Write(this.ClearCharsNotPermitted(registers[num2].Description) + ";");
                                        }
                                    }
                                    else
                                    {
                                        writer.Write(this.ClearCharsNotPermitted(registers[num2].Name) + ";");
                                    }
                                }
                                num2++;
                            }
                            DataLogConfig.DataLog[] coils = nexConfigProject.GetDevice(num).DataLogCfg.Coils;
                            num2 = 0;
                            while (num2 < coils.Length)
                            {
                                if (coils[num2].Enable)
                                {
                                    if (nexConfigProject.DeviceCount() > 1)
                                    {
                                        writer.Write(nexConfigProject.GetDevice(num).DeviceName + ": ");
                                    }
                                    if (coils[num2].Name == "")
                                    {
                                        if (coils[num2].Description == "")
                                        {
                                            writer.Write("Address: " + coils[num2].Address + ";");
                                        }
                                        else
                                        {
                                            writer.Write(this.ClearCharsNotPermitted(coils[num2].Description) + ";");
                                        }
                                    }
                                    else
                                    {
                                        writer.Write(this.ClearCharsNotPermitted(coils[num2].Name) + ";");
                                    }
                                }
                                num2++;
                            }
                        }
                        num++;
                    }
                    writer.WriteLine();
                }
                for (num3 = 0; num3 < this.DataSnapShots.Count; num3++)
                {
                    DataSnapShot shot = (DataSnapShot) this.DataSnapShots[num3];
                    writer.Write(shot.Time.ToString("dd/MM/yyyy HH.mm.ss") + ";");
                    for (num = 0; num < nexConfigProject.DeviceCount(); num++)
                    {
                        if (nexConfigProject.GetDevice(num).Enabled)
                        {
                            DataLogConfig.DataLog[] logArray3 = nexConfigProject.GetDevice(num).DataLogCfg.Registers;
                            num2 = 0;
                            while (num2 < logArray3.Length)
                            {
                                if (logArray3[num2].Enable)
                                {
                                    writer.Write(((((short) ((DataSnapShot.DataDevice) shot.DataDevices[num]).Registers.Data[num2]) * logArray3[num2].Gain)).ToString() + ";");                
                                }
                                num2++;
                            }
                            DataLogConfig.DataLog[] logArray4 = nexConfigProject.GetDevice(num).DataLogCfg.Coils;
                            for (num2 = 0; num2 < logArray4.Length; num2++)
                            {
                                if (logArray4[num2].Enable)
                                {
                                    writer.Write(((((short) ((DataSnapShot.DataDevice) shot.DataDevices[num]).Coils.Data[num2]) * logArray4[num2].Gain)).ToString() + ";");
                                }
                            }
                        }
                    }
                    writer.WriteLine();
                }
                flag = true;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return flag;
        }

        public DataStorage GetRange(DateTime start, DateTime stop)
        {
            int index = -1;
            int num3 = -1;
            DataStorage storage = new DataStorage(this.bmsProject);
            if (this.DataSnapShots.Count > 0)
            {
                for (int i = 0; i < this.DataSnapShots.Count; i++)
                {
                    DataSnapShot shot = (DataSnapShot) this.DataSnapShots[i];
                    if ((index < 0) && (shot.Time >= start))
                    {
                        if (i > 0)
                        {
                            index = i - 1;
                        }
                        else
                        {
                            index = i;
                        }
                    }
                    if (((num3 < 0) && (shot.Time <= ((DataSnapShot) this.DataSnapShots[this.DataSnapShots.Count - 1]).Time)) && (shot.Time >= stop))
                    {
                        num3 = i;
                        break;
                    }
                }
                storage.DataSnapShots = this.DataSnapShots.GetRange(index, (num3 - index) + 1);
            }
            return storage;
        }

        public int TotalRow()
        {
            int num2 = 0;
            for (int i = 0; i < this.DataSnapShots.Count; i++)
            {
                num2 += ((DataSnapShot.DataDevice) ((DataSnapShot) this.DataSnapShots[i]).DataDevices[0]).Registers.Data.Count;
            }
            return num2;
        }

        [Serializable]
        public class DataSnapShot
        {
            [XmlElement(Type=typeof(DataDevice))]
            public ArrayList DataDevices = new ArrayList(0);
            public DateTime Time = DateTime.Now;

            [Serializable]
            public class DataDevice
            {
                public DataValues Coils;
                public DataValues Registers;

                public DataDevice()
                {
                }

                public DataDevice(short[] coils, short[] registers, DataLogConfig dataLogCfg)
                {
                    this.Coils = new DataValues(coils, dataLogCfg.Coils);
                    this.Registers = new DataValues(registers, dataLogCfg.Registers);
                }

                [Serializable]
                public class DataValues
                {
                    [XmlElement(Type=typeof(short))]
                    public ArrayList Data;

                    public DataValues()
                    {
                    }

                    public DataValues(short[] values, DataLogConfig.DataLog[] cfg)
                    {
                        this.Data = new ArrayList(0);
                        for (int i = 0; i < cfg.Length; i++)
                        {
                            this.Data.Add(values[i]);
                        }
                    }
                }
            }
        }

        [Serializable]
        public class DataStorageIndex
        {
            public BmsProject bmsProject;
            public string[] fileDataStorageName;
            public DateTime[] fileTimeSnaphot;
            public DateTime lastSave;
            public bool logging;
            public int logNumber;

            public DataStorageIndex()
            {
                this.bmsProject = null;
                this.fileTimeSnaphot = new DateTime[0];
                this.fileDataStorageName = new string[0];
                this.lastSave = new DateTime();
            }

            public DataStorageIndex(BmsProject project)
            {
                this.bmsProject = project;
                this.fileTimeSnaphot = new DateTime[0];
                this.fileDataStorageName = new string[0];
            }

            public void AddFileStorage(DateTime timeFileStart, string fileName)
            {
                int num = this.fileTimeSnaphot.Length + 1;
                DateTime[] array = new DateTime[num];
                string[] strArray = new string[num];
                this.fileTimeSnaphot.CopyTo(array, 0);
                this.fileDataStorageName.CopyTo(strArray, 0);
                array[num - 1] = timeFileStart;
                strArray[num - 1] = fileName;
                this.fileTimeSnaphot = array;
                this.fileDataStorageName = strArray;
            }

            public int Count()
            {
                return this.fileTimeSnaphot.Length;
            }

            public TimeSpan DurationTime()
            {
                if ((this.fileTimeSnaphot != null) && (this.fileTimeSnaphot.Length > 0))
                {
                    return this.lastSave.Subtract(this.fileTimeSnaphot[0]);
                }
                return new TimeSpan(0L);
            }

            public string DurationTimeToString()
            {
                string str = "";
                int days = this.DurationTime().Days;
                int hours = this.DurationTime().Hours;
                int minutes = this.DurationTime().Minutes;
                int seconds = this.DurationTime().Seconds;
                if (days > 0)
                {
                    str = str + days.ToString() + " Days, ";
                }
                if (hours > 0)
                {
                    str = str + hours.ToString() + " h, ";
                }
                if (minutes > 0)
                {
                    str = str + minutes.ToString() + " min, ";
                }
                if (seconds > 0)
                {
                    str = str + seconds.ToString() + " sec";
                }
                return str;
            }

            public string GetStartRecordingToString()
            {
                return this.fileTimeSnaphot[0].ToString("dd-MM-yyyy HH.mm.ss");
            }

            public class DataStorageIndexComparer : IComparer
            {
                public int Compare(object objx, object objy)
                {
                    if (((objx != null) && (objy != null)) && !((DataStorage.DataStorageIndex) objx).fileTimeSnaphot[0].Equals(((DataStorage.DataStorageIndex) objy).fileTimeSnaphot[0]))
                    {
                        if (((DataStorage.DataStorageIndex) objx).fileTimeSnaphot[0] < ((DataStorage.DataStorageIndex) objy).fileTimeSnaphot[0])
                        {
                            return -1;
                        }
                        if (((DataStorage.DataStorageIndex) objx).fileTimeSnaphot[0] > ((DataStorage.DataStorageIndex) objy).fileTimeSnaphot[0])
                        {
                            return 1;
                        }
                    }
                    return 0;
                }
            }
        }
    }
}

