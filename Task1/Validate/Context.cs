using System;
using Task1.Model;

namespace Temp.Validate
{
    public class Context
    {
        private Attribute _attribute = null;
        public char Separator { get; set; }

        public Context(Attribute attribute, char separator)
        {
            Separator = separator;
            NextTo(attribute);
        }

        public void NextTo(Attribute attribute)
        {
            _attribute = attribute;
            _attribute.SetContext(this);
        }

        public string RewriteInput(string input, Client client)
        {
            return _attribute.Validate(input, client);
        }
    }
}
