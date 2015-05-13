using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities
{
    public class GeoEntity : EntityBase
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public IEnumerable<float> Coordinates { get; set; }
    }
}
