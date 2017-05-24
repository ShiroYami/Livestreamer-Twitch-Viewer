using LivestreamerTwitchViewer.Models;
using LivestreamerTwitchViewer.V5;
using LivestreamerTwitchViewer.V5.Models;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TwitchCSharp.Clients;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Client
{
    class AuthenticatedClient
    {
        private static List<HostStream> m_hostStreamsList;
        private Scroll m_scroll;
        private static int m_totalRequestCompleted;
        private static int m_pageZize = 100;

        public static int stack = 0;
        public static double t0 = 0;
        public static double t1;
        public static double delta;

        public static List<HostStream> HostStreamsList { get { return m_hostStreamsList; } }
        public Scroll Scroll { get { return m_scroll; } }
        public static int PageSize { get { return m_pageZize; } }

        public AuthenticatedClient (Scroll scroll)
        {
            m_hostStreamsList = new List<HostStream>();
            m_scroll = scroll;
            m_totalRequestCompleted = 0;
        }

        public async Task GetHostedStreams(int p_offset)
        {
            UserFollows userFollows = await GetFollowedAsync(p_offset);
            foreach (UserFollow userFollow in userFollows.Follows)
            {
                double uid = userFollow.Channel.Id;
                //Console.WriteLine(userFollow.Channel.Name + "  " + userFollow.Channel.Id);
                GetHost(uid);
            }
        }

        private async Task<UserFollows> GetFollowedAsync(int p_offset)
        {
            UserFollows uf = await TwitchClient.GetUserFollowsAsyncV5(PageSize, (p_offset - 1) * PageSize);
            return uf;
        }

        private void GetHost(double userId)
        {
            Uri uri = new Uri(Globals.HostURL + userId);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadStringCompleted);
            client.DownloadStringAsync(uri);
        }

        private async void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {            
            string json = e.Result;
            Host host = new Host(JSON.Parse(json));
            if (t0 == 0)
            {
                t0 = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
            if (host.Hosts.TargetLogin != null && host.Hosts.TargetLogin != String.Empty)
            {
                StreamResult str = await TwitchClient.GetStreamAsyncV5(host.Hosts.TargetId.ToString());
                HostStream hs = new HostStream(host, str.Stream);
                if (hs != null && hs.Stream != null)
                {
                    m_hostStreamsList.Add(hs);
                    Console.WriteLine("stack  " + stack);
                }
            }
            m_totalRequestCompleted++;
            //Console.WriteLine("totalRequestCompleted : " + m_totalRequestCompleted);
            if (m_totalRequestCompleted == Globals.TotalFollowed)
            {
                m_totalRequestCompleted = 0;
                Console.WriteLine("stackMax : " + stack);
                Scroll.HostRefresh();
            }
        }

        public async static Task<int> GetTotalFollowed()
        {
            UserFollows follows = await TwitchClient.GetUserFollowsAsyncV5();
            return follows.Total;
        }

        public static void ResetHostStreamList()
        {
            m_hostStreamsList = new List<HostStream>(); 
        }

    }
}
