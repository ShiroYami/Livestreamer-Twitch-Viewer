using Livestreamer_Twitch_Viewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TwitchCSharp.Clients;
using TwitchCSharp.Models;
using static System.String;
using MSG = System.Windows.MessageBox;


namespace LivestreamerTwitchViewer
{
    /// <summary>
    /// Logique d'interaction pour Scroll.xaml
    /// </summary>
    public partial class Scroll : Window
    {

        public TwitchList<Stream> followed;
        private bool toClose = false;

        public Scroll()
        {
            InitializeComponent();
            InitWindow();
            Refresh();
            AddItemsToComboQuality();
        }

        private void AddItemsToComboQuality()
        {
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Qualities.txt"))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Qualities.txt");
                while (!file.EndOfStream)
                {
                    Quality.Items.Add(file.ReadLine());
                }
                Quality.SelectedItem = Quality.Items[0];
            }
        }
        
        private void InitWindow()
        {
            //setup webbrowser
            Helper.SetSilent(TwitchChatBrowser, true);
            TwitchChatBrowser.Visibility = Visibility.Hidden;
            TwitchChatBrowser.Navigated += Navigated;
        }

        private void Navigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            TwitchChatBrowser.Visibility = Visibility.Visible;
        }

        private bool Refresh()
        {
            TwitchAuthenticatedClient tempClient = null;
            try
            {
                tempClient = new TwitchAuthenticatedClient(Globals.ClientId, Globals.Authkey);
            }
            catch
            {
                MSG.Show("You auth key is invalid", "Try again!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                toClose = true;
                return false;
            }
            User user = tempClient.GetMyUser();
            if (user == null || IsNullOrWhiteSpace(user.Name))
            {
                return false;
            }
            MSG.Show("Connected as " + user.Name, "Success", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Globals.Client = tempClient;
            Globals.Status.Username = user.Name;
            Globals.Status.Displayname = user.DisplayName;
            Globals.Quality = " source";
            return true;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (toClose)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
        }

        private void update_size(object sender, SizeChangedEventArgs e)
        {
            SetPanelsSizes();
        }

        private void SetPanelsSizes()
        {
            double size = (this.ActualWidth - (double)335) / (double)4;
            scrollStream.Width = this.ActualWidth - (double)280;
            streamPanel.Width = this.ActualWidth - (double)280;
            fullPanel.Width = this.ActualWidth - (double)20;
            panelRight1.Width = size;
            panelRight2.Width = size;
            panelRight3.Width = size;
            panelRight4.Width = size;
            TwitchChatBrowser.Height = this.ActualHeight - (double)350;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            followed = Globals.Client.GetFollowedStreams();
            //remove elements for refresh
            removeStackElement();
            for (int i = 0; i < followed.List.Count; i++)
            {
                // Create images preview.
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();                
                Uri uri = new Uri(followed.List[i].Preview.Large);
                BitmapImage bmp = new BitmapImage(uri);
                img.Source = bmp;

                // Create Images cover.
                System.Windows.Controls.Image img2 = new System.Windows.Controls.Image();
                img2.HorizontalAlignment = HorizontalAlignment.Right;
                img2.VerticalAlignment = VerticalAlignment.Bottom;
                img2.Stretch = Stretch.None;
                Console.WriteLine("Game : " + followed.List[i].Game);
                try
                {
                    Game game = Globals.Client.SearchGames(followed.List[i].Game).List[0];
                    img2.ToolTip = game.Name;
                    Uri uri2 = new Uri(game.Box.Small);
                    BitmapImage bmp2 = new BitmapImage(uri2);
                    img2.Source = bmp2;
                }
                catch
                {
                    img2.ToolTip = "I AM ERROR";
                    Uri uri2 = new Uri(followed.List[i].Preview.Small);
                    BitmapImage bmp2 = new BitmapImage(uri2);
                    img2.Source = bmp2;
                }                

                // Create grid.
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 0);
                grid.Children.Add(img);
                grid.Children.Add(img2);

                // Create Textblock
                TextBlock title = new TextBlock();
                title.Text = followed.List[i].Channel.Status;
                title.Height = 40;
                title.FontSize = 16;
                title.TextWrapping = TextWrapping.Wrap;
                title.FontWeight = FontWeights.Bold;

                // Create buttons.
                Button myButton = new Button();
                myButton.Content = followed.List[i].Channel.Name;
                myButton.Click += new RoutedEventHandler(buttonG_Click);

                // Add image and button in the right panel.
                switch (i % 4)
                {
                    case 0:
                        panelRight1.Children.Add(grid);
                        panelRight1.Children.Add(title);
                        panelRight1.Children.Add(myButton);
                        break;
                    case 1:
                        panelRight2.Children.Add(grid);
                        panelRight2.Children.Add(title);
                        panelRight2.Children.Add(myButton);
                        break;
                    case 2:
                        panelRight3.Children.Add(grid);
                        panelRight3.Children.Add(title);
                        panelRight3.Children.Add(myButton);
                        break;
                    case 3:
                        panelRight4.Children.Add(grid);
                        panelRight4.Children.Add(title);
                        panelRight4.Children.Add(myButton);
                        break;
                }
            }
        }

        private void removeStackElement()
        {
            for (int i = panelRight1.Children.Count - 1; i >= 0; i--)
            {
                panelRight1.Children.RemoveAt(i);
            }
            for (int i = panelRight2.Children.Count - 1; i >= 0; i--)
            {
                panelRight2.Children.RemoveAt(i);
            }
            for (int i = panelRight3.Children.Count - 1; i >= 0; i--)
            {
                panelRight3.Children.RemoveAt(i);
            }
            for (int i = panelRight4.Children.Count - 1; i >= 0; i--)
            {
                panelRight4.Children.RemoveAt(i);
            }
        }

        private void buttonG_Click(object sender, RoutedEventArgs e)
        {
            String name = (sender as Button).Content.ToString();
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // Commande à exécuter
            process.StartInfo.Arguments = Globals.Livestreamer + Globals.Authrequest + Globals.Authkey + Globals.TwitchLink + name + Globals.Quality;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            String name = textBoxStream.Text;
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // Commande à exécuter
            process.StartInfo.Arguments = Globals.Livestreamer + Globals.Authrequest + Globals.Authkey + Globals.TwitchLink + name + Globals.Quality;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                tb.Text = "Enter Channel Name:";
            }
        }

        public void TextBox_LostFocusChat(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                tb.Text = "Chat Room Name:";
            }
        }

        private void Quality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.Quality = " " + e.AddedItems[0];
        }

        private void loadChat_Click(object sender, RoutedEventArgs e)
        {
            TwitchChatBrowser.Navigate(String.Format(Globals.ChatPopupUrl, textBoxStreamChat.Text));
        }

        private void Resize(object sender, RoutedEventArgs e)
        {
            if (TwitchChatBrowser.Width == 240)
            {
                PanelChat.Width = 500;
                TwitchChatBrowser.Width = 490;
            }
            else
            {
                PanelChat.Width = 250;
                TwitchChatBrowser.Width = 240;
            }
        }

        private void qualityAdder_Click(object sender, RoutedEventArgs e)
        {
            Quality win = new Quality();
            bool? res = win.ShowDialog();
            if ((res != null) || (res == true))
            {
                if (win.Result != null && win.Result != "")
                {
                    if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Qualities.txt"))
                    {
                        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Qualities.txt", win.Result + Environment.NewLine);
                    }
                    else
                    {
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Qualities.txt", win.Result + Environment.NewLine);
                    }
                    Quality.Items.Add(win.Result);
                }
            }
        }
    }
}
