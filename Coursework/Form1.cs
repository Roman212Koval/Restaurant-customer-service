using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coursework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String connectionStr;
        Form3 form;
        Form5 form5;
        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder["Data Source"] = "PC-ROMA";
            //connectionStringBuilder["Initial Catalog"] = "Laba6";
            connectionStringBuilder["Initial Catalog"] = "Course";
            connectionStringBuilder["Integrated Security"] = true;
            connectionStr = connectionStringBuilder.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                try
                {
                    connection.Open();
                    form = new Form3(connectionStringBuilder.ConnectionString);
                    
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            //CreateList();
            //CreateBox();
            //comboBox5.SelectedIndexChanged += comboBox5_SelectedIndexChanged;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                try
                {
                    string login1 = login.Text;
                    string password1 = password.Text;
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = @"SELECT Count(UserId) from Users where Login='" + login1 + "' and Password= '" + password1 + "' ;";                    

                    int corect_log = Convert.ToInt32( command.ExecuteScalar());

                    if (corect_log == 0)
                    {
                        MessageBox.Show("Невірний вхід");
                    }
                    else
                    {
                        MessageBox.Show("Все вірно");
                        SqlCommand command1 = new SqlCommand();
                        command1.Connection = connection;
                        command1.CommandText = @"SELECT Admin from Users where Login='" + login1  +"';";
                        bool corect_adm = Convert.ToBoolean( command1.ExecuteScalar());
                        if (corect_adm)
                        {
                            form.Show();
                            
                        }
                        else
                        {
                            form5 = new Form5(connectionStr, login1);
                            form5.Show();

                        }
                        this.Hide();
                    }
                    connection.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(".......Не получилось прочитать из БД. " + exception.Message);
                }
            }
        }
    }
}
