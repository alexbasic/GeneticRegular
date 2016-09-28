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
        static long mx = 0;
        static long mn = 0;

        static int counter = 0;
        static long accumTime = 0;

        static void Main(string[] args)
        {
            var testWord = "Самарская обл., Безенчукский р-н, пгт. Безенчук, ул. Квартальная";
            var testWord2 = "Самарская обл., Безенчукский р-н, пгт. Безенчук, ул. Мира";

            var expected = "Квартальная";
            var expected2 = "Мира";

            Func<string, double> fitnessFunction = rxp =>
            {
                var t = (rxp.Contains("{{")) ? -100 : 0;
                return ((Calculate(rxp, testWord, expected) + Calculate(rxp, testWord2, expected2))/2)+t;
            };

            var generator = new RegularExpressionGenerator(fitnessFunction);

            generator.Run(11, 20);

            Console.WriteLine(generator.TheBestExpression.Expression);
        }

        private static double Calculate(string expression, string testWord, string expected) 
        {
            counter++;

            var fithessWeight = 0d;

            Regex regex = null;
            Match match = null;

            var startTime = DateTime.Now.Ticks;
            try
            {
                regex = new Regex(expression, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10));
                match = regex.Match(testWord);
            }
            catch
            {
                return -1000;
            }

            var deltaTicks = DateTime.Now.Ticks - startTime;
            if (mx < deltaTicks) mx = deltaTicks;
            //if (mn > deltaTicks) mn = deltaTicks;

            accumTime += deltaTicks;

            if (accumTime / 200 > deltaTicks) fithessWeight += 10;

            if (counter > 199) mx = 0;

            if (match.Success)
            {
                var sim = Levenshtein.GetSimilarity(match.Value, expected);
                fithessWeight += sim * 1000;
            }

            return fithessWeight;
        }
    }



    public class Genetic
    {
        public IEnumerable<TermExpression> Population { get; set; }
    }
}
