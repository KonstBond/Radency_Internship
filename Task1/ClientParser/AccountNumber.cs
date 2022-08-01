using System;
using Task1.Client;

namespace Task1.ClientParser
{
    public class AccountNumber : Attribute
    {
        public override string Validate(string input, RawClient client)
        {
            string accountNumber = input.Substring(0, input.IndexOf(_context.SeparatorAttribute));
            input = input.Remove(0, accountNumber.Length + 1).Trim();

            if (!(accountNumber.Length > 0) || string.IsNullOrEmpty(accountNumber) || 
                string.IsNullOrWhiteSpace(accountNumber) || !long.TryParse(accountNumber, out long result))
            {
                return "ERROR: Problem with account number";
            }
            else
                client.AccountNumber = result;

            _context.NextTo(new Service());
            return input;
        }
    }
}