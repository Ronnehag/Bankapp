using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public static class BankSearch
    {
        public static Account GetCustomerAccountById(List<Customer> customers, int input)
        {
            foreach (var customer in customers)
            {
                foreach (var account in customer.CustomerAccounts)
                {
                    if (account.Key == input)
                        return account.Value;
                }
            }
            Console.WriteLine("Inget konto hittades med angivet kontonummer.");
            Console.ReadLine();
            return null;
        }

        public static Customer GetCustomerByAccountOrCustomerId(List<Customer> customers, int input)
        {
            foreach (var customer in customers)
            {
                if (customer.CustomerId == input || customer.CustomerAccounts.ContainsKey(input))
                {
                    return customer;
                }
            }
            Console.WriteLine("Ingen kund hittades med det angivna kund- eller kontonumret.");
            Console.ReadLine();
            return null;
        }

        public static Customer GetCustomerById(List<Customer> customers, int input)
        {
            var customer = customers.FirstOrDefault(i => i.CustomerId == input);
            if (customer == null)
            {
                Console.WriteLine("Ingen kund med angivet kundnummer hittades.");
                Console.ReadLine();
            }
            return customer;

        }

        public static List<Customer> GetCustomersByNameOrCity(List<Customer> customers, string input)
        {
            var result = customers.Where(c => c.Name.ToLower().Contains(input.ToLower()) ||
                                              c.City.ToLower().Contains(input.ToLower()))
                                              .OrderBy(c => c.CustomerId).ToList();

            if (result.Any()) return result;
            Console.WriteLine("Ingen kund hittades, prova vara mer specifik med din sökning.");
            Console.ReadLine();
            return null;
        }

        public static List<Transaction> GetCustomerTransfers(List<Transaction> transactions)
        {
            return transactions.Where(i => i.TooAccountNumber != 0).OrderByDescending(i => i.Date).ToList();
        }

        public static List<Transaction> GetCustomerWithdrawsAndDeposits(List<Transaction> transactions)
        {
            return transactions.Where(i => i.TooAccountNumber == 0).OrderByDescending(i => i.Date).ToList();
        }

        public static List<Account> GetAllAccountsNegativeBalance(List<Customer> customers)
        {
            return (from customer in customers
                from account in customer.CustomerAccounts
                where account.Value.Balance < 0
                select account.Value).ToList();
        }

        public static List<Account> GetAllAccountsSavingsRatesPositiveBalance(List<Customer> customers)
        {
            return (from customer in customers
                from account in customer.CustomerAccounts
                where account.Value.SavingsRate != 0 && account.Value.Balance > 0
                select account.Value).ToList();

        }
    }
}
