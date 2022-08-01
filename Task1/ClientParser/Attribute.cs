using Task1.Client;

namespace Task1.ClientParser
{
    public abstract class Attribute
    {
        protected Context _context;

        public void SetContext(Context context)
        {
            _context = context;
        }

        public abstract string Validate(string input, RawClient client);
    }
}
