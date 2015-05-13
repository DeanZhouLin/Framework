using Newtonsoft.Json;

namespace CommonEntityLib.Entities.user
{
    /// <summary>
    /// 用户等级信息
    /// </summary>
    public class UserRank : EntityBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
    }
}
