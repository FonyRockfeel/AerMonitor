namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormDeviceCommands : Form
    {
        private Button button1;
        private Button buttonAddCoils;
        private Button buttonAddReg;
        private Button buttonCancel;
        private Button buttonDelete;
        private Button buttonOk;
        private CheckedListBox checkedListBoxCommands;
        private IContainer components;
        public Device device;
        private GroupBox groupBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private bool loading;
        private NumericUpDown numericUpDownAddress;
        private NumericUpDown numericUpDownRegLenght;
        private TextBox textBoxDescription;

        public FormDeviceCommands()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device.ReadInputRegistersCommand command = new Device.ReadInputRegistersCommand(true, 0, 10) {
                Description = "Command Read InputRegisters (0x04)" + this.device.ReadRegistersCommandArray.Count.ToString()
            };
            this.device.ReadRegistersCommandArray.Add(command);
            this.LoadCommands();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Device.ReadRegistersCommand command = new Device.ReadRegistersCommand(true, 0, 10) {
                Description = "Command Read Holding Registers (0x03)" + this.device.ReadRegistersCommandArray.Count.ToString()
            };
            this.device.ReadRegistersCommandArray.Add(command);
            this.LoadCommands();
        }

        private void buttonAddCoils_Click(object sender, EventArgs e)
        {
            Device.ReadCoilsCommand command = new Device.ReadCoilsCommand(true, 0, 10) {
                Description = "Command Read Coils" + this.device.ReadCoilsCommandArray.Count.ToString()
            };
            this.device.ReadCoilsCommandArray.Add(command);
            this.LoadCommands();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = 0;
            if (this.checkedListBoxCommands.SelectedIndex >= 0)
            {
                selectedIndex = this.checkedListBoxCommands.SelectedIndex;
                this.device.DeleteReadCommand(this.checkedListBoxCommands.SelectedIndex);
            }
            this.LoadCommands();
            if (this.checkedListBoxCommands.Items.Count > selectedIndex)
            {
                this.checkedListBoxCommands.SelectedIndex = selectedIndex;
            }
            else if (this.checkedListBoxCommands.Items.Count > 0)
            {
                this.checkedListBoxCommands.SelectedIndex = this.checkedListBoxCommands.Items.Count - 1;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.device.GetRegistersAddress().Length > 0)
            {
                int num1 = this.device.GetRegistersAddress()[this.device.GetRegistersAddress().Length - 1];
            }
            if (this.device.GetCoilsAddress().Length > 0)
            {
                int num2 = this.device.GetCoilsAddress()[this.device.GetCoilsAddress().Length - 1];
            }
            DataLogConfig copy = new DataLogConfig(this.device.GetRegistersAddress(), this.device.GetCoilsAddress());
            if (this.device.DataLogCfg == null)
            {
                this.device.DataLogCfg = copy;
            }
            else
            {
                this.device.DataLogCfg.CopyTo(copy);
                this.device.DataLogCfg = copy;
            }
            this.device.SortCommandByAddress();
        }

        private void checkedListBoxCommands_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!this.loading && (this.checkedListBoxCommands.SelectedIndex >= 0))
            {
                this.device.EnableReadCommand(this.checkedListBoxCommands.SelectedIndex, e.NewValue == CheckState.Checked);
            }
        }

        private void checkedListBoxCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.loading)
            {
                this.loading = true;
                if (this.checkedListBoxCommands.SelectedIndex >= 0)
                {
                    this.buttonDelete.Enabled = true;
                    this.groupBox1.Enabled = true;
                    object readCommand = this.device.GetReadCommand(this.checkedListBoxCommands.SelectedIndex);
                    if (readCommand == null)
                    {
                        return;
                    }
                    if (readCommand.GetType() == typeof(Device.ReadRegistersCommand))
                    {
                        this.numericUpDownRegLenght.Maximum = 127M;
                        Device.ReadRegistersCommand command = (Device.ReadRegistersCommand) readCommand;
                        this.numericUpDownAddress.Value = command.StartAddres;
                        this.numericUpDownRegLenght.Value = command.Size;
                        this.textBoxDescription.Text = command.Description;
                    }
                    if (readCommand.GetType() == typeof(Device.ReadInputRegistersCommand))
                    {
                        this.numericUpDownRegLenght.Maximum = 127M;
                        Device.ReadInputRegistersCommand command2 = (Device.ReadInputRegistersCommand) readCommand;
                        this.numericUpDownAddress.Value = command2.StartAddres;
                        this.numericUpDownRegLenght.Value = command2.Size;
                        this.textBoxDescription.Text = command2.Description;
                    }
                    if (readCommand.GetType() == typeof(Device.ReadCoilsCommand))
                    {
                        this.numericUpDownRegLenght.Maximum = 2000M;
                        Device.ReadCoilsCommand command3 = (Device.ReadCoilsCommand) readCommand;
                        this.numericUpDownAddress.Value = command3.StartAddres;
                        this.numericUpDownRegLenght.Value = command3.Size;
                        this.textBoxDescription.Text = command3.Description;
                    }
                }
                else
                {
                    this.buttonDelete.Enabled = false;
                    this.groupBox1.Enabled = false;
                }
                this.loading = false;
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

        private void FormDeviceCommands_Load(object sender, EventArgs e)
        {
            this.LoadCommands();
        }

        private void InitializeComponent()
        {
            this.checkedListBoxCommands = new CheckedListBox();
            this.buttonAddReg = new Button();
            this.groupBox1 = new GroupBox();
            this.textBoxDescription = new TextBox();
            this.label4 = new Label();
            this.numericUpDownRegLenght = new NumericUpDown();
            this.label3 = new Label();
            this.numericUpDownAddress = new NumericUpDown();
            this.label2 = new Label();
            this.buttonDelete = new Button();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.buttonAddCoils = new Button();
            this.button1 = new Button();
            this.groupBox1.SuspendLayout();
            this.numericUpDownRegLenght.BeginInit();
            this.numericUpDownAddress.BeginInit();
            base.SuspendLayout();
            this.checkedListBoxCommands.FormattingEnabled = true;
            this.checkedListBoxCommands.Location = new Point(12, 12);
            this.checkedListBoxCommands.Name = "checkedListBoxCommands";
            this.checkedListBoxCommands.ScrollAlwaysVisible = true;
            this.checkedListBoxCommands.Size = new Size(0x1bc, 0xf4);
            this.checkedListBoxCommands.TabIndex = 0;
            this.checkedListBoxCommands.ItemCheck += new ItemCheckEventHandler(this.checkedListBoxCommands_ItemCheck);
            this.checkedListBoxCommands.SelectedIndexChanged += new EventHandler(this.checkedListBoxCommands_SelectedIndexChanged);
            this.buttonAddReg.Location = new Point(0x1ce, 0x9b);
            this.buttonAddReg.Name = "buttonAddReg";
            this.buttonAddReg.Size = new Size(0xb9, 0x17);
            this.buttonAddReg.TabIndex = 4;
            this.buttonAddReg.Text = "Add Read Holding Registers (0x03)";
            this.buttonAddReg.TextAlign = ContentAlignment.MiddleLeft;
            this.buttonAddReg.UseVisualStyleBackColor = true;
            this.buttonAddReg.Click += new EventHandler(this.buttonAdd_Click);
            this.groupBox1.Controls.Add(this.textBoxDescription);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownRegLenght);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownAddress);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new Point(0x1ce, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x101, 0x89);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Read Commands";
            this.textBoxDescription.Location = new Point(0x4d, 0x35);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new Size(170, 20);
            this.textBoxDescription.TabIndex = 3;
            this.textBoxDescription.Tag = "";
            this.textBoxDescription.TextChanged += new EventHandler(this.textBoxDescription_TextChanged);
            this.textBoxDescription.Leave += new EventHandler(this.textBoxDescription_Leave);
            this.label4.AutoSize = true;
            this.label4.Location = new Point(8, 0x38);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x3f, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Description:";
            this.numericUpDownRegLenght.Location = new Point(0xbf, 0x1b);
            int[] bits = new int[4];
            bits[0] = 0x7f;
            this.numericUpDownRegLenght.Maximum = new decimal(bits);
            int[] numArray2 = new int[4];
            numArray2[0] = 1;
            this.numericUpDownRegLenght.Minimum = new decimal(numArray2);
            this.numericUpDownRegLenght.Name = "numericUpDownRegLenght";
            this.numericUpDownRegLenght.Size = new Size(0x38, 20);
            this.numericUpDownRegLenght.TabIndex = 2;
            this.numericUpDownRegLenght.Tag = "2";
            this.numericUpDownRegLenght.TextAlign = HorizontalAlignment.Right;
            int[] numArray3 = new int[4];
            numArray3[0] = 1;
            this.numericUpDownRegLenght.Value = new decimal(numArray3);
            this.numericUpDownRegLenght.Leave += new EventHandler(this.numericUpDownRegLenght_Leave);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x8e, 0x1d);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x2b, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lenght:";
            this.numericUpDownAddress.Location = new Point(0x3e, 0x1b);
            int[] numArray4 = new int[4];
            numArray4[0] = 0xffff;
            this.numericUpDownAddress.Maximum = new decimal(numArray4);
            this.numericUpDownAddress.Name = "numericUpDownAddress";
            this.numericUpDownAddress.Size = new Size(0x3f, 20);
            this.numericUpDownAddress.TabIndex = 1;
            this.numericUpDownAddress.TextAlign = HorizontalAlignment.Right;
            this.numericUpDownAddress.Leave += new EventHandler(this.numericUpDownAddress_Leave);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(8, 0x1d);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x30, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Address:";
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new Point(0x187, 0x101);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x41, 0x17);
            this.buttonDelete.TabIndex = 7;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.buttonOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonOk.DialogResult = DialogResult.OK;
            this.buttonOk.Location = new Point(0x247, 0x101);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new Size(0x37, 0x17);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
            this.buttonCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(0x284, 0x101);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(0x4b, 0x17);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonAddCoils.Location = new Point(0x1ce, 0xd5);
            this.buttonAddCoils.Name = "buttonAddCoils";
            this.buttonAddCoils.Size = new Size(0xb9, 0x17);
            this.buttonAddCoils.TabIndex = 11;
            this.buttonAddCoils.Text = "Add Read Coils (0x01)";
            this.buttonAddCoils.TextAlign = ContentAlignment.MiddleLeft;
            this.buttonAddCoils.UseVisualStyleBackColor = true;
            this.buttonAddCoils.Click += new EventHandler(this.buttonAddCoils_Click);
            this.button1.Location = new Point(0x1ce, 0xb8);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0xb9, 0x17);
            this.button1.TabIndex = 12;
            this.button1.Text = "Add Read Input Registers (0x04)";
            this.button1.TextAlign = ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new EventHandler(this.button1_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.buttonCancel;
            base.ClientSize = new Size(0x2d7, 0x11f);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.buttonAddCoils);
            base.Controls.Add(this.buttonAddReg);
            base.Controls.Add(this.buttonDelete);
            base.Controls.Add(this.buttonOk);
            base.Controls.Add(this.buttonCancel);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.checkedListBoxCommands);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FormDeviceCommands";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Commands Device";
            base.Load += new EventHandler(this.FormDeviceCommands_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.numericUpDownRegLenght.EndInit();
            this.numericUpDownAddress.EndInit();
            base.ResumeLayout(false);
        }

        private void LoadCommands()
        {
            int num;
            string str;
            int index = 0;
            this.loading = true;
            this.checkedListBoxCommands.Items.Clear();
            for (num = 0; num < this.device.ReadRegistersCommandArray.Count; num++)
            {
                if (this.device.ReadRegistersCommandArray[num].GetType() == typeof(Device.ReadRegistersCommand))
                {
                    Device.ReadRegistersCommand command = (Device.ReadRegistersCommand) this.device.ReadRegistersCommandArray[num];
                    str = this.StringItemDescription(index, command.StartAddres, command.Size, command.Description);
                    this.checkedListBoxCommands.Items.Add(str);
                    if (command.EnableCommnand)
                    {
                        this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Checked);
                    }
                    else
                    {
                        this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Unchecked);
                    }
                }
                if (this.device.ReadRegistersCommandArray[num].GetType() == typeof(Device.ReadInputRegistersCommand))
                {
                    Device.ReadInputRegistersCommand command2 = (Device.ReadInputRegistersCommand) this.device.ReadRegistersCommandArray[num];
                    str = this.StringItemDescription(index, command2.StartAddres, command2.Size, command2.Description);
                    this.checkedListBoxCommands.Items.Add(str);
                    if (command2.EnableCommnand)
                    {
                        this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Checked);
                    }
                    else
                    {
                        this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Unchecked);
                    }
                }
                index++;
            }
            for (num = 0; num < this.device.ReadCoilsCommandArray.Count; num++)
            {
                Device.ReadCoilsCommand command3 = (Device.ReadCoilsCommand) this.device.ReadCoilsCommandArray[num];
                str = this.StringItemDescription(index, command3.StartAddres, command3.Size, command3.Description);
                this.checkedListBoxCommands.Items.Add(str);
                if (command3.EnableCommnand)
                {
                    this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Checked);
                }
                else
                {
                    this.checkedListBoxCommands.SetItemCheckState(this.checkedListBoxCommands.Items.Count - 1, CheckState.Unchecked);
                }
                index++;
            }
            if (this.checkedListBoxCommands.Items.Count == 0)
            {
                this.buttonDelete.Enabled = false;
            }
            this.loading = false;
        }

        private void numericUpDownAddress_Leave(object sender, EventArgs e)
        {
            this.UpdateCommandValues();
        }

        private void numericUpDownRegLenght_Leave(object sender, EventArgs e)
        {
            this.UpdateCommandValues();
        }

        private string StringItemDescription(int index, ushort startAdd, ushort lenght, string description)
        {
            int num;
            int num2 = 0;
            string str = "";
            for (num = 0; num < this.device.ReadRegistersCommandArray.Count; num++)
            {
                if (num2 == index)
                {
                    if (this.device.ReadRegistersCommandArray[num].GetType() == typeof(Device.ReadRegistersCommand))
                    {
                        str = "(0x03)READ REGISTERS - Address: ";
                    }
                    if (this.device.ReadRegistersCommandArray[num].GetType() == typeof(Device.ReadInputRegistersCommand))
                    {
                        str = "(0x04)INPUT REGISTERS - Address: ";
                    }
                    string str2 = str;
                    str = str2 + startAdd.ToString() + " Lenght: " + lenght.ToString() + " (" + description + ")";
                }
                num2++;
            }
            for (num = 0; num < this.device.ReadCoilsCommandArray.Count; num++)
            {
                if (num2 == index)
                {
                    str = "READ COILS(0x01) - Address: " + startAdd.ToString() + " Lenght: " + lenght.ToString() + " (" + description + ")";
                }
                num2++;
            }
            return str;
        }

        private void textBoxDescription_Leave(object sender, EventArgs e)
        {
            int selectedIndex = this.checkedListBoxCommands.SelectedIndex;
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            this.UpdateCommandValues();
        }

        private void UpdateCommandValues()
        {
            int index = 0;
            if (!this.loading && (this.checkedListBoxCommands.SelectedIndex >= 0))
            {
                index = this.checkedListBoxCommands.SelectedIndex;
                this.device.SetReadCommand(index, this.checkedListBoxCommands.GetItemCheckState(index) == CheckState.Checked, (ushort) this.numericUpDownAddress.Value, (ushort) this.numericUpDownRegLenght.Value, this.textBoxDescription.Text);
                string str = this.StringItemDescription(this.checkedListBoxCommands.SelectedIndex, (ushort) this.numericUpDownAddress.Value, (ushort) this.numericUpDownRegLenght.Value, this.textBoxDescription.Text);
                this.checkedListBoxCommands.Items[index] = str;
                this.textBoxDescription.Focus();
                this.textBoxDescription.DeselectAll();
                this.textBoxDescription.SelectionStart = this.textBoxDescription.Text.Length;
            }
        }
    }
}

