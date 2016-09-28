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

        public ICollection<TermExpression> TheBests { get; set; }

        public IEnumerable<Token> tokensTable = new List<Token> 
        {
            //classes of symbols
            new Token { Id = 0, Name = "[...]", Arity = 1, Value = "[{0}]", IsTerminal = false },
            new Token { Id = 1, Name = "[^...]", Arity = 1, Value = "[^{0}]", IsTerminal = false },
            new Token { Id = 2, Name = "[...-...]", Arity = 2, Value = "[{0}-{1}]", IsTerminal = false },
            new Token { Id = 3, Name = "[^...-...]", Arity = 2, Value = "[^{0}-{1}]", IsTerminal = false },

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

        public void Run(int initialPopulactionCount, int initialVectorSize)
        {
            var mutationFactor = 5;
            var factorSimpleMutation = 7;

            var generationCounter = 0;
            var maxGenerations = 1000000;

            CreateInitalPopulation(initialPopulactionCount, initialVectorSize);

            TheBests = new List<TermExpression>();

            while (generationCounter < maxGenerations)
            {
                generationCounter++;
                CalculateFitnessFunctionForPopulation();
                TheBestExpression = GetTheBestOne();
                Console.WriteLine("generation - {0}; population - {1}; TheBest: {2}, Factor: {3}", generationCounter, Population.Count, TheBestExpression.Expression, TheBestExpression.FithessFactor);
                if (TheBestExpression.FithessFactor > 0)
                {
                    if (TheBests.Count<100) TheBests.Add(TheBestExpression); 
                }
                if (TheBestExpression.FithessFactor > 999)
                {
                    TheBests.Add(TheBestExpression);
                    break;
                }

                //1
                var selected = Selection(Population, initialPopulactionCount);
                //2
                var _newPopulation = RecombinePopulation(selected);
                //3
                MutatePopulation(_newPopulation, mutationFactor, factorSimpleMutation);

                //Update population
                Population = _newPopulation;
            }
        }

        private ICollection<TermExpression> RecombinePopulation(ICollection<TermExpression> selected)
        {
            var newPopulation = new List<TermExpression>();
            foreach (var item in selected)
            {
                foreach (var secondItem in selected)
                {
                    if (item.Equals(secondItem)) continue;
                    var newTermExpressions = Recombine(item, secondItem);
                    newPopulation.AddRange(newTermExpressions);
                }
            }
            return newPopulation;
        }

        private ICollection<TermExpression> Recombine(TermExpression item, TermExpression secondItem)
        {
            var firstLength = item.ExpressionTokens.Length;
            var seconfLength = secondItem.ExpressionTokens.Length;
            var tokenLength = (firstLength > seconfLength) ? seconfLength : firstLength;

            var takePositions = new Random().Next(tokenLength - 1);

            var firstParticleOfFirst = item.ExpressionTokens.Take(takePositions).AsEnumerable();
            var SecondParticleOfFirst = item.ExpressionTokens.Skip(takePositions).AsEnumerable();
            var firstParticleOfSecond = secondItem.ExpressionTokens.Take(takePositions).AsEnumerable();
            var SecondParticleOfSecond = secondItem.ExpressionTokens.Skip(takePositions).AsEnumerable();

            var result = new List<TermExpression>
            {
                new TermExpression
                {
                    ExpressionTokens = Enumerable.Concat(firstParticleOfFirst, SecondParticleOfSecond).ToArray(),
                },
                new TermExpression
                {
                    ExpressionTokens = Enumerable.Concat(firstParticleOfSecond, SecondParticleOfFirst).ToArray(),
                }
            };

            return result;
        }

        private ICollection<TermExpression> Selection(ICollection<TermExpression> Population, int populationLimit)
        {
            var countForsrlrction = populationLimit; // (int)(Population.Count / 2);
            return Population.OrderByDescending(x => x.FithessFactor).Take(countForsrlrction).ToList();
        }

        private void CreateInitalPopulation(int size, int vectorSize)
        {
            if (size < 1) throw new Exception("Very small population size");
            if (vectorSize < 1) throw new Exception("Very small vector size");

            Population = new List<TermExpression>();

            var rnd = new Random();

            for (var counter = 0; counter < size; counter++)
            {
                var tokens = new Token[vectorSize];
                for(var i=0; i<tokens.Length; i++)
                {
                    var position = rnd.Next(tokensTable.Count()-1);
                    tokens[i] = tokensTable.ElementAt(position);
                }
                Population.Add(new TermExpression { ExpressionTokens = tokens });
            }
        }

        private void CalculateFitnessFunctionForPopulation()
        {
            foreach (var item in Population)
            {
                item.FithessFactor = FitnessFunction(item.Expression);
            }
        }

        private TermExpression GetTheBestOne()
        {
            var bestTerm = Population.First();
            foreach (var item in Population)
            {
                var fithessFactor = FitnessFunction(item.Expression);
                var bestFithessFactor = FitnessFunction(bestTerm.Expression);

                if (fithessFactor > bestFithessFactor) bestTerm = item;
            }

            bestTerm.FithessFactor = FitnessFunction(bestTerm.Expression);

            return bestTerm;
        }

        private void MutatePopulation(IEnumerable<TermExpression> population, int mutationFactor, int factorSimpleMutation)
        {
            var rnd = new Random();
            foreach (var item in population)
            {
                if (rnd.Next(mutationFactor) == 0) MutateSingle(item, factorSimpleMutation);
            }
        }

        private void MutateSingle(TermExpression expression, int factorSimpleMutation)
        {
            var lengthTokensTable = tokensTable.Count();
            var rnd = new Random();
            var tablePosition = rnd.Next(lengthTokensTable - 1);

            var lengthEpression = expression.ExpressionTokens.Length;
            var position = rnd.Next(lengthEpression - 1);

            var typeMutation = rnd.Next(factorSimpleMutation);
            if (typeMutation != 0)
            {
                //mutate element
                expression.ExpressionTokens[position] = tokensTable.ElementAt(tablePosition);
            }
            else
            {
                //add element to end
                var token = tokensTable.ElementAt(tablePosition);
                var expr = expression.ExpressionTokens.ToList();
                expr.Add(token);
                if (!token.IsTerminal)
                {
                    var terminals = tokensTable.Where(x => x.IsTerminal);
                    var countTerminals = terminals.Count();
                    for (var i = 0; i < token.Arity; i++)
                    {
                        expr.Add(terminals.ElementAt(rnd.Next(countTerminals - 1)));
                    }
                }
            }
        }
    }
}
