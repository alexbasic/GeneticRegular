using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticRegularGenerator
{
    public class Token
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Arity { get; set; }
        public string Value { get; set; }
        public bool IsTerminal { get; set; }
    }
}
