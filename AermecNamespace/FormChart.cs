namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;

    public class FormChart : Form
    {
        private Button buttonConfig;
        private Button buttonPause;
        private Button buttonZoomIn;
        private Button buttonZoomOut;
        private Chart chart1;
        private ChartConfigCollection.ChartConfig chartsConfig;
        private const string CoilsAreaName = "CoilArea";
        private IContainer components;
        private BmsProject currentBmsProject;
        private Device[] currentDevices;
        private Supervisor currentSupervisor;
        private DataLogConfig datalogTemp;
        private DataLogConfig[] graphDataLogConfig;
        private string graphName;
        public int graphNumber;
        private int graphNumberDevice;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label label2;
        private Label labelPoint;
        private Label labelTime;
        [DecimalConstant(0, 0x80, (uint) 0, (uint) 0, (uint) 500)]
        private static readonly decimal minValChart = -500M;
        public bool newGraph;
        private int numberOfCoilSeries;
        private int numberOfRegisterSeries;
        private bool pause;
        private const string RegistersAreaName = "RegisterArea";

        public FormChart()
        {
            this.graphName = "";
            this.InitializeComponent();
        }

        public FormChart(BmsProject bmsProject, Supervisor supervisor) : this()
        {
            if ((bmsProject != null) && (supervisor != null))
            {
                this.newGraph = true;
                this.currentBmsProject = bmsProject;
                this.currentSupervisor = supervisor;
                this.ConfigChart();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.pause)
            {
                this.buttonPause.Text = "Pause";
                this.pause = false;
            }
            else
            {
                this.buttonPause.Text = ">";
                this.pause = true;
            }
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            FormChartConfig config = new FormChartConfig(this.currentBmsProject) {
                devices = this.currentDevices,
                newDataLogConfig = this.graphDataLogConfig,
                chartsConfig = this.chartsConfig
            };
            if (config.ShowDialog() == DialogResult.OK)
            {
                this.chartsConfig = config.chartsConfig;
                this.ConfigChart();
            }
        }

        private void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            if (double.IsNaN(e.NewPosition) && double.IsNaN(e.NewSize))
            {
                this.SetTitle(0.0, 0.0);
            }
            else
            {
                this.SetTitle(e.NewPosition, e.NewSize);
            }
        }

        private void chart1_CursorPositionChanging(object sender, CursorEventArgs e)
        {
            this.SetPosition(e.Axis, e.NewPosition);
        }

        private void ConfigAllSeries()
        {
            int num4 = 0;
            if (this.chartsConfig != null)
            {
                int num2;
                int num3;
                this.chart1.Series.Clear();
                int index = 0;
                while (index < this.currentBmsProject.DeviceArrayList.Count)
                {
                    this.datalogTemp = this.currentBmsProject.GetDevice(index).DataLogCfg;
                    num2 = 0;
                    while (num2 < this.datalogTemp.Registers.Length)
                    {
                        if (this.datalogTemp.Registers[num2].Enable)
                        {
                            string str;
                            ChartConfigCollection.ChartConfig.Serie serieRegisters = new ChartConfigCollection.ChartConfig.Serie();
                            if (this.datalogTemp.Registers[num2].Name != "")
                            {
                                str = this.currentBmsProject.GetDevice(index).DeviceName + ": " + this.datalogTemp.Registers[num2].Name;
                            }
                            else
                            {
                                str = this.currentBmsProject.GetDevice(index).DeviceName + ": " + this.datalogTemp.Registers[num2].Name + "-" + this.datalogTemp.Registers[num2].Description;
                            }
                            Series item = new Series(str) {
                                Enabled = false
                            };
                            num3 = 0;
                            while (num3 < this.chartsConfig.graphRegisters.Count)
                            {
                                serieRegisters = this.chartsConfig.GetSerieRegisters(num3);
                                item.ChartType = SeriesChartType.FastLine;
                                item.XValueType = ChartValueType.Time;
                                if ((serieRegisters.DeviceId == this.currentBmsProject.GetDevice(index).ModBusID) && (serieRegisters.Address == this.datalogTemp.Registers[num2].Address))
                                {
                                    item.Color = serieRegisters.LineColor;
                                    item.BorderWidth = serieRegisters.Line;
                                    item.ChartArea = "RegisterArea";
                                    item.Enabled = true;
                                    foreach (Series series2 in this.chart1.Series)
                                    {
                                        if (series2.Name == item.Name)
                                        {
                                            item.Name = item.Name + "#" + num4.ToString();
                                            num4++;
                                            break;
                                        }
                                    }
                                    this.chart1.Series.Add(item);
                                }
                                num3++;
                            }
                        }
                        num2++;
                    }
                    index++;
                }
                for (index = 0; index < this.currentBmsProject.DeviceArrayList.Count; index++)
                {
                    this.datalogTemp = this.currentBmsProject.GetDevice(index).DataLogCfg;
                    for (num2 = 0; num2 < this.datalogTemp.Coils.Length; num2++)
                    {
                        if (this.datalogTemp.Coils[num2].Enable)
                        {
                            string str2;
                            ChartConfigCollection.ChartConfig.Serie serieCoils = new ChartConfigCollection.ChartConfig.Serie();
                            if (this.datalogTemp.Coils[num2].Name != "")
                            {
                                str2 = this.currentBmsProject.GetDevice(index).DeviceName + ": " + this.datalogTemp.Coils[num2].Name;
                            }
                            else
                            {
                                str2 = this.currentBmsProject.GetDevice(index).DeviceName + ": " + this.datalogTemp.Coils[num2].Name + "-" + this.datalogTemp.Coils[num2].Description;
                            }
                            Series series3 = new Series(str2) {
                                Enabled = false
                            };
                            for (num3 = 0; num3 < this.chartsConfig.graphCoils.Count; num3++)
                            {
                                serieCoils = this.chartsConfig.GetSerieCoils(num3);
                                series3.ChartType = SeriesChartType.StepLine;
                                series3.XValueType = ChartValueType.Time;
                                if ((serieCoils.DeviceId == this.currentBmsProject.GetDevice(index).ModBusID) && (serieCoils.Address == this.datalogTemp.Coils[num2].Address))
                                {
                                    series3.Color = serieCoils.LineColor;
                                    series3.BorderWidth = serieCoils.Line;
                                    series3.ChartArea = "CoilArea" + num3.ToString();
                                    series3.Enabled = true;
                                    foreach (Series series4 in this.chart1.Series)
                                    {
                                        if (series4.Name == series3.Name)
                                        {
                                            series3.Name = series3.Name + "#" + num4.ToString();
                                            num4++;
                                            break;
                                        }
                                    }
                                    this.chart1.Series.Add(series3);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ConfigChart()
        {
            if ((this.currentSupervisor != null) && (this.chartsConfig != null))
            {
                if (this.newGraph)
                {
                    this.ConfigNewChartAreas();
                }
                if (this.newGraph)
                {
                    this.ConfigAllSeries();
                }
            }
        }

        private void ConfigNewChartAreas()
        {
            int num2 = 0;
            int num3 = 5;
            int num5 = 0;
            if (this.chartsConfig != null)
            {
                this.chart1.ChartAreas.Clear();
                int num4 = ((100 - num3) - 3) / 8;
                if (this.chartsConfig.graphRegisters.Count > 0)
                {
                    num2 = 20;
                    num4 = (((100 - num2) - num3) - 3) / 8;
                    if (this.chartsConfig.graphCoils.Count > 0)
                    {
                        if (this.chartsConfig.graphCoils.Count < num4)
                        {
                            num2 = (100 - num3) - (this.chartsConfig.graphCoils.Count * 8);
                        }
                    }
                    else
                    {
                        num2 = 90;
                    }
                    ChartArea item = new ChartArea("RegisterArea") {
                        AxisX = { LabelStyle = { 
                            Format = "dd/MM HH.mm.ss",
                            Interval = 0.0,
                            IntervalOffsetType = DateTimeIntervalType.Auto
                        } },
                        CursorX = { 
                            Interval = 0.1,
                            IntervalType = DateTimeIntervalType.Seconds,
                            IntervalOffsetType = DateTimeIntervalType.Seconds,
                            IsUserEnabled = true,
                            IsUserSelectionEnabled = true,
                            LineColor = Color.Red,
                            SelectionColor = Color.LightSteelBlue
                        },
                        CursorY = { 
                            Interval = 0.1,
                            IsUserEnabled = true,
                            IsUserSelectionEnabled = true,
                            LineColor = Color.Red,
                            SelectionColor = Color.LightSteelBlue
                        },
                        Position = { 
                            Height = num2,
                            Width = 75f,
                            X = 2f,
                            Y = 5f
                        }
                    };
                    this.chart1.ChartAreas.Add(item);
                    if (this.chartsConfig.graphCoils.Count > 0)
                    {
                        this.chart1.ChartAreas["RegisterArea"].AxisX.LabelStyle.Enabled = false;
                    }
                }
                for (int i = 0; (i < this.chartsConfig.graphCoils.Count) && (i < num4); i++)
                {
                    ChartArea area2 = new ChartArea("CoilArea" + i.ToString()) {
                        AxisX = { LabelStyle = { 
                            Format = "dd/MM HH.mm.ss",
                            Interval = 0.0,
                            IntervalOffsetType = DateTimeIntervalType.Auto
                        } },
                        Position = { 
                            Height = 5f,
                            Width = 75f,
                            X = 2f,
                            Y = (((5 * i) + num3) + num2) + 3
                        }
                    };
                    float y = area2.Position.Y;
                    num5 += (5 * (i + 1)) + 3;
                    area2.AxisY.MajorGrid.Enabled = false;
                    area2.AxisY.MajorTickMark.Enabled = false;
                    area2.AxisY.MinorGrid.Enabled = false;
                    area2.AxisY.MinorTickMark.Enabled = false;
                    area2.AxisY.Interval = 1.0;
                    area2.CursorX.Interval = 0.1;
                    area2.CursorX.IntervalType = DateTimeIntervalType.Seconds;
                    area2.CursorX.IntervalOffsetType = DateTimeIntervalType.Seconds;
                    area2.CursorX.IsUserEnabled = true;
                    area2.CursorX.IsUserSelectionEnabled = true;
                    area2.CursorX.LineColor = Color.Red;
                    area2.CursorX.SelectionColor = Color.LightSteelBlue;
                    area2.CursorY.Interval = 0.1;
                    area2.CursorY.IsUserEnabled = true;
                    area2.CursorY.IsUserSelectionEnabled = true;
                    area2.CursorY.LineColor = Color.Red;
                    area2.CursorY.SelectionColor = Color.LightSteelBlue;
                    area2.AxisY.Maximum = 1.0;
                    area2.AxisY.Minimum = 0.0;
                    if ((i + 1) < this.chartsConfig.graphCoils.Count)
                    {
                        area2.AxisX.LabelStyle.Enabled = false;
                    }
                    else
                    {
                        area2.Position.Height = 8f;
                        area2.AxisX.LabelStyle.Enabled = true;
                    }
                    this.chart1.ChartAreas.Add(area2);
                    this.chart1.ChartAreas["RegisterArea"].InnerPlotPosition.Auto = true;
                    this.chart1.ChartAreas[area2.Name].InnerPlotPosition.Auto = true;
                    this.chart1.ChartAreas[area2.Name].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
                    this.chart1.ChartAreas[area2.Name].AlignmentStyle = AreaAlignmentStyles.All;
                    this.chart1.ChartAreas[area2.Name].AlignWithChartArea = "RegisterArea";
                }
            }
        }

        private void ConfigSeries()
        {
            if (this.chartsConfig != null)
            {
                this.chart1.Series.Clear();
                int index = 0;
                while (index < this.chartsConfig.graphRegisters.Count)
                {
                    ChartConfigCollection.ChartConfig.Serie serieRegisters = this.chartsConfig.GetSerieRegisters(index);
                    Series item = new Series(serieRegisters.SerieDescription) {
                        Color = serieRegisters.LineColor,
                        ChartType = SeriesChartType.FastLine,
                        XValueType = ChartValueType.Time,
                        BorderWidth = serieRegisters.Line,
                        ChartArea = "RegisterArea"
                    };
                    this.chart1.Series.Add(item);
                    index++;
                }
                for (index = 0; index < this.chartsConfig.graphCoils.Count; index++)
                {
                    ChartConfigCollection.ChartConfig.Serie serieCoils = this.chartsConfig.GetSerieCoils(index);
                    Series series2 = new Series(serieCoils.SerieDescription) {
                        Color = serieCoils.LineColor,
                        ChartType = SeriesChartType.FastLine,
                        XValueType = ChartValueType.Time,
                        BorderWidth = serieCoils.Line,
                        ChartArea = "CoilArea" + index.ToString()
                    };
                    this.chart1.Series.Add(series2);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormChart_Load(object sender, EventArgs e)
        {
            this.labelTime.Text = DateTime.Now.ToString("dd/MM HH.mm.ss");
            this.labelPoint.Text = "0,0";
            this.graphName = "Graph " + this.graphNumber;
            this.Text = this.graphName;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonPause = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPoint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Cursor = System.Windows.Forms.Cursors.Cross;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.DodgerBlue;
            series1.Legend = "Legend1";
            series1.MarkerSize = 50;
            series1.Name = "Series1";
            series1.SmartLabelStyle.CalloutLineColor = System.Drawing.Color.Red;
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series4.BorderWidth = 2;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Legend = "Legend1";
            series4.Name = "Series4";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series5.BorderWidth = 2;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series5.Legend = "Legend1";
            series5.Name = "Series5";
            series6.BorderWidth = 2;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series6.Legend = "Legend1";
            series6.Name = "Series6";
            series7.BorderWidth = 2;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series7.Legend = "Legend1";
            series7.Name = "Series7";
            series8.BorderWidth = 2;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series8.Legend = "Legend1";
            series8.Name = "Series8";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series9.Legend = "Legend1";
            series9.Name = "Series9";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series10.Legend = "Legend1";
            series10.Name = "Series10";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Series.Add(series6);
            this.chart1.Series.Add(series7);
            this.chart1.Series.Add(series8);
            this.chart1.Series.Add(series9);
            this.chart1.Series.Add(series10);
            this.chart1.Size = new System.Drawing.Size(1018, 616);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            title1.Name = "Title1";
            title1.Position.Auto = false;
            title1.Position.Width = 94F;
            title1.Position.X = 3F;
            title1.Position.Y = 3F;
            this.chart1.Titles.Add(title1);
            this.chart1.CursorPositionChanging += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(this.chart1_CursorPositionChanging);
            // 
            // buttonPause
            // 
            this.buttonPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPause.Location = new System.Drawing.Point(6, 22);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(73, 23);
            this.buttonPause.TabIndex = 1;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonConfig);
            this.groupBox1.Controls.Add(this.buttonZoomOut);
            this.groupBox1.Controls.Add(this.buttonZoomIn);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.buttonPause);
            this.groupBox1.Location = new System.Drawing.Point(12, 621);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(992, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chart Controll...";
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(6, 70);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonConfig.TabIndex = 13;
            this.buttonConfig.Text = "Config...";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Location = new System.Drawing.Point(622, 61);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(75, 23);
            this.buttonZoomOut.TabIndex = 9;
            this.buttonZoomOut.Text = "Zoom Out";
            this.buttonZoomOut.UseVisualStyleBackColor = true;
            this.buttonZoomOut.Visible = false;
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Location = new System.Drawing.Point(622, 33);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(75, 23);
            this.buttonZoomIn.TabIndex = 6;
            this.buttonZoomIn.Text = "Zoom In";
            this.buttonZoomIn.UseVisualStyleBackColor = true;
            this.buttonZoomIn.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelTime);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.labelPoint);
            this.groupBox2.Location = new System.Drawing.Point(712, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 73);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cursor";
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.BackColor = System.Drawing.SystemColors.Control;
            this.labelTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTime.Location = new System.Drawing.Point(59, 40);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(207, 19);
            this.labelTime.TabIndex = 2;
            this.labelTime.Text = "time";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Value:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Time:";
            // 
            // labelPoint
            // 
            this.labelPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelPoint.BackColor = System.Drawing.SystemColors.Control;
            this.labelPoint.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPoint.Location = new System.Drawing.Point(56, 21);
            this.labelPoint.Name = "labelPoint";
            this.labelPoint.Size = new System.Drawing.Size(112, 19);
            this.labelPoint.TabIndex = 3;
            this.labelPoint.Text = "value";
            // 
            // FormChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 733);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chart1);
            this.Name = "FormChart";
            this.Text = "FormChart";
            this.Load += new System.EventHandler(this.FormChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public void LoadChart(DataStorage data, DataLogConfig datalogconfig)
        {
            int num5;
            int num = 0;
            int index = -1;
            this.buttonPause.Visible = false;
            this.buttonConfig.Visible = false;
            ChartArea area = this.chart1.ChartAreas[0];
            area.AxisX.LabelStyle.Format = "dd/MM HH.mm.ss";
            area.AxisX.LabelStyle.Interval = 0.0;
            area.AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Auto;
            area.CursorX.Interval = 0.1;
            area.CursorX.IntervalType = DateTimeIntervalType.Seconds;
            area.CursorX.IntervalOffsetType = DateTimeIntervalType.Seconds;
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.CursorX.LineColor = Color.Red;
            area.CursorX.SelectionColor = Color.LightSteelBlue;
            area.CursorY.Interval = 0.1;
            area.CursorY.IsUserEnabled = true;
            area.CursorY.IsUserSelectionEnabled = true;
            area.CursorY.LineColor = Color.Red;
            area.CursorY.SelectionColor = Color.LightSteelBlue;
            this.chart1.ChartAreas[0].Axes[0].LabelStyle.Interval = 0.0;
            if (data.DataSnapShots.Count > 200)
            {
                num5 = data.DataSnapShots.Count / 200;
            }
            else
            {
                num5 = 0;
            }
            for (int i = 0; i < data.DataSnapShots.Count; i++)
            {
                DataStorage.DataSnapShot shot = (DataStorage.DataSnapShot) data.DataSnapShots[i];
                short[] array = new short[((DataStorage.DataSnapShot.DataDevice) shot.DataDevices[0]).Registers.Data.Count];
                ((DataStorage.DataSnapShot.DataDevice) shot.DataDevices[0]).Registers.Data.CopyTo(array);
                index = -1;
                num = 0;
                while (num < this.chart1.Series.Count)
                {
                    Series series = this.chart1.Series[num];
                    while (++index < datalogconfig.Registers.Length)
                    {
                        if (datalogconfig.Registers[index].Enable)
                        {
                            decimal num4 = array[index] * datalogconfig.Registers[index].Gain;
                            if (num4 > -500M)
                            {
                                series.BorderDashStyle = ChartDashStyle.Solid;
                                series.Points.AddXY(shot.Time.ToOADate(), new object[] { num4 });
                            }
                            else
                            {
                                series.BorderDashStyle = ChartDashStyle.Dash;
                                series.Points.AddXY(shot.Time.ToOADate(), 0.0);
                            }
                            break;
                        }
                    }
                    num++;
                }
                i += num5;
            }
            index = -1;
            for (num = 0; num < this.chart1.Series.Count; num++)
            {
                Series series2 = this.chart1.Series[num];
                while (++index < datalogconfig.Registers.Length)
                {
                    if (datalogconfig.Registers[index].Enable)
                    {
                        if (datalogconfig.Registers[index].Name == "")
                        {
                            series2.Name = datalogconfig.Registers[index].Description;
                        }
                        else
                        {
                            series2.Name = datalogconfig.Registers[index].Name;
                        }
                        break;
                    }
                }
            }
        }

        public void LoadChart(DateTime timestamp, short[] data, DataLogConfig datalogconfig)
        {
            int num = 0;
            int index = -1;
            this.chart1.ChartAreas[0].Axes[0].LabelStyle.Interval = 0.0;
            for (num = 0; num < this.chart1.Series.Count; num++)
            {
                Series series = this.chart1.Series[num];
                while (++index < datalogconfig.Registers.Length)
                {
                    if (datalogconfig.Registers[index].Enable)
                    {
                        decimal num3 = data[index] * datalogconfig.Registers[index].Gain;
                        if (num3 > -500M)
                        {
                            series.BorderDashStyle = ChartDashStyle.Solid;
                            series.Points.AddXY(timestamp.ToOADate(), new object[] { num3 });
                        }
                        else
                        {
                            series.BorderDashStyle = ChartDashStyle.Dash;
                            series.Points.AddXY(timestamp.ToOADate(), 0.0);
                        }
                        break;
                    }
                }
            }
            index = -1;
            for (num = 0; num < this.chart1.Series.Count; num++)
            {
                Series series2 = this.chart1.Series[num];
                while (++index < datalogconfig.Registers.Length)
                {
                    if (datalogconfig.Registers[index].Enable)
                    {
                        if (datalogconfig.Registers[index].Name == "")
                        {
                            series2.Name = datalogconfig.Registers[index].Description;
                        }
                        else
                        {
                            series2.Name = datalogconfig.Registers[index].Name;
                        }
                        break;
                    }
                }
            }
            this.chart1.ResetAutoValues();
            this.chart1.Invalidate();
        }

        public void PlotAllSeries()
        {
            DateTime now = DateTime.Now;
            int index = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            if (!this.pause)
            {
                int num7 = 0x7d0;
                int num8 = 0x7d0;
                if (this.chartsConfig != null)
                {
                    int num2;
                    decimal num3;
                    short[] registersFromDevice;
                    for (index = 0; index < this.currentBmsProject.DeviceArrayList.Count; index++)
                    {
                        this.datalogTemp = this.currentBmsProject.GetDevice(index).DataLogCfg;
                        registersFromDevice = this.currentSupervisor.GetRegistersFromDevice(index);
                        num2 = 0;
                        while ((num2 < this.datalogTemp.Registers.Length) && (num6 < this.chartsConfig.graphRegisters.Count))
                        {
                            ChartConfigCollection.ChartConfig.Serie serieRegisters = this.chartsConfig.GetSerieRegisters(num6);
                            if (this.datalogTemp.Registers[num2].Address == serieRegisters.Address)
                            {
                                Series series = this.chart1.Series[num4];
                                num4++;
                                num6++;
                                num3 = registersFromDevice[num2] * this.datalogTemp.Registers[num2].Gain;
                                if (num3 > -500M)
                                {
                                    series.BorderDashStyle = ChartDashStyle.Solid;
                                    series.Points.AddXY(now.ToOADate(), new object[] { num3 });
                                }
                                else
                                {
                                    series.BorderDashStyle = ChartDashStyle.Dash;
                                }
                            }
                            num2++;
                        }
                    }
                    for (index = 0; index < this.currentBmsProject.DeviceArrayList.Count; index++)
                    {
                        this.datalogTemp = this.currentBmsProject.GetDevice(index).DataLogCfg;
                        registersFromDevice = this.currentSupervisor.GetCoilsFromDevice(index);
                        for (num2 = 0; (num2 < this.datalogTemp.Coils.Length) && (num5 < this.chartsConfig.graphCoils.Count); num2++)
                        {
                            ChartConfigCollection.ChartConfig.Serie serieCoils = this.chartsConfig.GetSerieCoils(num5);
                            Series series2 = this.chart1.Series[num4];
                            if (serieCoils.Address == this.datalogTemp.Coils[num2].Address)
                            {
                                num3 = registersFromDevice[num2];
                                num4++;
                                num5++;
                                series2.BorderDashStyle = ChartDashStyle.Solid;
                                series2.Points.AddXY(now.ToOADate(), new object[] { num3 });
                            }
                        }
                    }
                    this.chart1.ResetAutoValues();
                    if (this.chart1.Series.Count != 0)
                    {
                        while (this.chart1.Series[0].Points.Count > num7)
                        {
                            for (index = 0; index < this.chart1.Series.Count; index++)
                            {
                                while (this.chart1.Series[index].Points.Count > num8)
                                {
                                    this.chart1.Series[index].Points.RemoveAt(0);
                                }
                            }
                        }
                        this.chart1.Invalidate();
                    }
                }
            }
        }

        public void PlotChart()
        {
            DateTime now = DateTime.Now;
            int index = 0;
            int num3 = 0;
            if (!this.pause)
            {
                int num4 = 0x7d0;
                int num5 = 0x7d0;
                if (this.chartsConfig != null)
                {
                    decimal num2;
                    for (index = 0; index < this.chartsConfig.graphRegisters.Count; index++)
                    {
                        Series series = this.chart1.Series[num3];
                        series.Name = this.chartsConfig.GetSerieRegisters(index).SerieDescription;
                        num3++;
                        num2 = this.currentSupervisor.GetRegisterFromModbusId(this.chartsConfig.GetSerieRegisters(index).DeviceId, this.chartsConfig.GetSerieRegisters(index).Address, true);
                        if (num2 > -500M)
                        {
                            series.BorderDashStyle = ChartDashStyle.Solid;
                            series.Points.AddXY(now.ToOADate(), new object[] { num2 });
                        }
                        else
                        {
                            series.BorderDashStyle = ChartDashStyle.Dash;
                        }
                    }
                    for (index = 0; index < this.chartsConfig.graphCoils.Count; index++)
                    {
                        Series series2 = this.chart1.Series[num3];
                        series2.Name = this.chartsConfig.GetSerieCoils(index).SerieDescription;
                        num3++;
                        num2 = this.currentSupervisor.GetCoilFromModbusId(this.chartsConfig.GetSerieCoils(index).DeviceId, this.chartsConfig.GetSerieCoils(index).Address, true);
                        if (num2 > -500M)
                        {
                            series2.BorderDashStyle = ChartDashStyle.Solid;
                            series2.Points.AddXY(now.ToOADate(), new object[] { num2 });
                        }
                        else
                        {
                            series2.BorderDashStyle = ChartDashStyle.Dash;
                        }
                    }
                    this.chart1.ResetAutoValues();
                    if (this.chart1.Series.Count != 0)
                    {
                        while (this.chart1.Series[0].Points.Count > num4)
                        {
                            for (index = 0; index < this.chart1.Series.Count; index++)
                            {
                                while (this.chart1.Series[index].Points.Count > num5)
                                {
                                    this.chart1.Series[index].Points.RemoveAt(0);
                                }
                            }
                        }
                        this.chart1.Invalidate();
                    }
                }
            }
        }

        private void PlotChart(int deviceId, DataLogConfig datalogconfig)
        {
            int num;
            this.currentSupervisor.GetRegistersFromDevice(deviceId);
            this.chart1.Series.Clear();
            this.chart1.ChartAreas.Clear();
            for (num = 0; num < datalogconfig.Registers.Length; num++)
            {
                bool enable = datalogconfig.Registers[num].Enable;
            }
            for (num = 0; num < datalogconfig.Coils.Length; num++)
            {
                if (datalogconfig.Coils[num].Enable)
                {
                    Series item = new Series("Serie" + num.ToString());
                    this.chart1.Series.Add(item);
                }
            }
        }

        public void PlotChart(short[] data, DataLogConfig datalogconfig)
        {
            DateTime now = DateTime.Now;
            int num = 0;
            int index = -1;
            if (!this.pause)
            {
                int num4 = 200;
                int num5 = 200;
                num = 0;
                while (num < this.chart1.Series.Count)
                {
                    Series series = this.chart1.Series[num];
                    while (++index < datalogconfig.Registers.Length)
                    {
                        if (datalogconfig.Registers[index].Enable)
                        {
                            decimal num3 = data[index] * datalogconfig.Registers[index].Gain;
                            if (num3 > -500M)
                            {
                                series.BorderDashStyle = ChartDashStyle.Solid;
                                series.Points.AddXY(now.ToOADate(), new object[] { num3 });
                            }
                            else
                            {
                                series.BorderDashStyle = ChartDashStyle.Dash;
                            }
                            break;
                        }
                    }
                    num++;
                }
                this.chart1.ResetAutoValues();
                while (this.chart1.Series[0].Points.Count > num4)
                {
                    num = 0;
                    goto Label_0136;
                Label_00F7:
                    this.chart1.Series[num].Points.RemoveAt(0);
                Label_0113:
                    if (this.chart1.Series[num].Points.Count > num5)
                    {
                        goto Label_00F7;
                    }
                    num++;
                Label_0136:
                    if (num < this.chart1.Series.Count)
                    {
                        goto Label_0113;
                    }
                }
                index = -1;
                for (num = 0; num < this.chart1.Series.Count; num++)
                {
                    Series series2 = this.chart1.Series[num];
                    while (++index < datalogconfig.Registers.Length)
                    {
                        if (datalogconfig.Registers[index].Enable)
                        {
                            if (datalogconfig.Registers[index].Name == "")
                            {
                                series2.Name = datalogconfig.Registers[index].Description;
                            }
                            else
                            {
                                series2.Name = datalogconfig.Registers[index].Name;
                            }
                            break;
                        }
                    }
                }
                this.chart1.Invalidate();
            }
        }

        private void SetPosition(Axis axis, double position)
        {
            if (!double.IsNaN(position))
            {
                string str = "";
                string str2 = "";
                if (axis.AxisName == AxisName.X)
                {
                    str = DateTime.FromOADate(position).ToString("dddd dd/MM HH.mm.ss");
                    this.labelTime.Text = str;
                }
                else
                {
                    str2 = position.ToString();
                    this.labelPoint.Text = str2;
                }
                this.Text = this.graphName + " X: " + this.labelTime.Text + " , Y: " + this.labelPoint.Text;
            }
        }

        private void SetTitle(double position, double size)
        {
            DateTime time = DateTime.FromOADate(position);
            TimeSpan span = DateTime.FromOADate(position + size).Subtract(time);
            this.chart1.Titles["Title1"].Text = "Start Date: ";
            Title local1 = this.chart1.Titles["Title1"];
            local1.Text = local1.Text + time.ToLongDateString();
            Title local2 = this.chart1.Titles["Title1"];
            local2.Text = local2.Text + "\n Number of Hours: ";
            Title local3 = this.chart1.Titles["Title1"];
            local3.Text = local3.Text + span.TotalHours.ToString();
        }

        private void UpdateAllSeries()
        {
            int num4 = 0;
            if (this.chartsConfig != null)
            {
                int index = 0;
                while (index < this.currentBmsProject.DeviceArrayList.Count)
                {
                    this.datalogTemp = this.currentBmsProject.GetDevice(index).DataLogCfg;
                    for (int i = 0; i < this.datalogTemp.Registers.Length; i++)
                    {
                        if (this.datalogTemp.Registers[i].Enable)
                        {
                            ChartConfigCollection.ChartConfig.Serie serieRegisters = new ChartConfigCollection.ChartConfig.Serie();
                            Series series = this.chart1.Series[num4];
                            num4++;
                            series.Enabled = false;
                            for (int j = 0; j < this.chartsConfig.graphRegisters.Count; j++)
                            {
                                serieRegisters = this.chartsConfig.GetSerieRegisters(j);
                                if ((serieRegisters.DeviceId == this.currentBmsProject.GetDevice(index).ModBusID) && (serieRegisters.Address == this.datalogTemp.Registers[i].Address))
                                {
                                    series.Color = serieRegisters.LineColor;
                                    series.BorderWidth = serieRegisters.Line;
                                    series.ChartArea = "RegisterArea";
                                    series.Enabled = true;
                                }
                            }
                        }
                    }
                    index++;
                }
                while (this.datalogTemp.Registers.Length < this.chart1.Series.Count)
                {
                    this.chart1.Series.RemoveAt(this.datalogTemp.Registers.Length);
                }
                for (index = 0; index < this.chartsConfig.graphCoils.Count; index++)
                {
                    ChartConfigCollection.ChartConfig.Serie serieCoils = this.chartsConfig.GetSerieCoils(index);
                    Series item = new Series(serieCoils.SerieDescription) {
                        Color = serieCoils.LineColor,
                        ChartType = SeriesChartType.FastLine,
                        XValueType = ChartValueType.Time,
                        BorderWidth = serieCoils.Line,
                        ChartArea = "CoilArea" + index.ToString()
                    };
                    this.chart1.Series.Add(item);
                }
            }
        }

        public void UpdateChart()
        {
            if (this.currentDevices == null)
            {
                this.currentDevices = new Device[] { this.currentSupervisor.deviceDB.GetDevice(0) };
            }
            this.buttonPause.Visible = true;
            this.PlotAllSeries();
        }

        private void UpdateChartAreas()
        {
            if (this.chartsConfig != null)
            {
                for (int i = 0; i < this.chartsConfig.graphCoils.Count; i++)
                {
                    ChartArea item = new ChartArea("CoilArea" + i.ToString()) {
                        AxisX = { LabelStyle = { 
                            Format = "dd/MM HH.mm.ss",
                            Interval = 0.0,
                            IntervalOffsetType = DateTimeIntervalType.Auto
                        } }
                    };
                    this.chart1.ChartAreas.Add(item);
                }
            }
        }

        private void UpdateSeries()
        {
            bool flag = false;
            if (this.chartsConfig != null)
            {
                int num2;
                int index = 0;
                while (index < this.chartsConfig.graphRegisters.Count)
                {
                    new Series(this.chartsConfig.GetSerieRegisters(index).SerieDescription);
                    num2 = 0;
                    while (num2 < this.chart1.Series.Count)
                    {
                        num2++;
                    }
                    index++;
                }
                index = 0;
                while (index < this.chartsConfig.graphRegisters.Count)
                {
                    ChartConfigCollection.ChartConfig.Serie serieRegisters = this.chartsConfig.GetSerieRegisters(index);
                    Series item = new Series(serieRegisters.SerieDescription) {
                        Color = serieRegisters.LineColor,
                        ChartType = SeriesChartType.FastLine,
                        XValueType = ChartValueType.Time,
                        BorderWidth = serieRegisters.Line,
                        ChartArea = "RegisterArea"
                    };
                    for (num2 = 0; num2 < this.chart1.Series.Count; num2++)
                    {
                        if (this.chart1.Series[num2].Name == item.Name)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.chart1.Series.Add(item);
                    }
                    flag = false;
                    index++;
                }
                for (index = 0; index < this.chartsConfig.graphCoils.Count; index++)
                {
                    ChartConfigCollection.ChartConfig.Serie serieCoils = this.chartsConfig.GetSerieCoils(index);
                    Series series2 = new Series(serieCoils.SerieDescription) {
                        Color = serieCoils.LineColor,
                        ChartType = SeriesChartType.FastLine,
                        XValueType = ChartValueType.Time,
                        BorderWidth = serieCoils.Line,
                        ChartArea = "CoilArea" + index.ToString()
                    };
                    this.chart1.Series.Add(series2);
                }
            }
        }
    }
}

