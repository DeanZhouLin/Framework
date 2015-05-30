using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class Entity : EntityBase
    {
        /// <summary>
        /// 微博创建时间
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }
        /// <summary>
        /// 微博ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        /// <summary>
        /// 微博MID
        /// </summary>
        [JsonProperty(PropertyName = "mid")]
        public string MID { get; set; }
        /// <summary>
        /// 字符串型的微博ID
        /// </summary>
        [JsonProperty(PropertyName = "idstr")]
        public string IDStr { get; set; }
        /// <summary>
        /// 微博信息内容
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        /// <summary>
        /// 微博来源
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }
        /// <summary>
        /// 是否已收藏，true：是，false：否
        /// </summary>
        [JsonProperty(PropertyName = "favorited")]
        public bool Favorited { get; set; }
        /// <summary>
        /// 是否被截断，true：是，false：否
        /// </summary>
        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }
        /// <summary>
        /// 回复ID
        /// </summary>
        [JsonProperty(PropertyName = "in_reply_to_status_id")]
        public string InReplyToStuatusID { get; set; }
        /// <summary>
        /// 回复UID
        /// </summary>
        [JsonProperty(PropertyName = "in_reply_to_user_id")]
        public string InReplyToUserID { get; set; }
        /// <summary>
        /// 回复人昵称
        /// </summary>
        [JsonProperty(PropertyName = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }
        /// <summary>
        /// 缩略图片地址，没有时不返回此字段
        /// </summary>
        [JsonProperty(PropertyName = "thumbnail_pic")]
        public string ThumbnailPictureUrl { get; set; }
        /// <summary>
        /// 中等尺寸图片地址，没有时不返回此字段
        /// </summary>
        [JsonProperty(PropertyName = "bmiddle_pic")]
        public string MiddleSizePictureUrl { get; set; }
        /// <summary>
        /// 原始图片地址，没有时不返回此字段
        /// </summary>
        [JsonProperty(PropertyName = "original_pic")]
        public string OriginalPictureUrl { get; set; }
        /// <summary>
        /// 地理信息字段
        /// </summary>
        [JsonProperty(PropertyName = "geo")]
        public GeoEntity GEO { get; set; }
        /// <summary>
        /// 微博作者的用户信息字段
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public user.Entity User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "retweeted_status")]
        public Entity RetweetedStatus { get; set; }
        /// <summary>
        /// 转发数
        /// </summary>
        [JsonProperty(PropertyName = "reposts_count")]
        public int RepostsCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        [JsonProperty(PropertyName = "comments_count")]
        public int CommentsCount { get; set; }
        /// <summary>
        /// 表态数
        /// </summary>
        [JsonProperty(PropertyName = "attitudes_count")]
        public int AttitudesCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("annotations")]
        public object Annotations { get; set; }
        /// <summary>
        /// 暂未支持
        /// </summary>
        //[JsonProperty(PropertyName = "mlevel")]
        //public int Mlevel { get; set; }
        /// <summary>
        /// 微博的可见性及指定可见分组信息
        /// </summary>
        [JsonProperty(PropertyName = "visible")]
        public object Visible { get; set; }

        [JsonProperty(PropertyName = "pic_urls")]
        public IEnumerable<PicUrl> PicUrls { get; set; }

    }
}
