using Newtonsoft.Json;

namespace CommonEntityLib.Entities
{
    public class PrivacyEntity : EntityBase
    {
        [JsonProperty("badge")]
        public int Badge { get; set; }
        [JsonProperty("comment")]
        public int Comment { get; set; }
        [JsonProperty("geo")]
        public int GEO { get; set; }
        [JsonProperty("message")]
        public int Message { get; set; }
        [JsonProperty("mobile")]
        public int Mobile { get; set; }
        [JsonProperty("realname")]
        public int RealName { get; set; }
    }
}
