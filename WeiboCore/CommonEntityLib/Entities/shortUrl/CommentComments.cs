using System.Collections.Generic;
using CommonEntityLib.Entities.comment;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class CommentComments : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("share_comments")]
        public IEnumerable<Entity> Referers { get; set; }
    }
}
