using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace UltrwaWare.xyz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection SQLcon;
        string connectionString = "SERVER=remotemysql.com;PORT=3306;DATABASE=xrVr9XdMhC;UID=xrVr9XdMhC;PASSWORD=859oajPzoO;"; //UID = username
        int Users = 1;
        string hwidnumber;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            try
            {
                SQLcon = new MySqlConnection();
                SQLcon.ConnectionString = connectionString;
                SQLcon.Open();
                string query = "SELECT * FROM UltraWareUsers where ID";
                MySqlCommand cmd = new MySqlCommand(query, SQLcon);
                MySqlDataReader Datareader = cmd.ExecuteReader();
                while (Datareader.Read())
                {
                    Users++;
                }
                Currentuserstxt.Text = "Current Users: " + Users + "!";
                SQLcon.Close();
            }
            catch (MySqlException ex)
            {
                CustomMessageBox messagebox = new CustomMessageBox("Error: " + ex.Message);
                messagebox.Show();
                //MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                SQLcon.Dispose();
            }
            Directory.CreateDirectory("C:\\Cheat");
            hwidnumber = Security.Value();
            LoadRemember();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) //For moving the window
                DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string date = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"); //Formating string to proper mysql format
            if ((string)LoginRegisterBtn.Content == "LOGIN")
            {
                try
                {
                    SQLcon = new MySqlConnection();
                    SQLcon.ConnectionString = connectionString;
                    SQLcon.Open();
                    string query = $"SELECT * FROM UltraWareUsers where Username='{Usernametxt.Text}' and Password='{Passwordtxt.Password}'"; //Create query to table in database provideed in connection string
                    MySqlCommand cmd = new MySqlCommand(query, SQLcon);
                    MySqlDataReader DataReader = cmd.ExecuteReader();
                    int Username = 0;
                    while (DataReader.Read())
                    {
                        Username++;

                    }
                    if (Username == 0)
                    {
                        CustomMessageBox messageboxUsername = new CustomMessageBox("Username or Password is Incorrect. Try again.");
                        messageboxUsername.Show();
                        return;
                    }
                    DataReader.Close();
                    string queryHWID = $"SELECT * FROM UltraWareUsers where Username='{Usernametxt.Text}' and Password='{Passwordtxt.Password}' and HWID='{hwidnumber}'"; //Create query to table in database provided in connection string
                    MySqlCommand cmdHWID = new MySqlCommand(queryHWID, SQLcon);
                    MySqlDataReader DataReaderHWID = cmdHWID.ExecuteReader();
                    int HWID = 0;
                    while (DataReaderHWID.Read())
                    {
                        HWID++;
                    }
                    DataReaderHWID.Close();
                    if (HWID == 0)
                    {
                        CustomMessageBox messagebox2 = new CustomMessageBox("You are logging on other PC! If u've changed the PC contact CRNYY on discord.");
                        messagebox2.Show();
                        return;
                    }
                    string queryTime = $"SELECT * FROM UltraWareUsers WHERE Username='{Usernametxt.Text}'"; // AND `Subscription` AND `ID` AND `Type`
                    MySqlCommand cmdTime = new MySqlCommand(queryTime, SQLcon);
                    MySqlDataReader DataReaderTime = cmdTime.ExecuteReader();
                    DateTime ExpireDate = new DateTime();
                    int Id = 0;
                    string TypeofAcc = "";
                    while (DataReaderTime.Read())
                    {
                        ExpireDate = (DateTime)DataReaderTime["Subscription"];
                        Id = (int)DataReaderTime["ID"];
                        TypeofAcc = (string)DataReaderTime["Type"];
                    }
                    DataReaderTime.Close();
                    if(TypeofAcc == "Free")
                    {
                        CustomMessageBox messageboxTime = new CustomMessageBox("Your account  is Free. Contact CRNYY on discord for upgrading Your account.");
                        messageboxTime.Show();
                        return;
                    }
                    if (DateTime.Compare(ExpireDate, DateTime.Now) < 1)
                    {
                        CustomMessageBox messageboxTime = new CustomMessageBox("Your subscription has expired");
                        messageboxTime.Show();
                        return;
                    }
                    if (Username == 1)
                    {
                        string queryLastLogin = $"UPDATE `UltraWareUsers` SET `LastLogin` = '{date}' WHERE `UltraWareUsers`. `ID` = '{Id}' AND `UltraWareUsers`. `Username` = '{Usernametxt.Text}'"; //For updating the lasttime when user loged in
                        MySqlCommand cmdLastLogin = new MySqlCommand(queryLastLogin, SQLcon);
                        MySqlDataReader DataReaderlastLogin = cmdLastLogin.ExecuteReader();
                        while (DataReaderlastLogin.Read())
                        {

                        }
                        CustomMessageBox messagebox = new CustomMessageBox("Welcome " + Usernametxt.Text + "!");
                        messagebox.Show();
                        LoggedMainWindow mainwindow = new LoggedMainWindow(Usernametxt.Text, Id);
                        mainwindow.Show();
                    }
                    SQLcon.Close();
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    CustomMessageBox messagebox = new CustomMessageBox("Error: " + ex.Message);
                    messagebox.Show();
                }
                finally
                {
                    SQLcon.Dispose();
                }
            }
            else //Register
            {
                try
                {
                    SQLcon = new MySqlConnection();
                    SQLcon.ConnectionString = connectionString;
                    SQLcon.Open();
                    string queryUsername = $"SELECT * FROM UltraWareUsers where Username='{Usernametxt.Text}'"; //Create query to table in database provideed in connection string
                    MySqlCommand cmdCheck = new MySqlCommand(queryUsername, SQLcon);
                    MySqlDataReader DataReaderCheck = cmdCheck.ExecuteReader();
                    int Username = 0;
                    int HWID = 0;
                    while (DataReaderCheck.Read())
                    {
                        Username++;
                    }
                    if (Username > 0)
                    {
                        CustomMessageBox messagebox2 = new CustomMessageBox("This username already exists!");
                        messagebox2.Show();
                        return;
                    }
                    DataReaderCheck.Close();
                    string queryHWID = $"SELECT * FROM UltraWareUsers where HWID='{hwidnumber}'"; //Create query to table in database provided in connection string
                    MySqlCommand cmdHWID = new MySqlCommand(queryHWID, SQLcon);
                    MySqlDataReader DataReaderHWID = cmdHWID.ExecuteReader();
                    while (DataReaderHWID.Read())
                    {
                        HWID++;
                    }
                    DataReaderHWID.Close();
                    if (HWID > 0)
                    {
                        CustomMessageBox messagebox2 = new CustomMessageBox("You have already created an account!");
                        messagebox2.Show();
                        return;
                    }

                    //When declaring columns u want to insert it has to be `(tylda) not '
                    string queryinsert = $"INSERT INTO UltraWareUsers (`ID`, `Username`, `Password`, `HWID`, `FirstLogin`) VALUES ('{Users}', '{Usernametxt.Text}', '{Passwordtxt.Password}', '{hwidnumber}', '{date}')"; 
                    MySqlCommand cmdinsert = new MySqlCommand(queryinsert, SQLcon);
                    MySqlDataReader DataReader = cmdinsert.ExecuteReader();
                    while (DataReader.Read())
                    {

                    }
                    CustomMessageBox messagebox = new CustomMessageBox("Successfully created new account of ID:" + Users + "!");
                    messagebox.Show();
                    Currentuserstxt.Text = "Current Users: " + ++Users + "!";

                    SQLcon.Close();
                }
                catch (MySqlException ex)
                {
                    CustomMessageBox messagebox = new CustomMessageBox("Error: " + ex.Message);
                    messagebox.Show();
                }
                finally
                {
                    SQLcon.Dispose();
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (SignUpButton.Text == "Create an account!")
            {
                SignInUpTB.Text = "Sign Up";
                LoginRegisterBtn.Content = "REGISTER";
                SignUpButton.Text = "Have already an account?";
            }
            else
            {
                SignInUpTB.Text = "Sign In";
                LoginRegisterBtn.Content = "LOGIN";
                SignUpButton.Text = "Create an account!";
            }
        }

        private void Discord_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/Nb8pEpR");
        }

        static string FilePath = "C:\\Cheat\\RememberUser.txt";
        public void SaveRemember()
        {
            File.WriteAllText(FilePath, Usernametxt.Text + "," + Passwordtxt.Password + "," + RememberMeCheck.IsChecked);
        }

        public void LoadRemember()
        {
            if (!File.Exists(FilePath))
                return;

            try
            {
                string user = File.ReadAllText(FilePath);
                string[] SubStrings = user.Split(',');
                Usernametxt.Text = SubStrings[0];
                Passwordtxt.Password = SubStrings[1];
                RememberMeCheck.IsChecked = StringToBool(SubStrings[2]);
            }
            catch(Exception ex) { }
        }

        bool StringToBool(string BoolAsString)
        {
            return BoolAsString == "True";
        }

        private void RememberMeCheck_Checked(object sender, RoutedEventArgs e)
        {
            SaveRemember();
        }
    }
}
