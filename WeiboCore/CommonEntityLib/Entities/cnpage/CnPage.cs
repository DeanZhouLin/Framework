using System.Collections.Generic;
using CommonEntityLib.Entities.status;
using Newtonsoft.Json;

namespace CommonEntityLib.Entities.cnpage
{
    public class CnPage
    {
        [JsonProperty(PropertyName = "cards")]
        public IEnumerable<Card> Cards { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "ok")]
        public int Ok { get; set; }
    }

    public class Card
    {
        [JsonProperty(PropertyName = "card_group")]
        public IEnumerable<CardGroup> CardGroups { get; set; }
    }

    public class CardGroup
    {
        [JsonProperty(PropertyName = "mblog")]
        public Entity Mblog { get; set; }

        [JsonProperty(PropertyName = "user")]
        public user.Entity User { get; set; }
    }
}
