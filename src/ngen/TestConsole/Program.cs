using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ngen.Core.IO;
using ngen.Core.Security;
using ngen.Data;
using ngen.Data.Blobs;
using ngen.Data.Model;
using ngen.Domain.IO;
using ngen.Domain.Security;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Finished. Press enter to exit");

            Console.ReadLine();
        }
    }
}
