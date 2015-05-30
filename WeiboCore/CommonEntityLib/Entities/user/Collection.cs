using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.user
{
    public class Collection : EntityBase
    {
        [JsonProperty("users")]
        public IEnumerable<Entity> Users { get; set; }
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
