using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class RefererUrl : EntityBase
    {
        [JsonProperty("clicks")]
        public string Clicks { get; set; }

        [JsonProperty("referer")]
        public string Referer { get; set; }
    }
}
