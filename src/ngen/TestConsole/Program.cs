using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ngen.Data;
using ngen.Data.Model;
using ngen.Domain.IO;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;

                var db = new ngenDbContext();

                var employee = db.Employees.First();

                var store = new LocalDocumentStore(db, employee);

                store.TransferProgress += Store_TransferProgress;

                var part = db.Parts.Single(p => p.DrawingNumber == "H17070A");

                var task = store.AddAsync("C:\\Users\\adam.tunbridge\\Documents\\HugeFuckingFile.stp", part);

                Task.WaitAll(task);
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

        private static void Store_TransferProgress(object sender, ngen.Domain.EventArgs.DocumentTransferEventArgs e)
        {
            ClearCurrentConsoleLine();

            switch (e.CurrentStatus)
            {
                    case DocumentTransferStatus.ComputingHash:
                    Console.Write($"{e.FileName}: Calculating hash {e.PercentComplete}%");
                    break;
                    case DocumentTransferStatus.Encrypting:
                    Console.Write($"{e.FileName}: Encrypting {e.PercentComplete}%");
                    break;
                    case DocumentTransferStatus.Complete:
                    Console.Write($"{e.FileName} has been stored successfully!");
                    break;
            }
        }

        private static void RecursivelyPrintGroups(WorkCentreGroup parent, int indent)
        {
            for (var i = 0; i < indent; i++)
                Console.Write("\t");

            Console.WriteLine(parent.Name);

            foreach (var child in parent.Children)
            {
                var newIndent = indent + 1;

                for (var i = 0; i < newIndent; i++)
                    Console.Write("\t");

                Console.WriteLine(child.Name);

                if (child.Children.Any())
                {
                    newIndent = newIndent + 1;

                    foreach (var subChild in child.Children)
                        RecursivelyPrintGroups(subChild, newIndent);
                }
            }
        }

        private static string GenerateRandomString(int length,
            string allowedChars = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than one.");

            if (string.IsNullOrEmpty(allowedChars))
                throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException(string.Format("allowedChars may contain no more than {0} characters.",
                    byteSize));
            
            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; (i < buf.Length) && (result.Length < length); ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - byteSize%allowedCharSet.Length;
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i]%allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}