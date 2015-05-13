using Newtonsoft.Json;

namespace CommonEntityLib.Entities.favorite
{
    public class TagEntity : EntityBase
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
