using Livestreamer_Twitch_Viewer;
using LivestreamerTwitchViewer.Models;
using LivestreamerTwitchViewer.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TwitchCSharp.Clients;
using TwitchCSharp.Models;
using SimpleJSON;
using static System.String;
using MSG = System.Windows.MessageBox;
using TwitchCSharp.Helpers;
using System.Collections;
using System.Windows.Threading;
using System.Threading.Tasks;
using LivestreamerTwitchViewer.V5;
using System.Linq;

namespace LivestreamerTwitchViewer
{
    /// <summary>
    /// Logique d'interaction pour Scroll.xaml
    /// </summary>

    public partial class Scroll : Window
    {

        //public TwitchList<Stream> followed;
        private bool toClose = false;
        private int m_offset = 0;

        public Scroll()
        {
            InitializeComponent();
            InitWindow();
            Refresh();
            AddItemsToComboQuality();
            //Toto();
            //System.Windows.Interop.ComponentDispatcher.ThreadIdle += new EventHandler(Update2);
        }

        async void Toto()
        {
            await Globals.AClient.GetHostedStreams(m_offset);            
        }

        async void SetTotalFollowed()
        {
            Globals.TotalFollowed = await AuthenticatedClient.GetTotalFollowed();
        }

        async void Update(object sender, EventArgs e)
        {
            if (m_offset < 11)
            {
                await Globals.AClient.GetHostedStreams(m_offset);
                m_offset++;
            }
        }

        public void Update2(object sender, EventArgs e)
        {
            Console.WriteLine("COUNT  " + AuthenticatedClient.HostStreamsList.Count);
            if (AuthenticatedClient.HostStreamsList[AuthenticatedClient.HostStreamsList.Count - 1].StreamResult.Status != TaskStatus.WaitingForActivation)
            {
                Console.WriteLine("DONE  " + AuthenticatedClient.HostStreamsList.Count);
                AuthenticatedClient.stackMax = 0;

                AuthenticatedClient.t1 = DateTime.Now.TimeOfDay.TotalMilliseconds;
                Console.WriteLine("t1 : " + AuthenticatedClient.t1);
                AuthenticatedClient.delta = AuthenticatedClient.t1 - AuthenticatedClient.t0;
                Console.WriteLine("Delta Time : " + AuthenticatedClient.delta);

                System.Windows.Interop.ComponentDispatcher.ThreadIdle -= new EventHandler(Update2);
                RemoveStackElement(true);
                //TwitchList<Stream> followed = HostreamToStreamList(AuthenticatedClient.HostStreamsList);
                TwitchList<Stream> followed = new TwitchList<Stream>();
                followed.List = AuthenticatedClient.HostStreamsList.Select(hostStream => hostStream.Stream).ToList();
                //List<Book> books_2 = books_1.Select(book => new Book(book.title)).ToList();
                RefreshStreamPanel(followed, true);

            }
        }

        #region Quality
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

        private void Quality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.Quality = " " + e.AddedItems[0];
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
        #endregion

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
            Globals.UserId = user.Id;
            Globals.AClient = new AuthenticatedClient(this);
            SetTotalFollowed();
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

        #region Resizer
        private void update_size(object sender, SizeChangedEventArgs e)
        {
            SetPanelsSizes();
        }

        private void SetPanelsSizes()
        {
            fullPanel.Width = this.ActualWidth - (double)20;
            scrollStream.Width = this.ActualWidth - (double)280;
            streamPanel.Width = this.ActualWidth - (double)280;
            scrollStreamHost.Width = this.ActualWidth - (double)280;
            streamPanelHost.Width = this.ActualWidth - (double)280;
            TwitchChatBrowser.Height = this.ActualHeight - (double)350;
            double size = (this.ActualWidth - (double)335) / (double)4;
            panelRight1.Width = size;
            panelRight2.Width = size;
            panelRight3.Width = size;
            panelRight4.Width = size;
            panelHostRight1.Width = size;
            panelHostRight2.Width = size;
            panelHostRight3.Width = size;
            panelHostRight4.Width = size;
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
        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            TwitchList<Stream>  followed = Globals.Client.GetFollowedStreams();
            RemoveStackElement(false);
            RefreshStreamPanel(followed, false);
        }

        #region Refresh Elements
        private void RemoveStackElement(bool p_isHost)
        {
            if (p_isHost)
            {
                for (int i = panelHostRight1.Children.Count - 1; i >= 0; i--)
                {
                    panelHostRight1.Children.RemoveAt(i);
                }
                for (int i = panelHostRight2.Children.Count - 1; i >= 0; i--)
                {
                    panelHostRight2.Children.RemoveAt(i);
                }
                for (int i = panelHostRight3.Children.Count - 1; i >= 0; i--)
                {
                    panelHostRight3.Children.RemoveAt(i);
                }
                for (int i = panelHostRight4.Children.Count - 1; i >= 0; i--)
                {
                    panelHostRight4.Children.RemoveAt(i);
                }
            }
            else
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
        }

