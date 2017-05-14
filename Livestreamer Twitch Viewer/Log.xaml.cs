using LivestreamerTwitchViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using MSG = System.Windows.MessageBox;

namespace Livestreamer_Twitch_Viewer
{
    /// <summary>
    /// Logique d'interaction pour Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        private string m_authkey;

        public String Authkey { get { return m_authkey; } }

        public Log()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            m_authkey = AuthKeyBox.Password;
            this.DialogResult = true;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // Commande à exécuter
            process.StartInfo.Arguments = Globals.CreateAuthKeyLink;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt"))
            {
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "AuthKey.txt");
                MSG.Show("Key deleted.", "Try again!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MSG.Show("No key found.", "Try again!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
