using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Info : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("result")]
        public bool Result { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("annotations")]
        public object Annotations { get; set; }
        [JsonProperty("last_modified")]
        public string LastModified { get; set; }
    }
}
