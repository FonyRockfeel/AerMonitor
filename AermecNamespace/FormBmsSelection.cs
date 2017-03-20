namespace AermecNamespace
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormBmsSelection : Form
    {
        public BmsProject BmsDatabase;
        private Button buttonAdd;
        private Button buttonCancel;
        private Button buttonClone;
        private Button buttonMod;
        private Button buttonOk;
        private Button buttonRemove;
        private IContainer components;
        private Label label1;
        private ListBox listBoxBms;
        private BmsProject[] userDatabase;

        public FormBmsSelection()
        {
            this.InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormProject project = new FormProject {
                newProject = true
            };
            if (project.ShowDialog() == DialogResult.OK)
            {
                this.LoadUserBms();
            }
        }

        private void buttonClone_Click(object sender, EventArgs e)
        {
            if (this.listBoxBms.SelectedIndex >= 0)
            {
                BmsProject userBms = FilesManage.LoadUserBmsFromFile((string) this.listBoxBms.SelectedItem).Clone();
                userBms.Name = userBms.Name + "Cloned";
                FilesManage.SaveUserBmsFile(userBms);
                this.LoadUserBms();
            }
        }

        private void buttonMod_Click(object sender, EventArgs e)
        {
            FormProject project = new FormProject();
            if (this.listBoxBms.SelectedIndex >= 0)
            {
                project.newBmsDatabase = FilesManage.LoadUserBmsFromFile((string) this.listBoxBms.SelectedItem);
                int selectedIndex = this.listBoxBms.SelectedIndex;
                if (project.ShowDialog() == DialogResult.OK)
                {
                    this.LoadUserBms();
                    this.listBoxBms.SelectedIndex = selectedIndex;
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.listBoxBms.SelectedIndex >= 0)
            {
                this.BmsDatabase = this.userDatabase[this.listBoxBms.SelectedIndex];
                this.BmsDatabase.CleanDataLogConfig();
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if ((this.listBoxBms.SelectedIndex >= 0) && (MessageBox.Show("Are you sure?", "Confirm remove BMS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                FilesManage.DeleteUserBmsFile((string) this.listBoxBms.SelectedItem);
                this.LoadUserBms();
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

        private void FormBmsSelection_Load(object sender, EventArgs e)
        {
            this.LoadUserBms();
        }

        private void InitializeComponent()
        {
            this.listBoxBms = new System.Windows.Forms.ListBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonMod = new System.Windows.Forms.Button();
            this.buttonClone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxBms
            // 
            this.listBoxBms.FormattingEnabled = true;
            this.listBoxBms.Location = new System.Drawing.Point(12, 25);
            this.listBoxBms.Name = "listBoxBms";
            this.listBoxBms.Size = new System.Drawing.Size(237, 251);
            this.listBoxBms.TabIndex = 0;
            this.listBoxBms.SelectedIndexChanged += new System.EventHandler(this.listBoxBms_SelectedIndexChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(193, 284);
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
            this.buttonCancel.Location = new System.Drawing.Point(255, 284);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "BMS Databases:";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(255, 25);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 9;
            this.buttonAdd.Text = "Добавить...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(255, 125);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 10;
            this.buttonRemove.Text = "Удалить";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonMod
            // 
            this.buttonMod.Enabled = false;
            this.buttonMod.Location = new System.Drawing.Point(255, 54);
            this.buttonMod.Name = "buttonMod";
            this.buttonMod.Size = new System.Drawing.Size(75, 23);
            this.buttonMod.TabIndex = 11;
            this.buttonMod.Text = "Изменить...";
            this.buttonMod.UseVisualStyleBackColor = true;
            this.buttonMod.Click += new System.EventHandler(this.buttonMod_Click);
            // 
            // buttonClone
            // 
            this.buttonClone.Enabled = false;
            this.buttonClone.Location = new System.Drawing.Point(255, 83);
            this.buttonClone.Name = "buttonClone";
            this.buttonClone.Size = new System.Drawing.Size(75, 23);
            this.buttonClone.TabIndex = 12;
            this.buttonClone.Text = "Копировать\\";
            this.buttonClone.UseVisualStyleBackColor = true;
            this.buttonClone.Click += new System.EventHandler(this.buttonClone_Click);
            // 
            // FormBmsSelection
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOk;
            this.ClientSize = new System.Drawing.Size(342, 319);
            this.Controls.Add(this.buttonClone);
            this.Controls.Add(this.buttonMod);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listBoxBms);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBmsSelection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BMS проект";
            this.Load += new System.EventHandler(this.FormBmsSelection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void listBoxBms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxBms.SelectedIndex > -1)
            {
                this.buttonRemove.Enabled = true;
                this.buttonMod.Enabled = true;
                this.buttonClone.Enabled = true;
                this.buttonOk.Enabled = true;
            }
            else
            {
                this.buttonRemove.Enabled = false;
                this.buttonMod.Enabled = false;
                this.buttonClone.Enabled = false;
                this.buttonOk.Enabled = false;
            }
        }

        private void LoadUserBms()
        {
            this.listBoxBms.Items.Clear();
            this.userDatabase = FilesManage.LoadUserBmsDatabaseFromFile();
            for (int i = 0; i < this.userDatabase.Length; i++)
            {
                if (this.userDatabase[i] != null)
                {
                    this.listBoxBms.Items.Add(this.userDatabase[i].Name);
                }
            }
            this.buttonOk.Enabled = false;
        }
    }
}

