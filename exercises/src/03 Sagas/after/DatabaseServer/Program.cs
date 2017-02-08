using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (new RavenHost())
            {
                Console.Read();
            }
        }
    }
}
