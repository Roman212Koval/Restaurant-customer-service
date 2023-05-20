using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coursework
{
    public partial class Form2 : Form
    {
        Form3 form3 = null;
        string connectionString;
        int countPage = 0;
        int curentPage = 0;

        public Form2(string str)
        {
            InitializeComponent();

            connectionString = str;
            CreateBox();
            ReadLine(2);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = Application.OpenForms[0];
            form.Close();
        }

        private string NameFromId(SqlConnection Pconnection, int id, string table)
        {
            string column = table + "Id";
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = Pconnection;
            selCommand.CommandText = @"SELECT Name from " + table + " where " + column + "= @Id ;";
            selCommand.Parameters.AddWithValue("@Id", id);
            string result = selCommand.ExecuteScalar().ToString();
            return result;
        }

        private string CountPage(SqlConnection Pconnection)
        {
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = Pconnection;
            selCommand.CommandText = @"SELECT COUNT(ProductId) from Product;";
            string result = selCommand.ExecuteScalar().ToString();
            return result;
        }

        private string IdFromName(SqlConnection Pconnection, string name, string table)
        {
            string column = table + "Id";
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = Pconnection;
            selCommand.CommandText = @"SELECT " + column + " from " + table + " where Name=@Name";
            selCommand.Parameters.AddWithValue("@Name", name);

            string result = selCommand.ExecuteScalar().ToString();
            return result;
        }

        private void ReadLine(int stringN)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM Product";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        if (stringN == i)
                        {
                            int Id = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            byte[] data = (byte[])reader.GetValue(2);
                            int massId = reader.GetInt32(3);
                            int categoryId = reader.GetInt32(4);
                            int countryId = reader.GetInt32(5);
                            int descId = reader.GetInt32(6);
                            int price = reader.GetSqlMoney(7).ToInt32();


                            textBox1.Text = title;
                            textBox2.Text = price.ToString();
                            textBox3.Text = Id.ToString();

                            //Пишем изображение в pictureBox1
                            using (MemoryStream inStream = new MemoryStream())
                            {
                                inStream.Write(data, 0, data.Length);
                                Image image = Image.FromStream(inStream);
                                pictureBox1.Image = new Bitmap(image);
                            }
                            reader.Close();
                            comboBox1.Text = NameFromId(connection, massId, "Mass");                           
                            comboBox2.Text = NameFromId(connection, categoryId, "Category");
                            comboBox3.Text = NameFromId(connection, countryId, "Country");
                            comboBox4.Text = NameFromId(connection, descId, "Descript");
                            break;
                        }
                        else i++;
                    }
                    countPage = Convert.ToInt32( CountPage(connection));
                    connection.Close();


                }

                catch (Exception exception)
                {
                    MessageBox.Show("1Не получилось прочитать из БД. " + exception.Message);
                }
            }
            //Обновляем список на форме
            //CreateList();      
            //comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }
        private void CreateBox()
        {
            comboBox1.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT Name FROM Mass";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader.GetInt32(0).ToString());
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }

            comboBox2.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT Name FROM Category";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }

            comboBox3.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT Name FROM Country";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox3.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
            comboBox4.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT Name FROM Descript";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox4.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не вдалось зчитати з БД. " + exception.Message);
                }
            }
        }


        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //Создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //Формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //Если в окне была нажата кнопка "ОК"
            {
                try
                {
                    textBox1.Text = open_dialog.SafeFileName;
                    pictureBox1.Image = new Bitmap(open_dialog.FileName);
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
             connection.Open();
            string MassId = IdFromName(connection, comboBox1.Text, "Mass");
            string CategoryId = IdFromName(connection, comboBox2.Text, "Category");
            string CountryId = IdFromName(connection, comboBox3.Text, "Country");
            string DescrId = IdFromName(connection, comboBox4.Text, "Descript");
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = @"INSERT INTO Product (Title, ImageData, MassId, CategoryId, CountryId, DescriptId, Price) VALUES
                    (@Title, @ImageData, @MassId, @CategoryId, @CountryId, @DescriptId, @Price)";
                command.Parameters.Add("@Title", SqlDbType.NVarChar, 30);
                command.Parameters.Add("@ImageData", SqlDbType.Image, 1000000);
                command.Parameters.Add("@MassId", SqlDbType.Int);
                command.Parameters.Add("@CategoryId", SqlDbType.Int);
                command.Parameters.Add("@CountryId", SqlDbType.Int);
                command.Parameters.Add("@DescriptId", SqlDbType.Int);
                command.Parameters.Add("@Price", SqlDbType.Money);

                command.Parameters["@Title"].Value = textBox1.Text;
                ImageConverter converter = new ImageConverter();
                command.Parameters["@ImageData"].Value = (byte[])converter.ConvertTo(pictureBox1.Image, typeof(byte[]));
                command.Parameters["@MassId"].Value = MassId;
                command.Parameters["@CategoryId"].Value = CategoryId;
                command.Parameters["@CountryId"].Value = CountryId;
                command.Parameters["@DescriptId"].Value = DescrId;
                command.Parameters["@Price"].Value = textBox2.Text;

                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Не вдалось внести дані в БД. " + exception.Message);
                transaction.Rollback();
            }
        }
    }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (curentPage == 0)
            {
                curentPage = countPage - 1;
            }
            else
            {
                curentPage--;
            }
            ReadLine(curentPage);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (curentPage == countPage - 1)
            {
                curentPage = 0;
            }
            else
            {
                curentPage++;
            }
            ReadLine(curentPage);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string MassId = IdFromName(connection, comboBox1.Text, "Mass");
                string CategoryId = IdFromName(connection, comboBox2.Text, "Category");
                string CountryId = IdFromName(connection, comboBox3.Text, "Country");
                string DescriptId = IdFromName(connection, comboBox4.Text, "Descript");


                    try
                    {
                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandText = @"UPDATE Product SET Title = @Title, ImageData=@ImageData, MassId=@MassId, 
                            CategoryId=@CategoryId, CountryId=@CountryId, DescriptId=@DescriptId, Price = @Price where ProductId=@ProductId";
                        command.Parameters.Add("@ProductId", SqlDbType.Int);
                        command.Parameters.Add("@Title", SqlDbType.NVarChar, 30);
                        command.Parameters.Add("@ImageData", SqlDbType.Image, 1000000);
                        command.Parameters.Add("@MassId", SqlDbType.Int);
                        command.Parameters.Add("@CategoryId", SqlDbType.Int);
                        command.Parameters.Add("@CountryId", SqlDbType.Int);
                        command.Parameters.Add("@DescriptId", SqlDbType.Int);
                        command.Parameters.Add("@Price", SqlDbType.Money);


                        command.Parameters["@ProductId"].Value = textBox3.Text;
                        command.Parameters["@Title"].Value = textBox1.Text;
                        ImageConverter converter = new ImageConverter();
                        command.Parameters["@ImageData"].Value = (byte[])converter.ConvertTo(pictureBox1.Image, typeof(byte[]));
                        command.Parameters["@MassId"].Value = MassId;
                        command.Parameters["@CategoryId"].Value = CategoryId;
                        command.Parameters["@CountryId"].Value = CountryId;
                        command.Parameters["@DescriptId"].Value = DescriptId;
                        command.Parameters["@Price"].Value = textBox2.Text;

                        command.ExecuteNonQuery();

                        //CreateList();

                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Не вдалось зберегти зміни в БД. " + exception.Message);
                    }               
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            form3 = new Form3(connectionString);
            form3.Show();
            this.Hide();
        }
    }
}
