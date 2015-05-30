using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class ShareCounts : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("share_counts")]
        public string Counts { get; set; }
    }
}
