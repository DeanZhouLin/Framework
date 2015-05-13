using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    /// <summary>
    /// 
    /// </summary>
    public class Pic : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "pic_id")]
        public string PicId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "thumbnail_pic")]
        public string ThumbnailPic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "bmiddle_pic")]
        public string BmiddlePic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "original_pic")]
        public string OriginalPic { get; set; }
    }
}
