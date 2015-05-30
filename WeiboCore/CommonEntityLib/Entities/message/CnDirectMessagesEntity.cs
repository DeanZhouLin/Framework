using System;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.message
{
    /// <summary>
    /// 私信模型
    /// </summary>
    public class CnDirectMessagesEntity : EntityBase
    {
        /// <summary>
        /// 私信时间
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        /// <summary>
        /// 私信内容
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// 私信发送者Uid
        /// </summary>
        [JsonProperty("uid")]
        public string Uid { get; set; }
        /// <summary>
        /// 私信发送者昵称
        /// </summary>
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
    }
}
