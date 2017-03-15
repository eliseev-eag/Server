using Kontur.GameStats.Server.Requests;
using Newtonsoft.Json;
using System;

namespace Kontur.GameStats.Server.Responces
{
    public class RecentMath
    {
        [JsonProperty(PropertyName = "server")]
        public string Server { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty(PropertyName = "results")]
        public MatchResultRequest Results { get; set; }
    }
}
