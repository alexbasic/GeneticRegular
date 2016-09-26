using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticRegularGenerator
{
    public class TermExpression
    {
        public string Expression {
            get 
            {
                return ParseNonTerminal(ExpressionTokens, 0);
            }
            internal set;
        }

        public Token[] ExpressionTokens { get; set; }

        public double FithessFactor { get; set; }

        private string ParseNonTerminal(Token[] expression, int position)
        {
            string result = "";
            var length = ExpressionTokens.Length;
            var arityCounter = expression[position].Arity;

            result += expression[position]; 

            while (arityCounter > 0)
            {
                if (ExpressionTokens[position].IsTerminal)
                {
                    result += ExpressionTokens[position];
                }
                else
                {
                    result += ParseNonTerminal(ExpressionTokens, position);
                }

                arityCounter--;
            }
            return string.Format(result, arguments);
        }
    }
}
