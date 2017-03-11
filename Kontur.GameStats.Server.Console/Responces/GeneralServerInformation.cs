using Kontur.GameStats.Server.Requests;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Responces
{
    public class GeneralServerInformation
    {

        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty(PropertyName = "info")]
        public AdvertiseRequest Info { get; set; }
    }
}
