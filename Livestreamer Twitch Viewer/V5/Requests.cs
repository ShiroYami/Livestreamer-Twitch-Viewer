namespace LivestreamerTwitchViewer.V5
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json.Serialization;
    using System.Threading.Tasks;
    using System;
    using Exceptions;

    public class Requests
    {
        public enum API
        {
            V3 = 3,
            V5 = 5,
            Void = 0
        }

        private static string m_clientIdInternal; 
        private static string m_accessTokenInternal;
        public static string ClientId { get { return m_clientIdInternal; } set { m_clientIdInternal = value; } }
        public static string AccessToken { get { return m_accessTokenInternal; } set { m_accessTokenInternal = value; } }

        public static string FormatOAuth(string token)
        {
            if (token.Contains(" "))
                return token.Split(' ')[1];
            return token;
        }

        public async static Task<T> GetGeneric<T>(string url, string accessToken = null, API api = API.V5, string clientId = null)
        {
            return JsonConvert.DeserializeObject<T>(await generalRequest(url, "GET", null, accessToken, api, clientId), TwitchLibJsonDeserializer);
        }

        public async static Task<T> GetSimpleGeneric<T>(string url)
        {
            return JsonConvert.DeserializeObject<T>(await simpleRequest(url), TwitchLibJsonDeserializer);
        }

        private async static Task<string> generalRequest(string url, string method, object payload = null, string accessToken = null, API api = API.V5, string clientId = null)
        {
            if (ClientId == null) ClientId = Globals.ClientId;
            if (AccessToken == null) AccessToken = Globals.Authkey;
            url = appendClientId(url, ClientId);

            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.ContentType = "application/json";

            if (api != API.Void)
                request.Accept = $"application/vnd.twitchtv.v{(int)api}+json";

            if (!string.IsNullOrEmpty(accessToken))
                request.Headers["Authorization"] = $"OAuth {FormatOAuth(accessToken)}";
            else if (!string.IsNullOrEmpty(AccessToken))
                request.Headers["Authorization"] = $"OAuth {AccessToken}";

            try
            {
                using (var response2 = await request.GetResponseAsync())
                {
                    var response = request.GetResponse();
                    using (var reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string data = await reader.ReadToEndAsync();
                        return data;
                    }
                }
            }
            catch (WebException ex) { handleWebException(ex); }
            return null;
        }

        private static void handleWebException(WebException e)
        {
            HttpWebResponse errorResp = e.Response as HttpWebResponse;
            if (errorResp == null)
                throw e;
            switch (errorResp.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException("Your request failed because either: \n 1. Your ClientID was invalid/not set.\n 2. You requested a username when the server was expecting a user ID.");
                case HttpStatusCode.Unauthorized:
                    throw new BadScopeException("Your request was blocked due to bad credentials (do you have the right scope for your access token?).");
                case HttpStatusCode.NotFound:
                    throw new BadResourceException("The resource you tried to access was not valid.");
                case (HttpStatusCode)422:
                    throw new NotPartneredException("The resource you requested is only available to channels that have been partnered by Twitch.");
                default:
                    throw e;
            }
        }

        public async static Task<string> simpleRequest(string url)
        {
            var tcs = new TaskCompletionSource<string>();
            var client = new WebClient();

            DownloadStringCompletedEventHandler h = null;
            h = (sender, args) =>
            {
                if (args.Cancelled)
                    tcs.SetCanceled();
                else if (args.Error != null)
                    tcs.SetException(args.Error);
                else
                    tcs.SetResult(args.Result);

                client.DownloadStringCompleted -= h;
            };

            client.DownloadStringCompleted += h;
            client.DownloadString(new Uri(url));

            return await tcs.Task;
        }

        private static string appendClientId(string url, string clientId = null)
        {
            if (clientId == null)
                return url.Contains("?")
                    ? $"{url}&client_id={ClientId}"
                    : $"{url}?client_id={ClientId}";
            else
                return url.Contains("?")
                    ? $"{url}&client_id={clientId}"
                    : $"{url}?client_id={clientId}";
        }

        #region SerialiazationSettings
        public static JsonSerializerSettings TwitchLibJsonDeserializer = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public class TwitchLibJsonSerializer
        {
            private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            public static string SerializeObject(object o)
            {
                return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
            }

            public class LowercaseContractResolver : DefaultContractResolver
            {
                protected override string ResolvePropertyName(string propertyName)
                {
                    return propertyName.ToLower();
                }
            }
        }
        #endregion
    }
}
