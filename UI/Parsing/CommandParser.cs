using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Banan.Tools.XbrlBench.UI.Parsing
{
    public class CommandParser
    {
        public ParsedCommand Parse(string line)
        {
            var tokens = Tokenise(line).ToList();
            if (tokens.Count<1)
            {
                throw new ArgumentException("Command is missing");
            }

            var result = new ParsedCommand
            {
                CommandName = tokens.First().Text
            };

            tokens.Add(new Token("-sentinel"));

            Token previousToken = null;
            foreach (var token in tokens.Skip(1))
            {
                if (token.IsPropertyName)
                {
                    if (previousToken != null && previousToken.IsPropertyName)
                    {
                        result.NamedParameters[previousToken.PropertyName] = true.ToString();
                    }

                }

                if (!token.IsPropertyName)
                {
                    if (previousToken != null && previousToken.IsPropertyName)
                    {
                        result.NamedParameters[previousToken.PropertyName] = token.Text;
                    }
                    else
                    {
                        result.PositionalParameters.Add(token.Text);
                    }
                }

                previousToken = token;
            }

            return result;
        }

        private IEnumerable<Token> Tokenise(string line)
        {
            var optionallyQuotedToken = new Regex(@"(?<=\s)("".+?""|[^""]\S*?[^""]|\w)(?=\s)");

            var matches = optionallyQuotedToken.Matches(" "+line+" ");
            return matches
                .Cast<Match>()
                .Select(m => m.Value)
                .Select(p => p.Trim())
                .Select(p => p.Trim('"'))
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => new Token(p));
        }

    }
}