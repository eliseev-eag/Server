using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Requests
{
    public class MatchResultRequest
    {
        [Required]
        [JsonProperty(PropertyName = "map")]
        public string Map { get; set; }

        [Required]
        [JsonProperty(PropertyName = "gameMode")]
        public string GameMode { get; set; }

        [Required]
        [JsonProperty(PropertyName = "fragLimit")]
        public int FragLimit { get; set; }

        [Required]
        [JsonProperty(PropertyName = "timeLimit")]
        public double TimeLimit { get; set; }

        [Required]
        [JsonProperty(PropertyName = "timeElapsed")]
        public double TimeElapsed { get; set; }

        [Required]
        [JsonProperty(PropertyName = "scoreboard")]
        public List<ScoreboardElement> Scoreboard { get; set; }

    }
}
