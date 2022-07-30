using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp.Validate
{
    public class Payment : Attribute
    {
        public override string Validate(string input, Client client)
        {
            string payment = input.Substring(0, input.IndexOf(_context.Separator));
            input = input.Remove(0, payment.Length + 1).Trim();

            if (!(payment.Length > 0) && string.IsNullOrEmpty(payment)
                && string.IsNullOrWhiteSpace(payment) && !decimal.TryParse(payment, out _))
            {
                return "ERROR: Problem with payment";
            }
            else
            {
                client.Payment = Convert.ToDecimal(payment, new CultureInfo("en-UA"));
            }

            _context.NextTo(new Date());
            return input;
        }
    }
}
