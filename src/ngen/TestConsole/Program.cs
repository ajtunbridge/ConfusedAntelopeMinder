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
                IPasswordProvider passwordProvider = new PBKDF2PasswordProvider();
                var pbkdf2Hash = passwordProvider.HashPassword("password");

                passwordProvider = new BCryptPasswordProvider();
                var bcryptHash = passwordProvider.HashPassword("password");

                Console.WriteLine($"Hash: {pbkdf2Hash}");
                Console.WriteLine($"Length: {pbkdf2Hash.Length} characters");
                Console.WriteLine();
                Console.WriteLine($"Hash: {bcryptHash}");
                Console.WriteLine($"Length: {bcryptHash.Length} characters");

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
