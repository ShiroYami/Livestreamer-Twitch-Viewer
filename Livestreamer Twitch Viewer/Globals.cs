using System;
using System.IO;
using TwitchCSharp.Clients;

namespace LivestreamerTwitchViewer
{
    class Globals
    {
        public const string CreateAuthKeyLink = "/C livestreamer --twitch-oauth-authenticate";
        public const string TwitchLink = " http://www.twitch.tv/";
        public const string ChatPopupUrl = "http://www.twitch.tv/{0}/chat?popout=";
        public const string ClientId = "sn06ntops2897ctlazbc1k7bf3u8s5x";
        public const string Livestreamer = "/C livestreamer ";
        public const string Authrequest = "--twitch-oauth-token ";

        public static readonly string AppdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string MyFolderPath = Path.Combine(AppdataPath, "SimpleTwitchHelper");
        public static readonly string ConfigFile = Path.Combine(MyFolderPath, "config.yml");
        public static readonly string CountdownFile = Path.Combine(MyFolderPath, "countdown.txt");
        public static readonly string LogFile = Path.Combine(MyFolderPath, "log.txt");

        public static TwitchAuthenticatedClient Client;
        public static string Quality = " source";
        public static string Authkey;// = "dwzej4ed3g1jrdibfpiwqwaod4ui1v";

        public static TwitchStatus Status = new TwitchStatus();
    }
}
