namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    public class FormDataLogMain : Form
    {
        private Button buttonApply;
        private Button buttonClose;
        private Button buttonDelete;
        private Button buttonExportReg;
        private Button buttonViewCharts;
        private string colNameDescription = "Description";
        private string colNameEnable = "Enable";
        private string colNameShortName = "Short Name";
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private ContextMenuStrip contextMenuStripNodes;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private DataStorage.DataStorageIndex dataIndex;
        private DataStorage.DataStorageIndex[] dataIndexes;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private DateTimePicker dateTimePicker3;
        private DateTimePicker dateTimePicker4;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem disableAllToolStripMenuItem;
        private ToolStripMenuItem enableAllToolStripMenuItem;
        private Thread exportThread;
        private ToolStripMenuItem exportToolStripMenuItem;
        private GroupBox groupBox1;
        private GroupBox groupBoxFilterData;
        private DataStorage.DataStorageIndex[] index;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label labelDuration;
        private bool modifica;
        private ProgressBar progressBar1;
        private RadioButton radioButtonAll;
        private RadioButton radioButtonCustom;
        private RadioButton radioButtonLastMonth;
        private RadioButton radioButtonLastWeek;
        private RadioButton radioButtonToday;
        private SaveFileDialog saveFileDialog1;
        private int selectedNodeIndex = -1;
        private TabControl tabControl1;
        private DataTable tableCoils;
        private DataTable tableRegisters;
        private TabPage tabPageCoils;
        private TabPage tabPageRegisters;
        private System.Windows.Forms.Timer timer1;
        private TreeView treeView1;

        public FormDataLogMain()
        {
            this.InitializeComponent();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (this.modifica)
            {
                this.SaveDataSelection();
            }
            this.buttonApply.Enabled = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            this.DeleteLog();
        }

        private void buttonExportReg_Click(object sender, EventArgs e)
        {
            if (this.DateTimeOk())
            {
                if (this.modifica)
                {
                    this.SaveDataSelection();
                }
                this.progressBar1.Value = 0;
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.timer1.Start();
                    this.Cursor = Cursors.WaitCursor;
                    base.Enabled = false;
                    this.progressBar1.Visible = true;
                    this.exportThread = new Thread(new ThreadStart(this.ExportCsv));
                    this.exportThread.Start();
                }
            }
        }

        private void buttonViewCharts_Click(object sender, EventArgs e)
        {
            FormChart chart = new FormChart();
            if ((this.dataIndex != null) && this.DateTimeOk())
            {
                this.Cursor = Cursors.WaitCursor;
                DataStorage data = FilesManage.LoadUserDataStorage(this.dataIndex, this.dateTimePicker3.Value, this.dateTimePicker4.Value);
                if (data == null)
                {
                    MessageBox.Show("Data invalid", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    this.Cursor = Cursors.Default;
                }
                else
                {
                    data = data.GetRange(this.dateTimePicker3.Value, this.dateTimePicker4.Value);
                    if (data.DataSnapShots.Count > 200)
                    {
                        int num1 = data.DataSnapShots.Count / 200;
                    }
                    chart.LoadChart(data, this.dataIndex.bmsProject.GetDevice(0).DataLogCfg);
                    chart.Show();
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void CreateDataGrid(DataTable table, DataGridView dataGridView)
        {
            dataGridView.DataSource = table;
            ((DataGridViewTextBoxColumn) dataGridView.Columns[this.colNameShortName]).MaxInputLength = 10;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(this.colNameEnable, typeof(bool));
            table.Columns.Add(this.colNameShortName, typeof(string));
            table.Columns.Add(this.colNameDescription, typeof(string));
            return table;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.index != null)
            {
                DataLogConfig dataLogCfg = this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg;
                if (e.ColumnIndex == 0)
                {
                    dataLogCfg.Registers[e.RowIndex].Enable = (bool) ((DataTable) this.dataGridView1.DataSource).Rows[e.RowIndex][0];
                }
                if (e.ColumnIndex == 1)
                {
                    dataLogCfg.Registers[e.RowIndex].Name = (string) ((DataTable) this.dataGridView1.DataSource).Rows[e.RowIndex][1];
                }
                if (e.ColumnIndex == 2)
                {
                    dataLogCfg.Registers[e.RowIndex].Description = (string) ((DataTable) this.dataGridView1.DataSource).Rows[e.RowIndex][2];
                }
                this.modifica = true;
                this.buttonApply.Enabled = true;
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.IsCurrentCellDirty)
            {
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.index != null)
            {
                DataLogConfig dataLogCfg = this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg;
                if (e.ColumnIndex == 0)
                {
                    dataLogCfg.Coils[e.RowIndex].Enable = (bool) ((DataTable) this.dataGridView2.DataSource).Rows[e.RowIndex][0];
                }
                if (e.ColumnIndex == 1)
                {
                    dataLogCfg.Coils[e.RowIndex].Name = (string) ((DataTable) this.dataGridView2.DataSource).Rows[e.RowIndex][1];
                }
                if (e.ColumnIndex == 2)
                {
                    dataLogCfg.Coils[e.RowIndex].Description = (string) ((DataTable) this.dataGridView2.DataSource).Rows[e.RowIndex][2];
                }
                this.modifica = true;
                this.buttonApply.Enabled = true;
            }
        }

        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGridView2.IsCurrentCellDirty)
            {
                this.dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private bool DateTimeOk()
        {
            if (this.dateTimePicker3.Value >= this.dateTimePicker4.Value)
            {
                MessageBox.Show("Time \"From\" must be major of Time \"To\"", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.LoadTree();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            this.LoadTree();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            this.DurationSelected();
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            if (this.dateTimePicker3.Value >= this.dateTimePicker4.Value)
            {
                MessageBox.Show("Date \"From\" must be major of Date \"To\"", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                this.DurationSelected();
            }
        }

        private void DeleteLog()
        {
            if (MessageBox.Show("Are you sure ?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
            {
                if (this.treeView1.SelectedNode.Level == 0)
                {
                    FilesManage.DeleteAllBmsLogs(this.treeView1.SelectedNode.Text);
                    this.LoadTree();
                }
                else if (this.treeView1.SelectedNode.Level == 1)
                {
                    FilesManage.DeleteUserDataStorage(this.dataIndexes[(int) this.treeView1.SelectedNode.Nodes[0].Tag]);
                    this.LoadTree();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteLog();
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.index != null)
            {
                int num;
                DataLogConfig dataLogCfg = this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg;
                if (this.tabControl1.SelectedIndex == 0)
                {
                    for (num = 0; num < dataLogCfg.Registers.Length; num++)
                    {
                        dataLogCfg.Registers[num].Enable = false;
                    }
                }
                if (this.tabControl1.SelectedIndex == 1)
                {
                    for (num = 0; num < dataLogCfg.Coils.Length; num++)
                    {
                        dataLogCfg.Coils[num].Enable = false;
                    }
                }
                this.LoadAddress(this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg);
                this.buttonApply.Enabled = true;
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

        private void DurationSelected()
        {
            this.labelDuration.Text = "";
            if (this.dateTimePicker3.Value < this.dateTimePicker4.Value)
            {
                TimeSpan span = this.dateTimePicker4.Value.Subtract(this.dateTimePicker3.Value);
                if (span.Days > 0)
                {
                    this.labelDuration.Text = span.Days.ToString() + " Days, ";
                }
                if (span.Hours > 0)
                {
                    this.labelDuration.Text = this.labelDuration.Text + span.Hours.ToString() + " Hours, ";
                }
                if (span.Minutes > 0)
                {
                    this.labelDuration.Text = this.labelDuration.Text + span.Minutes.ToString() + " Minutes ";
                }
                if (span.Seconds > 0)
                {
                    this.labelDuration.Text = this.labelDuration.Text + span.Seconds.ToString() + " Seconds ";
                }
            }
        }

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.index != null)
            {
                int num;
                DataLogConfig dataLogCfg = this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg;
                if (this.tabControl1.SelectedIndex == 0)
                {
                    for (num = 0; num < dataLogCfg.Registers.Length; num++)
                    {
                        dataLogCfg.Registers[num].Enable = true;
                    }
                }
                if (this.tabControl1.SelectedIndex == 1)
                {
                    for (num = 0; num < dataLogCfg.Coils.Length; num++)
                    {
                        dataLogCfg.Coils[num].Enable = true;
                    }
                }
                this.LoadAddress(this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg);
                this.buttonApply.Enabled = true;
            }
        }

        private void ExportComplete()
        {
            this.timer1.Stop();
            this.progressBar1.Value = 100;
            this.Cursor = Cursors.Default;
            base.Enabled = true;
            this.progressBar1.Visible = false;
        }

        private void ExportCsv()
        {
            if (this.dataIndex != null)
            {
                try
                {
                    DataStorage range = FilesManage.LoadUserDataStorage(this.dataIndex).GetRange(this.dateTimePicker3.Value, this.dateTimePicker4.Value);
                    BmsProject nexConfigProject = this.dataIndex.bmsProject.Clone();
                    range.ExportDataToCsv(nexConfigProject, this.saveFileDialog1.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                base.Invoke(new DelegateExportCSV(this.ExportComplete));
            }
        }

        private void ExportCsv(string filename)
        {
            if (this.dataIndex != null)
            {
                try
                {
                    DataStorage range = FilesManage.LoadUserDataStorage(this.dataIndex).GetRange(this.dateTimePicker3.Value, this.dateTimePicker4.Value);
                    this.dataIndex.bmsProject.CleanDataLogConfig();
                    range.ExportDataToCsv(this.dataIndex.bmsProject, this.saveFileDialog1.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void FormDataLogMain_Load(object sender, EventArgs e)
        {
            this.radioButtonCustom.Checked = true;
            this.radioButtonAll.Checked = true;
            this.LoadTree();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            TreeNode node = new TreeNode("Node1");
            TreeNode node2 = new TreeNode("Node0", new TreeNode[] { node });
            this.treeView1 = new TreeView();
            this.contextMenuStripNodes = new ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new ToolStripMenuItem();
            this.exportToolStripMenuItem = new ToolStripMenuItem();
            this.groupBox1 = new GroupBox();
            this.radioButtonCustom = new RadioButton();
            this.radioButtonLastMonth = new RadioButton();
            this.dateTimePicker2 = new DateTimePicker();
            this.radioButtonLastWeek = new RadioButton();
            this.radioButtonAll = new RadioButton();
            this.label2 = new Label();
            this.radioButtonToday = new RadioButton();
            this.label1 = new Label();
            this.dateTimePicker1 = new DateTimePicker();
            this.label3 = new Label();
            this.groupBoxFilterData = new GroupBox();
            this.labelDuration = new Label();
            this.label7 = new Label();
            this.dateTimePicker4 = new DateTimePicker();
            this.label4 = new Label();
            this.label5 = new Label();
            this.dateTimePicker3 = new DateTimePicker();
            this.tabControl1 = new TabControl();
            this.tabPageRegisters = new TabPage();
            this.dataGridView1 = new DataGridView();
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.enableAllToolStripMenuItem = new ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new ToolStripMenuItem();
            this.tabPageCoils = new TabPage();
            this.dataGridView2 = new DataGridView();
            this.buttonExportReg = new Button();
            this.buttonViewCharts = new Button();
            this.buttonClose = new Button();
            this.buttonDelete = new Button();
            this.saveFileDialog1 = new SaveFileDialog();
            this.buttonApply = new Button();
            this.label6 = new Label();
            this.progressBar1 = new ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripNodes.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxFilterData.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageRegisters.SuspendLayout();
            ((ISupportInitialize) this.dataGridView1).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPageCoils.SuspendLayout();
            ((ISupportInitialize) this.dataGridView2).BeginInit();
            base.SuspendLayout();
            this.treeView1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.treeView1.Location = new Point(15, 0x7c);
            this.treeView1.Name = "treeView1";
            node.Name = "Node1";
            node.Text = "Node1";
            node2.BackColor = Color.Gainsboro;
            node2.ForeColor = Color.Black;
            node2.Name = "Node0";
            node2.Text = "Node0";
            this.treeView1.Nodes.AddRange(new TreeNode[] { node2 });
            this.treeView1.Size = new Size(0x19f, 0x18f);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new MouseEventHandler(this.treeView1_MouseDown);
            this.contextMenuStripNodes.Items.AddRange(new ToolStripItem[] { this.deleteToolStripMenuItem, this.exportToolStripMenuItem });
            this.contextMenuStripNodes.Name = "contextMenuStrip2";
            this.contextMenuStripNodes.Size = new Size(120, 0x30);
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new Size(0x77, 0x16);
            this.deleteToolStripMenuItem.Text = "Delete...";
            this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new Size(0x77, 0x16);
            this.exportToolStripMenuItem.Text = "Export ...";
            this.groupBox1.Controls.Add(this.radioButtonCustom);
            this.groupBox1.Controls.Add(this.radioButtonLastMonth);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.radioButtonLastWeek);
            this.groupBox1.Controls.Add(this.radioButtonAll);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.radioButtonToday);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Location = new Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x1a2, 0x4a);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Filter...";
            this.radioButtonCustom.AutoSize = true;
            this.radioButtonCustom.Location = new Point(0x11, 0x30);
            this.radioButtonCustom.Name = "radioButtonCustom";
            this.radioButtonCustom.Size = new Size(0x45, 0x11);
            this.radioButtonCustom.TabIndex = 9;
            this.radioButtonCustom.TabStop = true;
            this.radioButtonCustom.Text = "Custom...";
            this.radioButtonCustom.UseVisualStyleBackColor = true;
            this.radioButtonCustom.CheckedChanged += new EventHandler(this.radioButtonCustom_CheckedChanged);
            this.radioButtonLastMonth.AutoSize = true;
            this.radioButtonLastMonth.Location = new Point(0x14e, 0x13);
            this.radioButtonLastMonth.Name = "radioButtonLastMonth";
            this.radioButtonLastMonth.Size = new Size(0x4e, 0x11);
            this.radioButtonLastMonth.TabIndex = 6;
            this.radioButtonLastMonth.TabStop = true;
            this.radioButtonLastMonth.Text = "Last Month";
            this.radioButtonLastMonth.UseVisualStyleBackColor = true;
            this.radioButtonLastMonth.CheckedChanged += new EventHandler(this.radioButtonLastMonth_CheckedChanged);
            this.dateTimePicker2.Format = DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new Point(0x103, 0x2e);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new Size(0x54, 20);
            this.dateTimePicker2.TabIndex = 4;
            this.dateTimePicker2.ValueChanged += new EventHandler(this.dateTimePicker2_ValueChanged);
            this.radioButtonLastWeek.AutoSize = true;
            this.radioButtonLastWeek.Location = new Point(0xe9, 0x13);
            this.radioButtonLastWeek.Name = "radioButtonLastWeek";
            this.radioButtonLastWeek.Size = new Size(0x4d, 0x11);
            this.radioButtonLastWeek.TabIndex = 5;
            this.radioButtonLastWeek.TabStop = true;
            this.radioButtonLastWeek.Text = "Last Week";
            this.radioButtonLastWeek.UseVisualStyleBackColor = true;
            this.radioButtonLastWeek.CheckedChanged += new EventHandler(this.radioButtonLastWeek_CheckedChanged);
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Location = new Point(0x7d, 0x13);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new Size(0x3e, 0x11);
            this.radioButtonAll.TabIndex = 8;
            this.radioButtonAll.TabStop = true;
            this.radioButtonAll.Text = "All Logs";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new EventHandler(this.radioButton1_CheckedChanged);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(230, 50);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x17, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To:";
            this.radioButtonToday.AutoSize = true;
            this.radioButtonToday.Location = new Point(0x12, 0x13);
            this.radioButtonToday.Name = "radioButtonToday";
            this.radioButtonToday.Size = new Size(0x37, 0x11);
            this.radioButtonToday.TabIndex = 7;
            this.radioButtonToday.TabStop = true;
            this.radioButtonToday.Text = "Today";
            this.radioButtonToday.UseVisualStyleBackColor = true;
            this.radioButtonToday.CheckedChanged += new EventHandler(this.radioButtonToday_CheckedChanged);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x5c, 50);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x21, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "From:";
            this.dateTimePicker1.Format = DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new Point(0x83, 0x2e);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new Size(0x54, 20);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.ValueChanged += new EventHandler(this.dateTimePicker1_ValueChanged);
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label3.Location = new Point(12, 0x5d);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x6b, 0x10);
            this.label3.TabIndex = 3;
            this.label3.Text = "Data Log saved:";
            this.groupBoxFilterData.Controls.Add(this.labelDuration);
            this.groupBoxFilterData.Controls.Add(this.label7);
            this.groupBoxFilterData.Controls.Add(this.dateTimePicker4);
            this.groupBoxFilterData.Controls.Add(this.label4);
            this.groupBoxFilterData.Controls.Add(this.label5);
            this.groupBoxFilterData.Controls.Add(this.dateTimePicker3);
            this.groupBoxFilterData.Enabled = false;
            this.groupBoxFilterData.Location = new Point(0x1bf, 5);
            this.groupBoxFilterData.Name = "groupBoxFilterData";
            this.groupBoxFilterData.Size = new Size(0x191, 0x4a);
            this.groupBoxFilterData.TabIndex = 4;
            this.groupBoxFilterData.TabStop = false;
            this.groupBoxFilterData.Text = "Select Data to Export or view in chart...";
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new Point(80, 0x30);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new Size(0x2f, 13);
            this.labelDuration.TabIndex = 6;
            this.labelDuration.Text = "selected";
            this.label7.AutoSize = true;
            this.label7.Location = new Point(0x18, 0x30);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x37, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Selected: ";
            this.dateTimePicker4.CustomFormat = "dd/MM/yyyy HH.mm.ss";
            this.dateTimePicker4.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker4.Location = new Point(0xf7, 0x13);
            this.dateTimePicker4.Name = "dateTimePicker4";
            this.dateTimePicker4.Size = new Size(0x94, 20);
            this.dateTimePicker4.TabIndex = 4;
            this.dateTimePicker4.ValueChanged += new EventHandler(this.dateTimePicker4_ValueChanged);
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0xda, 0x17);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x17, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "To:";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0x18, 0x17);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x21, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "From:";
            this.dateTimePicker3.CustomFormat = "dd/MM/yyyy HH.mm.ss";
            this.dateTimePicker3.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker3.Location = new Point(0x3f, 0x13);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new Size(140, 20);
            this.dateTimePicker3.TabIndex = 1;
            this.dateTimePicker3.ValueChanged += new EventHandler(this.dateTimePicker3_ValueChanged);
            this.tabControl1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControl1.Controls.Add(this.tabPageRegisters);
            this.tabControl1.Controls.Add(this.tabPageCoils);
            this.tabControl1.Location = new Point(0x1bf, 0x7c);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(0x197, 0x18f);
            this.tabControl1.TabIndex = 9;
            this.tabPageRegisters.Controls.Add(this.dataGridView1);
            this.tabPageRegisters.Location = new Point(4, 0x16);
            this.tabPageRegisters.Name = "tabPageRegisters";
            this.tabPageRegisters.Padding = new Padding(3);
            this.tabPageRegisters.Size = new Size(0x18f, 0x175);
            this.tabPageRegisters.TabIndex = 0;
            this.tabPageRegisters.Text = "Registers";
            this.tabPageRegisters.UseVisualStyleBackColor = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new Point(6, 6);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new Size(0x181, 0x169);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
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
            this.tabPageCoils.Controls.Add(this.dataGridView2);
            this.tabPageCoils.Location = new Point(4, 0x16);
            this.tabPageCoils.Name = "tabPageCoils";
            this.tabPageCoils.Padding = new Padding(3);
            this.tabPageCoils.Size = new Size(0x18f, 0x175);
            this.tabPageCoils.TabIndex = 1;
            this.tabPageCoils.Text = "Coils";
            this.tabPageCoils.UseVisualStyleBackColor = true;
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;
            this.dataGridView2.Location = new Point(6, 6);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new Size(0x181, 0x185);
            this.dataGridView2.TabIndex = 3;
            this.dataGridView2.CellValueChanged += new DataGridViewCellEventHandler(this.dataGridView2_CellValueChanged);
            this.dataGridView2.CurrentCellDirtyStateChanged += new EventHandler(this.dataGridView2_CurrentCellDirtyStateChanged);
            this.buttonExportReg.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonExportReg.Location = new Point(0x1c1, 0x211);
            this.buttonExportReg.Name = "buttonExportReg";
            this.buttonExportReg.Size = new Size(0x63, 0x17);
            this.buttonExportReg.TabIndex = 15;
            this.buttonExportReg.Text = "Export (.csv)...";
            this.buttonExportReg.UseVisualStyleBackColor = true;
            this.buttonExportReg.Click += new EventHandler(this.buttonExportReg_Click);
            this.buttonViewCharts.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonViewCharts.Enabled = false;
            this.buttonViewCharts.Location = new Point(0x22a, 0x211);
            this.buttonViewCharts.MinimumSize = new Size(0x75, 0x17);
            this.buttonViewCharts.Name = "buttonViewCharts";
            this.buttonViewCharts.Size = new Size(0x75, 0x17);
            this.buttonViewCharts.TabIndex = 0x10;
            this.buttonViewCharts.Text = "View Charts...";
            this.buttonViewCharts.UseVisualStyleBackColor = true;
            this.buttonViewCharts.Click += new EventHandler(this.buttonViewCharts_Click);
            this.buttonClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonClose.Location = new Point(0x307, 0x211);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(0x4b, 0x17);
            this.buttonClose.TabIndex = 0x11;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
            this.buttonDelete.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new Point(12, 0x211);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x56, 0x17);
            this.buttonDelete.TabIndex = 0x12;
            this.buttonDelete.Text = "Delete Log";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.saveFileDialog1.DefaultExt = "Csv";
            this.saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            this.buttonApply.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new Point(0x2b6, 0x211);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new Size(0x4b, 0x17);
            this.buttonApply.TabIndex = 0x13;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
            this.label6.AutoSize = true;
            this.label6.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label6.Location = new Point(0x1c0, 0x5d);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x116, 0x10);
            this.label6.TabIndex = 20;
            this.label6.Text = "Select what you want to export or view in chart:";
            this.progressBar1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.progressBar1.Location = new Point(0x68, 530);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(0x145, 0x17);
            this.progressBar1.TabIndex = 0x15;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x35e, 0x234);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.buttonApply);
            base.Controls.Add(this.buttonDelete);
            base.Controls.Add(this.buttonClose);
            base.Controls.Add(this.buttonViewCharts);
            base.Controls.Add(this.buttonExportReg);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.groupBoxFilterData);
            base.Controls.Add(this.tabControl1);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.treeView1);
            base.Controls.Add(this.label3);
            this.MinimumSize = new Size(870, 600);
            base.Name = "FormDataLogMain";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Data Storage";
            base.Load += new EventHandler(this.FormDataLogMain_Load);
            this.contextMenuStripNodes.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxFilterData.ResumeLayout(false);
            this.groupBoxFilterData.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageRegisters.ResumeLayout(false);
            ((ISupportInitialize) this.dataGridView1).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPageCoils.ResumeLayout(false);
            ((ISupportInitialize) this.dataGridView2).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadAddress(DataLogConfig dataLogCfg)
        {
            int num;
            DataLogConfig.DataLog log;
            this.tableRegisters = this.CreateDataTable();
            this.CreateDataGrid(this.tableRegisters, this.dataGridView1);
            for (num = 0; num < dataLogCfg.Registers.Length; num++)
            {
                log = dataLogCfg.Registers[num];
                this.tableRegisters.Rows.Add(new object[] { log.Enable, log.Name, log.Description });
            }
            this.dataGridView1.DataSource = this.tableRegisters;
            this.tableCoils = this.CreateDataTable();
            this.CreateDataGrid(this.tableCoils, this.dataGridView2);
            for (num = 0; num < dataLogCfg.Coils.Length; num++)
            {
                log = dataLogCfg.Coils[num];
                this.tableCoils.Rows.Add(new object[] { log.Enable, log.Name, log.Description });
            }
            this.dataGridView2.DataSource = this.tableCoils;
        }

        private void LoadTree()
        {
            int num;
            int num4 = 0;
            DateTime time = new DateTime(this.dateTimePicker1.Value.Year, this.dateTimePicker1.Value.Month, this.dateTimePicker1.Value.Day, 0, 0, 0, 0);
            DateTime time2 = new DateTime(this.dateTimePicker2.Value.Year, this.dateTimePicker2.Value.Month, this.dateTimePicker2.Value.Day, 0x17, 0x3b, 0x3b, 0x3e7);
            string[] dataLogProjectNames = FilesManage.GetDataLogProjectNames();
            this.treeView1.Nodes.Clear();
            if (dataLogProjectNames.Length > 0)
            {
                for (num = 0; num < dataLogProjectNames.Length; num++)
                {
                    num4 = 0;
                    TreeNode node = new TreeNode(dataLogProjectNames[num]) {
                        ContextMenuStrip = this.contextMenuStripNodes
                    };
                    this.treeView1.Nodes.Add(node);
                    this.treeView1.Nodes[num].BackColor = Color.Gainsboro;
                    this.index = FilesManage.GetDataLogIndex(dataLogProjectNames[num]);
                    for (int i = 0; i < this.index.Length; i++)
                    {
                        int num3;
                        node = new TreeNode(this.index[i].GetStartRecordingToString() + "     Duration: " + this.index[i].DurationTimeToString()) {
                            ContextMenuStrip = this.contextMenuStripNodes
                        };
                        if (this.radioButtonAll.Checked)
                        {
                            this.treeView1.Nodes[num].Nodes.Add(node);
                            num3 = 0;
                            while (num3 < this.index[i].bmsProject.DeviceArrayList.Count)
                            {
                                node = new TreeNode(this.index[i].bmsProject.GetDevice(num3).DeviceName) {
                                    Tag = i
                                };
                                this.treeView1.Nodes[num].Nodes[num4].Nodes.Add(node);
                                num3++;
                            }
                            num4++;
                        }
                        else if ((this.index[i].fileTimeSnaphot[0] >= time) && (this.index[i].fileTimeSnaphot[0] <= time2))
                        {
                            this.treeView1.Nodes[num].Nodes.Add(node);
                            for (num3 = 0; num3 < this.index[i].bmsProject.DeviceArrayList.Count; num3++)
                            {
                                node = new TreeNode(this.index[i].bmsProject.GetDevice(num3).DeviceName) {
                                    Tag = i
                                };
                                this.treeView1.Nodes[num].Nodes[num4].Nodes.Add(node);
                            }
                            num4++;
                        }
                    }
                }
            }
            for (num = 0; num < this.treeView1.Nodes.Count; num++)
            {
                this.treeView1.Nodes[num].Expand();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonAll.Checked)
            {
                this.LoadTree();
            }
        }

        private void radioButtonCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonCustom.Checked)
            {
                this.dateTimePicker1.Enabled = true;
                this.dateTimePicker2.Enabled = true;
            }
            else
            {
                this.dateTimePicker1.Enabled = false;
                this.dateTimePicker2.Enabled = false;
            }
        }

        private void radioButtonLastMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonLastMonth.Checked)
            {
                this.dateTimePicker2.Value = DateTime.Now;
                this.dateTimePicker1.Value = this.dateTimePicker2.Value.Subtract(new TimeSpan(30, 0, 0, 0));
            }
        }

        private void radioButtonLastWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonLastWeek.Checked)
            {
                this.dateTimePicker2.Value = DateTime.Now;
                this.dateTimePicker1.Value = this.dateTimePicker2.Value.Subtract(new TimeSpan(7, 0, 0, 0));
            }
        }

        private void radioButtonToday_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonToday.Checked)
            {
                if (this.dateTimePicker1.Value == DateTime.Today)
                {
                    this.LoadTree();
                }
                this.dateTimePicker1.Value = DateTime.Today;
                this.dateTimePicker2.Value = DateTime.Today.Add(new TimeSpan(0x17, 0x3b, 0x3b));
            }
        }

        private void SaveDataSelection()
        {
            FilesManage.UpdateUserDataIndex(this.dataIndex);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.progressBar1.Value < 100)
            {
                this.progressBar1.Value++;
            }
            else
            {
                this.progressBar1.Value = 0;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode.Level < 2)
            {
                this.buttonDelete.Enabled = true;
            }
            else
            {
                this.buttonDelete.Enabled = false;
            }
            if (this.treeView1.SelectedNode.Level == 1)
            {
                this.dataIndexes = FilesManage.GetDataLogIndex(this.treeView1.SelectedNode.Parent.Text);
            }
            if (this.treeView1.SelectedNode.Level == 2)
            {
                this.groupBoxFilterData.Enabled = true;
                this.dataGridView1.Visible = true;
                this.dataGridView2.Visible = true;
                this.buttonExportReg.Enabled = true;
                this.buttonViewCharts.Enabled = true;
                if (this.selectedNodeIndex != this.treeView1.SelectedNode.Parent.Index)
                {
                    this.selectedNodeIndex = this.treeView1.SelectedNode.Parent.Index;
                    this.dataIndexes = FilesManage.GetDataLogIndex(this.treeView1.SelectedNode.Parent.Parent.Text);
                }
                if (this.dataIndexes.Length > 0)
                {
                    this.dataIndex = this.dataIndexes[(int) this.treeView1.SelectedNode.Tag];
                    this.dateTimePicker3.Value = this.dataIndex.fileTimeSnaphot[0];
                    this.dateTimePicker4.Value = this.dataIndex.lastSave;
                    this.LoadAddress(this.dataIndex.bmsProject.GetDevice(this.treeView1.SelectedNode.Index).DataLogCfg);
                }
            }
            else
            {
                this.groupBoxFilterData.Enabled = false;
                this.dataGridView1.Visible = false;
                this.dataGridView2.Visible = false;
                this.dataIndex = null;
                this.buttonExportReg.Enabled = false;
                this.buttonViewCharts.Enabled = false;
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.treeView1.SelectedNode = ((TreeView) sender).GetNodeAt(e.X, e.Y);
            }
        }

        private delegate void DelegateExportCSV();
    }
}

