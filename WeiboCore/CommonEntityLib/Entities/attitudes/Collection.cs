using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.attitudes
{
    /// <summary>
    /// 表态集合
    /// </summary>
    public class Collection : EntityBase
    {
        /// <summary>
        /// 表态内容集合
        /// </summary>
        [JsonProperty(PropertyName = "attitudes")]
        public IEnumerable<Entity> Attitudes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "hasvisible")]
        public bool Hasvisible { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "previous_cursor")]
        public long PreviousCursor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "next_cursor")]
        public long NextCursor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "total_number")]
        public int TotalNumber { get; set; }
    }
}
