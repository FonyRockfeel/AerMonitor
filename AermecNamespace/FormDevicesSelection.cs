namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormDevicesSelection : Form
    {
        public BmsProject AermecDatabase;
        private Button buttonCancel;
        private Button buttonExport;
        private Button buttonImport;
        private Button buttonOk;
        private IContainer components;
        public int deviceSelected;
        private GroupBox groupBox1;
        public bool ImportAermecDatabase;
        private Label label1;
        private Label label6;
        private Label labelDBVersion;
        private ListBox listBoxDevices;
        private OpenFileDialog openFileDialog1;
        private RichTextBox richTextBoxNote;
        private SaveFileDialog saveFileDialog1;

        public FormDevicesSelection()
        {
            this.InitializeComponent();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FilesManage.SaveAermecDatabase(this.AermecDatabase, this.saveFileDialog1.FileName);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BmsProject project = FilesManage.LoadAermecDatabaseFromFile(this.openFileDialog1.FileName);
                if (this.AermecDatabase.DBVersion > project.DBVersion)
                {
                    if (MessageBox.Show("Your version of Aermec Database is newer than the file Database. Continue ? ", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        this.AermecDatabase = project;
                    }
                }
                else
                {
                    this.AermecDatabase = project;
                }
                this.LoadList();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.deviceSelected = this.listBoxDevices.SelectedIndex;
            if (this.ImportAermecDatabase)
            {
                FilesManage.SaveAermecDatabase(this.AermecDatabase);
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

        private void FormDispositivi_Load(object sender, EventArgs e)
        {
            this.LoadList();
        }

        private void InitializeComponent()
        {
            this.listBoxDevices = new System.Windows.Forms.ListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.labelDBVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBoxNote = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.Location = new System.Drawing.Point(11, 30);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.ScrollAlwaysVisible = true;
            this.listBoxDevices.Size = new System.Drawing.Size(611, 225);
            this.listBoxDevices.TabIndex = 0;
            this.listBoxDevices.SelectedIndexChanged += new System.EventHandler(this.listBoxDevices_SelectedIndexChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(548, 343);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(486, 343);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(55, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ок";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(162, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Версия базы данных:";
            // 
            // labelDBVersion
            // 
            this.labelDBVersion.AutoSize = true;
            this.labelDBVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDBVersion.Location = new System.Drawing.Point(313, 14);
            this.labelDBVersion.Name = "labelDBVersion";
            this.labelDBVersion.Size = new System.Drawing.Size(50, 13);
            this.labelDBVersion.TabIndex = 14;
            this.labelDBVersion.Text = "Версия";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Оборудование:";
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExport.Location = new System.Drawing.Point(11, 343);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(174, 23);
            this.buttonExport.TabIndex = 15;
            this.buttonExport.Text = "Экспортировать базу данных...";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonImport.Location = new System.Drawing.Point(191, 343);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(183, 23);
            this.buttonImport.TabIndex = 16;
            this.buttonImport.Text = "Импортировать базу данных...";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "Adb";
            this.saveFileDialog1.FileName = "AermecDevicesDatabase";
            this.saveFileDialog1.Filter = "Aermec DB(*.Adb)|*.Adb";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "AermecDevicesDatabase";
            this.openFileDialog1.Filter = "Aermec DB(*.Adb)|*.Adb";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBoxNote);
            this.groupBox1.Location = new System.Drawing.Point(11, 261);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(607, 76);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Примечание";
            // 
            // richTextBoxNote
            // 
            this.richTextBoxNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxNote.Location = new System.Drawing.Point(6, 19);
            this.richTextBoxNote.Name = "richTextBoxNote";
            this.richTextBoxNote.ReadOnly = true;
            this.richTextBoxNote.Size = new System.Drawing.Size(595, 51);
            this.richTextBoxNote.TabIndex = 0;
            this.richTextBoxNote.Text = "";
            // 
            // FormDevicesSelection
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(634, 378);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.labelDBVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listBoxDevices);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDevicesSelection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "База данных оборудования";
            this.Load += new System.EventHandler(this.FormDispositivi_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxDevices.SelectedIndex > -1)
            {
                this.buttonOk.Enabled = true;
            }
        }

        private void LoadList()
        {
            this.listBoxDevices.Items.Clear();
            for (int i = 0; i < this.AermecDatabase.DeviceCount(); i++)
            {
                Device device = this.AermecDatabase.GetDevice(i);
                this.listBoxDevices.Items.Add(device.DeviceName + " - " + device.Description);
            }
            this.labelDBVersion.Text = this.AermecDatabase.DBVersion.ToString();
            this.richTextBoxNote.Text = this.AermecDatabase.Note;
        }
    }
}

