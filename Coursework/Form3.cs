using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coursework
{
    public partial class Form3 : Form
    {
        string connectionString;
        Form1 form1;
        Form2 form;
        Form4 form4;
        public Form3(string str)
        {
            InitializeComponent();
            connectionString = str;
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = Application.OpenForms[0];
            form.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            form = new Form2(connectionString);
            form.Show();
            this.Hide();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            form1 = new Form1();
            form1.Show();
            this.Hide();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            form4 = new Form4(connectionString);
            form4.Show();
            this.Hide();
            
        }
    }
}
