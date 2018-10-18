using System;
using Bank.Data;

namespace Bank
{
    public class Transaction
    {
        public int AccountNumber { get; }
        public int TooAccountNumber { get; }
        public decimal Balance { get; }
        public string Date { get; }
        public decimal Money { get; }

        public Transaction(int accountNumber, decimal value, decimal balance)
        {
            AccountNumber = accountNumber;
            Balance = balance;
            Money = value;
            Date = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
        }

        public Transaction(int accountNumber, int tooAccountNumber, decimal value, decimal balanceAccountFrom)
        {
            AccountNumber = accountNumber;
            TooAccountNumber = tooAccountNumber;
            Money = value;
            Balance = balanceAccountFrom;
            Date = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
        }

        public static Transaction StoreTransaction(int accountNumber, decimal amount, decimal balance)
        {
            var transaction = new Transaction(accountNumber, amount, balance);
            FileManagement.SaveTransactionToTxt(transaction);
            return transaction;
        }

        public static Transaction StoreTransaction(int accountNumber, int accountNumberToo, decimal amount, decimal balance)
        {
            var transaction = new Transaction(accountNumber, accountNumberToo, amount, balance);
            FileManagement.SaveTransactionToTxt(transaction);
            return transaction;
        }

    }
}
