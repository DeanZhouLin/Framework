using Newtonsoft.Json;

namespace CommonEntityLib.Entities.search
{
    public class App : EntityBase
    {
        [JsonProperty("apps_name")]
        public string Name { get; set; }
        [JsonProperty("members_count")]
        public int Count { get; set; }
    }
}
