using Newtonsoft.Json;

namespace CommonEntityLib.Entities.search
{
    public class User : EntityBase
    {
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty("followers_count")]
        public int FollowersCount { get; set; }
        [JsonProperty("uid")]
        public string UID { get; set; }
    }
}
