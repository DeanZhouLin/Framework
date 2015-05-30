using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class Emotion : EntityBase
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("common")]
        public bool Common { get; set; }
        [JsonProperty("hot")]
        public bool Hot { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("phrase")]
        public string Phrase { get; set; }
        [JsonProperty("picid")]
        public string PictureID { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
