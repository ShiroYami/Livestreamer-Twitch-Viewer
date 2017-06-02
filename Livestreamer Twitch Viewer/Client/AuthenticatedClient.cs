using LivestreamerTwitchViewer.Models;
using LivestreamerTwitchViewer.V5.Models;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Client
{
    class AuthenticatedClient
    {
        private static List<HostStream> m_hostStreamsList;
        private Scroll m_scroll;
        private static int m_pageZize = 100;
        private static int m_totalRequestCompleted;

        public static List<HostStream> HostStreamsList { get { return m_hostStreamsList; } }
        public Scroll Scroll { get { return m_scroll; } }
        public static int PageSize { get { return m_pageZize; } }
        public static int TotalRequestCompleted { get { return m_totalRequestCompleted; }set { m_totalRequestCompleted = value; } }

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
            if (host.Hosts.TargetLogin != null && host.Hosts.TargetLogin != String.Empty)
            {
                StreamResult str = await TwitchClient.GetStreamAsyncV5(host.Hosts.TargetId.ToString());
                HostStream hs = new HostStream(host, str.Stream);
                if (hs != null && hs.Stream != null)
                {
                    m_hostStreamsList.Add(hs);
                }
            }
            m_totalRequestCompleted++;
            if (m_totalRequestCompleted == Globals.TotalFollowed)
            {
                m_totalRequestCompleted = 0;
                Scroll.AllRequestDone = true;
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
