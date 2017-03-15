using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Responces
{
    public class PopularServers
    {
        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "averageMatchesPerDay")]
        public double AverageMatchesPerDay { get; set; }
    }
}
