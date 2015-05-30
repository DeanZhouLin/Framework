using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Url : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("result")]
        public bool Result { get; set; }
    }
}
