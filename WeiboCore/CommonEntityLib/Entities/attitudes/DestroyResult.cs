using Newtonsoft.Json;

namespace CommonEntityLib.Entities.attitudes
{
    public class DestroyResult : EntityBase
    {
        /// <summary>
        /// 表态ID
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }
    }
}
