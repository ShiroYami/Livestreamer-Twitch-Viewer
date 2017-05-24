using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.V5.Models
{
    class UserFollows
    {
        #region Total
        [JsonProperty(PropertyName = "_total")]
        public int Total { get; protected set; }
        #endregion
        #region Follows
        [JsonProperty(PropertyName = "follows")]
        public UserFollow[] Follows { get; protected set; }
        #endregion
    }

    class UserFollow
    {
        #region CreatedAt
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; protected set; }
        #endregion
        #region Notifications
        [JsonProperty(PropertyName = "notifications")]
        public bool Notifications { get; protected set; }
        #endregion
        #region Channel
        [JsonProperty(PropertyName = "channel")]
        public Channel Channel { get; protected set; }
        #endregion
    }
}
