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
    }
}
