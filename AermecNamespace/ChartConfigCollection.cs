namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Xml.Serialization;

    [Serializable]
    public class ChartConfigCollection
    {
        [XmlElement(Type=typeof(ChartConfig))]
        public ArrayList ChartsConfig = new ArrayList();

        public void Add(ChartConfig chartConfig)
        {
            this.ChartsConfig.Add(chartConfig);
        }

        public ChartConfigCollection Clone()
        {
            int num = 0;
            ChartConfigCollection configs = new ChartConfigCollection();
            for (num = 0; num < this.ChartsConfig.Count; num++)
            {
                configs.ChartsConfig.Add(((ChartConfig) this.ChartsConfig[num]).Clone());
            }
            return configs;
        }

        public ChartConfig GetChartConfigFromId(int Id)
        {
            return (ChartConfig) this.ChartsConfig[Id];
        }

        public class ChartConfig
        {
            public string ChartName;
            [XmlElement(Type=typeof(Serie))]
            public ArrayList graphCoils;
            [XmlElement(Type=typeof(Serie))]
            public ArrayList graphRegisters;

            public ChartConfig()
            {
                this.ChartName = "";
                this.graphRegisters = new ArrayList();
                this.graphCoils = new ArrayList();
            }

            public ChartConfig(string name) : this()
            {
                this.ChartName = name;
            }

            public void AddSerieCoils(Serie serie)
            {
                this.graphCoils.Add(serie);
            }

            public void AddSerieRegister(Serie serie)
            {
                this.graphRegisters.Add(serie);
            }

            public ChartConfigCollection.ChartConfig Clone()
            {
                int num;
                ChartConfigCollection.ChartConfig config = new ChartConfigCollection.ChartConfig {
                    ChartName = this.ChartName
                };
                for (num = 0; num < this.graphRegisters.Count; num++)
                {
                    config.graphRegisters.Add(((Serie) this.graphRegisters[num]).Clone());
                }
                for (num = 0; num < config.graphCoils.Count; num++)
                {
                    config.graphCoils.Add(((Serie) this.graphCoils[num]).Clone());
                }
                return config;
            }

            public Serie GetSerieCoils(int index)
            {
                Serie serie = null;
                if (this.graphCoils.Count > 0)
                {
                    serie = (Serie) this.graphCoils[index];
                }
                return serie;
            }

            public Serie GetSerieRegisters(int index)
            {
                Serie serie = null;
                if (this.graphRegisters.Count > 0)
                {
                    serie = (Serie) this.graphRegisters[index];
                }
                return serie;
            }

            public class Serie
            {
                public int Address;
                public int DeviceId;
                public int Line;
                public Color LineColor;
                public string SerieDescription;

                public Serie()
                {
                    this.Line = 2;
                    this.DeviceId = 0;
                    this.Address = 0;
                    this.SerieDescription = "";
                    this.LineColor = Color.Red;
                    this.Line = 2;
                }

                public Serie(int deviceId, int address, string description, Color lineColor, int line)
                {
                    this.Line = 2;
                    this.DeviceId = deviceId;
                    this.Address = address;
                    this.SerieDescription = description;
                    this.LineColor = lineColor;
                    this.Line = line;
                }

                public ChartConfigCollection.ChartConfig.Serie Clone()
                {
                    return new ChartConfigCollection.ChartConfig.Serie { 
                        DeviceId = this.DeviceId,
                        Address = this.Address,
                        Line = this.Line,
                        LineColor = this.LineColor,
                        SerieDescription = this.SerieDescription
                    };
                }
            }
        }
    }
}

