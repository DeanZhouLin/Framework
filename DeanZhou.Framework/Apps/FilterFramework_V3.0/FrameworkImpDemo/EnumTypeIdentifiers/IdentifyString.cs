namespace DeanZhou.Framework
{
    public class IdentifyString : IEnumTypeIdentifier<string, StringEnumType>
    {
        public StringEnumType IdentifyItemTypeAsEnumType(string item)
        {
            StringEnumType result = StringEnumType.NN;

            if (string.IsNullOrEmpty(item))
            {
                result |= StringEnumType.Empty;
                return result;
            }

            if (item.Length <= 5)
            {
                result |= StringEnumType.ShortLen;
            }

            if (item.Length > 5 && item.Length < 20)
            {
                result |= StringEnumType.MiddleLen;
            }

            if (item.Length > 20)
            {
                result |= StringEnumType.LongLen;
            }

            int t;
            if (int.TryParse(item, out t))
            {
                result |= StringEnumType.Number;
            }

            return result;
        }

    }
}
