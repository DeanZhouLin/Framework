namespace DeanZhou.Framework
{
    public class DemoStringContentFilter : IFilter<string>
    {
        public bool DoFilter(string item)
        {
            return string.IsNullOrEmpty(item) || item.Contains("a") || item.Contains("1");
        }
    }
}
