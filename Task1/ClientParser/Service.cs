using System;
using Task1.Client;

namespace Task1.ClientParser
{
    public class Service : Attribute
    {
        public override string Validate(string input, RawClient client)
        {
            string service = input.Substring(0, input.Length);
            input = input.Remove(0, service.Length).Trim();

            if (!(service.Length > 0) && string.IsNullOrEmpty(service) && string.IsNullOrWhiteSpace(service))
                return "ERROR: Problem with name";
            else
                client.Service = service;

            _context.NextTo(new Name());

            return input;
        }
    }
}