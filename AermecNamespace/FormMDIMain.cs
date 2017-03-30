namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO.Ports;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class FormMDIMain : Form
    {
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem cascadeToolStripMenuItem;
        private ToolStripMenuItem charLoadToolStripMenuItem;
        private ToolStripMenuItem chartsToolStripMenuItem;
        private ToolStripMenuItem chartToolStripMenuItem;
        private int childFormNumber;
        private ToolStripMenuItem closeAllToolStripMenuItem;
        private ToolStripMenuItem coilsTableOfToolStripMenuItem;
        private ToolStripComboBox coilsToolStripMenuItem;
        private IContainer components;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private TimeSpan crono = new TimeSpan();
        private BmsProject currentBmsProject;
        private ToolStripMenuItem cutToolStripMenuItem;
        private DataLogManage dataLogManage;
        private ToolStripMenuItem dataLogToolStripMenuItem;
        private bool debug;
        private ToolStripMenuItem demoToolStripMenuItem;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem fileMenu;
        private FormChart formChart = new FormChart();
        private FormChart formChartRealTime = new FormChart();
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem importNewAermecDatabaseDevicesToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ArrayList listGraphs = new ArrayList();
        private MenuStrip menuStrip;
        private ToolStripMenuItem newChartToolStripMenuItem1;
        private bool newDataLogFile = true;
        private ToolStripButton newToolStripButton;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem newWindowToolStripMenuItem;
        private int numberOfCharts;
        private ToolStripButton openToolStripButton;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem printPreviewToolStripMenuItem;
        private ToolStripMenuItem printSetupToolStripMenuItem;
        private ToolStripMenuItem printToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem registersTableOfToolStripMenuItem;
        private ToolStripComboBox registersToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripButton saveToolStripButton;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private SerialPort serialPort1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private StatusStrip statusStrip;
        private Supervisor supervisorControl;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem testToolStripMenuItem1;
        private ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private ToolStripMenuItem tileVerticalToolStripMenuItem;
        private Timer timer1;
        private Timer timer2;
        private Timer timer3;
        private ToolStripMenuItem toolsMenu;
        private ToolStrip toolStrip;
        private ToolStripButton toolStripButtonStart;
        private ToolStripButton toolStripButtonStop;
        private ToolStripComboBox toolStripComboBoxCoils;
        private ToolStripComboBox toolStripComboBoxRegisters;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripStatusLabel toolStripStatusLabelCrono;
        private ToolStripStatusLabel toolStripStatusLabelError;
        private ToolTip toolTip;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem viewMenu;
        private ToolStripMenuItem windowsMenu;
        public double coefficient;

        public FormMDIMain()
        {
            this.InitializeComponent();
            this.optionsToolStripMenuItem.Visible = this.debug;
            // Новый код            
            coefficient = 1.18;            
            // -----------
        }

        private static FormMDIMain instance;

        public static FormMDIMain getInstance()
        {            
            if (instance == null)
            {
                instance = new FormMDIMain();
                instance.Hide();
                new Authorization().ShowDialog();
            }           
            return instance;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Info().ShowDialog();
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.LayoutMdi(MdiLayout.Cascade);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form form in base.MdiChildren)
            {
                form.Close();
            }
        }

        private void coilItem_Click(object sender, EventArgs e)
        {
            if (this.supervisorControl != null)
            {
                ToolStripMenuItem item = (ToolStripMenuItem) sender;
                byte tag = (byte) item.Tag;
                this.ShowDataTable(this.supervisorControl.deviceDB.GetDevice(tag).ModBusID, true);
            }
        }

        private void ConfigProjectSettings()
        {
            FormProject project = new FormProject();
            if (this.currentBmsProject != null)
            {
                project.newBmsDatabase = this.currentBmsProject;
                if (project.ShowDialog() == DialogResult.OK)
                {
                    this.currentBmsProject = project.newBmsDatabase;
                    this.InizializzaSistema();
                }
            }
        }

        private void DataReceivedFromAllDevices()
        {
            this.dataLogManage.UpdateData(this.supervisorControl);
            for (int i = 0; i < this.listGraphs.Count; i++)
            {
                ((FormChart) this.listGraphs[i]).UpdateChart();
            }
        }

        private void DataReceivedFromDevices(Supervisor.DeviceEventArgs e)
        {
            
            foreach (Form form in base.MdiChildren)
            {
                try
                {
                    if (((FormDataView) form).ModbusId == e.modbusID)
                    {
                        short[] coilsFromModbusID;
                        if (((FormDataView) form).Register_Coil)
                        {
                            coilsFromModbusID = this.supervisorControl.GetCoilsFromModbusID(((FormDataView) form).ModbusId);
                            ((FormDataView) form).UpdateData(coilsFromModbusID, this.supervisorControl.deviceDB.GetDeviceFromID(((FormDataView) form).ModbusId).DataLogCfg.Coils);
                        }
                        else
                        {
                            coilsFromModbusID = this.supervisorControl.GetRegistersFromModbusID(((FormDataView) form).ModbusId);
                            ((FormDataView) form).UpdateData(coilsFromModbusID, this.supervisorControl.deviceDB.GetDeviceFromID(((FormDataView) form).ModbusId).DataLogCfg.Registers);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void DeleteAllChartFromList()
        {
            for (int i = 0; i < this.listGraphs.Count; i++)
            {
                ((FormChart) this.listGraphs[i]).Close();
            }
            this.listGraphs.Clear();
            this.numberOfCharts = 0;
        }

        private void DeleteChartFromList(int numberId)
        {
            for (int i = 0; i < this.listGraphs.Count; i++)
            {
                if (((FormChart) this.listGraphs[i]).graphNumber == numberId)
                {
                    this.listGraphs.RemoveAt(i);
                    this.numberOfCharts--;
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

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void FormMDIMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((this.supervisorControl != null) && this.supervisorControl.IsRunning())
            {
                e.Cancel = true;
                MessageBox.Show("Stop recording data", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void FormMDIMain_Load(object sender, EventArgs e)
        {
            this.Text = Application.ProductName + " " + Application.ProductVersion;
        }

        private void importNewAermecDatabaseDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDevicesSelection selection = new FormDevicesSelection {
                ImportAermecDatabase = true,
                AermecDatabase = FilesManage.LoadAermecDatabaseFromFile()
            };
            if (selection.ShowDialog() == DialogResult.OK)
            {
                FilesManage.SaveAermecDatabase(selection.AermecDatabase);
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMDIMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.registersTableOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxRegisters = new System.Windows.Forms.ToolStripComboBox();
            this.coilsTableOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxCoils = new System.Windows.Forms.ToolStripComboBox();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newChartToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.charLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importNewAermecDatabaseDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registersToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            this.coilsToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelCrono = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelError = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.dataLogToolStripMenuItem,
            this.viewMenu,
            this.chartsToolStripMenuItem,
            this.toolsMenu,
            this.editMenu,
            this.windowsMenu,
            this.helpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.MdiWindowListItem = this.windowsMenu;
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1016, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator4,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.printSetupToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(48, 20);
            this.fileMenu.Text = "&Файл";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.newToolStripMenuItem.Text = "&Новый BMS проект";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.ShowNewForm);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.openToolStripMenuItem.Text = "&Открыть BMS проект";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenFile);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(229, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.saveToolStripMenuItem.Text = "&Сохранить";
            this.saveToolStripMenuItem.Visible = false;
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.saveAsToolStripMenuItem.Text = "Сохранить &как";
            this.saveAsToolStripMenuItem.Visible = false;
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(229, 6);
            this.toolStripSeparator4.Visible = false;
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.printToolStripMenuItem.Text = "&Печать";
            this.printToolStripMenuItem.Visible = false;
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
            this.printPreviewToolStripMenuItem.Visible = false;
            // 
            // printSetupToolStripMenuItem
            // 
            this.printSetupToolStripMenuItem.Name = "printSetupToolStripMenuItem";
            this.printSetupToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.printSetupToolStripMenuItem.Text = "Print Setup";
            this.printSetupToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(229, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.exitToolStripMenuItem.Text = "В&ыход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
            // 
            // dataLogToolStripMenuItem
            // 
            this.dataLogToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripSeparator9,
            this.toolStripMenuItem1});
            this.dataLogToolStripMenuItem.Name = "dataLogToolStripMenuItem";
            this.dataLogToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.dataLogToolStripMenuItem.Text = "&Проект";
            this.dataLogToolStripMenuItem.Click += new System.EventHandler(this.dataLogToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(122, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItem1.ShowShortcutKeys = false;
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
            this.toolStripMenuItem1.Text = "Load Data";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registersTableOfToolStripMenuItem,
            this.coilsTableOfToolStripMenuItem,
            this.testToolStripMenuItem,
            this.demoToolStripMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "&View";
            // 
            // registersTableOfToolStripMenuItem
            // 
            this.registersTableOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxRegisters});
            this.registersTableOfToolStripMenuItem.Name = "registersTableOfToolStripMenuItem";
            this.registersTableOfToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.registersTableOfToolStripMenuItem.Text = "Registers Table of...";
            // 
            // toolStripComboBoxRegisters
            // 
            this.toolStripComboBoxRegisters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxRegisters.Name = "toolStripComboBoxRegisters";
            this.toolStripComboBoxRegisters.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBoxRegisters.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxRegisters_SelectedIndexChanged);
            // 
            // coilsTableOfToolStripMenuItem
            // 
            this.coilsTableOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxCoils});
            this.coilsTableOfToolStripMenuItem.Name = "coilsTableOfToolStripMenuItem";
            this.coilsTableOfToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.coilsTableOfToolStripMenuItem.Text = "Coils Table of...";
            // 
            // toolStripComboBoxCoils
            // 
            this.toolStripComboBoxCoils.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxCoils.Name = "toolStripComboBoxCoils";
            this.toolStripComboBoxCoils.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBoxCoils.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxCoils_SelectedIndexChanged);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.testToolStripMenuItem.Text = "Test..";
            this.testToolStripMenuItem.Visible = false;
            // 
            // demoToolStripMenuItem
            // 
            this.demoToolStripMenuItem.Name = "demoToolStripMenuItem";
            this.demoToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.demoToolStripMenuItem.Text = "Demo...";
            this.demoToolStripMenuItem.Visible = false;
            // 
            // chartsToolStripMenuItem
            // 
            this.chartsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chartToolStripMenuItem,
            this.newChartToolStripMenuItem1,
            this.charLoadToolStripMenuItem,
            this.toolStripSeparator2});
            this.chartsToolStripMenuItem.Enabled = false;
            this.chartsToolStripMenuItem.Name = "chartsToolStripMenuItem";
            this.chartsToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.chartsToolStripMenuItem.Text = "&Charts";
            // 
            // chartToolStripMenuItem
            // 
            this.chartToolStripMenuItem.Name = "chartToolStripMenuItem";
            this.chartToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.chartToolStripMenuItem.Text = "Chart...";
            this.chartToolStripMenuItem.Visible = false;
            // 
            // newChartToolStripMenuItem1
            // 
            this.newChartToolStripMenuItem1.Name = "newChartToolStripMenuItem1";
            this.newChartToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.newChartToolStripMenuItem1.Text = "New Chart...";
            this.newChartToolStripMenuItem1.Click += new System.EventHandler(this.newChartToolStripMenuItem1_Click_1);
            // 
            // charLoadToolStripMenuItem
            // 
            this.charLoadToolStripMenuItem.Name = "charLoadToolStripMenuItem";
            this.charLoadToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.charLoadToolStripMenuItem.Text = "CharLoad";
            this.charLoadToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(136, 6);
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.importNewAermecDatabaseDevicesToolStripMenuItem,
            this.testToolStripMenuItem1});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(95, 20);
            this.toolsMenu.Text = "&Инструменты";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.optionsToolStripMenuItem.Text = "&Build Aermec Database";
            this.optionsToolStripMenuItem.Visible = false;
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // importNewAermecDatabaseDevicesToolStripMenuItem
            // 
            this.importNewAermecDatabaseDevicesToolStripMenuItem.Name = "importNewAermecDatabaseDevicesToolStripMenuItem";
            this.importNewAermecDatabaseDevicesToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.importNewAermecDatabaseDevicesToolStripMenuItem.Text = "Update Aermec Database Devices...";
            this.importNewAermecDatabaseDevicesToolStripMenuItem.Click += new System.EventHandler(this.importNewAermecDatabaseDevicesToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(259, 22);
            this.testToolStripMenuItem1.Text = "Test...";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator6,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator7,
            this.selectAllToolStripMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(99, 20);
            this.editMenu.Text = "&Редактировать";
            this.editMenu.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(161, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(161, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // windowsMenu
            // 
            this.windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindowToolStripMenuItem,
            this.cascadeToolStripMenuItem,
            this.tileVerticalToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.closeAllToolStripMenuItem});
            this.windowsMenu.Name = "windowsMenu";
            this.windowsMenu.Size = new System.Drawing.Size(68, 20);
            this.windowsMenu.Text = "&Windows";
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registersToolStripMenuItem,
            this.coilsToolStripMenuItem});
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newWindowToolStripMenuItem.Text = "&View Data Table of...";
            this.newWindowToolStripMenuItem.Visible = false;
            this.newWindowToolStripMenuItem.Click += new System.EventHandler(this.newWindowToolStripMenuItem_Click);
            // 
            // registersToolStripMenuItem
            // 
            this.registersToolStripMenuItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.registersToolStripMenuItem.Name = "registersToolStripMenuItem";
            this.registersToolStripMenuItem.Size = new System.Drawing.Size(212, 23);
            // 
            // coilsToolStripMenuItem
            // 
            this.coilsToolStripMenuItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.coilsToolStripMenuItem.Name = "coilsToolStripMenuItem";
            this.coilsToolStripMenuItem.Size = new System.Drawing.Size(152, 23);
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tileVerticalToolStripMenuItem.Text = "Tile &Vertical";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tileHorizontalToolStripMenuItem.Text = "Tile &Horizontal";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeAllToolStripMenuItem.Text = "C&lose All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.toolStripSeparator8,
            this.aboutToolStripMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(68, 20);
            this.helpMenu.Text = "&Помощь";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            this.contentsToolStripMenuItem.Visible = false;
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            this.indexToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(165, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.aboutToolStripMenuItem.Text = "&О компании";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.toolStripButtonStart,
            this.toolStripButtonStop});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1016, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "ToolStrip";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = global::Properties.Resources.Folder_new_icon;
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "New";
            this.newToolStripButton.Click += new System.EventHandler(this.ShowNewForm);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::Properties.Resources.Open_icon8508;
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "Open";
            this.openToolStripButton.Click += new System.EventHandler(this.OpenFile);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::Properties.Resources.save_icon_9;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonStart.Enabled = false;
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(42, 22);
            this.toolStripButtonStart.Text = "Старт";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonStop.Enabled = false;
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(38, 22);
            this.toolStripButtonStop.Text = "Стоп";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatusLabelCrono,
            this.toolStripProgressBar1,
            this.toolStripStatusLabelError});
            this.statusStrip.Location = new System.Drawing.Point(0, 478);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1016, 22);
            this.statusStrip.Stretch = false;
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabel.Text = "Status";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelCrono
            // 
            this.toolStripStatusLabelCrono.Name = "toolStripStatusLabelCrono";
            this.toolStripStatusLabelCrono.Size = new System.Drawing.Size(61, 17);
            this.toolStripStatusLabelCrono.Text = "    00:00:00";
            this.toolStripStatusLabelCrono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // toolStripStatusLabelError
            // 
            this.toolStripStatusLabelError.AutoSize = false;
            this.toolStripStatusLabelError.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelError.DoubleClickEnabled = true;
            this.toolStripStatusLabelError.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabelError.Name = "toolStripStatusLabelError";
            this.toolStripStatusLabelError.Size = new System.Drawing.Size(300, 17);
            this.toolStripStatusLabelError.Text = "Com. Stats.";
            this.toolStripStatusLabelError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabelError.DoubleClick += new System.EventHandler(this.toolStripStatusLabelError_DoubleClick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 5000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // FormMDIMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 500);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMDIMain";
            this.Text = "Aermec BMS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMDIMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMDIMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void InizializzaSistema()
        {
            try
            {                
                switch (this.currentBmsProject.dataSource)
                {
                    case DataSource.COM:
                        this.supervisorControl = new SupervisorCOM(this.currentBmsProject, this.serialPort1);                        
                        break;
                    case DataSource.TcpIP:
                        this.supervisorControl = new SupervisorTcpIP(this.currentBmsProject,
                                                                     this.currentBmsProject.BmsTcpIPConfig);                        
                        break;
                    default:
                        MessageBox.Show("Не задан источник данных");
                        break;
                }
                        
                this.dataLogManage = new DataLogManage(this.currentBmsProject);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.supervisorControl.DataDeviceUpdate += new EventHandler<Supervisor.DeviceEventArgs>(this.supervisorControl_DataDeviceUpdate);
            this.supervisorControl.DataAllDevicesUpdate += new EventHandler(this.supervisorControl_DataAllDevicesUpdate);
            this.supervisorControl.DeviceReadRegisterError += new EventHandler<Supervisor.DeviceErrorEventArgs>(this.supervisorControl_DeviceReadRegisterError);
            this.supervisorControl.DeviceReadCoilsError += new EventHandler<Supervisor.DeviceErrorEventArgs>(this.supervisorControl_DeviceReadCoilsError);
            this.supervisorControl.LogStopped += new EventHandler(this.supervisorControl_LogStopped);           
            this.LoadTables();            
            this.toolStripStatusLabelError.Text = "";
            this.toolStripButtonStart.Enabled = true;
            this.chartsToolStripMenuItem.Enabled = true;
            this.settingsToolStripMenuItem.Enabled = true;
            this.DeleteAllChartFromList();
        }

        private void LoadTables()
        {
            string str = "";
            foreach (Form form in base.MdiChildren)
            {
                form.Close();
            }
            if (this.supervisorControl != null)
            {                             
                this.toolStripComboBoxRegisters.Items.Clear();
                this.registersTableOfToolStripMenuItem.DropDownItems.Clear();
                int index = 0;                
                while (index < this.supervisorControl.deviceDB.DeviceArrayList.Count)
                {
                    if (this.supervisorControl.deviceDB.GetDevice(index).Enabled)
                    {
                        str = string.Concat(new object[] { "ID:", this.supervisorControl.deviceDB.GetDevice(index).ModBusID, " - ", this.supervisorControl.deviceDB.GetDevice(index).DeviceName });
                        ToolStripMenuItem item = new ToolStripMenuItem
                        {
                            Text = str
                        };
                        this.registersTableOfToolStripMenuItem.DropDownItems.Add(item);
                        item.Click += new EventHandler(this.registerItem_Click);
                        item.Tag = (byte)index;
                        this.registerItem_Click(item, new EventArgs());
                    }
                    index++;
                }                
                this.toolStripComboBoxCoils.Items.Clear();
                this.coilsTableOfToolStripMenuItem.DropDownItems.Clear();
                for (index = 0; index < this.supervisorControl.deviceDB.DeviceArrayList.Count; index++)
                {
                    if (this.supervisorControl.deviceDB.GetDevice(index).Enabled)
                    {
                        str = string.Concat(new object[] { "ID:", this.supervisorControl.deviceDB.GetDevice(index).ModBusID, " - ", this.supervisorControl.deviceDB.GetDevice(index).DeviceName });
                        ToolStripMenuItem item2 = new ToolStripMenuItem
                        {
                            Text = str
                        };
                        this.coilsTableOfToolStripMenuItem.DropDownItems.Add(item2);
                        item2.Click += new EventHandler(this.coilItem_Click);
                        item2.Tag = (byte)index;
                        this.coilItem_Click(item2, new EventArgs());
                    }
                }
                base.LayoutMdi(MdiLayout.TileVertical);
            }
        }

        private void LogStopped()
        {
            this.toolStripStatusLabel.Text = "Stopped ";
            this.timer1.Stop();
            this.toolStripButtonStart.Enabled = true;
            this.toolStripButtonStop.Enabled = false;
        }

        private void NewChart()
        {
            FormChart chart = new FormChart(this.currentBmsProject, this.supervisorControl);
            chart.FormClosing += new FormClosingEventHandler(this.newChart_FormClosing);
            this.numberOfCharts++;
            chart.graphNumber = this.numberOfCharts;
            this.listGraphs.Add(chart);
            chart.Show();
        }

        private void newChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DeleteChartFromList(((FormChart) sender).graphNumber);
        }

        private void newChartToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            this.NewChart();
        }

        private void NewProject()
        {
            FormProject project = new FormProject {
                newProject = true,
                newBmsDatabase = new BmsProject()
            };
            if (project.ShowDialog() == DialogResult.OK)
            {
                this.currentBmsProject = project.newBmsDatabase;
                this.Text = Application.ProductName + " - " + this.currentBmsProject.Name;
                this.InizializzaSistema();
            }
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.supervisorControl != null)
            {
                for (int i = 0; i < this.supervisorControl.deviceDB.DeviceArrayList.Count; i++)
                {
                    if (((Device) this.supervisorControl.deviceDB.DeviceArrayList[i]).Enabled)
                    {
                        this.ShowDataTable(((Device) this.supervisorControl.deviceDB.DeviceArrayList[i]).ModBusID, false);
                        this.ShowDataTable(((Device) this.supervisorControl.deviceDB.DeviceArrayList[i]).ModBusID, true);
                    }
                }
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            this.OpenProject();
        }

        private void OpenProject()
        {
            FormBmsSelection selection = new FormBmsSelection();
            if (selection.ShowDialog() == DialogResult.OK)
            {
                this.currentBmsProject = selection.BmsDatabase;
                this.Text = Application.ProductName + " - " + this.currentBmsProject.Name;
                this.InizializzaSistema();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDevicesDataB ab = new FormDevicesDataB {
                AermecDatabase = FilesManage.LoadAermecDatabaseFromFile()
            };
            if (ab.ShowDialog() == DialogResult.OK)
            {
                FilesManage.SaveAermecDatabase(ab.AermecDatabase);
            }
        }

        private void registerItem_Click(object sender, EventArgs e)
        {
            if (this.supervisorControl != null)
            {
                ToolStripMenuItem item = (ToolStripMenuItem) sender;
                byte tag = (byte) item.Tag;
                this.ShowDataTable(this.supervisorControl.deviceDB.GetDevice(tag).ModBusID, false);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = dialog.FileName;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConfigProjectSettings();
        }

        private void ShowDataTable(byte modbusId, bool register_coil)
        {
            Device deviceFromID = null;
            if (this.supervisorControl != null)
            {
                deviceFromID = this.supervisorControl.deviceDB.GetDeviceFromID(modbusId);
            }
            if (deviceFromID != null)
            {
                string str = "ID:" + modbusId.ToString() + " " + deviceFromID.DeviceName;
                if (register_coil)
                {
                    str = str + " Coils";
                }
                else
                {
                    str = str + " Registers";
                }
                Form[] mdiChildren = base.MdiChildren;
                foreach (Form form in base.MdiChildren)
                {
                    if (form.Text == str)
                    {
                        form.Show();
                        return;
                    }
                }
                FormDataView view = new FormDataView {
                    MdiParent = this,
                    Text = str,
                    ModbusId = modbusId,
                    Register_Coil = register_coil
                };
                if (register_coil)
                {
                    view.CreateTable(this.supervisorControl.deviceDB.GetDeviceFromID(modbusId).DataLogCfg.Coils);
                }
                else
                {
                    view.CreateTable(this.supervisorControl.deviceDB.GetDeviceFromID(modbusId).DataLogCfg.Registers);
                }
                view.Show();
            }
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            this.NewProject();
        }

        private void ShowTestForm()
        {
        }

        private void StartSupervisor()
        {            
            if ((this.supervisorControl != null) && !this.supervisorControl.IsRunning())
            {
                try
                {
                    this.dataLogManage = new DataLogManage(this.currentBmsProject);
                    this.supervisorControl.StartModbusMasterPool();
                    this.toolStripStatusLabel.Text = "Running ... ";
                    this.timer1.Start();
                    this.toolStripButtonStart.Enabled = false;
                    this.toolStripButtonStop.Enabled = true;
                    this.dataLogManage.StartLogging();
                    this.timer2.Start();
                    this.toolStripStatusLabelCrono.Text = "";
                    this.crono = new TimeSpan();
                    this.fileMenu.Enabled = false;
                    this.newToolStripButton.Enabled = false;
                    this.openToolStripButton.Enabled = false;
                    this.saveToolStripButton.Enabled = false;
                    this.settingsToolStripMenuItem.Enabled = false;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void StatusBarDisplayErrors(Supervisor.DeviceErrorEventArgs e)
        {
            this.toolStripStatusLabelError.ForeColor = Color.Red;
            string str = string.Concat(new object[] { e.Name, " ", e.ErrorCode.ToString(), "(", this.currentBmsProject.GetDeviceFromID(e.ModbusID).GetTotalErrors(), "/", this.currentBmsProject.GetDeviceFromID(e.ModbusID).GetTotalCommandSent(), ")" });
            this.toolStripStatusLabelError.Text = str;
            this.timer3.Stop();
            this.timer3.Start();
        }

        private void StopSupervisor()
        {
            if ((this.supervisorControl != null) && this.supervisorControl.IsRunning())
            {
                this.supervisorControl.StopModbusMasterPool();
                this.dataLogManage.StopLogging();
                this.timer2.Stop();
                this.fileMenu.Enabled = true;
                this.newToolStripButton.Enabled = true;
                this.openToolStripButton.Enabled = true;
                this.saveToolStripButton.Enabled = true;
                this.dataLogToolStripMenuItem.Enabled = true;
                this.settingsToolStripMenuItem.Enabled = true;
                if (MessageBox.Show(this, "Do you want to save log data ?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    this.dataLogManage.DeleteActualDataStorage();
                    this.newDataLogFile = false;
                }
                else
                {
                    this.newDataLogFile = true;
                }
            }
        }

        private void supervisorControl_DataAllDevicesUpdate(object sender, EventArgs e)
        {
            base.Invoke(new DelegateReceiveDataAllDevices(this.DataReceivedFromAllDevices));
        }

        private void supervisorControl_DataDeviceUpdate(object sender, Supervisor.DeviceEventArgs e)
        {
            base.Invoke(new DelegateReceiveData(this.DataReceivedFromDevices), new object[] { e });
        }

        private void supervisorControl_DeviceReadCoilsError(object sender, Supervisor.DeviceErrorEventArgs e)
        {
            base.Invoke(new DelegateDeviceError(this.StatusBarDisplayErrors), new object[] { e });
        }

        private void supervisorControl_DeviceReadRegisterError(object sender, Supervisor.DeviceErrorEventArgs e)
        {
            base.Invoke(new DelegateDeviceError(this.StatusBarDisplayErrors), new object[] { e });
        }

        private void supervisorControl_LogStopped(object sender, EventArgs e)
        {
            base.Invoke(new DelegateLogStopped(this.LogStopped));
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.LayoutMdi(MdiLayout.TileVertical);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.toolStripProgressBar1.Value < this.toolStripProgressBar1.Maximum)
            {
                this.toolStripProgressBar1.Value++;
            }
            else
            {
                this.toolStripProgressBar1.Value = 0;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.crono = this.crono.Add(new TimeSpan(0, 0, 0, 0, 500));
            if (this.crono.TotalDays > 1.0)
            {
                this.toolStripStatusLabelCrono.Text = Math.Truncate(this.crono.TotalDays).ToString() + "D " + this.crono.Hours.ToString("00") + ":" + this.crono.Minutes.ToString("00") + ":" + this.crono.Seconds.ToString("00");
            }
            else
            {
                this.toolStripStatusLabelCrono.Text = this.crono.Hours.ToString("00") + ":" + this.crono.Minutes.ToString("00") + ":" + this.crono.Seconds.ToString("00");
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabelError.Text = "Com. Stats.";
            this.toolStripStatusLabelError.ForeColor = Color.Black;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.StartSupervisor();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            this.timer2.Enabled = !this.timer2.Enabled;
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            this.StopSupervisor();
        }

        private void toolStripComboBoxCoils_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((this.supervisorControl != null) && (this.toolStripComboBoxCoils.SelectedIndex >= 0))
            {
                this.ShowDataTable(this.supervisorControl.deviceDB.GetDevice(this.toolStripComboBoxCoils.SelectedIndex).ModBusID, true);
            }
        }

        private void toolStripComboBoxRegisters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((this.supervisorControl != null) && (this.toolStripComboBoxRegisters.SelectedIndex >= 0))
            {
                this.ShowDataTable(this.supervisorControl.deviceDB.GetDevice(this.toolStripComboBoxRegisters.SelectedIndex).ModBusID, false);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormDataLogMain main = new FormDataLogMain();
            if (this.dataLogManage != null)
            {
                this.dataLogManage.UpdateIndexFile();
            }
            main.ShowDialog();
        }

        private void toolStripStatusLabelError_DoubleClick(object sender, EventArgs e)
        {
            if (this.currentBmsProject != null)
            {
                MessageBox.Show(this, string.Concat(new object[] { "Comunication errors: ", this.currentBmsProject.GetTotalErrors(), "/", this.currentBmsProject.GetTotalCommandSent() }), "Comunication errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private delegate void DelegateDeviceError(Supervisor.DeviceErrorEventArgs e);

        private delegate void DelegateLogStopped();

        private delegate void DelegateReceiveData(Supervisor.DeviceEventArgs e);

        private delegate void DelegateReceiveDataAllDevices();

        private void dataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


    }
}

