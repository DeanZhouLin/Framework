using Newtonsoft.Json;

namespace CommonEntityLib.Entities.friendship
{
    public class Result : EntityBase
    {
        [JsonProperty("target")]
        public Entity Target { get; set; }
        [JsonProperty("source")]
        public Entity Source { get; set; }
    }
}
