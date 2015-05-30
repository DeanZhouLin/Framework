using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.favorite
{
    public class Collection : EntityBase
    {
        [JsonProperty("favorites")]
        public IEnumerable<Entity> Favorites { get; set; }

        [JsonProperty("total_number")]
        public int TotalNumber { get; set; }
    }
}
