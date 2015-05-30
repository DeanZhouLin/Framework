using Newtonsoft.Json;

namespace CommonEntityLib.Entities.search
{
    public class School : EntityBase
    {
        [JsonProperty("school_name")]
        public string Name { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
