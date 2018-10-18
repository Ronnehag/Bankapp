using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bank
{
    public class Account
    {
        public const string AmountNegativeOrZeroMessage = "Du kan inte överföra en summa som är lägre eller lika med noll.";
        public const string TransferCantBeNegativeMessage = "Uttaget/överföringen kan inte vara ett negativt tal.";
        public const string InsufficientFundsMessage = "Ditt konto saknar täckning för uttaget/överföringen.";
        public const string IncorrectCreditLimitMessage = "Du kan inte ange en kreditgräns som är negativ eller noll.";
        public const string BalanceIsNegativeMessage = "Du kan inte ta bort kreditgränsen då kontot har ett negativt saldo.";
        public const string SavingsRateIsNegativeMessage = "Sparräntan som anges måste vara ett positivt tal.";
        public const string InterestPayableIsNegativeMessage = "Skuldräntan som anges måste vara ett positivt tal.";

        private const decimal DefaultBalance = 0m;
        private const decimal DefaultSavingsRate = 0m;
        private const decimal DefaultCreditLimit = 0m;
        private const decimal DefaultInterestPayable = 0m;
        private const decimal DaysAYear = 365m; // För att räkna ut dagsräntan, kan ändra till 360.
        private decimal _creditLimit;
        private decimal _balance;
        private decimal _addedCreditLimit;

        public List<Transaction> Transactions { get; } = new List<Transaction>();
        public int AccountNumber { get; private set; }
        public int CustomerId { get; private set; }
        public decimal AddedCreditLimit
        {
            get => _addedCreditLimit;
            private set => _addedCreditLimit = Math.Round(value, 2);
        }
        public decimal InterestPayable { get; private set; }
        public decimal SavingsRate { get; private set; }
        public decimal Balance
        {
            get => _balance;
            private set => _balance = Math.Round(value, 2);
        }
        public decimal CreditLimit
        {
            get
            {
                if (Balance >= 0 && AddedCreditLimit != 0)
                {
                    _creditLimit = AddedCreditLimit;
                }
                return _creditLimit;
            }
            private set => _creditLimit = Math.Round(value, 2);
        }

        public Account() {}

        public Account(int accountNumber, int customerId, decimal balance)
        {
            Balance = balance;
            CustomerId = customerId;
            AccountNumber = accountNumber;
            SavingsRate = DefaultSavingsRate;
            CreditLimit = DefaultCreditLimit;
            InterestPayable = DefaultInterestPayable;
        }

        public Account(int accountNumber, int customerId)
        {
            AccountNumber = accountNumber;
            Balance = DefaultBalance;
            CustomerId = customerId;
            SavingsRate = DefaultSavingsRate;
            CreditLimit = DefaultCreditLimit;
            InterestPayable = DefaultInterestPayable;
        }
        
        public void AccountInformation()
        {
            Console.WriteLine($"Konto #{AccountNumber}");
            Console.WriteLine($"Saldo: {Balance:C}");
            if (CreditLimit != 0)
            {
                Console.WriteLine($"Kreditgräns: {CreditLimit:C}");
            }
        }

        public void TransferMoney(Account accountToo, decimal amount)
        {
            if (amount <= 0m)
            {
                throw new AmountNegativeOrZeroException(AmountNegativeOrZeroMessage);
            }
            if (amount > Balance)
            {
                if (amount > Balance + CreditLimit)
                {
                    throw new InsufficientFundsException(InsufficientFundsMessage);
                }
                TransferWithCredit(accountToo, amount);
                return;
            }
            Balance -= amount;
            accountToo.Balance += amount;
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, accountToo.AccountNumber, -amount, Balance));
            accountToo.Transactions.Add(Transaction.StoreTransaction(accountToo.AccountNumber, AccountNumber, amount, accountToo.Balance));
            Console.WriteLine($"{amount:C} har överförts från konto #{AccountNumber}" +
                              $" till konto #{accountToo.AccountNumber}.");
        }

        public void AccountDeposit(decimal money)
        {
            if (money <= 0)
            {
                throw new AmountNegativeOrZeroException(AmountNegativeOrZeroMessage);
            }
            Balance += money;
            Console.WriteLine($"Du har satt in {money:C} till konto #{AccountNumber}.");
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, money, Balance));
        }

        public void AccountWithdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new AmountNegativeOrZeroException(TransferCantBeNegativeMessage);
            }
            if (amount > Balance)
            {
                if (amount > Balance + CreditLimit)
                {
                    throw new InsufficientFundsException(InsufficientFundsMessage);
                }
                CreditDebt(amount);
                return;
            }
            Balance -= amount;
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, -amount, Balance));
            Console.WriteLine($"Du har tagit ut {amount:C} från #{AccountNumber}.");
            Console.WriteLine($"Återstående saldo är {Balance:C}.");
        }

        private void TransferWithCredit(Account accountToo, decimal money)
        {
            Balance -= money;
            CreditLimit += Balance;
            accountToo.Balance += money;
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, accountToo.AccountNumber, -money, Balance));
            accountToo.Transactions.Add(Transaction.StoreTransaction(accountToo.AccountNumber, AccountNumber, money, accountToo.Balance));
            Console.WriteLine($"{money:C} har överförts från konto #{AccountNumber} till konto #{accountToo.AccountNumber}.");
            Console.WriteLine($"Saldot är {Balance:C} med en kvarstående kredit på {CreditLimit:C}");

        }

        private void CreditDebt(decimal withdrawnMoney)
        {
            Balance -= withdrawnMoney;
            CreditLimit += Balance;
            Console.WriteLine($"Du har tagit ut {withdrawnMoney:C} från #{AccountNumber}.");
            Console.WriteLine($"Saldot är {Balance:C} med en kvarstående kredit på {CreditLimit:C}");
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, -withdrawnMoney, Balance));
        }

        public void PrintAccountTransactions()
        {
            var withdrawsDepositsList = BankSearch.GetCustomerWithdrawsAndDeposits(Transactions);
            var transfersList = BankSearch.GetCustomerTransfers(Transactions);
            var totalTransfers = transfersList.Count;
            var totalWithdrawsDeposits = withdrawsDepositsList.Count;

            if (totalWithdrawsDeposits == 0 && totalTransfers == 0)
            {
                Console.WriteLine($"Inga transaktioner gjorda idag för konto #{AccountNumber}.");
                return;
            }

            Console.WriteLine($"Transaktioner för konto #{AccountNumber}");
            Console.WriteLine();
            if (totalWithdrawsDeposits != 0)
            {
                Console.WriteLine("INSÄTTNINGAR/UTTAG".PadLeft(40));
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("Datum".PadRight(20) + "Belopp".PadRight(30) + "Saldo");
                foreach (var transaction in withdrawsDepositsList)
                {
                    Console.WriteLine($"{transaction.Date}".PadRight(20) +
                                      $"{transaction.Money:C}".PadRight(30) +
                                      $"{transaction.Balance:C}");
                }
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }

            if (totalTransfers == 0) return;
            Console.WriteLine("ÖVERFÖRINGAR".PadLeft(34));
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Datum".PadRight(20) + "Belopp".PadRight(15) + "Konto".PadRight(15) + "Saldo");
            foreach (var transaction in transfersList)
            {
                Console.WriteLine($"{transaction.Date}".PadRight(20) +
                                  $"{transaction.Money:C}".PadRight(15) +
                                  $"#{transaction.TooAccountNumber}".PadRight(15) +
                                  $"{transaction.Balance:C}");
            }
        }

        public void AddSavingsRate(decimal rate)
        {
            if (rate <= 0)
            {
                throw new AmountNegativeOrZeroException(SavingsRateIsNegativeMessage);
            }
            SavingsRate = rate;
        }

        public void AddDailySavingsRate()
        {
            var startBalance = Balance;
            var rateToCalculate = SavingsRate / 100m;
            var dailyRate = rateToCalculate / DaysAYear;
            var value = Balance * dailyRate;
            value = Math.Round(value, 2);
            Balance = startBalance + value;
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, value, Balance));
        }

        public void RemoveSavingsRate() => SavingsRate = DefaultSavingsRate;

        public void AddCreditLimit(decimal limit)
        {
            if (limit <= 0)
            {
                throw new AmountNegativeOrZeroException(IncorrectCreditLimitMessage);
            }
            CreditLimit = limit;
            AddedCreditLimit = limit;
        }

        public void AddInterestPayable(decimal rate)
        {
            if (rate <= 0)
            {
                throw new AmountNegativeOrZeroException(InterestPayableIsNegativeMessage);
            }
            InterestPayable = rate;
        }

        public void AddDailyInterestPayable()
        {
            var startBalance = Balance;
            var rate = InterestPayable / 100m;
            var dailyRate = rate / 365m;
            var value = Balance * dailyRate;
            value = Math.Round(value, 2);
            Balance = startBalance + value;
            Transactions.Add(Transaction.StoreTransaction(AccountNumber, value, Balance));
        }

        public void RemoveCreditLimit()
        {
            if (Balance < 0)
            {
                throw new AmountNegativeOrZeroException(BalanceIsNegativeMessage);
            }
            AddedCreditLimit = DefaultCreditLimit;
            CreditLimit = DefaultCreditLimit;
            InterestPayable = DefaultInterestPayable;
            Console.WriteLine($"Kreditgränsen och skuldräntan för konto #{AccountNumber} är borttagen.");
        }

        public static Account ParseFromTxt(string line)
        {
            var columns = line.Split(';');
            return new Account
            {
                AccountNumber = int.Parse(columns[0]),
                CustomerId = int.Parse(columns[1]),
                Balance = decimal.Parse(columns[2], CultureInfo.InvariantCulture),

            };
        }

        public static int GenerateAccountNumber(List<Customer> customers)
        {
            var result = 13000;
            while (customers.Any(c => c.CustomerAccounts.ContainsKey(result)))
            {
                result++;
            }

            return result;
        }
    }
}
