using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class ServerInfo
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "gameModes")]
        [NotMapped]
        public List<string> GameModes { get; set; }
        [JsonIgnore]
        public ICollection<GameMode> GameModeCollection { get; set; }
    }
}