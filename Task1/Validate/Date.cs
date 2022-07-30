using System;
using Task1.Model;

namespace Temp.Validate
{
    public class Date : Attribute
    {
        public override string Validate(string input, Client client)
        {
            string date = input.Substring(0, input.IndexOf(_context.Separator));
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