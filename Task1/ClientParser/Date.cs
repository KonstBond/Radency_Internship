using System;
using Task1.Client;

namespace Task1.ClientParser
{
    public class Date : Attribute
    {
        public override string Validate(string input, RawClient client)
        {
            string date = input.Substring(0, input.IndexOf(_context.SeparatorAttribute));
            input = input.Remove(0, date.Length + 1).Trim();

            if (!(date.Length > 0) || string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date) ||
                !int.TryParse(date.Substring(0, 4), out int year) ||
                !int.TryParse(date.Substring(5, 2), out int day) ||
                !int.TryParse(date.Substring(8, 2), out int month))
            {
                
                return "ERROR: Problem with date";
            }
            else
                client.Date = new DateTime(year, month, day);

            _context.NextTo(new AccountNumber());
            return input;
        }
    }
}