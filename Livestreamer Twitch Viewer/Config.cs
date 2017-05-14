using System;

namespace LivestreamerTwitchViewer
{
    class Config
    {
        public string AuthKey { get; set; }
        public bool ShowAdControl { get; set; }
        public bool ShowAlert { get; set; }
        public bool PlaySound { get; set; }
        public string CountingMessage { get; set; }
        public string FinishedMessage { get; set; }
        public string StartupExecutables { get; set; }
        public string TwitterTemplate { get; set; }
    }
}
