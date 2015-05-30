using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class IDs : EntityBase
    {
        [JsonProperty("statuses")]
        public IEnumerable<string> Statuses { get; set; }
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
