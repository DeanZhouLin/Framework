using Newtonsoft.Json;

namespace CommonEntityLib.Entities.trend
{
    public class IsFollow : EntityBase
    {
        [JsonProperty("trend_id")]
        public string ID { get; set; }
        [JsonProperty("is_follow")]
        public bool Following { get; set; }
    }
}
