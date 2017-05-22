using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamerTwitchViewer.V5
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
    }
}
