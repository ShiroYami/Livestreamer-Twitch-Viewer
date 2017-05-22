using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using MSG = System.Windows.MessageBox;
using TwitchCSharp.Clients;
using static System.String;
using TwitchCSharp.Models;
using LivestreamerTwitchViewer;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Livestreamer_Twitch_Viewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TryGetAuthKey();            
        }

        private bool CheckForValidAuthKey()
        {
            bool validLogin = false;
            while (!validLogin)
            {
                bool? result = false;
                Log win = new Log();
                result = win.ShowDialog();

                if ((result == null) || (result != true))
                {
                    return false;
                }

                if (Login(win.Authkey))
                {
                    validLogin = true;
                }
                else
                {
                    MSG.Show("You auth key is invalid", "Try again!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            return true;
        }

        private void TryGetAuthKey()
        {
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt"))
            {
                string readText = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt");
                Globals.Authkey = readText;
            }
        }

        public bool Login(string authkey)
        {
            TwitchAuthenticatedClient tempClient = null;
            try
            {
                 tempClient = new TwitchAuthenticatedClient(Globals.ClientId, authkey);
            }
            catch
            {
                return false;
            }
            User user = tempClient.GetMyUser();
            TwitchList<TwitchCSharp.Models.Stream> followed = tempClient.GetFollowedStreams();

            if (user == null || IsNullOrWhiteSpace(user.Name))
            {
                return false;
            }
            MSG.Show("Your username  " + user.Name, "Try again!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Globals.Client = tempClient;
            Globals.Status.Username = user.Name;
            Globals.Status.Displayname = user.DisplayName;
            Globals.Authkey = authkey;
            if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt"))
            {
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt", Globals.Authkey);
            }
            return true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CheckForValidAuthKey();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Scroll scroll = new Scroll();
            scroll.Show();
            this.Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string exeToRun = AppDomain.CurrentDomain.BaseDirectory + @"Resources\livestreamer_setup.exe";
            Process.Start(exeToRun);
        }
    }
}
