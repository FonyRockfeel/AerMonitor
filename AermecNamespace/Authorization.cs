using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AermecNamespace
{
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
            this.ActiveControl = textBox1;
            textBox1.PasswordChar = '*';
            textBox1.MaxLength = 14;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string currentTime = DateTime.Now.ToShortTimeString();
            String hours = currentTime.Split(':')[0]; 
            String minutes = currentTime.Split(':')[1];
            if (hours == "0" || hours == "00")            
                hours = "23";            
            else
            {
                hours = (short.Parse(hours) - 1).ToString();
                if (hours.Length == 1)
                    hours = "0" + hours;
            }
            if (minutes.Length == 1)
                minutes = "0" + minutes;
           
            if (textBox1.Text != hours + "-" + minutes)            
                MessageBox.Show("Неверный пароль");            
            else
            {
                FormMDIMain.getInstance().coefficient = 1;
                this.Hide();
            }            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


    }
}
