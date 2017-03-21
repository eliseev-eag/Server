using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kontur.GameStats.Server.Requests
{
    public class AdvertiseRequest
    {
        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "gameModes")]
        public List<string> GameModes { get; set; }

        public override bool Equals(object obj)
        {
            var inform = obj as AdvertiseRequest;
            if (inform == null) return false;
            return Name == inform.Name
                && GameModes.OrderBy(i => i).SequenceEqual(GameModes.OrderBy(i => i));
        }
    }
}