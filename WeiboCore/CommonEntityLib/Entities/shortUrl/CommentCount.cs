using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class CommentCount : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("comment_counts")]
        public string CommentCounts { get; set; }
    }
}
