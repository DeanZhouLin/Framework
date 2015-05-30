using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.favorite
{
    public class IDEntity : EntityBase
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<TagEntity> Tags { get; set; }

        [JsonProperty("favorited_time")]
        public string FavoritedTime { get; set; }

    }
}
