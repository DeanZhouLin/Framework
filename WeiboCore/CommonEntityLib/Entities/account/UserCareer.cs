using Newtonsoft.Json;

namespace CommonEntityLib.Entities.account
{
    /// <summary>
    /// 用户的职业信息
    /// </summary>
    public class UserCareer : EntityBase
    {
        /// <summary>
        /// 公司城市代码ID
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public int City { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }

        /// <summary>
        /// 公司部门名称
        /// </summary>
        [JsonProperty(PropertyName = "department")]
        public string Department { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        [JsonProperty(PropertyName = "end")]
        public int End { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// 公司省份代码ID
        /// </summary>
        [JsonProperty(PropertyName = "province")]
        public int Province { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        /// <summary>
        /// 信息的可见属性，0：自己可见、1：我关注人可见、2：所有人可见；

        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public int Visible { get; set; }
    }
}
