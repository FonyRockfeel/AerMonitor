namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO.Ports;
    using System.Windows.Forms;

    public class FormProject : Form
    {
        private BmsProject aermecDatabase;
        private Button buttonAdd;
        private Button buttonCancel;
        private Button buttonConfigLog;
        private Button buttonDel;
        private Button buttonOk;
        private Button buttonRefresh;
        private CheckBox checkBoxAutoSaveRate;
        private CheckBox checkBoxEnable;
        private ComboBox comboBoxBaudRate;
        private ComboBox comboBoxParity;
        private ComboBox comboBoxPort;
        private ComboBox comboBoxStopBits;
        private IContainer components;
        private FolderBrowserDialog folderBrowserDialog1;
        private GroupBox groupBox1;
        private Label label1;
        private Label label10;
        private Label label12;
        private Label label13;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private ListBox listBoxNewDevices;
        private bool loading;
        public BmsProject newBmsDatabase;
        public bool newProject;
        private NumericUpDown numericUpDownModbusId;
        private NumericUpDown numericUpDownSaveRate;
        private NumericUpDown numericUpDownScanRate;
        private NumericUpDown numericUpDownTimeout;
        private SaveFileDialog saveFileDialog1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TextBox textBoxFileName;
        private TextBox textBoxName;
        private TabControl tabControl2;
        private TabPage tabPageCOMport;
        private TabPage tabPageTcpIP;
        private Label label11;
        private Label label14;
        private TextBox textBoxPort;
        private TextBox textBoxIPAddress;
        private ToolTip toolTip1;

        public FormProject()
        {
            this.InitializeComponent();
        }

        private void buttonAdd_Click_1(object sender, EventArgs e)
        {
            FormDevicesSelection selection = new FormDevicesSelection();
            byte modBusID = 0;
            this.aermecDatabase = FilesManage.LoadAermecDatabaseFromFile();
            selection.AermecDatabase = this.aermecDatabase;
            if ((selection.ShowDialog() == DialogResult.OK) && (selection.deviceSelected >= 0))
            {
                Device dev = ((Device) this.aermecDatabase.DeviceArrayList[selection.deviceSelected]).Clone();
                for (int i = 0; i < this.newBmsDatabase.DeviceArrayList.Count; i++)
                {
                    if (((Device) this.newBmsDatabase.DeviceArrayList[i]).ModBusID > modBusID)
                    {
                        modBusID = ((Device) this.newBmsDatabase.DeviceArrayList[i]).ModBusID;
                    }
                }
                modBusID = (byte) (modBusID + 1);
                dev.ModBusID = modBusID;
                this.newBmsDatabase.AddDevice(dev);
                if (this.newBmsDatabase.Name == "")
                {
                    this.newBmsDatabase.Name = "NewBMS";
                }
                this.LoadBms();
            }
        }

        private void buttonConfigLog_Click(object sender, EventArgs e)
        {
            FormLogger logger = new FormLogger();
            if (((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).DataLogCfg != null)
            {
                logger.device = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).Clone();
                if (logger.ShowDialog() == DialogResult.OK)
                {
                    this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex] = logger.device;
                }
            }
            else
            {
                MessageBox.Show("Не задано действие!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listBoxNewDevices.SelectedIndex;
            if (this.listBoxNewDevices.SelectedIndex >= 0)
            {
                this.newBmsDatabase.DeviceArrayList.RemoveAt(this.listBoxNewDevices.SelectedIndex);
            }
            this.LoadBms();
            if (selectedIndex > -1)
            {
                if (selectedIndex < this.listBoxNewDevices.Items.Count)
                {
                    this.listBoxNewDevices.SelectedIndex = selectedIndex;
                }
                else
                {
                    this.listBoxNewDevices.SelectedIndex = this.listBoxNewDevices.Items.Count - 1;
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.listBoxNewDevices.Items.Count == 0)
            {
                MessageBox.Show("Не выбрано устройство! Добавьте одно или несколько.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.textBoxFileName.Focus();
            }
            else if (this.textBoxFileName.Text == "")
            {
                MessageBox.Show("Пустое имя BMS!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.textBoxFileName.Focus();
            }
            else if (!this.newBmsDatabase.ModbusIdOk())
            {
                MessageBox.Show("Некоторые устройства имеют одинаковые Modbus ID! Modbus ID должны быть уникальными! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.textBoxFileName.Focus();
            }
            else
            {
                // В качестве источика данных выбран COM port
                if (tabControl2.SelectedTab == tabControl2.TabPages["tabPageCOMport"])
                {
                    if (this.comboBoxPort.SelectedItem != null)
                    {
                        this.newBmsDatabase.BmsSerialConfig.ComPort = this.comboBoxPort.SelectedItem.ToString();
                    }
                    else if (this.comboBoxPort.Items.Count > 0)
                    {
                        MessageBox.Show("Выберите источник данных!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.tabControl1.SelectedIndex = 1;
                        return;
                    }                    
                    this.newBmsDatabase.BmsSerialConfig.BaudRate = int.Parse(this.comboBoxBaudRate.SelectedItem.ToString());
                    this.newBmsDatabase.BmsSerialConfig.Parity = (Parity)this.comboBoxParity.SelectedIndex;
                    this.newBmsDatabase.BmsSerialConfig.StopBits = (StopBits)this.comboBoxStopBits.SelectedIndex;
                    this.newBmsDatabase.LogSaveRate = (int)this.numericUpDownSaveRate.Value;
                    this.newBmsDatabase.LogSaveRateAuto = this.checkBoxAutoSaveRate.Checked;
                    this.newBmsDatabase.dataSource = DataSource.COM;                    
                }
                // Источник данных TcpIP
                else if (tabControl2.SelectedTab == tabControl2.TabPages["tabPageTcpIP"])
                {
                    if (this.textBoxIPAddress != null && this.textBoxIPAddress.Text != "")
                    {
                        this.newBmsDatabase.BmsTcpIPConfig.IPaddress = this.textBoxIPAddress.Text.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Введите IP адрес!");
                        return;
                    }

                    if (this.textBoxPort != null && this.textBoxPort.Text != "0")
                    {
                        this.newBmsDatabase.BmsTcpIPConfig.Port = int.Parse(this.textBoxPort.Text);
                    }
                    else
                    {
                        MessageBox.Show("Введите порт!");
                        return;
                    }
                    this.newBmsDatabase.dataSource = DataSource.TcpIP;                    
                }
                this.SaveBmsFile();
                this.newBmsDatabase.CleanDataLogConfig();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.LoadPortAvailable();
        }

        private void checkBoxAuto_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownSaveRate.Enabled = !this.checkBoxAutoSaveRate.Checked;
        }

        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listBoxNewDevices.SelectedIndex >= 0)
            {
                ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).Enabled = this.checkBoxEnable.Checked;
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

        private void FormProject_Load(object sender, EventArgs e)
        {
            this.LoadPortAvailable();
            this.LoadBms();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownScanRate = new System.Windows.Forms.NumericUpDown();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.buttonConfigLog = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownTimeout = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownModbusId = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonDel = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.listBoxNewDevices = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPageCOMport = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPageTcpIP = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxAutoSaveRate = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownSaveRate = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModbusId)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPageCOMport.SuspendLayout();
            this.tabPageTcpIP.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveRate)).BeginInit();
            this.SuspendLayout();
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(320, 370);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(55, 23);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "Ок";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(381, 370);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileName.Location = new System.Drawing.Point(86, 338);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(370, 20);
            this.textBoxFileName.TabIndex = 8;
            this.textBoxFileName.Leave += new System.EventHandler(this.textBoxFileName_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 341);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "BMS имя:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(445, 320);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.buttonDel);
            this.tabPage2.Controls.Add(this.buttonAdd);
            this.tabPage2.Controls.Add(this.listBoxNewDevices);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(437, 294);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "BMS Config";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownScanRate);
            this.groupBox1.Controls.Add(this.checkBoxEnable);
            this.groupBox1.Controls.Add(this.buttonConfigLog);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericUpDownTimeout);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericUpDownModbusId);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(193, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 199);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Конфигурация оборудования";
            // 
            // numericUpDownScanRate
            // 
            this.numericUpDownScanRate.Location = new System.Drawing.Point(180, 108);
            this.numericUpDownScanRate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownScanRate.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownScanRate.Name = "numericUpDownScanRate";
            this.numericUpDownScanRate.Size = new System.Drawing.Size(52, 20);
            this.numericUpDownScanRate.TabIndex = 17;
            this.numericUpDownScanRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownScanRate.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownScanRate.ValueChanged += new System.EventHandler(this.numericUpDownScanRate_ValueChanged);
            // 
            // checkBoxEnable
            // 
            this.checkBoxEnable.AutoSize = true;
            this.checkBoxEnable.Location = new System.Drawing.Point(16, 26);
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.Size = new System.Drawing.Size(177, 17);
            this.checkBoxEnable.TabIndex = 16;
            this.checkBoxEnable.Text = "Enable Datalogging from device";
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            this.checkBoxEnable.CheckedChanged += new System.EventHandler(this.checkBoxEnable_CheckedChanged);
            // 
            // buttonConfigLog
            // 
            this.buttonConfigLog.Location = new System.Drawing.Point(17, 164);
            this.buttonConfigLog.Name = "buttonConfigLog";
            this.buttonConfigLog.Size = new System.Drawing.Size(126, 23);
            this.buttonConfigLog.TabIndex = 15;
            this.buttonConfigLog.Text = "Config Datalogger...";
            this.buttonConfigLog.UseVisualStyleBackColor = true;
            this.buttonConfigLog.Click += new System.EventHandler(this.buttonConfigLog_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(79, 49);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(153, 20);
            this.textBoxName.TabIndex = 7;
            this.textBoxName.Leave += new System.EventHandler(this.textBoxName_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Default Timeout (ms):";
            // 
            // numericUpDownTimeout
            // 
            this.numericUpDownTimeout.Location = new System.Drawing.Point(180, 134);
            this.numericUpDownTimeout.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownTimeout.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownTimeout.Name = "numericUpDownTimeout";
            this.numericUpDownTimeout.Size = new System.Drawing.Size(52, 20);
            this.numericUpDownTimeout.TabIndex = 4;
            this.numericUpDownTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTimeout.ValueChanged += new System.EventHandler(this.numericUpDownTimeout_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Default ScanRate (ms):";
            // 
            // numericUpDownModbusId
            // 
            this.numericUpDownModbusId.Location = new System.Drawing.Point(180, 82);
            this.numericUpDownModbusId.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownModbusId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownModbusId.Name = "numericUpDownModbusId";
            this.numericUpDownModbusId.Size = new System.Drawing.Size(52, 20);
            this.numericUpDownModbusId.TabIndex = 1;
            this.numericUpDownModbusId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownModbusId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownModbusId.ValueChanged += new System.EventHandler(this.numericUpDownModbusId_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Default Modbus ID :";
            // 
            // buttonDel
            // 
            this.buttonDel.Location = new System.Drawing.Point(100, 265);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(86, 23);
            this.buttonDel.TabIndex = 13;
            this.buttonDel.Text = "Удалить";
            this.buttonDel.UseVisualStyleBackColor = true;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(6, 265);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(88, 23);
            this.buttonAdd.TabIndex = 12;
            this.buttonAdd.Text = "Добавить...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click_1);
            // 
            // listBoxNewDevices
            // 
            this.listBoxNewDevices.FormattingEnabled = true;
            this.listBoxNewDevices.Location = new System.Drawing.Point(6, 25);
            this.listBoxNewDevices.Name = "listBoxNewDevices";
            this.listBoxNewDevices.Size = new System.Drawing.Size(180, 238);
            this.listBoxNewDevices.TabIndex = 0;
            this.listBoxNewDevices.SelectedIndexChanged += new System.EventHandler(this.listBoxNewDevices_SelectedIndexChanged_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "BMS список оборудования";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabControl2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(437, 294);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Источник данных";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPageCOMport);
            this.tabControl2.Controls.Add(this.tabPageTcpIP);
            this.tabControl2.Location = new System.Drawing.Point(5, 12);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(429, 279);
            this.tabControl2.TabIndex = 9;
            // 
            // tabPageCOMport
            // 
            this.tabPageCOMport.Controls.Add(this.label3);
            this.tabPageCOMport.Controls.Add(this.buttonRefresh);
            this.tabPageCOMport.Controls.Add(this.comboBoxPort);
            this.tabPageCOMport.Controls.Add(this.comboBoxStopBits);
            this.tabPageCOMport.Controls.Add(this.label4);
            this.tabPageCOMport.Controls.Add(this.label10);
            this.tabPageCOMport.Controls.Add(this.comboBoxBaudRate);
            this.tabPageCOMport.Controls.Add(this.comboBoxParity);
            this.tabPageCOMport.Controls.Add(this.label9);
            this.tabPageCOMport.Location = new System.Drawing.Point(4, 22);
            this.tabPageCOMport.Name = "tabPageCOMport";
            this.tabPageCOMport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCOMport.Size = new System.Drawing.Size(421, 253);
            this.tabPageCOMport.TabIndex = 0;
            this.tabPageCOMport.Text = "COM port";
            this.tabPageCOMport.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "COM Port:";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(286, 37);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 8;
            this.buttonRefresh.Text = "Обновить";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(157, 39);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
            this.comboBoxPort.TabIndex = 0;
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "Two"});
            this.comboBoxStopBits.Location = new System.Drawing.Point(157, 160);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(104, 21);
            this.comboBoxStopBits.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Baude Rate:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(79, 163);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Stop Bits:";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "1200",
            "9600",
            "19200",
            "38400"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(157, 77);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(104, 21);
            this.comboBoxBaudRate.TabIndex = 1;
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.comboBoxParity.Location = new System.Drawing.Point(157, 117);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(104, 21);
            this.comboBoxParity.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(79, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Parity:";
            // 
            // tabPageTcpIP
            // 
            this.tabPageTcpIP.Controls.Add(this.textBoxPort);
            this.tabPageTcpIP.Controls.Add(this.textBoxIPAddress);
            this.tabPageTcpIP.Controls.Add(this.label14);
            this.tabPageTcpIP.Controls.Add(this.label11);
            this.tabPageTcpIP.Location = new System.Drawing.Point(4, 22);
            this.tabPageTcpIP.Name = "tabPageTcpIP";
            this.tabPageTcpIP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTcpIP.Size = new System.Drawing.Size(421, 253);
            this.tabPageTcpIP.TabIndex = 1;
            this.tabPageTcpIP.Text = "TcpIP";
            this.tabPageTcpIP.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxAutoSaveRate);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.numericUpDownSaveRate);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(437, 294);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Data Log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoSaveRate
            // 
            this.checkBoxAutoSaveRate.AutoSize = true;
            this.checkBoxAutoSaveRate.Location = new System.Drawing.Point(32, 30);
            this.checkBoxAutoSaveRate.Name = "checkBoxAutoSaveRate";
            this.checkBoxAutoSaveRate.Size = new System.Drawing.Size(48, 17);
            this.checkBoxAutoSaveRate.TabIndex = 14;
            this.checkBoxAutoSaveRate.Text = "Auto";
            this.toolTip1.SetToolTip(this.checkBoxAutoSaveRate, "Max data save. More data space expensive to disk");
            this.checkBoxAutoSaveRate.UseVisualStyleBackColor = true;
            this.checkBoxAutoSaveRate.Visible = false;
            this.checkBoxAutoSaveRate.CheckedChanged += new System.EventHandler(this.checkBoxAuto_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(226, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Seconds";
            this.label13.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(86, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Save every:";
            this.label12.Visible = false;
            // 
            // numericUpDownSaveRate
            // 
            this.numericUpDownSaveRate.Location = new System.Drawing.Point(156, 29);
            this.numericUpDownSaveRate.Name = "numericUpDownSaveRate";
            this.numericUpDownSaveRate.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownSaveRate.TabIndex = 11;
            this.toolTip1.SetToolTip(this.numericUpDownSaveRate, "Seconds every you want to save the data. Not depend from speed of the devices");
            this.numericUpDownSaveRate.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(76, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "IP address:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(76, 82);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Port:";
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(142, 38);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(126, 20);
            this.textBoxIPAddress.TabIndex = 8;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(142, 79);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(126, 20);
            this.textBoxPort.TabIndex = 9;
            // 
            // FormProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(468, 405);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProject";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BMS";
            this.Load += new System.EventHandler(this.FormProject_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModbusId)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPageCOMport.ResumeLayout(false);
            this.tabPageCOMport.PerformLayout();
            this.tabPageTcpIP.ResumeLayout(false);
            this.tabPageTcpIP.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void listBoxNewDevices_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                if (this.listBoxNewDevices.SelectedIndex >= 0)
                {
                    this.loading = true;
                    this.groupBox1.Enabled = true;
                    this.buttonDel.Enabled = true;
                    this.textBoxName.Text = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).DeviceName;
                    this.numericUpDownModbusId.Value = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).ModBusID;
                    this.numericUpDownScanRate.Value = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).ScanRate;
                    this.numericUpDownTimeout.Value = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).Timeout;
                    this.checkBoxEnable.Checked = ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).Enabled;
                    this.loading = false;
                }
                else
                {
                    this.groupBox1.Enabled = false;
                    this.buttonDel.Enabled = false;
                }
            }
        }

        private void LoadBms()
        {
            this.listBoxNewDevices.Items.Clear();
            if (this.newBmsDatabase == null)
            {
                this.newBmsDatabase = new BmsProject();
            }
            for (int i = 0; i < this.newBmsDatabase.DeviceArrayList.Count; i++)
            {
                Device device = (Device) this.newBmsDatabase.DeviceArrayList[i];
                this.listBoxNewDevices.Items.Add(device.DeviceName + " - Modbus ID: " + device.ModBusID);
            }
            this.textBoxFileName.Text = this.newBmsDatabase.Name;
            //COM
            this.comboBoxPort.SelectedItem = this.newBmsDatabase.BmsSerialConfig.ComPort;
            this.comboBoxBaudRate.SelectedItem = this.newBmsDatabase.BmsSerialConfig.BaudRate.ToString();
            this.comboBoxParity.SelectedItem = this.newBmsDatabase.BmsSerialConfig.Parity.ToString();
            this.comboBoxStopBits.SelectedItem = this.newBmsDatabase.BmsSerialConfig.StopBits.ToString();
            this.numericUpDownSaveRate.Value = this.newBmsDatabase.LogSaveRate;
            this.checkBoxAutoSaveRate.Checked = this.newBmsDatabase.LogSaveRateAuto;
            // TcpIP
            this.textBoxIPAddress.Text = this.newBmsDatabase.BmsTcpIPConfig.IPaddress;
            this.textBoxPort.Text = this.newBmsDatabase.BmsTcpIPConfig.Port.ToString();

            this.buttonDel.Enabled = false;
            this.groupBox1.Enabled = false;
        }

        private void LoadPortAvailable()
        {
            string[] portNames = SerialPort.GetPortNames();
            this.comboBoxPort.Items.Clear();
            foreach (string str in portNames)
            {
                this.comboBoxPort.Items.Add(str);
            }
        }

        private void numericUpDownModbusId_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading && (this.listBoxNewDevices.SelectedIndex >= 0))
            {
                ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).ModBusID = (byte) this.numericUpDownModbusId.Value;
            }
        }

        private void numericUpDownScanRate_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).ScanRate = (int) this.numericUpDownScanRate.Value;
            }
        }

        private void numericUpDownTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                ((Device) this.newBmsDatabase.DeviceArrayList[this.listBoxNewDevices.SelectedIndex]).Timeout = (int) this.numericUpDownTimeout.Value;
            }
        }

        private void SaveBmsFile()
        {
            int num;
            BmsProject[] projectArray = FilesManage.LoadUserBmsDatabaseFromFile();
            if (this.newProject)
            {
                this.newBmsDatabase.Name = this.textBoxFileName.Text;
                for (num = 0; num < projectArray.Length; num++)
                {
                    if (((projectArray[num] != null) && (projectArray[num].Name.ToLower() == this.newBmsDatabase.Name.ToLower())) && (MessageBox.Show("BMS \"" + this.newBmsDatabase.Name + "\" already exist! Overwrite?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No))
                    {
                        return;
                    }
                }
            }
            else if (this.newBmsDatabase.Name != this.textBoxFileName.Text)
            {
                for (num = 0; num < projectArray.Length; num++)
                {
                    if ((projectArray[num].Name.ToLower() == this.textBoxFileName.Text.ToLower()) && (MessageBox.Show("BMS \"" + this.textBoxFileName.Text + "\" already exist! Overwrite?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No))
                    {
                        return;
                    }
                }
                this.newBmsDatabase.Name = this.textBoxFileName.Text;
            }
            FilesManage.SaveUserBmsFile(this.newBmsDatabase);
            base.DialogResult = DialogResult.OK;
        }

        private void textBoxFileName_Leave(object sender, EventArgs e)
        {
            this.newBmsDatabase.Name = this.textBoxFileName.Text;
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                string text = this.textBoxName.Text;
                int selectedIndex = this.listBoxNewDevices.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    ((Device) this.newBmsDatabase.DeviceArrayList[selectedIndex]).DeviceName = text;
                    this.listBoxNewDevices.Items[selectedIndex] = text;
                }
            }
        }
    }
}

