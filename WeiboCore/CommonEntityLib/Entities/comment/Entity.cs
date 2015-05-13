using Newtonsoft.Json;

namespace CommonEntityLib.Entities.comment
{
    public class Entity : EntityBase
    {
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "user")]
        public user.Entity User { get; set; }

        [JsonProperty(PropertyName = "mid")]
        public string MID { get; set; }

        [JsonProperty(PropertyName = "status")]
        public status.Entity Status { get; set; }

        [JsonProperty(PropertyName = "reply_comment")]
        public Entity ReplyComment { get; set; }
    }
}
