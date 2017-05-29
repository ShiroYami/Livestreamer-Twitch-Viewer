using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.V5.Models
{
    class SearchGames
    {
        #region Games
        [JsonProperty(PropertyName = "games")]
        public Game[] Games { get; protected set; }
        #endregion
    }
}
