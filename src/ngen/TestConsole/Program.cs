using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ngen.Core.Security;
using ngen.Data;
using ngen.Data.Blobs;
using ngen.Data.Model;
using ngen.Domain.IO;
using ngen.Domain.Security;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;

                SecureSettings.FileShareDirectory = @"\\YOGA2PRO\ngen_store";
                SecureSettings.CheckOutDirectory = @"C:\Users\ajtun\Documents\ngen\Checked Out";
                SecureSettings.EncryptionPassword = "THIS_IS_JUST_a-simple-password";

                CreateAdminAccount();
                CreateGuestAccount();
                CreateTestPart();

                using (var db = new ngenDbContext())
                {
                    var employee = db.Employees.Single(e => e.UserName == "guest");

                    var store = new LocalDocumentStore(db, employee);

                    store.TransferProgress += Store_TransferProgress;

                    //var part = db.Parts.Single(p => p.DrawingNumber == "ABC123");
                    //var task = store.AddAsync(@"C:\Users\ajtun\Documents\nans_step.png", part);

                    //var ver = db.DocumentVersions.First();
                    //var task = store.OpenTempAsync(ver);

                    var doc = db.Documents.First();
                    var task = store.CheckOutAsync(doc);

                    Task.WaitAll(task);
                }
            }
            catch (AggregateException aggEx)
            {
                var securityExceptions = aggEx.InnerExceptions.Where(x => x is SystemPermissionException).ToList();

                if (!securityExceptions.Any())
                {
                    Console.WriteLine("Something weird just happened!");
                }
                else
                {
                    foreach (var ex in securityExceptions)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }
                }
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

        private static void CreateAdminAccount()
        {
            using (var db = new ngenDbContext())
            {
                var emp = db.Employees.SingleOrDefault(e => e.UserName == "mr_robot");

                if (emp != null)
                    return;

                var role = new SystemRole
                {
                    Name = "BUILTIN_ADMIN",
                    Description = "Built in administrator account. Has complete control of the entire system!"
                };

                var perms = new SystemRolePermissions();
                perms.Grant(SystemPermission.Administrator);

                role.Permissions = perms.ToBytes();

                var person = new Person
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    DateOfBirth = DateTime.Today
                };

                var employee = new Employee
                {
                    UserName = "mr_robot",
                    Password = new BCryptPasswordProvider().HashPassword("correcthorsebatterystaple"),
                    IsActive = true,
                    SystemRole = role,
                    Person = person
                };

                db.SystemRoles.Add(role);
                db.People.Add(person);
                db.Employees.Add(employee);

                db.SaveChanges();

                Console.WriteLine();
                Console.WriteLine("Admin account created");
            }
        }

        private static void CreateGuestAccount()
        {
            using (var db = new ngenDbContext())
            {
                var emp = db.Employees.SingleOrDefault(e => e.UserName == "guest");

                if (emp != null)
                    return;

                var role = new SystemRole
                {
                    Name = "GUESTS",
                    Description = "Built in guest account for test purposes. Has no permission to do anything at all!"
                };

                var perms = new SystemRolePermissions();

                role.Permissions = perms.ToBytes();

                var person = new Person
                {
                    FirstName = "Guest",
                    LastName = "Account",
                    DateOfBirth = DateTime.Today
                };

                var employee = new Employee
                {
                    UserName = "guest",
                    Password = new BCryptPasswordProvider().HashPassword("password"),
                    IsActive = true,
                    SystemRole = role,
                    Person = person
                };

                db.SystemRoles.Add(role);
                db.People.Add(person);
                db.Employees.Add(employee);

                db.SaveChanges();

                Console.WriteLine();
                Console.WriteLine("Guest account created");
            }
        }

    private static void CreateTestPart()
        {
            using (var db = new ngenDbContext())
            {
                var part = db.Parts.SingleOrDefault(p => p.DrawingNumber == "ABC123");

                if (part != null)
                {
                    return;
                }

                var customer = new Customer
                {
                    FullName = "Test Customer Limited",
                    ShortName = "Test Customer"
                };

                part = new Part
                {
                    Customer = customer,
                    DrawingNumber = "ABC123",
                    Name = "Test Part"
                };

                var version = new PartVersion
                {
                    VersionNumber = "01",
                    Changes = "N/A",
                    Part = part
                };

                db.Customers.Add(customer);
                db.Parts.Add(part);
                db.PartVersions.Add(version);

                db.SaveChanges();

                Console.WriteLine();
                Console.WriteLine("Test part record created");
            }
        }
    }
}