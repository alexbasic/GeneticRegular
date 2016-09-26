using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneticRegularGenerator
{
    public class RegularExpressionGenerator
    {
        public TermExpression TheBestExpression { get; internal set; }

        public Func<string, double> FitnessFunction { get; set; }

        public IEnumerable<Token> tokensTable = new List<Token> 
        {
            //classes of symbols
            new Token { Id = 0, Name = "[{0}]", Arity = 1, Value = "[{0}]", IsTerminal = false },
            new Token { Id = 1, Name = "[^{0}]", Arity = 1, Value = "[^{0}]", IsTerminal = false },
            new Token { Id = 2, Name = "[{0}-{1}]", Arity = 2, Value = "[{0}-{1}]", IsTerminal = false },
            new Token { Id = 3, Name = "[^{0}-{1}]", Arity = 2, Value = "[^{0}-{1}]", IsTerminal = false },
            new Token { Id = 4, Name = ".", Arity = 0, Value = ".", IsTerminal = true },
            new Token { Id = 5, Name = "\\w", Arity = 0, Value = "\\w", IsTerminal = true },
            new Token { Id = 6, Name = "\\W", Arity = 0, Value = "\\W", IsTerminal = true },
            new Token { Id = 7, Name = "\\s", Arity = 0, Value = "\\s", IsTerminal = true },
            new Token { Id = 8, Name = "\\S", Arity = 0, Value = "\\S", IsTerminal = true },
            new Token { Id = 9, Name = "\\d", Arity = 0, Value = "\\d", IsTerminal = true },
            new Token { Id = 10, Name = "\\D", Arity = 0, Value = "\\D", IsTerminal = true },
            //repeats symbols
            new Token { Id = 11, Name = "{n,m}", Arity = 3, Value = "{0}{{{1},{2}}}", IsTerminal = false },
            new Token { Id = 12, Name = "{n,}", Arity = 2, Value = "{0}{{{1},}}", IsTerminal = false },
            new Token { Id = 13, Name = "{n}", Arity = 2, Value = "{0}{{{1}}}", IsTerminal = false },
            new Token { Id = 14, Name = "?", Arity = 1, Value = "{0}?", IsTerminal = false },
            new Token { Id = 15, Name = "+", Arity = 1, Value = "{0}+", IsTerminal = false },
            new Token { Id = 16, Name = "*", Arity = 1, Value = "{0}*", IsTerminal = false },
            //case symbols
            new Token { Id = 17, Name = "|", Arity = 2, Value = "{0}|{1}", IsTerminal = false },
            new Token { Id = 18, Name = "(...)", Arity = 1, Value = "({0})", IsTerminal = false },
            new Token { Id = 19, Name = "(?:...)", Arity = 1, Value = "(?:{0})", IsTerminal = false },
            //anchor symbols
            new Token { Id = 20, Name = "^", Arity = 1, Value = "^{0}", IsTerminal = false },
            new Token { Id = 21, Name = "$", Arity = 1, Value = "{0}$", IsTerminal = false },
            new Token { Id = 22, Name = "\\b...", Arity = 1, Value = "\\b{0}", IsTerminal = false },
            new Token { Id = 23, Name = "...\\b", Arity = 1, Value = "{0}\\b", IsTerminal = false },
            new Token { Id = 24, Name = "\\B...", Arity = 1, Value = "\\B{0}", IsTerminal = false },
            new Token { Id = 25, Name = "...\\B", Arity = 1, Value = "{0}\\B", IsTerminal = false },
        };

        private ICollection<TermExpression> Population { get; set; }

        private ICollection<TermExpression> _newPopulation { get; set; }

        private void AddSymbol(string name, int arity, string value, bool isTerminal)
        {
            var id = (tokensTable.Any()) ?
                (tokensTable.Max(x => x.Id) + 1) :
                0;
            ((List<Token>)tokensTable).Add(new Token { Id = id, Name = name, Arity = arity, Value = value, IsTerminal = isTerminal });
        }

        public RegularExpressionGenerator(Func<string, double> fitnessFunction)
        {
            FitnessFunction = fitnessFunction;

            var rusChars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            foreach (var rusChar in rusChars)
            {
                var upperCase = char.ToUpper(rusChar).ToString();
                var lowerCase = rusChar.ToString();
                AddSymbol(upperCase, 0, upperCase, true);
                AddSymbol(lowerCase, 0, lowerCase, true);
            }

            var numberChars = "0123456789";
            foreach (var numberChar in numberChars)
            {
                var asString = numberChar.ToString();
                AddSymbol(asString, 0, asString, true);
            }
        }

        public void Run()
        {
            var mutationFactor = 100;

            CreateInitalPopulation(10, 4);

            var generationCounter =0;
            var maxGenerations = 1000000;

            while(generationCounter<maxGenerations)
            {
                CalculateFitnessFunctionForPopulation();
                TheBestExpression = GetTheBestOne();
                if (TheBestExpression.FithessFactor > 0.9999) break;

                var rnd = new Random();
                if ( rnd.Next(mutationFactor) == mutationFactor)
                {
                    //mutation
                }
            }
        }

        private void CreateInitalPopulation(int size, int vectorSize)
        {
            if (size < 1) throw new Exception("Very small population size");
            if (vectorSize < 1) throw new Exception("Very small vector size");

            for (var counter = 0; counter < size; counter++)
            {
                Population.Add(new TermExpression { ExpressionTokens = new Token[vectorSize] });
            }
        }

        private void CalculateFitnessFunctionForPopulation()
        {
            foreach(var item in Population)
            {
                item.FithessFactor = FitnessFunction(item.Expression);
            }
        }

        private TermExpression GetTheBestOne()
        {
            var bestTerm = Population.First(); 
            foreach(var item in Population)
            {
                var fithessFactor = FitnessFunction(item.Expression);
                var bestFithessFactor = FitnessFunction(bestTerm.Expression);

                if (fithessFactor > bestFithessFactor) bestTerm = item;
            }

            bestTerm.FithessFactor = FitnessFunction(bestTerm.Expression);

            return bestTerm;
        }

        private void Mutate(TermExpression expression)
        {
            var lengthTokensTable = tokensTable.Count();
            var tablePosition = new Random().Next(lengthTokensTable-1);

            var lengthEpression = expression.ExpressionTokens.Length;
            var position = new Random().Next(lengthEpression-1);
            expression.ExpressionTokens[position] = tokensTable.ElementAt(tablePosition);
        }

        public void Recombine(TermExpression first, TermExpression second, out TermExpression firstOut, TermExpression secondOut, RecomBineType recomBineType)
        {

        }
    }

    public enum RecomBineType
    {
        SinglePoint,
        TwoPoint,
        MultiPoint
    }
}
