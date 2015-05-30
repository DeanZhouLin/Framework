using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Locations : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("locations")]
        public IEnumerable<Location> Referers { get; set; }
    }
}
