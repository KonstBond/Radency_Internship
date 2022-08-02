using System;
using System.Collections.Generic;

namespace Task1.Logger
{
    public class MetaInfo
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public List<string> InvalidFiles { get; set; } = new List<string>();
    }
}
