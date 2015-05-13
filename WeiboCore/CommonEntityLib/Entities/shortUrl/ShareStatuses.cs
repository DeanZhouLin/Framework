using System.Collections.Generic;
using CommonEntityLib.Entities.status;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class ShareStatuses : EntityBase
    {
        [JsonProperty("url_short")]
        public string ShortUrl { get; set; }
        [JsonProperty("url_long")]
        public string LongUrl { get; set; }
        [JsonProperty("share_statuses")]
        public IEnumerable<Entity> Referers { get; set; }
    }
}
