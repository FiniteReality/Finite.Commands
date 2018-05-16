using System;
using System.Collections.Generic;
using System.Text;

namespace Wumpus.Commands
{
    public partial class DefaultCommandParser
    {
        private enum TokenizerState
        {
            Normal,
            QuotedString,
        };

        public virtual bool IsQuoteCharacter(char quoteCharacter)
        {
            return quoteCharacter == '\'' || quoteCharacter == '"';
        }

        public virtual bool IsQuotedParameter(string parameter)
        {
            return (parameter.StartsWith("\"") && parameter.EndsWith("\"") ||
                parameter.StartsWith("'") && parameter.EndsWith("'"));
        }

        public virtual string[] Tokenize(string commandText)
        {
            var paramBuilder = new StringBuilder();
            var result = new List<string>();
            var state = TokenizerState.Normal;

            foreach (char c in commandText)
            {
                paramBuilder.Append(c);
                switch (state)
                {
                    case TokenizerState.Normal when char.IsWhiteSpace(c):
                        result.Add(paramBuilder.ToString());
                        paramBuilder.Clear();
                        break;
                    case TokenizerState.Normal when IsQuoteCharacter(c):
                        state = TokenizerState.QuotedString;
                        break;
                    case TokenizerState.QuotedString when IsQuoteCharacter(c)
                        && IsQuotedParameter(paramBuilder.ToString()):

                        state = TokenizerState.Normal;
                        result.Add(paramBuilder.ToString()
                            .Substring(1, paramBuilder.Length - 2));
                        paramBuilder.Clear();
                        break;
                }
            }

            if (state != TokenizerState.Normal)
            {
                throw new InvalidOperationException("Parse failed: bad state");
            }

            return result.ToArray();
        }
    }
}
