using Newtonsoft.Json;

namespace CommonEntityLib.Entities.attitudes
{
    /// <summary>
    /// 表态实体
    /// </summary>
    public class Entity : EntityBase
    {
        /// <summary>
        /// 表态ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// 表态内容
        /// </summary>
        [JsonProperty(PropertyName = "attitude")]
        public string Attitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "last_attitude")]
        public string LastAttitude { get; set; }

        /// <summary>
        /// 表态来源
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        /// <summary>
        /// 被表态微博
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public status.Entity Status { get; set; }

        /// <summary>
        /// 表态用户
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public user.Entity User { get; set; }
    }
}
