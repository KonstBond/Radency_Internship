﻿using System;
using Task1.Client;

namespace Task1.ClientParser
{
    internal class LastName : Attribute
    {
        public override string Validate(string input, RawClient client)
        {
            string lastName = input.Substring(0, input.IndexOf(_context.SeparatorAttribute));
            input = input.Remove(0, lastName.Length + 1).Trim();

            if (!(lastName.Length > 0) && string.IsNullOrEmpty(lastName) && string.IsNullOrWhiteSpace(lastName))
                return "ERROR: Problem with last name";
            else
                client.LastName = lastName;

            _context.NextTo(new Address());
            return input;
        }
    }
}