using System;

namespace Bank
{
    public class BankMenu
    {
        private bool _bankOnline = true;
        private readonly Bank _bank;

        public BankMenu()
        {
            _bank = new Bank();
            MainMenu();
        }

        private void MainMenu()
        {
            while (_bankOnline)
            {
                Console.Clear();
                Console.WriteLine("*******************************");
                Console.WriteLine("* VÄLKOMMEN TILL BANKAPP 1.1 *");
                Console.WriteLine("*******************************");
                Console.WriteLine("\t   HUVUDMENY");
                Console.WriteLine("*******************************");
                Console.WriteLine("[0] Avsluta och spara");
                Console.WriteLine("[1] Sök kund");
                Console.WriteLine("[2] Visa kundbild");
                Console.WriteLine("[3] Skapa kund");
                Console.WriteLine("[4] Ta bort kund");
                Console.WriteLine("[5] Skapa konto");
                Console.WriteLine("[6] Ta bort konto");
                Console.WriteLine("[7] Kontomeny");
                Console.WriteLine("[8] Kalkylera och applicera dagsräntor");
                MainMenuSelect();
            }
        }

        private void MainMenuSelect()
        {
            var input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.D0:
                    _bank.ShutDownBank();
                    _bankOnline = false;
                    break;

                case ConsoleKey.D1:
                    _bank.SearchCustomer();
                    break;

                case ConsoleKey.D2:
                    _bank.CustomerProfile();
                    break;

                case ConsoleKey.D3:
                    _bank.NewCustomer();
                    break;

                case ConsoleKey.D4:
                    _bank.RemoveCustomer();
                    break;

                case ConsoleKey.D5:
                    _bank.CreateNewAccount();
                    break;

                case ConsoleKey.D6:
                    _bank.RemoveAccountFromCustomer();
                    break;

                case ConsoleKey.D7:
                    AccountMenu();
                    break;

                case ConsoleKey.D8:
                    _bank.CalculateAllRates();
                    break;
            }
        }

        private void AccountMenu()
        {
            var accountMenu = true;
            do
            {
                Console.Clear();
                Console.WriteLine("*******************************");
                Console.WriteLine("* VÄLKOMMEN TILL BANKAPP 1.1 *");
                Console.WriteLine("*******************************");
                Console.WriteLine("\tKONTOHANTERING");
                Console.WriteLine("*******************************");
                Console.WriteLine("[0] Tillbaka");
                Console.WriteLine("[1] Insättning");
                Console.WriteLine("[2] Uttag");
                Console.WriteLine("[3] Överföring");
                Console.WriteLine("[4] Översikt transaktioner");
                Console.WriteLine("[5] Sparränta");
                Console.WriteLine("[6] Kreditgräns");
                var input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.D0:
                        accountMenu = false;
                        break;

                    case ConsoleKey.D1:
                        _bank.Deposit();
                        break;

                    case ConsoleKey.D2:
                        _bank.Withdraw();
                        break;

                    case ConsoleKey.D3:
                        _bank.Transfer();
                        break;

                    case ConsoleKey.D4:
                        _bank.AccountProfile();
                        break;

                    case ConsoleKey.D5:
                        _bank.SavingsRate();
                        break;

                    case ConsoleKey.D6:
                        _bank.CreditLimit();
                        break;
                }
            } while (accountMenu);
        }
    }
}