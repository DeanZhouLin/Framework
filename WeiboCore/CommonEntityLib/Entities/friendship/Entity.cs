using Newtonsoft.Json;

namespace CommonEntityLib.Entities.friendship
{
    public class Entity : EntityBase
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty("followed_by")]
        public bool FollowedBy { get; set; }
        [JsonProperty("following")]
        public bool Following { get; set; }
        [JsonProperty("notifications_enabled")]
        public bool NotificationsEnabled { get; set; }
    }
}
