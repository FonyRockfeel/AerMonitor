namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormDevicesDataB : Form
    {
        public BmsProject AermecDatabase;
        private Button buttonAdd;
        private Button buttonCancel;
        private Button buttonClone;
        private Button buttonDel;
        private Button buttonImport;
        private Button buttonLog;
        private Button buttonMod;
        private Button buttonOk;
        private IContainer components;
        private bool cryptography;
        private int currIndex;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private ListBox listBoxDevices;
        private bool loading;
        private NumericUpDown numericUpDownDBVersion;
        private NumericUpDown numericUpDownModbusId;
        private NumericUpDown numericUpDownScanRate;
        private NumericUpDown numericUpDownTimeout;
        private OpenFileDialog openFileDialog1;
        private RichTextBox richTextBoxNote;
        private TextBox textBoxDesc;
        private TextBox textBoxName;

        public FormDevicesDataB()
        {
            this.InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string name = "Unit";
            int num = this.listBoxDevices.Items.Count + 1;
            while (this.listBoxDevices.FindString(name + num.ToString()) != -1)
            {
                num++;
            }
            name = name + num.ToString();
            this.AermecDatabase.AddDevice(new Device(name));
            this.listBoxDevices.Items.Add(name);
        }

        private void buttonClone_Click(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                Device dev = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Clone();
                dev.DeviceName = dev.DeviceName + "Cloned";
                this.AermecDatabase.AddDevice(dev);
                this.listBoxDevices.Items.Add(dev.DeviceName);
            }
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            if (this.listBoxDevices.SelectedIndex >= 0)
            {
                int selectedIndex = this.listBoxDevices.SelectedIndex;
                this.AermecDatabase.DeviceArrayList.RemoveAt(this.listBoxDevices.SelectedIndex);
                this.listBoxDevices.Items.RemoveAt(this.listBoxDevices.SelectedIndex);
                if (selectedIndex < this.listBoxDevices.Items.Count)
                {
                    this.listBoxDevices.SelectedIndex = selectedIndex;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.AermecDatabase.ImportDevicesFromBmsProject(FilesManage.LoadAermecDatabaseFromFile(this.openFileDialog1.FileName));
                this.LoadDeviceDatabase();
            }
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            FormLogger logger = new FormLogger();
            if (((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).DataLogCfg != null)
            {
                logger.device = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Clone();
                logger.TotalAccess = true;
                if (logger.ShowDialog() == DialogResult.OK)
                {
                    this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex] = logger.device;
                }
            }
            else
            {
                MessageBox.Show("No Commands! Add Read commands first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonMod_Click(object sender, EventArgs e)
        {
            FormDeviceCommands commands = new FormDeviceCommands {
                device = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Clone()
            };
            if (commands.ShowDialog() == DialogResult.OK)
            {
                this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex] = commands.device;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.AermecDatabase.Note = this.richTextBoxNote.Text;
            this.AermecDatabase.SortDevices();
            FilesManage.SaveAermecDatabase(this.AermecDatabase);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormDispositivi_Load(object sender, EventArgs e)
        {
            this.AermecDatabase = FilesManage.LoadAermecDatabaseFromFile();
            this.LoadDeviceDatabase();
        }

        private void InitializeComponent()
        {
            this.listBoxDevices = new ListBox();
            this.buttonAdd = new Button();
            this.buttonCancel = new Button();
            this.buttonOk = new Button();
            this.buttonClone = new Button();
            this.buttonDel = new Button();
            this.label1 = new Label();
            this.numericUpDownModbusId = new NumericUpDown();
            this.label2 = new Label();
            this.numericUpDownScanRate = new NumericUpDown();
            this.numericUpDownTimeout = new NumericUpDown();
            this.buttonMod = new Button();
            this.label3 = new Label();
            this.label4 = new Label();
            this.textBoxName = new TextBox();
            this.label5 = new Label();
            this.groupBox1 = new GroupBox();
            this.buttonLog = new Button();
            this.textBoxDesc = new TextBox();
            this.label6 = new Label();
            this.numericUpDownDBVersion = new NumericUpDown();
            this.groupBox2 = new GroupBox();
            this.label7 = new Label();
            this.richTextBoxNote = new RichTextBox();
            this.buttonImport = new Button();
            this.openFileDialog1 = new OpenFileDialog();
            this.label8 = new Label();
            this.numericUpDownModbusId.BeginInit();
            this.numericUpDownScanRate.BeginInit();
            this.numericUpDownTimeout.BeginInit();
            this.groupBox1.SuspendLayout();
            this.numericUpDownDBVersion.BeginInit();
            this.groupBox2.SuspendLayout();
            base.SuspendLayout();
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.Location = new Point(12, 0x19);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.ScrollAlwaysVisible = true;
            this.listBoxDevices.Size = new Size(0x142, 0x149);
            this.listBoxDevices.TabIndex = 0;
            this.listBoxDevices.SelectedIndexChanged += new EventHandler(this.listBoxDevices_SelectedIndexChanged);
            this.buttonAdd.Location = new Point(11, 0x16b);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new Size(0x39, 0x17);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "Add...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new EventHandler(this.buttonAdd_Click);
            this.buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(0x1f6, 0x16c);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(0x4b, 0x17);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new EventHandler(this.button3_Click);
            this.buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonOk.DialogResult = DialogResult.OK;
            this.buttonOk.Location = new Point(0x1b9, 0x16c);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new Size(0x37, 0x17);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
            this.buttonClone.Enabled = false;
            this.buttonClone.Location = new Point(140, 0x16b);
            this.buttonClone.Name = "buttonClone";
            this.buttonClone.Size = new Size(0x39, 0x17);
            this.buttonClone.TabIndex = 6;
            this.buttonClone.Text = "Clone";
            this.buttonClone.UseVisualStyleBackColor = true;
            this.buttonClone.Click += new EventHandler(this.buttonClone_Click);
            this.buttonDel.Enabled = false;
            this.buttonDel.Location = new Point(0x4a, 0x16b);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new Size(60, 0x17);
            this.buttonDel.TabIndex = 7;
            this.buttonDel.Text = "Delete";
            this.buttonDel.UseVisualStyleBackColor = true;
            this.buttonDel.Click += new EventHandler(this.buttonDel_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(6, 0x59);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Modbus ID :";
            this.numericUpDownModbusId.Location = new Point(0xad, 0x57);
            int[] bits = new int[4];
            bits[0] = 0xff;
            this.numericUpDownModbusId.Maximum = new decimal(bits);
            int[] numArray2 = new int[4];
            numArray2[0] = 1;
            this.numericUpDownModbusId.Minimum = new decimal(numArray2);
            this.numericUpDownModbusId.Name = "numericUpDownModbusId";
            this.numericUpDownModbusId.Size = new Size(0x34, 20);
            this.numericUpDownModbusId.TabIndex = 1;
            this.numericUpDownModbusId.TextAlign = HorizontalAlignment.Right;
            int[] numArray3 = new int[4];
            numArray3[0] = 1;
            this.numericUpDownModbusId.Value = new decimal(numArray3);
            this.numericUpDownModbusId.ValueChanged += new EventHandler(this.numericUpDownModbusId_ValueChanged);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(6, 0x73);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default ScanRate (ms):";
            this.numericUpDownScanRate.Location = new Point(0xad, 0x71);
            int[] numArray4 = new int[4];
            numArray4[0] = 0x2710;
            this.numericUpDownScanRate.Maximum = new decimal(numArray4);
            int[] numArray5 = new int[4];
            numArray5[0] = 50;
            this.numericUpDownScanRate.Minimum = new decimal(numArray5);
            this.numericUpDownScanRate.Name = "numericUpDownScanRate";
            this.numericUpDownScanRate.Size = new Size(0x34, 20);
            this.numericUpDownScanRate.TabIndex = 3;
            this.numericUpDownScanRate.TextAlign = HorizontalAlignment.Right;
            int[] numArray6 = new int[4];
            numArray6[0] = 0x3e8;
            this.numericUpDownScanRate.Value = new decimal(numArray6);
            this.numericUpDownScanRate.ValueChanged += new EventHandler(this.numericUpDownScanRate_ValueChanged);
            this.numericUpDownTimeout.Location = new Point(0xad, 0x8b);
            int[] numArray7 = new int[4];
            numArray7[0] = 0x7d0;
            this.numericUpDownTimeout.Maximum = new decimal(numArray7);
            int[] numArray8 = new int[4];
            numArray8[0] = 10;
            this.numericUpDownTimeout.Minimum = new decimal(numArray8);
            this.numericUpDownTimeout.Name = "numericUpDownTimeout";
            this.numericUpDownTimeout.Size = new Size(0x34, 20);
            this.numericUpDownTimeout.TabIndex = 4;
            this.numericUpDownTimeout.TextAlign = HorizontalAlignment.Right;
            int[] numArray9 = new int[4];
            numArray9[0] = 0x3e8;
            this.numericUpDownTimeout.Value = new decimal(numArray9);
            this.numericUpDownTimeout.ValueChanged += new EventHandler(this.numericUpDownTimeout_ValueChanged);
            this.buttonMod.Location = new Point(0x5f, 0xa7);
            this.buttonMod.Name = "buttonMod";
            this.buttonMod.Size = new Size(80, 0x17);
            this.buttonMod.TabIndex = 3;
            this.buttonMod.Text = "Commands...";
            this.buttonMod.UseVisualStyleBackColor = true;
            this.buttonMod.Click += new EventHandler(this.buttonMod_Click);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(6, 0x8d);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x6b, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Default Timeout (ms):";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(6, 0x16);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x26, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Name:";
            this.textBoxName.Location = new Point(0x48, 0x13);
            this.textBoxName.MaxLength = 20;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new Size(0x99, 20);
            this.textBoxName.TabIndex = 7;
            this.textBoxName.Leave += new EventHandler(this.textBoxName_Leave);
            this.label5.AutoSize = true;
            this.label5.Location = new Point(6, 0x30);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x3f, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Description:";
            this.groupBox1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.groupBox1.Controls.Add(this.buttonLog);
            this.groupBox1.Controls.Add(this.textBoxDesc);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.buttonMod);
            this.groupBox1.Controls.Add(this.numericUpDownTimeout);
            this.groupBox1.Controls.Add(this.numericUpDownScanRate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownModbusId);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new Point(340, 0x9e);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0xed, 0xc4);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Configuration";
            this.buttonLog.Location = new Point(0xb5, 0xa7);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new Size(50, 0x17);
            this.buttonLog.TabIndex = 9;
            this.buttonLog.Text = "Log...";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new EventHandler(this.buttonLog_Click);
            this.textBoxDesc.Location = new Point(0x48, 0x2d);
            this.textBoxDesc.MaxLength = 30;
            this.textBoxDesc.Name = "textBoxDesc";
            this.textBoxDesc.Size = new Size(0x99, 20);
            this.textBoxDesc.TabIndex = 10;
            this.textBoxDesc.Leave += new EventHandler(this.textBoxDesc_Leave);
            this.label6.AutoSize = true;
            this.label6.Location = new Point(6, 0x12);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x85, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Aermec Database Version:";
            this.numericUpDownDBVersion.Location = new Point(0xab, 0x10);
            this.numericUpDownDBVersion.Name = "numericUpDownDBVersion";
            this.numericUpDownDBVersion.Size = new Size(0x36, 20);
            this.numericUpDownDBVersion.TabIndex = 12;
            this.numericUpDownDBVersion.ValueChanged += new EventHandler(this.numericUpDownDBVersion_ValueChanged);
            this.groupBox2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.richTextBoxNote);
            this.groupBox2.Controls.Add(this.numericUpDownDBVersion);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new Point(340, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0xed, 0x91);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(6, 0x2d);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x21, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Note:";
            this.richTextBoxNote.Location = new Point(6, 0x3d);
            this.richTextBoxNote.Name = "richTextBoxNote";
            this.richTextBoxNote.Size = new Size(0xe1, 0x4e);
            this.richTextBoxNote.TabIndex = 13;
            this.richTextBoxNote.Text = "";
            this.buttonImport.Location = new Point(0xdf, 0x16b);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new Size(0x70, 0x17);
            this.buttonImport.TabIndex = 0x11;
            this.buttonImport.Text = "Import Devices...";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new EventHandler(this.buttonImport_Click);
            this.openFileDialog1.FileName = "AermecDevicesDatabase";
            this.openFileDialog1.Filter = "Aermec DB(*.Adb)|*.Adb";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(12, 9);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0x6d, 13);
            this.label8.TabIndex = 0x12;
            this.label8.Text = "Devices in Database:";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.buttonCancel;
            base.ClientSize = new Size(0x24d, 0x18e);
            base.Controls.Add(this.label8);
            base.Controls.Add(this.buttonImport);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.buttonDel);
            base.Controls.Add(this.buttonClone);
            base.Controls.Add(this.buttonOk);
            base.Controls.Add(this.buttonCancel);
            base.Controls.Add(this.buttonAdd);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.listBoxDevices);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FormDevicesDataB";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Aermec Units Database ";
            base.Load += new EventHandler(this.FormDispositivi_Load);
            this.numericUpDownModbusId.EndInit();
            this.numericUpDownScanRate.EndInit();
            this.numericUpDownTimeout.EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.numericUpDownDBVersion.EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                if (this.listBoxDevices.SelectedIndex >= 0)
                {
                    this.loading = true;
                    this.groupBox1.Enabled = true;
                    this.buttonClone.Enabled = true;
                    this.buttonDel.Enabled = true;
                    this.textBoxName.Text = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).DeviceName;
                    this.textBoxDesc.Text = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Description;
                    this.numericUpDownModbusId.Value = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).ModBusID;
                    this.numericUpDownScanRate.Value = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).ScanRate;
                    this.numericUpDownTimeout.Value = ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Timeout;
                    this.loading = false;
                }
                else
                {
                    this.groupBox1.Enabled = false;
                    this.buttonClone.Enabled = false;
                    this.buttonDel.Enabled = false;
                }
            }
        }

        private void LoadDeviceDatabase()
        {
            this.listBoxDevices.Items.Clear();
            for (int i = 0; i < this.AermecDatabase.DeviceCount(); i++)
            {
                Device device = this.AermecDatabase.GetDevice(i);
                this.listBoxDevices.Items.Add(device.DeviceName);
            }
            this.numericUpDownDBVersion.Value = this.AermecDatabase.DBVersion;
            this.richTextBoxNote.Text = this.AermecDatabase.Note;
        }

        private void numericUpDownDBVersion_ValueChanged(object sender, EventArgs e)
        {
            this.AermecDatabase.DBVersion = (int) this.numericUpDownDBVersion.Value;
        }

        private void numericUpDownModbusId_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading && (this.listBoxDevices.SelectedIndex >= 0))
            {
                ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).ModBusID = (byte) this.numericUpDownModbusId.Value;
            }
        }

        private void numericUpDownScanRate_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).ScanRate = (int) this.numericUpDownScanRate.Value;
            }
        }

        private void numericUpDownTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Timeout = (int) this.numericUpDownTimeout.Value;
            }
        }

        private void textBoxDesc_Leave(object sender, EventArgs e)
        {
            if (this.listBoxDevices.SelectedIndex >= 0)
            {
                ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).Description = this.textBoxDesc.Text;
            }
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            string text = this.textBoxName.Text;
            if (this.listBoxDevices.SelectedIndex >= 0)
            {
                ((Device) this.AermecDatabase.DeviceArrayList[this.listBoxDevices.SelectedIndex]).DeviceName = text;
                this.listBoxDevices.Items[this.listBoxDevices.SelectedIndex] = text;
            }
        }
    }
}

