using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Requests
{
    public class AdvertiseRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "gameModes")]
        public List<string> GameModes { get; set; }

        public AdvertiseRequest()
        {
            GameModes = new List<string>();
        }
    }
}