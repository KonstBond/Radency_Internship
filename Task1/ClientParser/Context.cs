using Task1.Client;

namespace Task1.ClientParser
{
    public class Context
    {
        private Attribute _attribute = null;
        public char SeparatorAttribute { get; private set; }
        public string SeparatorAddress { get; private set; }

        public Context(Attribute attribute, char SeparatorAttribute, string SeparatorAddress)
        {
            this.SeparatorAttribute = SeparatorAttribute;
            this.SeparatorAddress = SeparatorAddress;
            NextTo(attribute);
        }

        public void NextTo(Attribute attribute)
        {
            _attribute = attribute;
            _attribute.SetContext(this);
        }

        public string RewriteInput(string input, RawClient client)
        {
            return _attribute.Validate(input, client);
        }
    }
}
