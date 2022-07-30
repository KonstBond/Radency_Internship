using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
