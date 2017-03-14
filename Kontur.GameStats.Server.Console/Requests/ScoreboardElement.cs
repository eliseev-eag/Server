using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Requests
{
    public class ScoreboardElement
    {
        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "frags")]
        public int Frags { get; set; }

        [Required]
        [JsonProperty(PropertyName = "kills")]
        public int Kills { get; set; }

        [Required]
        [JsonProperty(PropertyName = "deaths")]
        public int Deaths { get; set; }
    }
}