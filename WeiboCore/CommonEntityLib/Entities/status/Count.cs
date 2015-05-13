using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class Count : EntityBase
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("comments")]
        public string Comments { get; set; }
        [JsonProperty("reposts")]
        public string Reposts { get; set; }
    }
}
