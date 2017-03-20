namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormLogger : Form
    {
        private Button buttonCancel;
        private Button buttonExportReg;
        private Button buttonImportReg;
        private Button buttonOk;
        private string colNameAddress = "Address";
        private string colNameAscii = "Ascii";
        private string colNameDescription = "Description";
        private string colNameEnable = "Enable";
        private string colNameGain = "Gain";
        private string colNameShortName = "Name";
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        public Device device;
        private ToolStripMenuItem disableAllToolStripMenuItem;
        private ToolStripMenuItem enableAllToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private TabControl tabControl1;
        private DataTable tableCoils;
        private DataTable tableRegisters;
        private TabPage tabPage1;
        private TabPage tabPage2;
        public bool TotalAccess;

        public FormLogger()
        {
            this.InitializeComponent();
        }

        private void buttonExportReg_Click(object sender, EventArgs e)
        {
            this.SaveTableToLogger();
            this.ExportToCsv();
        }

        private void buttonImportReg_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                this.openFileDialog1.Title = "Import Registers Descriptions";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.device.DataLogCfg.ImportRegistersFromCsv(this.openFileDialog1.FileName);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            else
            {
                this.openFileDialog1.Title = "Import Coils Descriptions";
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.device.DataLogCfg.ImportCoilsFromCsv(this.openFileDialog1.FileName);
                    }
                    catch (Exception exception2)
                    {
                        MessageBox.Show(exception2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            this.LoadAddress();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.SaveTableToLogger();
        }

        private void CreateDataGrid(DataTable table, DataGridView dataGridView)
        {
            dataGridView.DataSource = table;
            dataGridView.Columns[this.colNameAddress].ReadOnly = true;
            ((DataGridViewTextBoxColumn) dataGridView.Columns[this.colNameShortName]).MaxInputLength = 10;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(this.colNameAddress, typeof(int));
            table.Columns.Add(this.colNameEnable, typeof(bool));
            table.Columns.Add(this.colNameGain, typeof(decimal));
            table.Columns.Add(this.colNameAscii, typeof(bool));
            table.Columns.Add(this.colNameShortName, typeof(string));
            table.Columns.Add(this.colNameDescription, typeof(string));
            return table;
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num;
            if (this.tabControl1.SelectedIndex == 0)
            {
                for (num = 0; num < this.tableRegisters.Rows.Count; num++)
                {
                    this.tableRegisters.Rows[num][this.colNameEnable] = false;
                }
            }
            if (this.tabControl1.SelectedIndex == 1)
            {
                for (num = 0; num < this.tableRegisters.Rows.Count; num++)
                {
                    this.tableCoils.Rows[num][this.colNameEnable] = false;
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

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num;
            if (this.tabControl1.SelectedIndex == 0)
            {
                for (num = 0; num < this.tableRegisters.Rows.Count; num++)
                {
                    this.tableRegisters.Rows[num][this.colNameEnable] = true;
                }
            }
            if (this.tabControl1.SelectedIndex == 1)
            {
                for (num = 0; num < this.tableRegisters.Rows.Count; num++)
                {
                    this.tableCoils.Rows[num][this.colNameEnable] = true;
                }
            }
        }

        private void ExportToCsv()
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                this.saveFileDialog1.Title = "Export Registers Descriptions";
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (!this.device.DataLogCfg.ExportRegistersToCsv(this.saveFileDialog1.FileName))
                        {
                            MessageBox.Show("File save error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            else
            {
                this.saveFileDialog1.Title = "Export Coils Descriptions";
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (!this.device.DataLogCfg.ExportCoilsToCsv(this.saveFileDialog1.FileName))
                        {
                            MessageBox.Show("File save error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    catch (Exception exception2)
                    {
                        MessageBox.Show(exception2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void FormLogger_Load(object sender, EventArgs e)
        {
            this.LoadAddress();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.tabControl1 = new TabControl();
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.enableAllToolStripMenuItem = new ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new ToolStripMenuItem();
            this.tabPage1 = new TabPage();
            this.tabPage2 = new TabPage();
            this.buttonImportReg = new Button();
            this.buttonExportReg = new Button();
            this.saveFileDialog1 = new SaveFileDialog();
            this.openFileDialog1 = new OpenFileDialog();
            ((ISupportInitialize) this.dataGridView1).BeginInit();
            ((ISupportInitialize) this.dataGridView2).BeginInit();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            base.SuspendLayout();
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new Point(6, 6);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new Size(0x305, 0x159);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new Point(6, 6);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new Size(0x305, 0x159);
            this.dataGridView2.TabIndex = 3;
            this.buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonOk.DialogResult = DialogResult.OK;
            this.buttonOk.Location = new Point(0x29c, 0x191);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new Size(0x37, 0x17);
            this.buttonOk.TabIndex = 7;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
            this.buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(730, 0x191);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(0x4b, 0x17);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.tabControl1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(0x319, 0x17f);
            this.tabControl1.TabIndex = 8;
            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] { this.enableAllToolStripMenuItem, this.disableAllToolStripMenuItem });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new Size(130, 0x30);
            this.enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            this.enableAllToolStripMenuItem.Size = new Size(0x81, 0x16);
            this.enableAllToolStripMenuItem.Text = "Enable All";
            this.enableAllToolStripMenuItem.Click += new EventHandler(this.enableAllToolStripMenuItem_Click);
            this.disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            this.disableAllToolStripMenuItem.Size = new Size(0x81, 0x16);
            this.disableAllToolStripMenuItem.Text = "Disable All";
            this.disableAllToolStripMenuItem.Click += new EventHandler(this.disableAllToolStripMenuItem_Click);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new Point(4, 0x16);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(0x311, 0x165);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Registers";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new Point(4, 0x16);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new Size(0x311, 0x165);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Coils";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.buttonImportReg.Anchor = AnchorStyles.Bottom;
            this.buttonImportReg.Location = new Point(0x75, 0x191);
            this.buttonImportReg.Name = "buttonImportReg";
            this.buttonImportReg.Size = new Size(0x63, 0x17);
            this.buttonImportReg.TabIndex = 15;
            this.buttonImportReg.Text = "Import (.csv)...";
            this.buttonImportReg.UseVisualStyleBackColor = true;
            this.buttonImportReg.Click += new EventHandler(this.buttonImportReg_Click);
            this.buttonExportReg.Anchor = AnchorStyles.Bottom;
            this.buttonExportReg.Location = new Point(12, 0x191);
            this.buttonExportReg.Name = "buttonExportReg";
            this.buttonExportReg.Size = new Size(0x63, 0x17);
            this.buttonExportReg.TabIndex = 14;
            this.buttonExportReg.Text = "Export (.csv)...";
            this.buttonExportReg.UseVisualStyleBackColor = true;
            this.buttonExportReg.Click += new EventHandler(this.buttonExportReg_Click);
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            this.openFileDialog1.DefaultExt = "csv";
            this.openFileDialog1.Filter = "Text files (*.csv)|*.csv";
            base.AcceptButton = this.buttonOk;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.buttonCancel;
            base.ClientSize = new Size(0x331, 0x1b4);
            base.Controls.Add(this.buttonImportReg);
            base.Controls.Add(this.buttonExportReg);
            base.Controls.Add(this.tabControl1);
            base.Controls.Add(this.buttonOk);
            base.Controls.Add(this.buttonCancel);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FormLogger";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Data Logger";
            base.Load += new EventHandler(this.FormLogger_Load);
            ((ISupportInitialize) this.dataGridView1).EndInit();
            ((ISupportInitialize) this.dataGridView2).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void LoadAddress()
        {
            int num;
            DataLogConfig.DataLog log;
            this.tableRegisters = this.CreateDataTable();
            this.CreateDataGrid(this.tableRegisters, this.dataGridView1);
            for (num = 0; num < this.device.DataLogCfg.Registers.Length; num++)
            {
                log = this.device.DataLogCfg.Registers[num];
                if (this.TotalAccess || log.UserAccess)
                {
                    this.tableRegisters.Rows.Add(new object[] { log.Address, log.Enable, log.Gain, log.Ascii, log.Name, log.Description });
                }
            }
            this.dataGridView1.DataSource = this.tableRegisters;
            this.tableCoils = this.CreateDataTable();
            this.CreateDataGrid(this.tableCoils, this.dataGridView2);
            for (num = 0; num < this.device.DataLogCfg.Coils.Length; num++)
            {
                log = this.device.DataLogCfg.Coils[num];
                if (this.TotalAccess || log.UserAccess)
                {
                    this.tableCoils.Rows.Add(new object[] { log.Address, log.Enable, log.Gain, log.Ascii, log.Name, log.Description });
                }
            }
            this.dataGridView2.DataSource = this.tableCoils;
        }

        private void SaveTableToLogger()
        {
            int num;
            DataLogConfig.DataLog registerFromAddress;
            for (num = 0; num < this.tableRegisters.Rows.Count; num++)
            {
                registerFromAddress = this.device.DataLogCfg.GetRegisterFromAddress((int) this.tableRegisters.Rows[num][this.colNameAddress]);
                registerFromAddress.Enable = (bool) this.tableRegisters.Rows[num][this.colNameEnable];
                if (this.TotalAccess)
                {
                    registerFromAddress.UserAccess = registerFromAddress.Enable;
                }
                if (!DBNull.Value.Equals(this.tableRegisters.Rows[num][this.colNameGain]))
                {
                    registerFromAddress.Gain = (decimal) this.tableRegisters.Rows[num][this.colNameGain];
                }
                else
                {
                    registerFromAddress.Gain = 1M;
                }
                registerFromAddress.Ascii = (bool) this.tableRegisters.Rows[num][this.colNameAscii];
                if (!DBNull.Value.Equals(this.tableRegisters.Rows[num][this.colNameShortName]))
                {
                    registerFromAddress.Name = ((string) this.tableRegisters.Rows[num][this.colNameShortName]).Replace("\"", "").Replace("\"", "");
                }
                else
                {
                    registerFromAddress.Name = "";
                }
                if (!DBNull.Value.Equals(this.tableRegisters.Rows[num][this.colNameDescription]))
                {
                    registerFromAddress.Description = ((string) this.tableRegisters.Rows[num][this.colNameDescription]).Replace("\"", "");
                }
                else
                {
                    registerFromAddress.Description = "";
                }
            }
            for (num = 0; num < this.tableCoils.Rows.Count; num++)
            {
                registerFromAddress = this.device.DataLogCfg.GetCoilFromAddress((int) this.tableCoils.Rows[num][this.colNameAddress]);
                registerFromAddress.Enable = (bool) this.tableCoils.Rows[num][this.colNameEnable];
                if (this.TotalAccess)
                {
                    registerFromAddress.UserAccess = registerFromAddress.Enable;
                }
                if (!DBNull.Value.Equals(this.tableCoils.Rows[num][this.colNameGain]))
                {
                    registerFromAddress.Gain = (decimal) this.tableCoils.Rows[num][this.colNameGain];
                }
                else
                {
                    registerFromAddress.Gain = 1M;
                }
                registerFromAddress.Ascii = (bool) this.tableCoils.Rows[num][this.colNameAscii];
                if (!DBNull.Value.Equals(this.tableCoils.Rows[num][this.colNameShortName]))
                {
                    registerFromAddress.Name = ((string) this.tableCoils.Rows[num][this.colNameShortName]).Replace("\"", "");
                }
                else
                {
                    registerFromAddress.Name = "";
                }
                if (!DBNull.Value.Equals(this.tableCoils.Rows[num][this.colNameDescription]))
                {
                    registerFromAddress.Description = ((string) this.tableCoils.Rows[num][this.colNameDescription]).Replace("\"", "");
                }
                else
                {
                    registerFromAddress.Description = "";
                }
            }
        }
    }
}

