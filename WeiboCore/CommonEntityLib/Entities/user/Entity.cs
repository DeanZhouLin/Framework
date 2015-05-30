using Newtonsoft.Json;

namespace CommonEntityLib.Entities.user
{
    public class Entity : EntityBase
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        /// <summary>
        /// 用户UID(字符型)
        /// </summary>
        [JsonProperty(PropertyName = "idstr")]
        public string IDStr { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        [JsonProperty(PropertyName = "screen_name")]
        public string ScreenName { get; set; }
        /// <summary>
        /// 友好显示名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 用户所在地区ID
        /// </summary>
        [JsonProperty(PropertyName = "province")]
        public string Province { get; set; }
        /// <summary>
        /// 用户所在城市ID
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        /// <summary>
        /// 用户所在地
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
        /// <summary>
        /// 用户描述
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        /// <summary>
        /// 用户博客地址
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        /// <summary>
        /// 用户头像地址
        /// </summary>
        [JsonProperty(PropertyName = "profile_image_url")]
        public string ProfileImageUrl { get; set; }
        /// <summary>
        /// 用户微博主页地址
        /// </summary>
        [JsonProperty(PropertyName = "profile_url")]
        public string ProfileUrl { get; set; }
        /// <summary>
        /// 用户的个性化域名
        /// </summary>
        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }
        /// <summary>
        /// 用户的微号        /// </summary>
        [JsonProperty(PropertyName = "weihao")]
        public string Weihao { get; set; }
        /// <summary>
        /// 性别，m：男、f：女、n：未知        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        /// <summary>
        /// 粉丝数        /// </summary>
        [JsonProperty(PropertyName = "followers_count")]
        public int FollowersCount { get; set; }
        /// <summary>
        /// 关注数        /// </summary>
        [JsonProperty(PropertyName = "friends_count")]
        public int FriendsCount { get; set; }
        /// <summary>
        /// 微博数        /// </summary>
        [JsonProperty(PropertyName = "statuses_count")]
        public int StatusesCount { get; set; }
        /// <summary>
        /// 收藏数        /// </summary>
        [JsonProperty(PropertyName = "favourites_count")]
        public long FavouritesCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }
        /// <summary>
        /// 当前登录用户是否已关注该用户
        /// </summary>
        [JsonProperty(PropertyName = "following")]
        public bool Following { get; set; }
        /// <summary>
        /// 是否允许所有人给我发私信        /// </summary>
        [JsonProperty(PropertyName = "allow_all_act_msg")]
        public bool AllowAllActMsg { get; set; }
        /// <summary>
        /// 是否允许带有地理信息
        /// </summary>
        [JsonProperty(PropertyName = "geo_enabled")]
        public bool GEOEnabled { get; set; }
        /// <summary>
        /// 是否是微博认证用户，即带V用户
        /// </summary>
        [JsonProperty(PropertyName = "verified")]
        public bool Verified { get; set; }
        /// <summary>
        /// 微博认证用户的类型        /// </summary>
        [JsonProperty(PropertyName = "verified_type")]
        public int VerifiedType { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [JsonProperty(PropertyName = "remark")]
        public string Remark { get; set; }
        /// <summary>
        /// 用户的最近一条微博信息字段        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public status.Entity Status { get; set; }
        /// <summary>
        /// 是否允许所有人对我的微博进行评论        /// </summary>
        [JsonProperty(PropertyName = "allow_all_comment")]
        public bool AllowAllComment { get; set; }
        /// <summary>
        /// 用户大头像地址
        /// </summary>
        [JsonProperty(PropertyName = "avatar_large")]
        public string AvatarLarge { get; set; }
        /// <summary>
        /// 认证原因
        /// </summary>
        [JsonProperty(PropertyName = "verified_reason")]
        public string VerifiedReason { get; set; }
        /// <summary>
        /// 该用户是否关注当前登录用户        /// </summary>
        [JsonProperty(PropertyName = "follow_me")]
        public bool FollowMe { get; set; }
        /// <summary>
        /// 用户的在线状态，0：不在线、1：在线        /// </summary>
        [JsonProperty(PropertyName = "online_status")]
        public int OnlineStatus { get; set; }
        /// <summary>
        /// 用户的互粉数
        /// </summary>
        [JsonProperty(PropertyName = "bi_followers_count")]
        public int BIFollowersCount { get; set; }
        /// <summary>
        /// 用户使用语言类型
        /// </summary>
        [JsonProperty(PropertyName = "lang")]
        public string Lang { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[JsonProperty(PropertyName = "star")]
        //public int Star { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[JsonProperty(PropertyName = "mbtype")]
        //public int MbType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[JsonProperty(PropertyName = "mbrank")]
        //public int MbRank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[JsonProperty(PropertyName = "block_word")]
        //public int BlockWord { get; set; }
        /// <summary>
        /// 用户信用分数
        /// </summary>
        [JsonProperty(PropertyName = "credit_score")]
        public int CreditScore { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        [JsonProperty(PropertyName = "urank")]
        public int Urank { get; set; }
    }
}
