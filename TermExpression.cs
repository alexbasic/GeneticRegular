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
                return Parse(ExpressionTokens, 0);
            }
        }

        public Token[] ExpressionTokens { get; set; }

        public double FithessFactor { get; set; }

        private string ParseNonTerminal(Token[] expression, int position)
        {
            var token = expression[position];

            List<string> arguments = new List<string>();

            for (var i = 0; i < token.Arity; i++)
            {
                position++;
                arguments.Add(Parse(expression, position));
            }

            return string.Format(token.Value, arguments);
        }

        private string ParseTerminal(Token[] expression, int position) 
        {
            return expression[position].Value;
        }

        private string Parse(Token[] expression, int position) 
        {
            string result = "";

            if (expression[position].IsTerminal)
            {
                result += ParseTerminal(expression, position);
            }
            else 
            {
                result += ParseNonTerminal(expression, position);
            }

            position++;

            //if (position < expression.Length - 1)
            //{
            //    result += Parse(expression, position);
            //}

            return result;
        }
    }
}
