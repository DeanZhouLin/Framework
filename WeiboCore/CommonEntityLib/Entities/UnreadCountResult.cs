using Newtonsoft.Json;

namespace CommonEntityLib.Entities
{
    public class UnreadCountResult : EntityBase
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("follower")]
        public string Follower { get; set; }
        [JsonProperty("cmt")]
        public string Comment { get; set; }
        [JsonProperty("dm")]
        public string DirectMessage { get; set; }
        [JsonProperty("mention_status")]
        public string MentionStatus { get; set; }
        [JsonProperty("mention_cmt")]
        public string MentionComment { get; set; }
        [JsonProperty("group")]
        public string Group { get; set; }
        [JsonProperty("private_group")]
        public string PrivateGroup { get; set; }
        [JsonProperty("notice")]
        public string Notice { get; set; }
        [JsonProperty("invite")]
        public string Invite { get; set; }
        [JsonProperty("badge")]
        public string Badge { get; set; }
        [JsonProperty("photo")]
        public string Photo { get; set; }

    }
}
