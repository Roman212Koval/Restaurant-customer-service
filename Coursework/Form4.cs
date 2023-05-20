﻿using System;
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
    public partial class Form4 : Form
    {
        List<string> IdClient = new List<string>();
        string connectionString;
        Form3 form3;
        public Form4(string str)
        {
            InitializeComponent();
            connectionString = str;

            ClientCount();           
            if (IdClient[0] != null)
            {
                ID.Text = IdClient[0];
            }   
        }

        public static List<TextBox> TextBoxes = new List<TextBox>();
        public void CreateItem()
        {
            TextBoxes.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "SELECT * FROM Orders Where UserId="+ ID.Text + ";";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        int Clientid = reader.GetInt32(0);
                        int ProductId = reader.GetInt32(1);
                        int Number = reader.GetInt32(2);

                        Label label = new Label();
                        label.Name = "label2";
                        label.Text = "//";

                        //          TEXTBOX         //
                        TextBox newTextBox = new TextBox();
                        TextBox lastOldTextBox = TextBoxes.LastOrDefault();
                        newTextBox.Text = $"{ProductId}";
                        newTextBox.ReadOnly = true;                        
                        newTextBox.Name = $"TextP{ProductId}";

                        TextBox TextCount = new TextBox();
                        TextCount.Name = $"TextC{ProductId}";
                        TextCount.Size = new System.Drawing.Size(30, 20);
                        TextCount.Text = $"{Number}";
                        
    
                        Button ButtonDel = new Button();
                        ButtonDel.Name = $"Btn{ProductId}";
                        ButtonDel.Text = "Видалити";
                        ButtonDel.Click += ButtonDel_Click;

                        Button ButtonUpd = new Button();
                        ButtonUpd.Name = $"BtU{ProductId}";
                        ButtonUpd.Text = "Редагувати";
                        ButtonUpd.Click += ButtonUpd_Click;


                        if (lastOldTextBox == null)
                        {
                            label.Location = new Point(20, 15);
                            newTextBox.Location = new Point(20, 50);
                        }
                        else
                        {
                            newTextBox.Location = new Point(lastOldTextBox.Location.X, lastOldTextBox.Location.Y + 30);
                        }                        
                        TextCount.Location = new Point(newTextBox.Location.X + 104, newTextBox.Location.Y);
                        ButtonDel.Location = new Point(newTextBox.Location.X + 148, newTextBox.Location.Y);
                        ButtonUpd.Location = new Point(newTextBox.Location.X + 220, newTextBox.Location.Y);

                        TextBoxes.Add(newTextBox);
                        groupBox2.Controls.Add(label);
                        groupBox2.Controls.Add(newTextBox);
                        groupBox2.Controls.Add(ButtonUpd);
                        groupBox2.Controls.Add(TextCount);
                        groupBox2.Controls.Add(ButtonDel);


                    }
                    reader.Close();
                    for (int j = 0; j < TextBoxes.Count(); j++)
                    {
                        TextBoxes[j].Text =  NameFromId(connection, Convert.ToInt32( TextBoxes[j].Text), "Product");
                    }
                    /*
                    comboBox1.Text = NameFromId(connection, VudavnykId, "Vudavnyk");
                    comboBox2.Text = NameFromId(connection, AvtorId, "Avtor");
                    comboBox3.Text = NameFromId(connection, GenreId, "Genre");
                    comboBox4.Text = NameFromId(connection, ProviderId, "Provider");*/
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не получилось прочитать из БД. " + exception.Message);
                }
            }


        }

        private void ButtonDel_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            string num = button.Name.Trim(new char[] {'B', 't', 'n'});;
            button.Dispose();

            var textProduct = groupBox2.Controls["TextP" + num];
            var textCount = groupBox2.Controls["TextC" + num];
            var updBtn = groupBox2.Controls["BtU" + num];
            textProduct.Dispose();
            textCount.Dispose();
            updBtn.Dispose();

            DelOrder(num);      //Видалення з БД
        }

        private void ButtonUpd_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            string num = button.Name.Trim(new char[] { 'B', 't', 'U' }); ;

            var textProduct = groupBox2.Controls["TextP" + num];
            var textCount = groupBox2.Controls["TextC" + num];

            string prod = IdFromName(textProduct.Text, "Product");

            UpdOrder(textCount.Text, prod);     
        }

        private void save_Click(object sender, EventArgs e)
        {

        }

        private void delete_Click(object sender, EventArgs e)
        {
            groupBox2.Controls.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "DELETE FROM Orders WHERE UserId=" + ID.Text + ";";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    IdClient.Clear();
                    ClientCount();
                    if (IdClient[0] != null)
                    {
                        ID.Text = IdClient[0];
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
        }
        public void DelOrder(string num)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "DELETE FROM Orders WHERE UserId=" + ID.Text + "AND ProductId="+ num;
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();


                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
        }

        public void UpdOrder(string num, string product)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "UPDATE Orders SET Number = " + num + " WHERE ProductID =" + product + " AND UserID ="+ID.Text;
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    connection.Close();

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
        }

        public void ClientCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT(UserId) FROM Orders";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    IdClient.Clear();
                    while (reader.Read())                    
                    {
                        IdClient.Add($"{reader.GetInt32(0)}");
                        
                    }
                    reader.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("----Не вдалось зчитати з БД. " + exception.Message);
                }
            }
        }

        private string NameFromId(SqlConnection Pconnection, int id, string table)
        {
            string column = table + "Id";
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = Pconnection;
            selCommand.CommandText = @"SELECT Title from " + table + " where " + column + "= @Id ;";
            selCommand.Parameters.AddWithValue("@Id", id);
            string result = selCommand.ExecuteScalar().ToString();
            return result;
        }

        private string IdFromName(string name, string table)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string column = table + "Id";
                    SqlCommand selCommand = new SqlCommand();
                    selCommand.Connection = connection;
                    selCommand.CommandText = @"SELECT " + column + " from " + table + " where Title='" + name + "';";
                    string result = selCommand.ExecuteScalar().ToString();
                    connection.Close();
                    return result;

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
            return null;
        }

        private void back_Click(object sender, EventArgs e)
        {            
            form3 = new Form3(connectionString);
            form3.Show();
            this.Hide();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = IdClient.IndexOf(ID.Text);
            if (num == 0)
            {
                num = IdClient.Count() - 1;
            }
            else
            {
                num = num - 1;
            }
            ID.Text = IdClient[num];            
            groupBox2.Controls.Clear();
            CreateItem();
            GetUserLogin(connectionString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int num = IdClient.IndexOf(ID.Text);
            if (num == IdClient.Count() - 1)
            {
                num = 0;
            }
            else 
            {
                num = num + 1;
            }
            ID.Text = IdClient[num];            
            groupBox2.Controls.Clear();
            CreateItem();
            GetUserLogin(connectionString);
        }

        private void GetUserLogin(string connectionStr)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = connection;
            selCommand.CommandText = @"SELECT  Login UserId from Users where UserId= @UserId ;";
            selCommand.Parameters.AddWithValue("@UserId", ID.Text);
            string result = selCommand.ExecuteScalar().ToString();
            connection.Close();
            var label = groupBox2.Controls["label2"];
            label.Text = result;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
