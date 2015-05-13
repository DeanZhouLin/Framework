using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities
{
    public class VerifyNickNameResult : EntityBase
    {
        [JsonProperty("is_legal")]
        public bool IsLegal { get; set; }

        [JsonProperty("suggest_nickname")]
        public IEnumerable<string> SuggestNickName { get; set; }
    }
}
