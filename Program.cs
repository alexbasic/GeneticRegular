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
            //Regex regex = new Regex("World", RegexOptions.IgnoreCase);

            var t = new RegularExpressionGenerator();
        }
    }

    

    public class Genetic
    {
        public IEnumerable<TermExpression> Population { get; set; }
    }
}
