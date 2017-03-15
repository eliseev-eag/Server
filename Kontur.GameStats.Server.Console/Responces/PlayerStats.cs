using Newtonsoft.Json;
using System;

namespace Kontur.GameStats.Server.Responces
{
    public class PlayerStats
    {
        [JsonProperty(PropertyName = "totalMatchesPlayed")]
        public long TotalMatchesPlayed { get; set; }

        [JsonProperty(PropertyName = "totalMatchesWon")]
        public long TotalMatchesWon { get; set; }

        [JsonProperty(PropertyName = "favoriteServer")]
        public string FavoriteServer { get; set; }

        [JsonProperty(PropertyName = "uniqueServers")]
        public int UniqueServers { get; set; }

        [JsonProperty(PropertyName = "favoriteGameMode")]
        public string FavoriteGameMode { get; set; }

        [JsonProperty(PropertyName = "averageScoreboardPercent")]
        public double AverageScoreboardPercent { get; set; }

        [JsonProperty(PropertyName = "maximumMatchesPerDay")]
        public int maximumMatchesPerDay { get; set; }

        [JsonProperty(PropertyName = "averageMatchesPerDay")]
        public double AverageMatchesPerDay { get; set; }

        [JsonProperty(PropertyName = "lastMatchPlayed")]
        public DateTime LastMatchPlayed { get; set; }

        [JsonProperty(PropertyName = "killToDeathRatio")]
        public double KillToDeathRatio { get; set; }
        


    }
}
