using Newtonsoft.Json;

namespace CommonEntityLib.Entities.shortUrl
{
    public class Location : EntityBase
    {
        [JsonProperty("clicks")]
        public string Clicks { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("location")]
        public string Name { get; set; }
    }
}
