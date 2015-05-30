using Newtonsoft.Json;

namespace CommonEntityLib.Entities.status
{
    public class PicUrl : EntityBase
    {
        [JsonProperty(PropertyName = "thumbnail_pic")]
        public string ThumbnailPictureUrl { get; set; }
    }
}