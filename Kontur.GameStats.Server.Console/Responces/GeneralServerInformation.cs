using Kontur.GameStats.Server.Requests;
using Newtonsoft.Json;
using System.Linq;

namespace Kontur.GameStats.Server.Responces
{
    public class GeneralServerInformation
    {
        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty(PropertyName = "info")]
        public AdvertiseRequest Info { get; set; }

        public override bool Equals(object obj)
        {
            var inform = obj as GeneralServerInformation;
            if (inform == null) return false;
            return Endpoint == inform.Endpoint && Info.Name == inform.Info.Name
                && Info.GameModes.OrderBy(i => i).SequenceEqual(inform.Info.GameModes.OrderBy(i => i));
        }
    }
}
