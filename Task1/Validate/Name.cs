﻿using System;
using Task1.Model;

namespace Temp.Validate
{
    public class Name : Attribute
    {
        public override string Validate(string input, Client client)
        {
            string name = input.Substring(0, input.IndexOf(_context.Separator));
            input = input.Remove(0, name.Length + 1).Trim();

            if (!(name.Length > 0) && string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(name))
                return "ERROR: Problem with name";
            else
                client.Name = name;

            _context.NextTo(new LastName());
            return input;
        }
    }
}
