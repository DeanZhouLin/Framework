namespace DeanZhou.Framework
{
    public class StringLenFilter : IFilter<string>
    {
        public bool DoFilter(string item)
        {
            return string.IsNullOrEmpty(item) || item.Length < 25;
        }
    }
}
