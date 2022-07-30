﻿using System;
using Task1.Model;

namespace Temp.Validate
{
    public abstract class Attribute
    {
        protected Context _context;

        public void SetContext(Context context)
        {
            _context = context;
        }

        public abstract string Validate(string input, Client client);
    }
}
