using System.Collections.Generic;

namespace DeanZhou.Framework
{

    public class DemoStringFilterCore : ComplexFilterCore<string, DemoStringEnumType>
    {
        private DemoStringFilterCore()
        {
        }
    }


    public class StringFilterCore : ComplexFilterCore<string, StringEnumType>
    {
        private StringFilterCore()
        {
        }

        public static StringFilterCore NewInstance()
        {
            StringFilterCore sfc = new StringFilterCore();

            //设置对应类型最多获取数量
            sfc.SetMinGetCount(StringEnumType.Number, 2).SetMinGetCount(StringEnumType.Empty, 1);
            sfc.SetMinGetCount(StringEnumType.LongLen | StringEnumType.MiddleLen, 3);

            //设置类型识别器
            sfc.RegistEnumTypeIdentifier(new IdentifyString());

            //设置过滤器
            sfc.AddFilter(new StringContentFilter(), new StringLenFilter());
            return sfc;
        }

        public static StringFilterCore NewInstanceByString()
        {
            StringFilterCore sfc = new StringFilterCore();

            //
            sfc.SetMinGetCount("Number", 2).SetMinGetCount("Empty", 1);
            sfc.SetMinGetCount("LongLen|MiddleLen", 3);

            //
            sfc.RegistEnumTypeIdentifier("DeanZhou.Framework", "DeanZhou.Framework.IdentifyString");

            //
            sfc.AddFilter("DeanZhou.Framework", "DeanZhou.Framework.StringContentFilter", "DeanZhou.Framework.StringLenFilter");

            return sfc;
        }


        public static StringFilterCore NewNormalInstance()
        {
            return new StringFilterCore();
        }

        public Dictionary<string, StringEnumType> Test()
        {
            var ls = new List<string> { "asdfsdf", "12345678", "fd23", "11", "12345", "sfdk2", "23", "3" };
            Dictionary<string, StringEnumType> res = NewInstance().GetFilteredResult(ls);
            return res;
        }

    }
}
