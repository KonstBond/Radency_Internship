namespace Temp.Validate
{
    public class AccountNumber : Attribute
    {
        public override string Validate(string input, Client client)
        {
            string accountNumber = input.Substring(0, input.IndexOf(_context.Separator));
            input = input.Remove(0, accountNumber.Length + 1).Trim();

            if (!(accountNumber.Length > 0) || string.IsNullOrEmpty(accountNumber) || 
                string.IsNullOrWhiteSpace(accountNumber) || !Int64.TryParse(accountNumber, out long result))
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