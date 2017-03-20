namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    [Serializable]
    public class BmsProject
    {
        // COM port
        public SerialConfig BmsSerialConfig = new SerialConfig();
        // TcpIP
        public TcpIPConfig BmsTcpIPConfig = new TcpIPConfig();

        // выбранный источник данных
        public DataSource dataSource;

        public ChartConfigCollection Charts = new ChartConfigCollection();
        public int DBVersion = 1;
        [XmlElement(Type=typeof(Device))]
        public ArrayList DeviceArrayList = new ArrayList();
        public int LogSaveRate;
        public bool LogSaveRateAuto;
        public string Name = "";
        public string Note = "";

        public void AddDevice(Device dev)
        {
            if (this.DeviceArrayList == null)
            {
                this.DeviceArrayList = new ArrayList();
            }
            this.DeviceArrayList.Add(dev);
        }

        public void CleanDataLogConfig()
        {
            for (int i = 0; i < this.DeviceCount(); i++)
            {
                this.GetDevice(i).DataLogCfg.DeleteRegistersCoilsDisabled();
            }
        }

        public BmsProject Clone()
        {
            if (this.DeviceArrayList == null)
            {
                return null;
            }
            BmsProject project = new BmsProject {
                DBVersion = this.DBVersion,
                Name = this.Name,
                LogSaveRate = this.LogSaveRate,
                LogSaveRateAuto = this.LogSaveRateAuto,
                BmsSerialConfig = this.BmsSerialConfig.Clone()
            };
            for (int i = 0; i < this.DeviceArrayList.Count; i++)
            {
                project.DeviceArrayList.Add(((Device) this.DeviceArrayList[i]).Clone());
            }
            return project;
        }

        public int DeviceCount()
        {
            if (this.DeviceArrayList == null)
            {
                return 0;
            }
            return this.DeviceArrayList.Count;
        }

        public Device GetDevice(int index)
        {
            if (this.DeviceArrayList == null)
            {
                return null;
            }
            if (this.DeviceArrayList.Count == 0)
            {
                return null;
            }
            return (Device) this.DeviceArrayList[index];
        }

        public Device GetDeviceFromID(int modbusID)
        {
            if (this.DeviceArrayList != null)
            {
                if (this.DeviceArrayList.Count == 0)
                {
                    return null;
                }
                for (int i = 0; i < this.DeviceArrayList.Count; i++)
                {
                    if (((Device) this.DeviceArrayList[i]).ModBusID == modbusID)
                    {
                        return (Device) this.DeviceArrayList[i];
                    }
                }
            }
            return null;
        }

        public int GetTotalCommandSent()
        {
            int num2 = 0;
            if (this.DeviceArrayList.Count > 0)
            {
                for (int i = 0; i < this.DeviceArrayList.Count; i++)
                {
                    num2 += ((Device) this.DeviceArrayList[i]).GetTotalCommandSent();
                }
            }
            return num2;
        }

        public int GetTotalErrors()
        {
            int num2 = 0;
            if (this.DeviceArrayList.Count > 0)
            {
                for (int i = 0; i < this.DeviceArrayList.Count; i++)
                {
                    num2 += ((Device) this.DeviceArrayList[i]).GetTotalErrors();
                }
            }
            return num2;
        }

        public void ImportDevicesFromBmsProject(BmsProject importedProject)
        {
            bool flag = false;
            if ((importedProject != null) && (importedProject.DeviceArrayList.Count != 0))
            {
                for (int i = 0; i < importedProject.DeviceArrayList.Count; i++)
                {
                    for (int j = 0; j < this.DeviceArrayList.Count; j++)
                    {
                        if (importedProject.GetDevice(i).DeviceName == this.GetDevice(j).DeviceName)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        flag = false;
                        importedProject.GetDevice(i).DeviceName = importedProject.GetDevice(i).DeviceName.Insert(0, "(Imported) ");
                        this.AddDevice(importedProject.GetDevice(i));
                    }
                    else
                    {
                        this.AddDevice(importedProject.GetDevice(i));
                    }
                }
            }
        }

        public bool ModbusIdOk()
        {
            if (this.DeviceArrayList.Count >= 2)
            {
                for (int i = 0; i < this.DeviceArrayList.Count; i++)
                {
                    for (int j = 1; j < this.DeviceArrayList.Count; j++)
                    {
                        if ((i != j) && (((Device) this.DeviceArrayList[i]).ModBusID == ((Device) this.DeviceArrayList[j]).ModBusID))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void SortDevices()
        {
            new BmsProject();
            this.DeviceArrayList.Sort(new DeviceComparer());
        }

        public class DeviceComparer : IComparer
        {
            public int Compare(object objx, object objy)
            {
                if ((objx != null) && (objy != null))
                {
                    return ((Device) objx).DeviceName.CompareTo(((Device) objy).DeviceName);
                }
                return 0;
            }
        }
    }

    public enum DataSource
    {
        COM,
        TcpIP
    }
}

