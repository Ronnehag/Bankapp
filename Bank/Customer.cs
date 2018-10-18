using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public class Customer
    {
        public Dictionary<int, Account> CustomerAccounts { get; } = new Dictionary<int, Account>();
        public int CustomerId { get; private set; }
        public bool DeleteableCustomer => TotalBalance == 0;
        public string OrganisationNumber { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string Region { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }
        public string PhoneNumber { get; private set; }
        public decimal TotalBalance
        {
            get
            {
                return CustomerAccounts.Sum(i => i.Value.Balance);
            }
        }

        public Customer()
        { }

        public Customer(int customerId, string organisationNr,
            string name, string address, string city, string region, string zipCode, string country, string phoneNr)
        {
            CustomerId = customerId;
            OrganisationNumber = organisationNr;
            Name = name;
            Address = address;
            City = city;
            Region = region;
            ZipCode = zipCode;
            Country = country;
            PhoneNumber = phoneNr;
        }

        public void AddBankAccount(Account account, int id) => CustomerAccounts.Add(id, account);

        public void DeleteBankAccount(int accountNumber)
        {
            if (!CustomerAccounts.ContainsKey(accountNumber))
            {
                Console.WriteLine("Kunden har inget konto med angivet kontonummer.");
                return;
            }
            if (GetBalanceFromAccount(accountNumber) == 0m)
            {
                Console.WriteLine($"Kontot med kontonummer #{accountNumber} är borttaget från kund {CustomerId}.");
                CustomerAccounts.Remove(accountNumber);
            }
            else
            {
                Console.WriteLine("För att ta bort ett konto måste saldot vara 0.");
            }
        }

        public void CustomerShortDetails()
        {
            Console.WriteLine($"{CustomerId}: {Name}");
        }

        public void CustomerAccountsDetails()
        {
            Console.WriteLine("Kontonummer".PadRight(15) + "Saldo".PadRight(15) + "Sparränta".PadRight(15) +
                              "Kreditgräns".PadRight(15) + "Skuldränta");
            foreach (var account in CustomerAccounts)
            {
                Console.WriteLine($"#{account.Key}".PadRight(15) +
                                  $"{account.Value.Balance:C}".PadRight(15) +
                                  $"{account.Value.SavingsRate}%".PadRight(15) +
                                  $"{account.Value.AddedCreditLimit:C}".PadRight(15) +
                                  $"{account.Value.InterestPayable}%");
            }
        }

        public void CustomerFullDetails()
        {
            Console.WriteLine("Kundnummer: #" + CustomerId);
            Console.WriteLine("Organisationsnummer: " + OrganisationNumber);
            Console.WriteLine($"Namn: {Name}");
            Console.WriteLine($"Adress: {Address}");
            Console.WriteLine($"{ZipCode} {City.ToUpper()}");
            Console.WriteLine($"Region: {Region}");
            Console.WriteLine($"Telefonnummer: {PhoneNumber}");
            Console.WriteLine("----------------------------------------------------------------------");
            CustomerAccountsDetails();
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("Totalt saldo:".PadRight(15) + $"{TotalBalance:C}");
        }

        public decimal GetBalanceFromAccount(int accountNumber)
        {
            return CustomerAccounts.FirstOrDefault(a => a.Key == accountNumber).Value.Balance;
        }

        public static Customer ParseFromTxt(string line)
        {
            var columns = line.Split(';');
            return new Customer
            {
                CustomerId = Int32.Parse(columns[0]),
                OrganisationNumber = columns[1],
                Name = columns[2],
                Address = columns[3],
                City = columns[4],
                Region = columns[5],
                ZipCode = columns[6],
                Country = columns[7],
                PhoneNumber = columns[8]
            };
        }

        public static int GenerateCustomerId(List<Customer> customers)
        {
            var result = 1000;
            while (customers.Any(id => id.CustomerId == result))
            {
                result++;
            }

            return result;
        }
    }
}
