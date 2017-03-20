namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormChartConfig : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private Button buttonCancel;
        private Button buttonOk;
        public ChartConfigCollection.ChartConfig chartsConfig;
        private ChartConfigCollection.ChartConfig chartTemp;
        private const string CoilsType = "Coils";
        private ColorDialog colorDialog1;
        private int colorMem;
        private ComboBox comboBoxDevice;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private DataGridView dataGridView1;
        private ArrayList dataLogConfigs;
        private DataLogConfig datalogTemp;
        public Device[] devices;
        private Label label1;
        private ListBox listBoxDataAvailable;
        public DataLogConfig[] newDataLogConfig;
        private Color[] palette;
        private BmsProject project;
        private RadioButton radioButtonCoils;
        private RadioButton radioButtonRegisters;
        private const string RegistersType = "Registers";
        private SplitContainer splitContainer1;
        private DataTable tableDataSelected;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;

        public FormChartConfig()
        {
            this.chartTemp = new ChartConfigCollection.ChartConfig();
            this.dataLogConfigs = new ArrayList();
            this.palette = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Coral, Color.Cyan, Color.Fuchsia, Color.Lime, Color.Yellow, Color.Green, Color.Black, Color.Turquoise };
            this.colorMem = -1;
            this.InitializeComponent();
        }

        public FormChartConfig(BmsProject bmsProject) : this()
        {
            if (bmsProject != null)
            {
                this.project = bmsProject;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.SaveSeries();
            this.LoadSeries();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.tableDataSelected.Rows.Clear();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.SaveSeries();
        }

        private void comboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadAddress();
        }

        private void CreateDataGrid(DataTable table, DataGridView dataGridView)
        {
            this.tableDataSelected = table;
            dataGridView.DataSource = table;
            dataGridView.Columns[0].ReadOnly = true;
            dataGridView.Columns[1].ReadOnly = true;
            dataGridView.Columns[2].ReadOnly = true;
            dataGridView.Columns[3].Visible = false;
            dataGridView.Columns[5].Visible = false;
            dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataGridViewButtonColumn dataGridViewColumn = new DataGridViewButtonColumn();
            this.dataGridView1.Columns.Add(dataGridViewColumn);
            dataGridViewColumn.HeaderText = "Delete";
            dataGridViewColumn.Text = "Delete";
            dataGridViewColumn.Name = "button";
            dataGridViewColumn.UseColumnTextForButtonValue = true;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ModbusID", typeof(int));
            table.Columns.Add("Address", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Color", typeof(Color));
            table.Columns.Add("Line Color", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.PrimaryKey = new DataColumn[] { table.Columns["Address"] };
            return table;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Color black = new Color();
            black = Color.Black;
            if (((e.ColumnIndex == 4) && (e.RowIndex > -1)) && (this.colorDialog1.ShowDialog() == DialogResult.OK))
            {
                black = this.colorDialog1.Color;
                this.tableDataSelected.Rows[e.RowIndex][e.ColumnIndex - 1] = black;
                this.dataGridView1.CurrentCell.Style.BackColor = black;
                this.dataGridView1.CurrentCell.Selected = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 6) && (e.RowIndex > -1))
            {
                this.tableDataSelected.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.dataGridView1.Rows[e.RowIndex].Cells[4].Style.BackColor = (Color) this.tableDataSelected.Rows[e.RowIndex][3];
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormChartConfig_Load(object sender, EventArgs e)
        {
            this.LoadDevices();
            this.CreateDataGrid(this.CreateDataTable(), this.dataGridView1);
            this.LoadSeries();
        }

        private Color GetColorFromPalette()
        {
            this.colorMem++;
            if (this.colorMem >= this.palette.Length)
            {
                this.colorMem = 0;
            }
            return this.palette[this.colorMem];
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.radioButtonRegisters = new RadioButton();
            this.radioButtonCoils = new RadioButton();
            this.comboBoxDevice = new ComboBox();
            this.label1 = new Label();
            this.button1 = new Button();
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripMenuItem();
            this.colorDialog1 = new ColorDialog();
            this.dataGridView1 = new DataGridView();
            this.splitContainer1 = new SplitContainer();
            this.listBoxDataAvailable = new ListBox();
            this.button2 = new Button();
            this.button3 = new Button();
            this.contextMenuStrip1.SuspendLayout();
            ((ISupportInitialize) this.dataGridView1).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            base.SuspendLayout();
            this.buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonOk.DialogResult = DialogResult.OK;
            this.buttonOk.Location = new Point(0x363, 0x12f);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new Size(0x37, 0x17);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
            this.buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(0x3a1, 0x12f);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(0x4b, 0x17);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.radioButtonRegisters.AutoSize = true;
            this.radioButtonRegisters.Checked = true;
            this.radioButtonRegisters.Location = new Point(12, 0x36);
            this.radioButtonRegisters.Name = "radioButtonRegisters";
            this.radioButtonRegisters.Size = new Size(0x45, 0x11);
            this.radioButtonRegisters.TabIndex = 11;
            this.radioButtonRegisters.TabStop = true;
            this.radioButtonRegisters.Text = "Registers";
            this.radioButtonRegisters.UseVisualStyleBackColor = true;
            this.radioButtonRegisters.CheckedChanged += new EventHandler(this.radioButtonRegisters_CheckedChanged);
            this.radioButtonCoils.AutoSize = true;
            this.radioButtonCoils.Location = new Point(0x57, 0x36);
            this.radioButtonCoils.Name = "radioButtonCoils";
            this.radioButtonCoils.Size = new Size(0x2f, 0x11);
            this.radioButtonCoils.TabIndex = 12;
            this.radioButtonCoils.Text = "Coils";
            this.radioButtonCoils.UseVisualStyleBackColor = true;
            this.radioButtonCoils.CheckedChanged += new EventHandler(this.radioButtonCoils_CheckedChanged);
            this.comboBoxDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new Point(12, 0x19);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new Size(0xc4, 0x15);
            this.comboBoxDevice.TabIndex = 15;
            this.comboBoxDevice.SelectedIndexChanged += new EventHandler(this.comboBoxDevice_SelectedIndexChanged);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x2c, 13);
            this.label1.TabIndex = 0x10;
            this.label1.Text = "Device:";
            this.button1.ContextMenuStrip = this.contextMenuStrip1;
            this.button1.Location = new Point(0x13c, 0x15);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x55, 0x19);
            this.button1.TabIndex = 0x13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItem1, this.toolStripMenuItem2 });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new Size(0x9c, 0x30);
            this.toolStripMenuItem1.BackColor = Color.Gainsboro;
            this.toolStripMenuItem1.ForeColor = SystemColors.ControlText;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(0x9b, 0x16);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(0x9b, 0x16);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            this.colorDialog1.AllowFullOpen = false;
            this.colorDialog1.SolidColorOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new Size(610, 0xd3);
            this.dataGridView1.TabIndex = 0x15;
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentClick += new DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.RowsAdded += new DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.RowsRemoved += new DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            this.splitContainer1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainer1.Location = new Point(12, 0x4d);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.listBoxDataAvailable);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new Size(0x3e0, 0xdb);
            this.splitContainer1.SplitterDistance = 370;
            this.splitContainer1.TabIndex = 0x16;
            this.listBoxDataAvailable.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listBoxDataAvailable.FormattingEnabled = true;
            this.listBoxDataAvailable.Location = new Point(3, 3);
            this.listBoxDataAvailable.Name = "listBoxDataAvailable";
            this.listBoxDataAvailable.Size = new Size(0x16a, 0xd4);
            this.listBoxDataAvailable.TabIndex = 0x13;
            this.listBoxDataAvailable.SelectedIndexChanged += new EventHandler(this.listBoxDataAvailable_SelectedIndexChanged);
            this.listBoxDataAvailable.DoubleClick += new EventHandler(this.listBoxDataAvailable_DoubleClick);
            this.button2.Location = new Point(0x1a1, 0x15);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x4b, 0x17);
            this.button2.TabIndex = 0x17;
            this.button2.Text = "Create";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button3.Location = new Point(0x1fd, 0x15);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x17);
            this.button3.TabIndex = 0x18;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new EventHandler(this.button3_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x3f8, 0x152);
            base.Controls.Add(this.splitContainer1);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.comboBoxDevice);
            base.Controls.Add(this.radioButtonCoils);
            base.Controls.Add(this.radioButtonRegisters);
            base.Controls.Add(this.buttonOk);
            base.Controls.Add(this.buttonCancel);
            base.MinimizeBox = false;
            base.Name = "FormChartConfig";
            base.ShowIcon = false;
            this.Text = "Charts Configuration";
            base.Load += new EventHandler(this.FormChartConfig_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((ISupportInitialize) this.dataGridView1).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void listBoxDataAvailable_DoubleClick(object sender, EventArgs e)
        {
            string str = "";
            string str2 = "";
            if (this.comboBoxDevice.SelectedIndex >= 0)
            {
                int selectedIndex;
                int modBusID = this.project.GetDevice(this.comboBoxDevice.SelectedIndex).ModBusID;
                str2 = this.project.GetDevice(this.comboBoxDevice.SelectedIndex).DeviceName + ": ";
                if (this.radioButtonRegisters.Checked)
                {
                    selectedIndex = this.listBoxDataAvailable.SelectedIndex;
                    if (this.datalogTemp.Registers[selectedIndex].Name != "")
                    {
                        str = str2 + this.datalogTemp.Registers[selectedIndex].Name;
                    }
                    else
                    {
                        str = str2 + this.datalogTemp.Registers[selectedIndex].Description;
                    }
                    if (this.tableDataSelected.Rows.Find(this.datalogTemp.Registers[selectedIndex].Address) == null)
                    {
                        this.tableDataSelected.Rows.Add(new object[] { modBusID, this.datalogTemp.Registers[selectedIndex].Address, str, this.GetColorFromPalette(), "", "Registers" });
                    }
                }
                if (this.radioButtonCoils.Checked)
                {
                    selectedIndex = this.listBoxDataAvailable.SelectedIndex;
                    if (this.datalogTemp.Coils[selectedIndex].Name != "")
                    {
                        str = str2 + this.datalogTemp.Coils[selectedIndex].Name;
                    }
                    else
                    {
                        str = str2 + this.datalogTemp.Coils[selectedIndex].Description;
                    }
                    if (this.tableDataSelected.Rows.Find(this.datalogTemp.Coils[selectedIndex].Address) == null)
                    {
                        this.tableDataSelected.Rows.Add(new object[] { modBusID, this.datalogTemp.Coils[selectedIndex].Address, str, this.GetColorFromPalette(), "", "Coils" });
                    }
                }
            }
        }

        private void listBoxDataAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void LoadAddress()
        {
            if (this.comboBoxDevice.SelectedIndex >= 0)
            {
                int num;
                this.datalogTemp = this.project.GetDevice(this.comboBoxDevice.SelectedIndex).DataLogCfg;
                this.listBoxDataAvailable.Items.Clear();
                if (this.radioButtonRegisters.Checked)
                {
                    for (num = 0; num < this.datalogTemp.Registers.Length; num++)
                    {
                        this.listBoxDataAvailable.Items.Add(this.datalogTemp.Registers[num].Address.ToString("0000") + " - " + this.datalogTemp.Registers[num].Name + " : " + this.datalogTemp.Registers[num].Description);
                    }
                }
                if (this.radioButtonCoils.Checked)
                {
                    for (num = 0; num < this.datalogTemp.Coils.Length; num++)
                    {
                        this.listBoxDataAvailable.Items.Add(this.datalogTemp.Coils[num].Address.ToString("0000") + " - " + this.datalogTemp.Coils[num].Name + " : " + this.datalogTemp.Coils[num].Description);
                    }
                }
            }
        }

        private void LoadDevices()
        {
            int index = 0;
            this.comboBoxDevice.Items.Clear();
            for (index = 0; index < this.project.DeviceCount(); index++)
            {
                this.comboBoxDevice.Items.Add(string.Concat(new object[] { "ID:", this.project.GetDevice(index).ModBusID, " ", this.project.GetDevice(index).DeviceName }));
            }
            if (this.comboBoxDevice.Items.Count > 0)
            {
                this.comboBoxDevice.SelectedIndex = 0;
            }
        }

        private void LoadSeries()
        {
            if (this.chartsConfig != null)
            {
                this.tableDataSelected.Clear();
                int num = 0;
                while (num < this.chartsConfig.graphRegisters.Count)
                {
                    ChartConfigCollection.ChartConfig.Serie serie = (ChartConfigCollection.ChartConfig.Serie) this.chartsConfig.graphRegisters[num];
                    this.tableDataSelected.Rows.Add(new object[] { serie.DeviceId, serie.Address, serie.SerieDescription, serie.LineColor, "", "Registers" });
                    num++;
                }
                for (num = 0; num < this.chartsConfig.graphCoils.Count; num++)
                {
                    ChartConfigCollection.ChartConfig.Serie serie2 = (ChartConfigCollection.ChartConfig.Serie) this.chartsConfig.graphCoils[num];
                    this.tableDataSelected.Rows.Add(new object[] { serie2.DeviceId, serie2.Address, serie2.SerieDescription, serie2.LineColor, "", "Coils" });
                }
            }
        }

        private void radioButtonCoils_CheckedChanged(object sender, EventArgs e)
        {
            this.LoadAddress();
        }

        private void radioButtonRegisters_CheckedChanged(object sender, EventArgs e)
        {
            this.LoadAddress();
        }

        private void SaveSeries()
        {
            if (this.tableDataSelected.Rows.Count > 0)
            {
                this.chartsConfig = new ChartConfigCollection.ChartConfig("Chart");
                for (int i = 0; i < this.tableDataSelected.Rows.Count; i++)
                {
                    ChartConfigCollection.ChartConfig.Serie serie = new ChartConfigCollection.ChartConfig.Serie((int) this.tableDataSelected.Rows[i]["ModbusID"], (int) this.tableDataSelected.Rows[i]["Address"], (string) this.tableDataSelected.Rows[i]["Name"], (Color) this.tableDataSelected.Rows[i]["Color"], 2);
                    if (((string) this.tableDataSelected.Rows[i]["Type"]) == "Registers")
                    {
                        this.chartsConfig.AddSerieRegister(serie);
                    }
                    if (((string) this.tableDataSelected.Rows[i]["Type"]) == "Coils")
                    {
                        this.chartsConfig.AddSerieCoils(serie);
                    }
                }
            }
        }
    }
}

