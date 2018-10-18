using System;
using System.Collections.Generic;
using Bank.Data;

namespace Bank
{
    public class Bank
    {
        private readonly List<Customer> _customers;

        public Bank()
        {
            FileManagement.LoadDataFromTxt();
            _customers = FileManagement.CustomersList;
            Console.ReadKey();
        }

        public void ShutDownBank()
        {
            Console.Clear();
            FileManagement.SaveDataToTxt(_customers);
        }

        public void SearchCustomer()
        {
            Console.Clear();
            Console.WriteLine("* Sök kund *");
            Console.Write("Namn eller postort? ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return;
            var customerList = BankSearch.GetCustomersByNameOrCity(_customers, input);
            if (customerList == null) return;
            foreach (var customer in customerList)
            {
                customer.CustomerShortDetails();
            }

            Console.ReadLine();
        }

        public void CustomerProfile()
        {
            Console.Clear();
            Console.WriteLine("* Visa kundbild *");
            var input = Validation.InputInt("Kundnummer eller kontonummer? ");
            var customer = BankSearch.GetCustomerByAccountOrCustomerId(_customers, input);
            if (customer == null) return;
            Console.Clear();
            customer.CustomerFullDetails();
            Console.ReadLine();
        }

        public void NewCustomer()
        {
            Console.Clear();
            Console.WriteLine("* Skapa ny kund *");
            Console.Write("Organisations nummer? ");
            var orgnr = Console.ReadLine();
            Console.Write("Namn? ");
            var name = Console.ReadLine();
            Console.Write("Adress? ");
            var address = Console.ReadLine();
            Console.Write("Postnummer? ");
            var zipcode = Console.ReadLine();
            Console.Write("Postort? ");
            var city = Console.ReadLine();
            Console.Write("Region? ");
            var region = Console.ReadLine();
            Console.Write("Land? ");
            var country = Console.ReadLine();
            Console.Write("Telefonnummer? ");
            var phonenr = Console.ReadLine();
            if (Validation.ValidCustomer(name, orgnr, address, zipcode, city, country, region, phonenr))
            {
                AddCustomer(orgnr, name, address, zipcode, city, region, country, phonenr);
            }
            else
            {
                Console.WriteLine("För att skapa en kund måste namn, organisationsnummer, adress, postnummer och postort anges.");
                Console.WriteLine("En kund kan heller inte ha ett namn, stad eller land som innehåller siffror.");
            }
            Console.ReadLine();
        }

        private void AddCustomer(string orgnr, string name, string address, string zipcode, string city, string region,
            string country, string phonenr)
        {
            var customerId = Customer.GenerateCustomerId(_customers);
            var accountNumber = Account.GenerateAccountNumber(_customers);
            var customer = new Customer(customerId, orgnr, name, address, city, region, zipcode, country, phonenr);
            customer.AddBankAccount(new Account(accountNumber, customerId), accountNumber);
            _customers.Add(customer);
            Console.WriteLine($"Kund {name} med kundnummer {customerId} har skapats.");
            Console.WriteLine($"Kunden har fått ett konto med nummer #{accountNumber}.");
        }

        public void RemoveCustomer()
        {
            Console.Clear();
            Console.WriteLine("* Ta bort kund *");
            var input = Validation.InputInt("Kundnummer? ");
            var customer = BankSearch.GetCustomerById(_customers, input);
            if (customer == null) return;
            Console.WriteLine($"\n{customer.CustomerId}: {customer.Name}");
            Console.WriteLine("Är du säker på att du vill ta bort denna kund? j/n\n");
            var userInput = Console.ReadKey();
            switch (userInput.Key)
            {
                case ConsoleKey.J:
                    if (customer.DeleteableCustomer)
                    {
                        _customers.Remove(customer);
                        Console.WriteLine($"{customer.Name} är borttagen.");
                    }
                    else
                        Console.WriteLine($"\nDu kan inte ta bort kunden {customer.Name} då det finns pengar kvar på ett eller flera konton.");
                    break;
                case ConsoleKey.N:
                    return;
            }

            Console.ReadLine();
        }

        public void CreateNewAccount()
        {
            Console.Clear();
            Console.WriteLine("* Skapa konto *");
            var input = Validation.InputInt("Kundnummer? ");
            var customer = BankSearch.GetCustomerById(_customers, input);
            if (customer == null) return;
            customer.CustomerShortDetails();
            var accountId = Account.GenerateAccountNumber(_customers);
            Console.WriteLine();
            Console.WriteLine("Vill du lägga till ett nytt konto för denna kund? j/n");
            var newAccount = true;
            do
            {
                var select = Console.ReadKey();
                switch (select.Key)
                {
                    case ConsoleKey.J:
                        customer.AddBankAccount(new Account(accountId, customer.CustomerId), accountId);
                        Console.WriteLine();
                        Console.WriteLine("Ett nytt konto har skapats till kund " +
                                          $"{customer.CustomerId} med kontonummer #{accountId}");
                        newAccount = false;
                        break;
                    case ConsoleKey.N:
                        newAccount = false;
                        break;
                }
            } while (newAccount);
            Console.ReadLine();
        }

        public void RemoveAccountFromCustomer()
        {
            Console.Clear();
            Console.WriteLine("* Ta bort konto *");
            var input = Validation.InputInt("Kundnummer eller kontonummer? ");
            var customer = BankSearch.GetCustomerByAccountOrCustomerId(_customers, input);
            if (customer == null) return;
            Console.WriteLine();
            Console.WriteLine($"Konton för kund #{customer.CustomerId} : {customer.Name}");
            Console.WriteLine("-------------------------------------------------------------");
            customer.CustomerAccountsDetails();
            Console.WriteLine();
            var accountNumber = Validation.InputInt("Vilket konto vill du ta bort? ");
            customer.DeleteBankAccount(accountNumber);
            Console.ReadLine();
        }

        public void Deposit()
        {
            Console.Clear();
            Console.WriteLine("* Insättning *");
            var input = Validation.InputInt("Kontonummer? ");
            var customerAccount = BankSearch.GetCustomerAccountById(_customers, input);
            if (customerAccount == null) return;
            var value = Validation.InputDecimal("Hur mycket pengar vill du sätta in? ");
            try
            {
                customerAccount.AccountDeposit(value);
            }
            catch (AmountNegativeOrZeroException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        public void Withdraw()
        {
            Console.Clear();
            Console.WriteLine("* Uttag *");
            var input = Validation.InputInt("Kontonummer? ");
            var customerAccount = BankSearch.GetCustomerAccountById(_customers, input);
            if (customerAccount == null) return;
            customerAccount.AccountInformation();
            var value = Validation.InputDecimal("Vilket belopp vill du ta ut? ");
            try
            {
                customerAccount.AccountWithdraw(value);
            }
            catch (AmountNegativeOrZeroException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InsufficientFundsException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        public void Transfer()
        {
            Console.Clear();
            Console.WriteLine("* Överföring *");
            var accountNumberOne = Validation.InputInt("Från kontonummer: ");
            var accountFrom = BankSearch.GetCustomerAccountById(_customers, accountNumberOne);
            if (accountFrom == null) return;
            var accountNumberTwo = Validation.InputInt("Till kontonummer: ");
            var accountToo = BankSearch.GetCustomerAccountById(_customers, accountNumberTwo);
            if (accountToo == null) return;
            var amount = Validation.InputDecimal("Belopp att överföra? ");
            try
            {
                accountFrom.TransferMoney(accountToo, amount);
            }
            catch (AmountNegativeOrZeroException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InsufficientFundsException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        public void AccountProfile()
        {
            Console.Clear();
            Console.WriteLine("* Kontobild *");
            var input = Validation.InputInt("Kontonummer? ");
            var customerAccount = BankSearch.GetCustomerAccountById(_customers, input);
            if (customerAccount == null) return;
            customerAccount.PrintAccountTransactions();
            Console.ReadLine();
        }

        public void SavingsRate()
        {
            Console.Clear();
            Console.WriteLine("* Sparränta *");
            var input = Validation.InputInt("Kontonummer? ");
            var customerAccount = BankSearch.GetCustomerAccountById(_customers, input);
            if (customerAccount == null) return;
            Console.WriteLine("1) Lägg till/ändra sparränta ");
            Console.WriteLine("2) Ta bort sparränta ");
            var select = Console.ReadKey();
            Console.WriteLine();
            try
            {
                switch (select.Key)
                {
                    case ConsoleKey.D1:
                        var rate = Validation.InputDecimal("Ange sparränta(%): ");
                        customerAccount.AddSavingsRate(rate);
                        Console.WriteLine($"Sparränta {rate}% är adderad till kontot.");
                        break;
                    case ConsoleKey.D2:
                        customerAccount.RemoveSavingsRate();
                        Console.WriteLine($"Sparräntan för konto #{input} är borttagen.");
                        break;
                }
            }
            catch (AmountNegativeOrZeroException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        public void CreditLimit()
        {
            Console.Clear();
            Console.WriteLine("* Kreditgräns *");
            var input = Validation.InputInt("Kontonummer? ");
            var customerAccount = BankSearch.GetCustomerAccountById(_customers, input);
            if (customerAccount == null) return;
            Console.WriteLine("1) Lägg till/ändra kreditgräns");
            Console.WriteLine("2) Ta bort kreditgräns");
            var select = Console.ReadKey();
            Console.WriteLine();
            try
            {
                switch (select.Key)
                {
                    case ConsoleKey.D1:
                        var creditLimit = Validation.InputDecimal("Ange kreditgräns(kr): ");
                        customerAccount.AddCreditLimit(creditLimit);
                        var rate = Validation.InputDecimal("Ange skuldränta(%): ");
                        customerAccount.AddInterestPayable(rate);
                        Console.WriteLine($"En kreditgräns på {creditLimit:C} är adderad till kontot.");
                        Console.WriteLine($"Skuldräntan är satt till {rate}%");
                        break;
                    case ConsoleKey.D2:
                        customerAccount.RemoveCreditLimit();
                        break;
                }
            }
            catch (AmountNegativeOrZeroException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        public void CalculateAllRates()
        {
            Console.Clear();
            var toAddInterestPayable = BankSearch.GetAllAccountsNegativeBalance(_customers);
            foreach (var account in toAddInterestPayable)
            {
                account.AddDailyInterestPayable();
            }

            var toAddSavingsRates = BankSearch.GetAllAccountsSavingsRatesPositiveBalance(_customers);
            foreach (var account in toAddSavingsRates)
            {
                account.AddDailySavingsRate();
            }
            Console.WriteLine("Dagens sparränta och skuldränta är beräknad och tillagd på aktuella konton.");
            Console.WriteLine("...Transaktionerna är överförda och sparade till txt-fil.");
            Console.ReadLine();
        }
    }
}
