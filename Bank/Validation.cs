using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public static class Validation
    {
        public static int InputInt(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Du måste ange ett heltal.");
            }

            return result;
        }

        public static decimal InputDecimal(string prompt)
        {
            decimal result;
            Console.Write(prompt);
            while (!decimal.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Felaktig inmatning.");
            }
            return result;
        }

        public static bool ValidCustomer(string name, string orgnr, string address, string zipcode, string city,
            string country, string region, string phonenr)
        {
            return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(orgnr) &&
                   !string.IsNullOrEmpty(address) &&
                   !string.IsNullOrEmpty(zipcode) && !string.IsNullOrEmpty(city) &&
                   !name.Any(char.IsNumber) && !city.Any(char.IsNumber) &&
                   !country.Any(char.IsNumber) && !region.Any(char.IsNumber) &&
                   !phonenr.Any(char.IsLetter);
        }
    }
}
