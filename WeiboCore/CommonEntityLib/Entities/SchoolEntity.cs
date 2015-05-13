using Newtonsoft.Json;

namespace CommonEntityLib.Entities
{
    public class SchoolEntity : EntityBase
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
