using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Referers : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("referers")]
        public IEnumerable<RefererUrl> RefererUrls { get; set; }
    }
}
