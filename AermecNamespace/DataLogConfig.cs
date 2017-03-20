namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class DataLogConfig
    {
        public DataLog[] Coils;
        private int e;
        public DataLog[] Registers;
        
        // новый код
        // public DataLog[] thresholdCoils;
        // public DataLog[] thresholdRegisters;

        public DataLogConfig() : this(0, 0)
        {
        }

        public DataLogConfig(int registersLenght, int coilsLenght) : this(new int[registersLenght], new int[coilsLenght])
        {
        }

        public DataLogConfig(int[] registersAddresses, int[] coilsAddresses)
        {
            int num;
            this.Registers = new DataLog[registersAddresses.Length];
            this.Coils = new DataLog[coilsAddresses.Length];
            // новый код
            // this.thresholdRegisters = new DataLog[registersAddresses.Length]; 
            // this.thresholdCoils = new DataLog[coilsAddresses.Length];
            
            for (num = 0; num < this.Registers.Length; num++)
            {
                this.Registers[num] = new DataLog();
                this.Registers[num].Address = registersAddresses[num];
                this.Registers[num].Description = "Description of Register " + registersAddresses[num].ToString();
            }
            for (num = 0; num < this.Coils.Length; num++)
            {
                this.Coils[num] = new DataLog();
                this.Coils[num].Address = coilsAddresses[num];
                this.Coils[num].Description = "Description of Coil " + coilsAddresses[num].ToString();
            }
        }

        public DataLogConfig(int[] registersAddresses, int[] coilsAddresses, bool[] ascii)
        {
            int num;
            this.Registers = new DataLog[registersAddresses.Length];
            this.Coils = new DataLog[coilsAddresses.Length];
            for (num = 0; num < this.Registers.Length; num++)
            {
                this.Registers[num] = new DataLog();
                this.Registers[num].Address = registersAddresses[num];
                this.Registers[num].Description = "Description of Register " + registersAddresses[num].ToString();
            }
            for (num = 0; num < this.Coils.Length; num++)
            {
                this.Coils[num] = new DataLog();
                this.Coils[num].Address = coilsAddresses[num];
                this.Coils[num].Description = "Description of Coil " + coilsAddresses[num].ToString();
            }
        }

        public DataLogConfig(int[] registersAddresses, int[] coilsAddresses, string[] registersDesc, string[] coilsDesc)
        {
            int num;
            this.Registers = new DataLog[registersAddresses.Length];
            this.Coils = new DataLog[coilsAddresses.Length];
            for (num = 0; num < this.Registers.Length; num++)
            {
                this.Registers[num] = new DataLog();
                this.Registers[num].Address = registersAddresses[num];
                if (registersAddresses[num] < registersDesc.Length)
                {
                    this.Registers[num].Description = registersDesc[registersAddresses[num]];
                }
                else
                {
                    this.Registers[num].Description = "";
                }
            }
            for (num = 0; num < this.Coils.Length; num++)
            {
                this.Coils[num] = new DataLog();
                this.Coils[num].Address = coilsAddresses[num];
                if (coilsAddresses[num] < coilsDesc.Length)
                {
                    this.Coils[num].Description = coilsDesc[coilsAddresses[num]];
                }
                else
                {
                    this.Coils[num].Description = "";
                }
            }
        }

        public DataLogConfig Clone()
        {
            int num;
            DataLogConfig config = new DataLogConfig(this.Registers.Length, this.Coils.Length);
            for (num = 0; num < this.Registers.Length; num++)
            {
                config.Registers[num] = this.Registers[num].Clone();
            }
            for (num = 0; num < this.Coils.Length; num++)
            {
                config.Coils[num] = this.Coils[num].Clone();
            }
            return config;
        }

        public void CopyTo(DataLogConfig copy)
        {
            int num;
            int num2;
            for (num = 0; num < this.Registers.Length; num++)
            {
                int address = this.Registers[num].Address;
                num2 = 0;
                while (num2 < copy.Registers.Length)
                {
                    if (this.Registers[num].Address == copy.Registers[num2].Address)
                    {
                        copy.Registers[num2].Enable = this.Registers[num].Enable;
                        copy.Registers[num2].Gain = this.Registers[num].Gain;
                        copy.Registers[num2].Name = this.Registers[num].Name;
                        copy.Registers[num2].Description = this.Registers[num].Description;
                    }
                    num2++;
                }
            }
            for (num = 0; num < this.Coils.Length; num++)
            {
                int num3 = this.Coils[num].Address;
                for (num2 = 0; num2 < copy.Coils.Length; num2++)
                {
                    if (this.Coils[num].Address == copy.Coils[num2].Address)
                    {
                        copy.Coils[num2].Enable = this.Coils[num].Enable;
                        copy.Coils[num2].Gain = this.Coils[num].Gain;
                        copy.Coils[num2].Name = this.Coils[num].Name;
                        copy.Coils[num2].Description = this.Coils[num].Description;
                    }
                }
            }
        }

        public void DeleteRegistersCoilsDisabled()
        {
            int num;
            ArrayList list = new ArrayList();
            for (num = 0; num < this.Registers.Length; num++)
            {
                if (this.Registers[num].Enable)
                {
                    list.Add(this.Registers[num]);
                }
            }
            this.Registers = new DataLog[list.Count];
            list.CopyTo(this.Registers);
            list.Clear();
            for (num = 0; num < this.Coils.Length; num++)
            {
                if (this.Coils[num].Enable)
                {
                    list.Add(this.Coils[num]);
                }
            }
            this.Coils = new DataLog[list.Count];
            list.CopyTo(this.Coils);
        }

        public bool ExportCoilsToCsv(string filePath)
        {
            TextWriter writer = null;
            bool flag = false;
            int index = 0;
            try
            {
                writer = new StreamWriter(filePath, false, Encoding.Default);
                for (int i = 0; i <= this.Coils.Last<DataLog>().Address; i++)
                {
                    if (this.Coils[index].Address == i)
                    {
                        writer.WriteLine(string.Concat(new object[] { i.ToString(), ";", this.Coils[index].Gain, ";", this.Coils[index].Name, ";", this.Coils[index].Description }));
                        index++;
                    }
                    else
                    {
                        writer.WriteLine(i.ToString() + "; ; ");
                    }
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

        public bool ExportRegistersToCsv(string filePath)
        {
            TextWriter writer = null;
            bool flag = false;
            int index = 0;
            try
            {
                writer = new StreamWriter(filePath, false, Encoding.Default);
                for (int i = 0; i <= this.Registers.Last<DataLog>().Address; i++)
                {
                    if (this.Registers[index].Address == i)
                    {
                        writer.WriteLine(string.Concat(new object[] { i.ToString(), ";", this.Registers[index].Gain, ";", this.Registers[index].Name, ";", this.Registers[index].Description }));
                        index++;
                    }
                    else
                    {
                        writer.WriteLine(i.ToString() + "; ; ");
                    }
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

        public DataLog GetCoilFromAddress(int address)
        {
            for (int i = 0; i < this.Coils.Length; i++)
            {
                if (this.Coils[i].Address == address)
                {
                    return this.Coils[i];
                }
            }
            return null;
        }

        public DataLog GetRegisterFromAddress(int address)
        {
            for (int i = 0; i < this.Registers.Length; i++)
            {
                if (this.Registers[i].Address == address)
                {
                    return this.Registers[i];
                }
            }
            return null;
        }

        public void ImportCoilsFromCsv(string filePath)
        {
            TextReader reader = null;
            string[] separator = new string[1];
            int index = 0;
            separator[0] = "\n";
            try
            {
                reader = new StreamReader(filePath, Encoding.Default);
                string[] strArray = reader.ReadToEnd().Replace("\r", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
                separator[0] = ";";
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray2 = strArray[i].Split(separator, StringSplitOptions.None);
                    if (strArray2.Length < 3)
                    {
                        throw new Exception("File format not valid!");
                    }
                    if ((index < this.Coils.Length) && (this.Coils[index].Address == i))
                    {
                        this.Coils[index].Gain = decimal.Parse(strArray2[1]);
                        this.Coils[index].Name = strArray2[2];
                        this.Coils[index].Description = strArray2[3];
                        index++;
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public void ImportRegistersFromCsv(string filePath)
        {
            TextReader reader = null;
            string[] separator = new string[1];
            int index = 0;
            separator[0] = "\n";
            try
            {
                reader = new StreamReader(filePath, Encoding.Default);
                string[] strArray = reader.ReadToEnd().Replace("\r", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
                separator[0] = ";";
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray2 = strArray[i].Split(separator, StringSplitOptions.None);
                    if (strArray2.Length < 3)
                    {
                        throw new Exception("File format not valid!");
                    }
                    if ((index < this.Registers.Length) && (this.Registers[index].Address == i))
                    {
                        this.Registers[index].Gain = decimal.Parse(strArray2[1]);
                        this.Registers[index].Name = strArray2[2];
                        this.Registers[index].Description = strArray2[3];
                        index++;
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        [Serializable]
        public class DataLog
        {
            public int Address = 0;
            public bool Ascii = false;
            public string Description = "";
            public bool Enable = true;
            public decimal Gain = 1M;
            public string Name = "";
            public bool UserAccess = true;
            public decimal Value = 0M;

            public DataLogConfig.DataLog Clone()
            {
                return new DataLogConfig.DataLog { 
                    Enable = this.Enable,
                    UserAccess = this.UserAccess,
                    Address = this.Address,
                    Description = this.Description,
                    Gain = this.Gain,
                    Value = this.Value,
                    Name = this.Name,
                    Ascii = this.Ascii
                };
            }
        }
    }
}

