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
    public partial class Form5 : Form
    {
        string connectionString;
        int CountPr = 0;
        Form1 form1;
        Form6 form6;


        public Form5(string str, string login)
        {
            InitializeComponent();
            connectionString = str;
            GetUserId(str, login);
            CountProduct();
            label1.Text = "Вітаємо вас, " + login;
            FillingItem();
            comboBox1.Items.Add("Назва");
            comboBox1.Items.Add("Ціна");

        }
        private List<string> NameFromId(string connectionStr, string id)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = connection;
            selCommand.CommandText = @"SELECT Title, Price from Product where ProductId= @Id ;";
            selCommand.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = selCommand.ExecuteReader();

            List<string> result = new List<string>();
            while (reader.Read())
            {
                string title = reader.GetString(0);
                string price = reader.GetSqlMoney(1).ToString();
                result.Add(title);
                result.Add(price);
            }


            return result;
        }
        private void GetUserId(string connectionStr, string login)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = connection;
            selCommand.CommandText = @"SELECT UserId from Users where Login= @Login ;";
            selCommand.Parameters.AddWithValue("@Login", login);
            string result = selCommand.ExecuteScalar().ToString();
            ID.Text = result;
        }

        private void CountProduct()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = @"select COUNT(ProductId) from Product";
                    CountPr = Convert.ToInt32(command.ExecuteScalar());

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не виконати запит. " + exception.Message);
                }
            }
        }

        private void OrderSave()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    foreach (var key in Orders.Keys.ToArray())
                    {
                        connection.Open();
                        SqlCommand selCommand = new SqlCommand();
                        selCommand.Connection = connection;
                        selCommand.CommandText = @"insert into Orders (UserId, ProductId, Number) values (@UserId, @ProductId, @Number) ;";
                        selCommand.Parameters.AddWithValue("@UserId", ID.Text);
                        selCommand.Parameters.AddWithValue("@ProductId", key);
                        selCommand.Parameters.AddWithValue("@Number", Orders[key]);
                        SqlDataReader reader = selCommand.ExecuteReader();
                        connection.Close();
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Не виконати запит. " + exception.Message);
                }
            }
        }


        public class MenuItem
        {
            public string price, titel, mass, category, country, descript, id;
            public string[] Data;
            public byte[] imgData;
            public MenuItem(byte[] img, string t, string p, string m, string ca, string co, string d, int i)
            {
                imgData = img;
                titel = t;
                price = p + "грн";
                mass = m + "";
                category = ca;
                country = co;
                descript = d;
                id = Convert.ToString(i);
                Data = new string[] { price, mass, category, country };
            }


            //string[] data = { price, "BMW", "Ford", "Mazda" };
            public List<GroupBox> CreateItem(GroupBox groupBox, List<GroupBox> GroupBoxList)
            {
                GroupBox currentGroupBox = new GroupBox();
                currentGroupBox.Size = new Size(110, 200);
                currentGroupBox.Text = "";
                currentGroupBox.Name = "Group" + id;


                //GroupBoxList.Last();

                if (GroupBoxList.Count % 10 == 0)
                {
                    groupBox.Controls.Clear();
                    currentGroupBox.Location = new Point(5, 0);
                }
                else
                {
                    if ((GroupBoxList.Count % 10) % 5 == 0)
                    {
                        currentGroupBox.Location = new Point(5, GroupBoxList.Last().Location.Y + 200);
                    }
                    else
                    {
                        currentGroupBox.Location = new Point(GroupBoxList.Last().Location.X + 120, GroupBoxList.Last().Location.Y);
                    }

                }

                Label tit = new Label();
                tit.Text = titel;
                tit.Location = new Point(5, 10);
                currentGroupBox.Controls.Add(tit);


                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(100, 100);
                pictureBox.Location = new Point(5, 17);
                using (MemoryStream inStream = new MemoryStream())
                {
                    inStream.Write(imgData, 0, imgData.Length);
                    Image image = Image.FromStream(inStream);
                    pictureBox.Image = new Bitmap(image);
                }
                currentGroupBox.Controls.Add(pictureBox);

                var y = 120;
                foreach (var itm in Data)
                {
                    Label cb = new Label();
                    cb.Text = itm;
                    cb.Size = new Size(70, 18);
                    cb.Location = new Point(5, y);
                    currentGroupBox.Controls.Add(cb);
                    y += 20;
                }

                Button button = new Button();
                button.Name = "btn" + id;
                button.Text = "+";
                button.Size = new Size(20, 30);
                button.Location = new Point(80, 140);
                button.Click += ButtonAdd_Click;
                currentGroupBox.Controls.Add(button);

                GroupBoxList.Add(currentGroupBox);
                groupBox.Controls.Add(currentGroupBox);

                return GroupBoxList;
            }

            public void ButtonAdd_Click(object sender, EventArgs e)
            {
                var button = (Button)sender;

                string num = button.Name.Trim(new char[] { 'b', 't', 'n' }); ;
                if (Orders.ContainsKey(num))
                {
                    Orders[num] = Orders[num] + 1;
                }
                else
                {
                    Form5.Orders.Add(num, 1);
                }

                /*
                var textProduct = groupBox2.Controls["TextP" + num];
                var textCount = groupBox2.Controls["TextC" + num];
                textProduct.Dispose();
                textCount.Dispose();*/

            }
        }


        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        public void FillingItem()
        {
            groupBox5.Controls.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Dictionary<string, string> Mass = NFromId("Mass");
                Dictionary<string, string> Category = NFromId("Category");
                Dictionary<string, string> Country = NFromId("Country");

                try
                {
                    connection.Open();
                    string sql = "SELECT top 100 * FROM Product";
                    string sql2 = "select top " + Convert.ToString(GroupBoxList.Count) + " * FROM Product ";
                    if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked || checkBox4.Checked)
                    {
                        sql += " WHERE ";
                        sql2 += " WHERE ";
                    }
                    if (checkBox1.Checked)
                    {
                        sql += " CategoryId = 1 ";
                        sql2 += " CategoryId = 1 ";
                    }
                    if (checkBox2.Checked)
                    {
                        if (checkBox1.Checked)
                        {
                            sql += " OR "; sql2 += " OR ";
                        }
                        sql += " CategoryId = 2 "; sql2 += " CategoryId = 2 ";
                    }
                    if (checkBox3.Checked)
                    {
                        if (checkBox1.Checked || checkBox2.Checked)
                        {
                            sql += " OR "; sql2 += " OR ";
                        }
                        sql += " CategoryId = 3 "; sql2 += " CategoryId = 3 ";
                    }
                    if (checkBox4.Checked)
                    {
                        if (checkBox1.Checked || checkBox2.Checked || checkBox3.Checked)
                        {
                            sql += " OR "; sql2 += " OR ";
                        }
                        sql += " CategoryId = 4 "; sql2 += " CategoryId = 4 ";
                    }
                    if (comboBox1.Text == "Назва")
                    {
                        sql += " Order by Title "; sql2 += " Order by Title ";
                    }
                    else
                    {
                        sql += " Order by Price "; sql2 += " Order by Price ";
                    }
                    if (checkBox5.Checked)
                    {
                        sql += " DESC "; sql2 += " DESC ";
                    }
                    sql = "select * from (" + sql + ") x except select * from (" + sql2 + ") y";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {

                        int Id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        byte[] data = (byte[])reader.GetValue(2);
                        string massId =  reader.GetInt32(3).ToString();
                        string categoryId = reader.GetInt32(4).ToString();
                        string countryId = reader.GetInt32(5).ToString();
                        int descId = reader.GetInt32(6);
                        string price = reader.GetSqlMoney(7).ToString();
                        //Пишем изображение в pictureBox1

                        MenuItem new1 = new MenuItem(data, title, price, Mass[massId], Category[categoryId], Country[countryId], "Смачна", Id);
                        GroupBoxList = new1.CreateItem(groupBox5, GroupBoxList);
                        new1.imgData = null;
                        new1 = null;
                        if (GroupBoxList.Count % 10 == 0 && GroupBoxList.Count != 0)
                        {
                            //GroupBoxList.Last().Location = new Point(-115, 0);
                            break;
                        }

                    }
                    //countPage = Convert.ToInt32(CountPage(connection));
                    connection.Close();


                }

                catch (Exception exception)
                {
                    MessageBox.Show("1Не получилось прочитать из БД. " + exception.Message);
                }
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }



        public List<GroupBox> GroupBoxList = new List<GroupBox>();
        public static Dictionary<string, int> Orders = new Dictionary<string, int>();
        

        public Dictionary<string, string> NFromId(string table)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand selCommand = new SqlCommand();
            selCommand.Connection = connection;
            selCommand.CommandText = @"SELECT "+ table +"Id , Name from " + table;
            SqlDataReader reader = selCommand.ExecuteReader();

            while (reader.Read())
            {
                string id = reader.GetInt32(0).ToString();
                string name;
                if (table == "Mass")
                {
                     name = reader.GetInt32(1).ToString();
                }
                else
                {
                     name = reader.GetString(1);
                }
                 
                dict.Add(id, name);
            }
            connection.Close();
            return dict;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FillingItem();
        }
        public void ReadBasket()
        {
            groupBox7.Controls.Clear();
            decimal price = 0;
            int count = 0;
            int y = 10;
            foreach (var key in Orders.Keys.ToArray())
            {
                List<string> list = NameFromId(connectionString, key);

                Label label = new Label();
                label.Name = "blp" + key;
                label.Text = list[0];
                label.Location = new Point(50, y);
                groupBox7.Controls.Add(label);

                Label label2 = new Label();
                label2.Name = "blc" + key;
                label2.Size = new Size(30, 25);
                label2.Text = Orders[key].ToString();
                label2.Location = new Point(150 , y);
                groupBox7.Controls.Add(label2);

                Label label3 = new Label();
                label3.Name = "Price" + key;
                label3.Size = new Size(45, 25);
                label3.Text = list[1];
                label3.Location = new Point(181, y);
                groupBox7.Controls.Add(label3);

                Button buttonPl = new Button();
                buttonPl.Name = "Pl" + key;
                buttonPl.Text = "+";
                buttonPl.Size = new Size(40, 25);
                buttonPl.Location = new Point(250, y);
                buttonPl.Click += Plus_Click;
                groupBox7.Controls.Add(buttonPl);

                Button buttonMi = new Button();
                buttonMi.Name = "Mi" + key;
                buttonMi.Text = "-";
                buttonMi.Size = new Size(40, 25);
                buttonMi.Location = new Point(300, y);
                buttonMi.Click += Minus_Click;
                groupBox7.Controls.Add(buttonMi);

                Button buttonDel = new Button();
                buttonDel.Name = "bdel" + key;
                buttonDel.Text = "Видалити";
                buttonDel.Size = new Size(100, 25);
                buttonDel.Location = new Point(350, y);
                buttonDel.Click += delOrder_Click;
                groupBox7.Controls.Add(buttonDel);


                y = y + 30;
                count = count + Orders[key];
                double d = Convert.ToDouble(list[1]); 
                price = price + (Convert.ToInt32(Orders[key]) * (int)Math.Round(d, MidpointRounding.AwayFromZero)); 

            }
            Label label11 = new Label();
            label11.Name = "All" ;
            label11.Text = "Всього";
            label11.Location = new Point(50, y);
            groupBox7.Controls.Add(label11);

            Label label22 = new Label();
            label22.Name = "AllCOunt";
            label22.Size = new Size(30, 25);
            label22.Text = count.ToString();
            label22.Location = new Point(150, y);
            groupBox7.Controls.Add(label22);

            Label label33 = new Label();
            label33.Name = "AllPrice" ;
            label33.Size = new Size(45, 25);
            label33.Text = price.ToString();
            label33.Location = new Point(181, y);
            groupBox7.Controls.Add(label33);


        }
        private void delOrder_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            string num = button.Name.Trim(new char[] { 'b', 'd', 'e', 'l' });
            Orders.Remove(num);
            ReadBasket();
        }

        private void Plus_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            string num = button.Name.Trim(new char[] { 'P', 'l' });
            Orders[num] = Orders[num] + 1;
            ReadBasket();
        }

        private void Minus_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            string num = button.Name.Trim(new char[] { 'M', 'i' });
            if(Orders[num] == 1)
            {
                Orders.Remove(num);
            }
            else
            {
                Orders[num] = Orders[num] - 1;
            }
            
            ReadBasket();
        }
        private void del_Click(object sender, EventArgs e)
        {
            groupBox7.Controls.Clear();
            Orders.Clear();
        }

        private void BasketClick(object sender, EventArgs e)
        {
            ReadBasket();
        }

        private void save_Click(object sender, EventArgs e)
        {
            if(Orders.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }
            OrderSave();
            MessageBox.Show("Замовлення підтверджено");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for(var i = 0; i < 21; i++)
            {
                if (GroupBoxList.Count != 0)
                {
                    GroupBoxList.RemoveAt(GroupBoxList.Count - 1);
                    
                }
            }
            button6_Click(sender, EventArgs.Empty);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;

            GroupBoxList.Clear();
            FillingItem();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GroupBoxList.Clear();
            FillingItem();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            form6 = new Form6(connectionString, ID.Text);
            form6.Show();
            this.Hide();
        }
    }
}
