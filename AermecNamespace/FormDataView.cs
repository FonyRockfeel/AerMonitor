namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    public class FormDataView : Form
    {
        private int[] address;
        private bool[] asciiView;
        private byte[] byteTmp;
        private IContainer components;
        private int count;
        private short[] data;
        private DataGridView dataGridView1;
        private string[] description;
        private decimal[] gain;
        public byte ModbusId;
        public bool Register_Coil;
        public int registersLength;
        private Timer timer1;
        private bool hasError;
        private String errorParam;
        private AddThreshold addThresholdForm;
        private AlertForm alertForm;
        private double coefficient;        

        // новый код 
        public String[] thresholdLow;
        public String[] thresholdHigh;

        public FormDataView()
        {            
           this.InitializeComponent();

        }      

        public void CreateTable(DataLogConfig.DataLog[] registers)
        {
            registersLength = registers.Length;
            int num;
            int num3 = 0;
            int[] numArray = new int[registers.Length];
            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn {
                HeaderText = "Адрес",
                Name = "Address",
                ValueType = typeof(int)
            };
            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn {
                HeaderText = "Значение",
                Name = "Value",
                ValueType = typeof(decimal)
            };
            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn {
                HeaderText = "Нижний предел значений",
                Name = "ThresholdLow",
                ValueType = typeof(String)
            };
            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn
            {
                HeaderText = "Верхний предел значений",
                Name = "ThresholdHigh",
                ValueType = typeof(String)
            };
            DataGridViewTextBoxColumn column5 = new DataGridViewTextBoxColumn
            {
                HeaderText = "Описание",
                Name = "Description",
                ValueType = typeof(string)
            };
            this.dataGridView1.Columns.Clear();            
            this.dataGridView1.Columns.Add(column1);
            this.dataGridView1.Columns.Add(column2);
            this.dataGridView1.Columns.Add(column3);
            this.dataGridView1.Columns.Add(column4);
            this.dataGridView1.Columns.Add(column5);

            for (int i = 0; i < 5; i++)
            {
                this.dataGridView1.Columns[i].ReadOnly = true;
                this.dataGridView1.Columns[i].Width = 60;
            }

            //this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


            for (num = 0; num < registers.Length; num++)
            {
                if (registers[num].Enable)
                {
                    numArray[num] = registers[num].Address;
                }
            }
            this.dataGridView1.RowCount = num;
            this.address = new int[num];
            this.description = new string[num];
            this.gain = new decimal[num];
            this.data = new short[num];
            this.asciiView = new bool[num];
            this.byteTmp = new byte[num];

            // новый код
            this.thresholdLow = new String[num];
            this.thresholdHigh = new String[num];

            for (num = 0; num < registers.Length; num++)
            {
                if (registers[num].Enable)
                {
                    this.address[num] = registers[num].Address;
                    this.description[num] = registers[num].Description;
                    this.gain[num] = registers[num].Gain;
                    this.asciiView[num] = registers[num].Ascii;  
                }
            }            
        }

       

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    e.Value = this.address[e.RowIndex];
                    return;

                case 1:
                    if (this.data != null)
                    {
                        if (this.asciiView[e.RowIndex])
                        {
                            for (int i = 0; i < this.byteTmp.Length; i++)
                            {
                                this.byteTmp[i] = (byte) this.data[i];
                            }
                            if (this.data[e.RowIndex] == 0xb0)
                            {
                                e.Value =  "\x00b0";
                            }
                            else
                            {
                                e.Value = Encoding.UTF8.GetString(this.byteTmp, e.RowIndex, 1);
                            }
                        }
                        else
                        {
                            e.Value = this.data[e.RowIndex] * this.gain[e.RowIndex];
                        }
                        this.count++;
                        return;
                    }
                    e.Value = 0;
                    return;

                case 2:
                    e.Value = this.thresholdLow[e.RowIndex];                   
                    return;

                case 3:                    
                    e.Value = this.thresholdHigh[e.RowIndex];                    
                    return;

                case 4:
                    e.Value = this.description[e.RowIndex];
                    return;
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(768, 542);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView1_CellValueNeeded);
            // 
            // timer1
            // 
            //this.timer1.Enabled = true;
            this.timer1.Interval = 35000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormDataView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormDataView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

            coefficient = FormMDIMain.getInstance().coefficient;           
        }

        // Рабочая функция
        public void UpdateData(short[] values, DataLogConfig.DataLog[] dataLogConf)
        {            
            this.hasError = false;
            this.errorParam = "";
            try
            {
                for (int i = 0; i < registersLength; i++)
                {
                    this.data[i] = (short)(values[dataLogConf[i].Address] * coefficient);

                    // новый код begin
                    if ((this.thresholdLow[i] != null && this.thresholdLow[i].Length != 0 && this.data[i] < short.Parse(this.thresholdLow[i]))
                        || (this.thresholdHigh[i] != null && this.thresholdHigh[i].Length != 0 && this.data[i] > short.Parse(this.thresholdHigh[i])))
                    {
                        this.hasError = true;
                        this.errorParam = errorParam + "Address: " + this.address[i] + " " +
                            this.description[i] + "\n";
                    }
                    // end                    
                }
                if (this.hasError)
                {
                    if (alertForm != null)
                    {
                        alertForm.Close();
                    }
                    alertForm = new AlertForm();
                    alertForm.label2.Text = errorParam;
                    alertForm.Show();
                    timer1.Start();
                }
                else
                {
                    timer1.Stop();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            this.dataGridView1.InvalidateColumn(1);
        }

        // Тестовая функция
        public void UpdateData()
        {           
            Random rand = new Random();
            this.hasError = false;
            this.errorParam = "";
            try
            {                
                for (int i = 0; i < registersLength; i++)
                {
                    this.data[i] = (short)(rand.Next(200)*coefficient);
                    // новый код begin
                    if ((this.thresholdLow[i] != null && this.thresholdLow[i].Length != 0 && this.data[i] < short.Parse(this.thresholdLow[i]))
                        || (this.thresholdHigh[i] != null && this.thresholdHigh[i].Length != 0  && this.data[i] > short.Parse(this.thresholdHigh[i])))
                    {
                        this.hasError = true;
                        this.errorParam = errorParam + "Address: " + this.address[i] + " " + 
                            this.description[i] + "\n";
                    } 
                    // end                    
                }
                if (this.hasError)
                {
                    if (alertForm != null)
                    {
                        alertForm.Close();
                    }
                    alertForm = new AlertForm();
                    alertForm.label2.Text = errorParam;
                    alertForm.Show();
                    timer1.Start();
                }
                else
                {
                    timer1.Stop();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            this.dataGridView1.InvalidateColumn(1);
        }
        

        // новый код
        public DataGridViewCellEventArgs cellEvent;

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("gjvtyzkjcnm pyfxtybt");
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // если произошел клик по колонке "Ограничение" реагируем на событие
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                if (addThresholdForm != null)
                {
                    addThresholdForm.Close();
                }
                addThresholdForm = new AddThreshold();
                this.cellEvent = e;
                if (e.ColumnIndex == 2)
                {                   
                    addThresholdForm.textBox1.Text = this.thresholdLow[e.RowIndex];                    
                }
                else
                {
                    addThresholdForm.textBox1.Text = this.thresholdHigh[e.RowIndex];
                }
                
                addThresholdForm.formDataView = this;
                addThresholdForm.columnIndex = e.ColumnIndex;
                addThresholdForm.Show();
            }
        }        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (hasError)
            {
                alertForm.Close();
                alertForm = new AlertForm();
                alertForm.label2.Text = errorParam;
                alertForm.Show();
            }
            else
            {
                alertForm.Close();
            }
        }
    }
}