        private void RefreshStreamPanel(TwitchList<Stream> followed, bool p_isHost)
        {
            for (int i = 0; i < followed.List.Count; i++)
            {
                Stream stream = followed.List[i];
                if (stream != null)
                {
                    // Create images preview.
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    string url = stream.Preview.Large;
                    Uri uri = new Uri(url);
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bmp.UriSource = uri;
                    bmp.EndInit();
                    img.Source = bmp;

                    // Create Images cover.
                    System.Windows.Controls.Image img2 = new System.Windows.Controls.Image();
                    img2.HorizontalAlignment = HorizontalAlignment.Right;
                    img2.VerticalAlignment = VerticalAlignment.Bottom;
                    img2.Stretch = Stretch.None;
                    try
                    {
                        Game game = Globals.Client.SearchGames(stream.Game).List[0];
                        img2.ToolTip = game.Name;
                        Uri uri2 = new Uri(game.Box.Small);
                        BitmapImage bmp2 = new BitmapImage(uri2);
                        img2.Source = bmp2;
                    }
                    catch
                    {
                        img2.ToolTip = "I AM ERROR";
                        Uri uri2 = new Uri(stream.Preview.Small);
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
                    title.Text = stream.Channel.Status;
                    title.Height = 40;
                    title.FontSize = 16;
                    title.TextWrapping = TextWrapping.Wrap;
                    title.FontWeight = FontWeights.Bold;

                    // Create buttons.
                    Button myButton = new Button();
                    myButton.Content = stream.Channel.Name;
                    myButton.Click += new RoutedEventHandler(buttonG_Click);

                    // Add image and button in the right panel.
                    if (p_isHost)
                    {
                        switch (i % 4)
                        {
                            case 0:
                                panelHostRight1.Children.Add(grid);
                                panelHostRight1.Children.Add(title);
                                panelHostRight1.Children.Add(myButton);
                                break;
                            case 1:
                                panelHostRight2.Children.Add(grid);
                                panelHostRight2.Children.Add(title);
                                panelHostRight2.Children.Add(myButton);
                                break;
                            case 2:
                                panelHostRight3.Children.Add(grid);
                                panelHostRight3.Children.Add(title);
                                panelHostRight3.Children.Add(myButton);
                                break;
                            case 3:
                                panelHostRight4.Children.Add(grid);
                                panelHostRight4.Children.Add(title);
                                panelHostRight4.Children.Add(myButton);
                                break;
                        }
                    }
                    else
                    {
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
            }
        }
        #endregion

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

        #region Text Focus
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
        #endregion        

        private void loadChat_Click(object sender, RoutedEventArgs e)
        {
            TwitchChatBrowser.Navigate(String.Format(Globals.ChatPopupUrl, textBoxStreamChat.Text));
        }

        private TwitchList<Stream> HostreamToStreamList(List<HostStream> p_hostStreamList)
        {
            try
            {
                TwitchList<Stream> streamList = new TwitchList<Stream>();
                streamList.List = new List<Stream>();
                Console.WriteLine("Count A : " + p_hostStreamList.Count);
                foreach (HostStream hostStream in p_hostStreamList)
                {
                    Console.WriteLine("Count : " + p_hostStreamList.Count);
                    if (hostStream != null)
                    {
                        if (hostStream.Stream != null)
                        {
                            try
                            {
                                streamList.List.Add(hostStream.Stream);
                                //Console.WriteLine("game : " + hostStream.Stream.Channel.Game);
                            }
                            catch
                            {
                                Console.WriteLine("ERROR");
                            }
                        }
                        else { Console.WriteLine("Stream Null"); }
                    }
                    else { Console.WriteLine("HostStream Null"); }
                }
                return streamList;
            }
            catch
            {
                Console.WriteLine("LARGE ERROR");
                return null;
            }
        }

        private void loadHost_Click(object sender, RoutedEventArgs e)
        {
            //await Globals.AClient.GetHostedStreams(index);
            System.Windows.Interop.ComponentDispatcher.ThreadIdle += new EventHandler(NextPage);
        }

        private async void NextPage(object sender, EventArgs e)
        {
            if (m_offset <= Globals.TotalFollowed / AuthenticatedClient.PageSize)
            {
                m_offset++;
                await Globals.AClient.GetHostedStreams(m_offset);
            }
            else
            {
                System.Windows.Interop.ComponentDispatcher.ThreadIdle -= new EventHandler(NextPage);
            }
        }
        
    }
}
