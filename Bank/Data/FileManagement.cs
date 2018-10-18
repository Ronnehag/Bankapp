using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Bank.Data
{
    public static class FileManagement
    {
        public static List<Customer> CustomersList { get; } = new List<Customer>();
        private static readonly List<Account> Accounts = new List<Account>();
        private const string LoadPath = @"Data\bankdata.txt";

        public static void LoadDataFromTxt()
        {
            using (var reader = new StreamReader(LoadPath))
            {
                var totalCustomers = int.Parse(reader.ReadLine());
                for (int i = 0; i < totalCustomers; i++)
                {
                    var line = reader.ReadLine();
                    CustomersList.Add(Customer.ParseFromTxt(line));
                }

                var totalAccounts = int.Parse(reader.ReadLine());
                for (int i = 0; i < totalAccounts; i++)
                {
                    var line = reader.ReadLine();
                    Accounts.Add(Account.ParseFromTxt(line));
                }
            }

            InitilizeBankAccounts();
            var totalBalance = Accounts.Sum(i => i.Balance);
            Console.WriteLine($"Läser in fil från: {LoadPath}...Klart!");
            Console.WriteLine($"Antal kunder: {CustomersList.Count}");
            Console.WriteLine($"Antal Konton: {Accounts.Count}");
            Console.WriteLine($"Totalt Saldo: {totalBalance:C}");
        }

        private static void InitilizeBankAccounts()
        {
            //var query =
            //    (from customer in CustomersList
            //     join account in Accounts on customer.CustomerId equals account.CustomerId
            //         into accountGroup
            //     select new Customer
            //     {
            //         CustomerId = customer.CustomerId,
            //         Name = customer.Name,
            //         Address = customer.Address,
            //         City = customer.City,
            //         Country = customer.Country,
            //         OrganisationNumber = customer.OrganisationNumber,
            //         PhoneNumber = customer.PhoneNumber,
            //         Region = customer.Region,
            //         ZipCode = customer.ZipCode,
            //         Accounts = accountGroup.ToList()
            //     };

            foreach (var account in Accounts)
            {
                foreach (var customer in CustomersList)
                {
                    if (customer.CustomerId != account.CustomerId) continue;
                    customer.AddBankAccount(account, account.AccountNumber);
                }
            }
        }

        public static void SaveDataToTxt(List<Customer> customers)
        {
            var timeNow = DateTime.Now.ToString("yyyyMMdd-HHmm");
            var filePath = $@".\{timeNow}.txt";
            customers = customers.OrderBy(c => c.CustomerId).ThenBy(c => c.CustomerAccounts.Keys).ToList();
            var totalCustomers = customers.Count;
            var totalAccounts = customers.Sum(c => c.CustomerAccounts.Count);
            var totalBalance = customers.Sum(c => c.TotalBalance);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(totalCustomers);
                foreach (var customer in customers)
                {
                    writer.WriteLine($"{customer.CustomerId};{customer.OrganisationNumber};" +
                                     $"{customer.Name};{customer.Address};{customer.City};" +
                                     $"{customer.Region};{customer.ZipCode};" +
                                     $"{customer.Country};{customer.PhoneNumber}");
                }
                writer.WriteLine(totalAccounts);
                foreach (var customer in customers)
                {
                    foreach (var account in customer.CustomerAccounts)
                    {
                        writer.WriteLine($"{account.Key};{customer.CustomerId};" +
                                         $"{account.Value.Balance.ToString(CultureInfo.InvariantCulture)}");
                    }
                }
            }
            Console.WriteLine($"Sparar till {filePath}");
            Thread.Sleep(2000);
            Console.WriteLine("Påbörjar skrivning av data");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.Write(".");
            Thread.Sleep(200);
            Console.WriteLine("Filen är sparad.");
            Thread.Sleep(200);
            Console.WriteLine($"Antal kunder: {totalCustomers}");
            Console.WriteLine($"Antal konton: {totalAccounts}");
            Console.WriteLine($"Totalt saldo: {totalBalance:C}");
            Console.WriteLine("Tryck på valfri tangent för att avsluta programmet...");
            Console.ReadLine();
        }

        public static void SaveTransactionToTxt(Transaction transaction)
        {
            const string filePath = @".\transaktionslogg.txt";
            using (var writer = File.AppendText(filePath))
            {
                if (transaction.TooAccountNumber == 0) // Uttag & Insättningar
                {
                    writer.WriteLine($"{transaction.Date};" +
                                     $"{transaction.AccountNumber};;" +
                                     $"{transaction.Money.ToString((CultureInfo.InvariantCulture))};" +
                                     $"{transaction.Balance.ToString(CultureInfo.InvariantCulture)}");
                }
                else // Överföringar
                {
                    writer.WriteLine($"{transaction.Date};" +
                                     $"{transaction.AccountNumber};" +
                                     $"{transaction.TooAccountNumber};" +
                                     $"{transaction.Money.ToString((CultureInfo.InvariantCulture))};" +
                                     $"{transaction.Balance.ToString(CultureInfo.InvariantCulture)}");
                }
            }
        }
    }
}
