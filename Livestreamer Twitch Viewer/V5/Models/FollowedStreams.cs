using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.V5.Models
{
    class FollowedStreams
    {
        #region Total
        /// <summary>Property representing the followed Streams count.</summary>
        [JsonProperty(PropertyName = "_total")]
        public int Total { get; protected set; }
        #endregion
        #region Streams
        /// <summary>Property representing the followed Streams.</summary>
        [JsonProperty(PropertyName = "streams")]
        public Stream[] Streams { get; protected set; }
        #endregion
    }
}
