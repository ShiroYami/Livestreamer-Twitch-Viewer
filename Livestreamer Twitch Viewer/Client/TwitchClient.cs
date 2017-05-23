using LivestreamerTwitchViewer.V5;
using LivestreamerTwitchViewer.V5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Client
{
    class TwitchClient
    {

        public async static Task<StreamResult> GetStreamAsyncV5(string channelId, string streamType = "all")
        {
            string optionalQuery = $"?stream_type={streamType}";
            return await Requests.GetGeneric<StreamResult>($"https://api.twitch.tv/kraken/streams/{channelId}", null, Requests.API.V5);
        }

        public async static Task<UserFollows> GetUserFollowsAsyncV5(int? limit = null, int? offset = null)
        {
            string userId = Globals.UserId.ToString();
            Requests.ClientId = Globals.ClientId;
            List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
            if (limit != null)
                queryParameters.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            if (offset != null)
                queryParameters.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
            string optionalQuery = string.Empty;
            if (queryParameters.Count > 0)
            {
                for (int i = 0; i < queryParameters.Count; i++)
                {
                    if (i == 0) { optionalQuery = $"?{queryParameters[i].Key}={queryParameters[i].Value}"; }
                    else { optionalQuery += $"&{queryParameters[i].Key}={queryParameters[i].Value}"; }
                }
            }
            return await Requests.GetGeneric<UserFollows>($"https://api.twitch.tv/kraken/users/{userId}/follows/channels{optionalQuery}", null, Requests.API.V5);
        }

        public async static Task<FollowedStreams> GetFollowedStreamsAsyncV5(int? limit = null, int? offset = null)
        {
            Requests.AccessToken = Globals.Authkey;
            List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
            if (limit != null)
                queryParameters.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            if (offset != null)
                queryParameters.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
            string optionalQuery = string.Empty;
            if (queryParameters.Count > 0)
            {
                for (int i = 0; i < queryParameters.Count; i++)
                {
                    if (i == 0) { optionalQuery = $"?{queryParameters[i].Key}={queryParameters[i].Value}"; }
                    else { optionalQuery += $"&{queryParameters[i].Key}={queryParameters[i].Value}"; }
                }
            }
            return await Requests.GetGeneric<FollowedStreams>($"https://api.twitch.tv/kraken/streams/followed{optionalQuery}", null, Requests.API.V5);
        }

        public async static Task<SearchGames> SearchGamesAsyncV5(string encodedSearchQuery, bool? live = null)
        {
            string optionalQuery = (live != null) ? $"?live={live}" : string.Empty;
            return await Requests.GetGeneric<SearchGames>($"https://api.twitch.tv/kraken/search/games?query={encodedSearchQuery}{optionalQuery}", null, Requests.API.V5);
        }

    }
}
