using Newtonsoft.Json;

namespace CommonEntityLib.Entities.account
{
    /// <summary>
    /// 用户的教育信息
    /// </summary>
    public class UserEducation : EntityBase
    {
        /// <summary>
        /// 学校所在地区代码
        /// </summary>
        [JsonProperty(PropertyName = "area")]
        public int Area { get; set; }

        /// <summary>
        /// 学校所在城市代码
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public int City { get; set; }

        /// <summary>
        /// 学校院系名称
        /// </summary>
        [JsonProperty(PropertyName = "department")]
        public string Department { get; set; }

        /// <summary>
        /// 学校院系ID
        /// </summary>
        [JsonProperty(PropertyName = "department_id")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// 是否认证
        /// </summary>
        [JsonProperty(PropertyName = "is_verified")]
        public string IsVerified { get; set; }

        /// <summary>
        /// 学校所在省份代码
        /// </summary>
        [JsonProperty(PropertyName = "province")]
        public int Province { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [JsonProperty(PropertyName = "school")]
        public string School { get; set; }

        /// <summary>
        /// 学校ID
        /// </summary>
        [JsonProperty(PropertyName = "school_id")]
        public int SchoolId { get; set; }

        /// <summary>
        /// 学校类型，1：大学、2：高中、3：中专技校、4：初中、5：小学
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        /// <summary>
        /// 信息的可见属性，0：自己可见、1：我关注人可见、2：所有人可见；
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public int Visible { get; set; }

        /// <summary>
        /// 用户入学年份
        /// </summary>
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }
    }
}
