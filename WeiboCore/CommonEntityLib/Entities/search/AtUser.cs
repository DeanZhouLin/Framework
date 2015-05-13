using Newtonsoft.Json;

namespace CommonEntityLib.Entities.search
{
    public class AtUser : EntityBase
    {
        [JsonProperty("nickname")]
        public string NickName { get; set; }
        [JsonProperty("remark")]
        public string Remark { get; set; }
        [JsonProperty("uid")]
        public string UID { get; set; }
    }
}
