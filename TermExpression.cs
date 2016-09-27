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
                var position = 0;
                var result = "";
                while (position<ExpressionTokens.Length)
                {
                    result += ParseNonTerminal(ExpressionTokens, ref position);
                }
                return result;
            }
        }

        public Token[] ExpressionTokens { get; set; }

        public double FithessFactor { get; set; }

        private string ParseTerminal(Token[] expression, int position) 
        {
            return expression[position].Value;
        }

        //start
        private string ParseNonTerminal(Token[] expression, ref int position)
        {
            var token = expression[position];

            var args = new List<string>();
            for (var i = 0; i < token.Arity;i++ )
            {
                position++;
                string argToken = (expression[position].IsTerminal) ? ParseTerminal(expression, position) : ParseNonTerminal(expression, ref position);
                args.Add(argToken);
            }

            position++;


            return string.Format(token.Value, args.ToArray());
        }
    }
}
