﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.favorite
{
    public class IDCollection : EntityBase
    {
        [JsonProperty("favorites")]
        public IEnumerable<IDEntity> Favorites { get; set; }

        [JsonProperty("total_number")]
        public int TotalNumber { get; set; }
    }
}
