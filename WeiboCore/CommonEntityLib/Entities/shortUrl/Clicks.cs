using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Clicks : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("clicks")]
        public string ClickCounts { get; set; }
    }
}
