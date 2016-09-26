using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneticRegularGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var testWord = "Самарская обл., Безенчукский р-н, пгт. Безенчук, ул. Квартальная";

            var expected = "Квартальная";

            //Func<string, double> fitnessFunction = rxp =>
            //{
            //    Regex regex = new Regex(rxp, RegexOptions.IgnoreCase);

            //    var fithessWeight = 0d;

            //    if (regex.IsMatch(testWord)) 
            //    {
            //        fithessWeight += 100;
            //    }

            //    var match = regex.Match(testWord);
            //    var deltaLength = (match.Value.Length < expected.Length)? 
            //        (expected.Length - match.Value.Length):
            //        (match.Value.Length - expected.Length);
            //    fithessWeight -= deltaLength * 1000;

            //    return fithessWeight;
            //};

            //var generator = new RegularExpressionGenerator(fitnessFunction);

            //generator.Run(1000, 20);

            var t = new TermExpression 
            {
                ExpressionTokens = new Token[] 
                {
                    new Token{Arity = 1, IsTerminal = false, Value = "({0})"},
                    new Token{Arity = 0, IsTerminal = true, Value = "у"},
                    new Token{Arity = 0, IsTerminal = true, Value = "л"},
                    new Token{Arity = 0, IsTerminal = true, Value = "."},
                    new Token{Arity = 0, IsTerminal = true, Value = "\\s"},
                }
            };

            Console.WriteLine(t.Expression);
        }
    }



    public class Genetic
    {
        public IEnumerable<TermExpression> Population { get; set; }
    }
}
