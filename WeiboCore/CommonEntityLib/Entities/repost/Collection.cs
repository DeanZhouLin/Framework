using System.Collections.Generic;
using CommonEntityLib.Entities.status;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.repost
{
    public class Collection : EntityBase
    {
        [JsonProperty(PropertyName = "reposts")]
        public IEnumerable<Entity> Statuses { get; set; }
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
