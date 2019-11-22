using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UltrwaWare.xyz
{
    /// <summary>
    /// Interaction logic for LoggedMainWindow.xaml
    /// </summary>
    public partial class LoggedMainWindow : Window
    {
        MySqlConnection SQLcon;
        string connectionString = "SERVER=remotemysql.com;PORT=3306;DATABASE=585LcKMwog;UID=585LcKMwog;PASSWORD=W38lGldT7N"; //UID = username
        string Username; 
        int IDUser;
        string DLLPath = "C:\\Cheat\\a.dll";
        string DLLLink = "https://github.com/CRNYY/CRNYY-s-Cheat-Users/raw/master/Internal.dll";//"https://drive.google.com/uc?authuser=0&id=17L51qMMWQotWSKQ3uYNOm6oHa_JFy2vg&export=download"; //https://drive.google.com/file/d/17L51qMMWQotWSKQ3uYNOm6oHa_JFy2vg/view?usp=sharing
        public LoggedMainWindow(string username, int IdUser)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Username = username;
            IDUser = IdUser;
            Usernametxt.Text = Username + " ( ID: " + IDUser + ")";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox Injecting = new CustomMessageBox("Injecting..");
            Injecting.Show();
            WebClient Client = new WebClient();
            FileInfo fi = new FileInfo(DLLPath);
            try
            {
                //fi.Attributes = FileAttributes.Normal;
                File.Delete(DLLPath);
            }
            catch (Exception ex) { }
            Client.DownloadFile(DLLLink, DLLPath);
            DllInjectorResult res = Injector.GetInstance.Inject("csgo", DLLPath);
            fi.Attributes = FileAttributes.Hidden;
            Injecting.Close();
            CustomMessageBox Result = new CustomMessageBox(res.ToString());
            Result.Show();
        }

        private void Discord_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/Nb8pEpR");
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox Logout = new CustomMessageBox("Logging Out of this account...");
            Logout.Show();
            this.Close();
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void UploadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("C:\\Cheat\\" + ConfigBox.Text + ".ini"))
            {
                CustomMessageBox Doesntexist = new CustomMessageBox(ConfigBox.Text + " doesn't exist.");
                Doesntexist.Show();
                return;
            }
            SQLcon = new MySqlConnection();
            SQLcon.ConnectionString = connectionString;
            SQLcon.Open();
            string queryAlready = $"SELECT * FROM usersconfigs WHERE ConfigName = '{NameOfConfigtxt.Text}'";
            MySqlCommand cmdAlready = new MySqlCommand(queryAlready, SQLcon);
            MySqlDataReader DataReaderAlready = cmdAlready.ExecuteReader();
            int AlreadyExists = 0;
            while (DataReaderAlready.Read() && AlreadyExists == 0)
            {
                AlreadyExists++;
            }
            if(AlreadyExists > 0)
            {
                CustomMessageBox Alreadyexist = new CustomMessageBox(NameOfConfigtxt.Text + " already exists.");
                Alreadyexist.Show();
                SQLcon.Close();
                return;
            }
            DataReaderAlready.Close();

            string[] lines = File.ReadAllLines("C:\\Cheat\\" + ConfigBox.Text + ".ini");
            int i = 0;
            CustomMessageBox Uploading = new CustomMessageBox("Uploading " + NameOfConfigtxt.Text + ".");
            Uploading.Show();
            foreach (var line in lines)
            {
                string query = $"INSERT INTO usersconfigs (`Username`, `ConfigName`, `LineOfConfig`, `IndexOfLine`) VALUES ('{Username + i}', '{NameOfConfigtxt.Text}', '{line}', '{i}')";
                MySqlCommand cmd = new MySqlCommand(query, SQLcon);
                MySqlDataReader DataReader = cmd.ExecuteReader();
                while (DataReader.Read()) { }
                i++;
                DataReader.Close();
            }
            Uploading.Close();
            CustomMessageBox Success = new CustomMessageBox(NameOfConfigtxt.Text + " successfully uploaded.");
            Success.Show();
            SQLcon.Close();
        }

        private void DownloadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("C:\\Cheat\\" + NameOfConfigtxt.Text + ".ini"))
            {
                CustomMessageBox exist = new CustomMessageBox(NameOfConfigtxt.Text + " already exists.");
                exist.Show();
                return;
            }
            SQLcon = new MySqlConnection();
            SQLcon.ConnectionString = connectionString;
            SQLcon.Open();
            string queryAlready = $"SELECT * FROM usersconfigs WHERE ConfigName = '{NameOfConfigtxt.Text}'"; //This "*" is important
            MySqlCommand cmdAlready = new MySqlCommand(queryAlready, SQLcon);
            MySqlDataReader DataReaderAlready = cmdAlready.ExecuteReader();
            int AlreadyExists = 0;
            while (DataReaderAlready.Read() && AlreadyExists == 0)
            {
                AlreadyExists++;
            }
            if (AlreadyExists == 0)
            {
                CustomMessageBox Alreadyexist = new CustomMessageBox(NameOfConfigtxt.Text + " doesn't exist.");
                Alreadyexist.Show();
                SQLcon.Close();
                return;
            }
            DataReaderAlready.Close();
            List<string> lines = new List<string>();
            string query = $"SELECT * FROM usersconfigs WHERE ConfigName='{NameOfConfigtxt.Text}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLcon);
            MySqlDataReader DataReader = cmd.ExecuteReader();
            int line = 0;
            while (DataReader.Read())
            {
                lines.Add((string)DataReader["LineOfConfig"]);
                //lines[0] = ;
                //line++;
            }
            SQLcon.Close();
            File.WriteAllLines("C:\\Cheat\\" + NameOfConfigtxt.Text + ".ini", lines.ToArray());
            CustomMessageBox Success = new CustomMessageBox("Successfully downloaded " + NameOfConfigtxt.Text + " to the file.");
            Success.Show();
        }
    }
}
