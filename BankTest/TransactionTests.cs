using Bank;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankTest
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        [ExpectedException(typeof(InsufficientFundsException))]
        public void WithdrawMoreThanBalance()
        {
            // Arrange
            decimal startBalance = 500m;
            decimal debitAmount = 550m;
            var account = new Account(1, 30, startBalance);


            // Act
            account.AccountWithdraw(debitAmount);

            // Expecting an exception thrown
        }

        [TestMethod]
        public void WithdrawNegativeAmount()
        {
            // Arrange
            decimal startBalance = 200.20m;
            decimal debitAmount = -150m;
            var account = new Account(1, 1, startBalance);

            try
            {   // Act
                account.AccountWithdraw(debitAmount);
            }
            catch (AmountNegativeOrZeroException e)
            {
                // Assert
                StringAssert.Contains(e.Message, Account.TransferCantBeNegativeMessage);
                return;
            }
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void WithdrawWithCreditLimit()
        {
            // Arrange
            var startBalance = 1000.20m;
            var debit = 3000m;
            var expected = -1999.80m;

            var account = new Account(1, 1, startBalance);
            account.AddCreditLimit(4000m);

            // Act
            account.AccountWithdraw(debit);

            // Assert
            var actually = account.Balance;
            Assert.AreEqual(expected, actually);

        }

        [TestMethod]
        public void DepositNegativeAmount()
        {
            // Arrange
            decimal startBalance = 500m;
            decimal depositAmount = -500m;
            var account = new Account(1, 30, startBalance);

            try
            {
                // Act
                account.AccountDeposit(depositAmount);
            }
            catch (AmountNegativeOrZeroException e)
            {
                //Assert
                StringAssert.Contains(e.Message, Account.AmountNegativeOrZeroMessage);
                return;
            }
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void TransferMoreThanBalance()
        {
            // Arrange
            var startBalance = 1000m;
            var transferAmount = 1200m;
            var accountFrom = new Account(1, 30, startBalance);
            var accountToo = new Account(2, 31, startBalance);

            // Act
            try
            {
                accountFrom.TransferMoney(accountToo, transferAmount);
            }
            catch (InsufficientFundsException e)
            {
                // Assert
                StringAssert.Contains(e.Message, Account.InsufficientFundsMessage);
                return;
            }
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void TransferNegativeAmount()
        {
            // Arange
            var startBalance = 1000m;
            var transferAmount = -500m;
            var accountFrom = new Account(1, 30, startBalance);
            var accountToo = new Account(2, 31, startBalance);

            try
            {
                // Act
                accountFrom.TransferMoney(accountToo, transferAmount);
            }
            catch (AmountNegativeOrZeroException e)
            {
                // Assert
                StringAssert.Contains(e.Message, Account.AmountNegativeOrZeroMessage);
                return;
            }
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void SavingsRateCalculatesCorrectly()
        {
            // Arrange
            var startBalance = 5000m;
            var savingsRate = 5m;
            var expected = 5000.68m;
            // 5% Årsränta till dagsränta = (0,05 / 365) = 0,0001369863013698630136986301369863
            // * 5000 = 0,68493150684931506849315068493151 = ~ 0,68
            // 5000 + 0,68 = 5000,68 SEK

            var account = new Account(1, 30, startBalance);

            // Act
            account.AddSavingsRate(savingsRate);
            account.AddDailySavingsRate();

            // Assert
            var result = account.Balance;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void InterestPayableCalculatesCorrectly()
        {
            // Arrange
            var startBalance = 0m;
            var creditLimit = 1000m;
            var debit = 1000m;
            var rate = 10m;
            var expectedResult = -1000.27m;
            // 10% Årsränta till dagsränta = (0,10 / 365) = 0,0002739726027397260273972602739
            // * (-1000) = -0,2739726027397260273972602739726 = ~ -0,27
            // -1000 + -0,27 = -1000,27 SEK

            var account = new Account(1, 30, startBalance);
            account.AddCreditLimit(creditLimit);
            account.AddInterestPayable(rate);

            // Act
            account.AccountWithdraw(debit);
            account.AddDailyInterestPayable();

            // Assert
            var actual = account.Balance;
            Assert.AreEqual(actual, expectedResult);

        }
    }
}
