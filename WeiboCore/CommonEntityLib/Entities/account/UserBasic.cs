using Newtonsoft.Json;

namespace CommonEntityLib.Entities.account
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class UserBasic : EntityBase
    {
        /// <summary>
        /// 用户生日信息
        /// </summary>
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// 用户生日隐私类型，0：保密、1：只显示月日、2：只显示星座、3：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "birthday_visible")]
        public int BirthdayVisible { get; set; }

        /// <summary>
        /// 用户所在城市代码ID
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public int City { get; set; }

        /// <summary>
        /// 用户创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// 用户证件号码
        /// </summary>
        [JsonProperty(PropertyName = "credentials_num")]
        public string CredentialsNum { get; set; }

        /// <summary>
        /// 用户证件类型
        /// </summary>
        [JsonProperty(PropertyName = "credentials_type")]
        public int CredentialsType { get; set; }

        /// <summary>
        /// 用户描述信息
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// 用户的个性化域名
        /// </summary>
        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }

        /// <summary>
        /// 用户联系邮箱地址
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// 用户邮箱地址隐私类型，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "email_visible")]
        public int EmailVisible { get; set; }

        /// <summary>
        /// 用户性别，m：男、f：女、n：未知

        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 用户UID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        /// <summary>
        /// 用户使用语言类型
        /// </summary>
        [JsonProperty(PropertyName = "lang")]
        public string Lang { get; set; }

        /// <summary>
        /// 用户所在地
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// 用户MSN账号信息
        /// </summary>
        [JsonProperty(PropertyName = "msn")]
        public string MSN { get; set; }

        /// <summary>
        /// 用户MSN账号隐私类型，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "msn_visible")]
        public int MSNVisible { get; set; }

        /// <summary>
        /// 友好显示名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 用户头像地址
        /// </summary>
        [JsonProperty(PropertyName = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// 用户所在省份代码ID
        /// </summary>
        [JsonProperty(PropertyName = "province")]
        public int Province { get; set; }

        /// <summary>
        /// 用户QQ号码
        /// </summary>
        [JsonProperty(PropertyName = "qq")]
        public string QQ { get; set; }

        /// <summary>
        /// 用户QQ号码隐私类型，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "qq_visible")]
        public int QQVisible { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        [JsonProperty(PropertyName = "real_name")]
        public string RealName { get; set; }

        /// <summary>
        /// 用户真实姓名隐私类型，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "real_name_visible")]
        public int RealNameVisible { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [JsonProperty(PropertyName = "screen_name")]
        public string ScreenName { get; set; }

        /// <summary>
        /// 用户博客地址
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// 用户博客地址隐私类型，0：自己可见、1：我关注人可见、2：所有人可见
        /// </summary>
        [JsonProperty(PropertyName = "url_visible")]
        public int UrlVisible { get; set; }
    }
}
