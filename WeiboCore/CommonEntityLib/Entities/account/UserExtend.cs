using Newtonsoft.Json;

namespace CommonEntityLib.Entities.account
{
    /// <summary>
    /// 用户的扩展信息
    /// </summary>
    public class UserExtend : EntityBase
    {
        /// <summary>
        /// 用户血型信息
        /// </summary>
        [JsonProperty(PropertyName = "blood_type")]
        public string BloodType { get; set; }

        /// <summary>
        /// 用户血型信息的可见属性，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "blood_type_visible")]
        public int BloodTypeVisible { get; set; }

        /// <summary>
        /// 用户性取向信息 0：未设置，1：男，2：女，3：男和女；
        /// </summary>
        [JsonProperty(PropertyName = "sexual_orientation")]
        public string SexualOrientation { get; set; }

        /// <summary>
        /// 用户性取向信息的可见属性，0：自己可见、1：我关注人可见、2：所有人可见；
        /// </summary>
        [JsonProperty(PropertyName = "sexual_orientation_visible")]
        public int SexualOrientationVisible { get; set; }

        /// <summary>
        /// 用户婚恋信息 0：未设置，1：单身，2：求交往，3：暗恋中，4：暧昧中，5：恋爱中，6：订婚，7：已婚，8：分居，9：离异，10：丧偶；
        /// </summary>
        [JsonProperty(PropertyName = "single")]
        public string Single { get; set; }

        /// <summary>
        /// 用户婚恋信息的可见属性，0：自己可见、1：我关注人可见、2：所有人可见；
        /// </summary>
        [JsonProperty(PropertyName = "single_visible")]
        public int SingleVisible { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }
    }
}
