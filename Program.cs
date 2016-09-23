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
            Regex regex = new Regex("World", RegexOptions.IgnoreCase);
        }
    }

    public class RegularExpressionGenerator
    {
        public string RegularExpression { get; internal set; }

        public IEnumerable<Token> tokensTable = new List<Token> 
        {
            new Token { Id = 0, Name = "[{0}]", Arity = 1, Value = "[{0}]", IsTerminal = false },
            new Token { Id = 1, Name = "[^{0}]", Arity = 1, Value = "[^{0}]", IsTerminal = false },
            new Token { Id = 0, Name = "[{0}-{1}]", Arity = 2, Value = "[{0}-{1}]", IsTerminal = false },
            new Token { Id = 1, Name = "[^{0}-{1}]", Arity = 2, Value = "[^{0}-{1}]", IsTerminal = false },
            new Token { Id = 2, Name = ".", Arity = 0, Value = ".", IsTerminal = true },
            new Token { Id = 3, Name = "\\w", Arity = 0, Value = "\\w", IsTerminal = true },
            new Token { Id = 4, Name = "\\W", Arity = 0, Value = "\\W", IsTerminal = true },
            new Token { Id = 5, Name = "\\s", Arity = 0, Value = "\\s", IsTerminal = true },
            new Token { Id = 6, Name = "\\S", Arity = 0, Value = "\\S", IsTerminal = true },
            new Token { Id = 7, Name = "\\d", Arity = 0, Value = "\\d", IsTerminal = true },
            new Token { Id = 8, Name = "\\D", Arity = 0, Value = "\\D", IsTerminal = true },
            new Token { Id = 9, Name = "{n,m}", Arity = 3, Value = "{0}{{{1},{2}}}", IsTerminal = false },
            new Token { Id = 10, Name = "{n,}", Arity = 2, Value = "{0}{{{1},}}", IsTerminal = false },
            new Token { Id = 11, Name = "{n}", Arity = 2, Value = "{0}{{{1}}}", IsTerminal = false },
            new Token { Id = 12, Name = "?", Arity = 1, Value = "{0}?", IsTerminal = false },
            new Token { Id = 12, Name = "+", Arity = 1, Value = "{0}+", IsTerminal = false },
            new Token { Id = 12, Name = "*", Arity = 1, Value = "{0}*", IsTerminal = false },
            new Token { Id = 12, Name = "|", Arity = 2, Value = "{0}|{1}", IsTerminal = false },
        };
    }

    public class Genetic
    {
        public IEnumerable<TermExpression> Population { get; set; }
    }

    public class TermExpression
    {
        public string Expression { get; }

        public Token[] ExpressionTokens { get; set; }
    }

    public class Token
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Arity { get; set; }
        public string Value { get; set; }
        public bool IsTerminal { get; set; }
    }
}
