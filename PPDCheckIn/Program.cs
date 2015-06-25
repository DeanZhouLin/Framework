using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeanZhou.Framework.Apps;

namespace PPDCheckIn
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginAndCheckIn.Sync();
            Console.ReadLine();
        }
    }
}
