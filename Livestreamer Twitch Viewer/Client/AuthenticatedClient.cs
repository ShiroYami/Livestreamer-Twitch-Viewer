using LivestreamerTwitchViewer.Models;
using LivestreamerTwitchViewer.V5;
using RestSharp;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Clients;
using TwitchCSharp.Helpers;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Client
{
    class AuthenticatedClient
    {
        private static List<HostStream> m_hostStreamsList;
        private Scroll m_scroll;
        private int m_totalRequestCompleted;
        private static int m_pageZize = 100;
        public static int stack = 0;
        public static int stackMax = 0;

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

        private TwitchList<FollowedChannel> GetFollowed(int p_offset)
        {
            PagingInfo page = new PagingInfo();
            page.PageSize = PageSize;
            page.Page = p_offset;
            TwitchList<FollowedChannel> li = Globals.Client.GetFollowedChannels(Globals.Client.GetMyUser().Name, page);
            return li;
        }

        private async Task GetHost(double userId)
        {
            Uri uri = new Uri(Globals.HostURL + userId);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadStringCompleted);
            await Task.Yield();
            client.DownloadStringAsync(uri);
        }

        private async void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {            
            string json = e.Result;
            Host host = new Host(JSON.Parse(json));
            if (t0 == 0)
            {
                t0 = DateTime.Now.TimeOfDay.TotalMilliseconds;
                Console.WriteLine("t0 : " + t0);
            }
            if (host.Hosts.TargetLogin != null && host.Hosts.TargetLogin != String.Empty)
            {
                StreamResult str = await GetStreamAsyncV5(host.Hosts.TargetId.ToString());
                HostStream hs = new HostStream(host, str.Stream);
                if (hs != null && hs.Stream != null)
                {
                    m_hostStreamsList.Add(hs);
                    Console.WriteLine("stack  " + stack);
                }
            }
            m_totalRequestCompleted++;
            if (m_totalRequestCompleted == Globals.TotalFollowed)
            {
                m_totalRequestCompleted = 0;
                stackMax = stack;
                Console.WriteLine("stackMax : " + stackMax);
                System.Windows.Interop.ComponentDispatcher.ThreadIdle += new EventHandler(Scroll.Update2);
            }
        }

        public async Task GetHostedStreams(int p_offset)
        {
            TwitchList<FollowedChannel> followedList = GetFollowed(p_offset);
            foreach(FollowedChannel followedChannel in followedList.List)
            {
                double uid = followedChannel.Channel.Id;
                //Console.WriteLine(followedChannel.Channel.Name + "  " + followedChannel.Channel.Id);
                await GetHost(uid);
            }
        }

        public async Task<StreamResult> GetStreamAsync(string channel)
        {
            var request = GetRequest("streams/{channel}", Method.GET);
            request.AddUrlSegment("channel", channel);
            var response = await Globals.Client.restClient.ExecuteTaskAsync<StreamResult>(request);
            return response.Data;            
        }

        public async Task<StreamResult> GetStreamAsyncV5(string channelId, string streamType = "all")
        {
            string optionalQuery = $"?stream_type={streamType}";
            return await Requests.GetGeneric<StreamResult>($"https://api.twitch.tv/kraken/streams/{channelId}", null, Requests.API.V5);
        }

        private RestRequest GetRequest(string url, Method method)
        {
            return new RestRequest(url, method);
        }

        public async static Task<int> GetTotalFollowed()
        {
            UserFollows follows = await GetUserFollowsAsync();
            return follows.Total;
        }

        public async static Task<UserFollows> GetUserFollowsAsync()
        {
            string userId = Globals.UserId.ToString();
            Requests.ClientId = Globals.ClientId;
            return await Requests.GetGeneric<UserFollows>($"https://api.twitch.tv/kraken/users/{userId}/follows/channels", null, Requests.API.V5);
        }

    }
}
