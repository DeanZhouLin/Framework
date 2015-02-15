namespace DeanZhou.Framework
{
    public class IdentifyDemoString : IEnumTypeIdentifier<string, DemoStringEnumType>
    {
        public DemoStringEnumType IdentifyItemTypeAsEnumType(string item)
        {
            DemoStringEnumType result = DemoStringEnumType.NN;

            if (item.Length > 2)
            {
                result |= DemoStringEnumType.LongLen;
            }

            int t;
            if (int.TryParse(item, out t))
            {
                result |= DemoStringEnumType.AllNumber;
            }

            return result;
        }

    }
}
