using Newtonsoft.Json;

namespace CommonEntityLib.Entities.search
{
    public class Status : EntityBase
    {
        [JsonProperty("suggestion")]
        public string Suggestion { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
