using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.user
{
    public class IDCollection : EntityBase
    {
        [JsonProperty("ids")]
        public IEnumerable<string> Users { get; set; }
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }
        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
