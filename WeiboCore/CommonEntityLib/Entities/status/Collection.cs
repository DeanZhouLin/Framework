using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class Collection : EntityBase
    {
        [JsonProperty(PropertyName = "statuses")]
        public IEnumerable<Entity> Statuses { get; set; }
        [JsonProperty(PropertyName = "reposts")]
        public IEnumerable<Entity> Reposts { get; set; }
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
