using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSIS_Status
{
    class Program
    {
        static void Main(string[] args)
        {
            new ssis().get();
            Console.ReadLine();
        }
    }
}
