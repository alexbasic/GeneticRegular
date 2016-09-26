using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticRegularGenerator
{
    public class TermExpression
    {
        public string Expression { get; internal set; }

        public Token[] ExpressionTokens { get; set; }

        public double FithessFactor { get; set; }
    }
}
