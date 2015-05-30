using Newtonsoft.Json;

namespace CommonEntityLib.Entities.user
{
    public class Count : EntityBase
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("followers_count")]
        public string FollowerCount { get; set; }
        [JsonProperty("friends_count")]
        public string FriendCount { get; set; }
        [JsonProperty("statuses_count")]
        public string StatusCount { get; set; }
    }
}
