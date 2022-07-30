using System;
using Task1.Model;

namespace Temp.Validate
{
    public class Address : Attribute
    {
        public override string Validate(string input, Client client)
        {
            string address = input.Substring(1, input.LastIndexOf('\"') - 1);
            input = input.Remove(0, address.Length + 3).Trim();

            if (!(address.Length > 0) && string.IsNullOrEmpty(address) && string.IsNullOrWhiteSpace(address))
                return "ERROR: Problem with address";
            else
                client.Address = address;

            _context.NextTo(new Payment());
            return input;
        }
    }
}
