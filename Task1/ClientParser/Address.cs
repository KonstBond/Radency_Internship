using System;
using Task1.Client;

namespace Task1.ClientParser
{
    public class Address : Attribute
    {
        public override string Validate(string input, RawClient client)
        {
            string address;
            if (_context.SeparatorAddress == @"""""""")
            {
                address = input.Substring(3, input.LastIndexOf(_context.SeparatorAddress) - 3);
                input = input.Remove(0, address.Length + 7);
            }
            else
            {
                address = input.Substring(1, input.LastIndexOf(_context.SeparatorAddress) - 1);
                input = input.Remove(0, address.Length + 3).Trim();
            }
                
            if (!(address.Length > 0) && string.IsNullOrEmpty(address) && string.IsNullOrWhiteSpace(address))
                return "ERROR: Problem with address";
            else
                client.Address = address;

            _context.NextTo(new Payment());
            return input;
        }
    }
}
