using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Responces
{
    public class ServerStats
    {
        [JsonProperty(PropertyName = "totalMatchesPlayed")]
        public long TotalMatchesPlayed { get; set; }

        [JsonProperty(PropertyName = "maximumMatchesPerDay")]
        public int MaximumMatchesPerDay { get; set; }

        [JsonProperty(PropertyName = "averageMatchesPerDay")]
        public double AverageMatchesPerDay { get; set; }

        [JsonProperty(PropertyName = "maximumPopulation")]
        public int MaximumPopulation { get; set; }

        [JsonProperty(PropertyName = "averagePopulation")]
        public double AveragePopulation { get; set; }

        [JsonProperty(PropertyName = "top5GameModes")]
        public IEnumerable<string> Top5GameModes { get; set; }

        [JsonProperty(PropertyName = "top5Maps")]
        public IEnumerable<string> Top5Maps { get; set; }
    }
}
