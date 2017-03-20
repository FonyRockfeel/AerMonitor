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
    public partial class AddThreshold : Form
    {
        public FormDataView formDataView;
        public int columnIndex;

        public AddThreshold()
        {
            InitializeComponent();
            this.ActiveControl = textBox1;
            textBox1.MaxLength = 6;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String inputValue = this.textBox1.Text;            
            try
            {
                if (columnIndex == 2 && CorrectInput(2, this.formDataView.cellEvent.RowIndex, inputValue))
                {
                    this.formDataView.thresholdLow[this.formDataView.cellEvent.RowIndex] = inputValue;
                    this.Close();
                }
                else if (columnIndex == 3 && CorrectInput(3, this.formDataView.cellEvent.RowIndex, inputValue))
                {
                    this.formDataView.thresholdHigh[this.formDataView.cellEvent.RowIndex] = inputValue;
                    this.Close();
                }
                //this.formDataView.UpdateData(); // вызов тестовой функции (потом закоментить)
            }
            catch {}
        }             
           
        

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // проверяем чтобы нижний предел не превышал верхний
        private bool CorrectInput(int columnIndex, int rowIndex, String inputValue)
        {           
            try
            {
                if (inputValue.Length == 0) return true;
                inputValue = short.Parse(inputValue).ToString(); // проверяем, является ли строка числом
                if ((columnIndex == 2 && this.formDataView.thresholdHigh[rowIndex] != null
                        && short.Parse(this.formDataView.thresholdHigh[rowIndex]) < short.Parse(inputValue))
                || (columnIndex == 3 && this.formDataView.thresholdLow[rowIndex] != null
                        && short.Parse(this.formDataView.thresholdLow[rowIndex]) > short.Parse(inputValue)))
                {
                    MessageBox.Show("Нижний предел не может превышать верхний");
                    return false;
                }

                return true;
            }            
            catch
            {
                MessageBox.Show("Неверный формат данных");
                return false;                
            }            
        }

    }
}
