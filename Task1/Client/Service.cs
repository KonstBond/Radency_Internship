using System.Collections.Generic;

namespace Task1.Client
{
    public class Service
    {
        public string Name { get; set; }
        public IEnumerable<Payer> Payers { get; set; }
        public decimal Total { get; set; }
    }
}
