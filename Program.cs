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
            var testWord = "Самарская обл., Безенчукский р-н, пгт. Безенчук, ул. Квартальная";

            var expected = "Квартальная";

            Func<string, double> fitnessFunction = rxp =>
            {
                var fithessWeight = 0d;

                Regex regex = null;
                Match match = null;

                try
                {
                    regex = new Regex(rxp, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10));
                    match = regex.Match(testWord);
                }
                catch
                {
                    return -1000;
                }

                if (match.Success)
                {
                    var sim = Levenshtein.GetSimilarity(match.Value, expected);
                    fithessWeight += sim * 1000;
                }

                return fithessWeight;
            };

            var generator = new RegularExpressionGenerator(fitnessFunction);

            generator.Run(10, 20);

            Console.WriteLine(generator.TheBestExpression.Expression);
        }
    }



    public class Genetic
    {
        public IEnumerable<TermExpression> Population { get; set; }
    }
}
