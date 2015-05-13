using Newtonsoft.Json;

namespace CommonEntityLib.Entities.trend
{
    public class Trend : EntityBase
    {
        [JsonProperty("trend_id")]
        public string ID { get; set; }
        [JsonProperty("hotword")]
        public string HotWord { get; set; }
        [JsonProperty("num")]
        public string Number { get; set; }
    }
}
