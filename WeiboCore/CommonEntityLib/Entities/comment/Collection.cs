using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.comment
{
    public class Collection : EntityBase
    {
        /// <summary>
        /// 评论集合
        /// </summary>
        [JsonProperty(PropertyName = "comments")]
        public IEnumerable<Entity> Comments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "previous_cursor")]
        public string ProviousCursor { get; set; }


        [JsonProperty(PropertyName = "next_cursor")]
        public string NextCursor { get; set; }


        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }


        [JsonProperty(PropertyName = "hasvisible")]
        public bool HasVisible { get; set; }
    }
}
