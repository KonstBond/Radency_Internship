using System;
using System.Collections.Generic;

namespace Task1.Client
{
    public class ResultClient
    {
        public string City { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public decimal Total { get; set; }
    }
}
