using System.Collections.Generic;

namespace CommonEntityLib.Entities.trend
{
    public class HotTrends : EntityBase
    {
        public Dictionary<string, List<Keyword>> Trends { get; set; }
        public string AsOf { get; set; }
    }
}
