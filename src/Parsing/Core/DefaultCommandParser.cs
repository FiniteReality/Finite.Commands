using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands.Parsing
{
    internal sealed class DefaultCommandParser : ICommandParser
    {
        private readonly ICommandStore _commandStore;

        public DefaultCommandParser(ICommandStore commandStore)
        {
            _commandStore = commandStore;
        }

        public ValueTask ParseAsync(CommandContext context, string message,
            CancellationToken cancellationToken = default)
        {
            var store = _commandStore;

            foreach (var token in Lex(message))
            {
                if (store.HasNestedCommands)
                {
                    store = store.GetNestedCommands(token);
                }
            }

            return default;
        }

        // Internal for unit tests
        internal static IEnumerable<CommandPath> Lex(string message)
        {
            var start = Index.Start;

            var state = ParserState.TokenBegin;
            for (int x = 0; x < message.Length; x++)
            {
                var c = message[x];

                switch (state)
                {
                    case ParserState.TokenBegin:
                    {
                        start = Index.FromStart(x);

                        state = IsOpenQuotationCharacter(c)
                            ? ParserState.QuotedToken
                            : ParserState.UnquotedToken;

                        break;
                    }
                    case ParserState.QuotedToken:
                    {
                        if (IsCloseQuotationCharacter(c, message[start]))
                        {
                            var end = Index.End;
                            if (x != message.Length - 1)
                                end = Index.FromStart(x + 1);

                            var range = start..end;

                            yield return new CommandPath(message, range);
                            state = ParserState.TokenEnd;
                        }
                        break;
                    }
                    case ParserState.UnquotedToken:
                    {
                        if (IsWhitespaceCharacter(c))
                        {
                            var end = Index.FromStart(x);
                            var range = start..end;

                            yield return new CommandPath(message, range);
                            state = ParserState.TokenEnd;
                        }
                        break;
                    }
                    case ParserState.TokenEnd:
                    {
                        if (!IsWhitespaceCharacter(c))
                        {
                            goto case ParserState.TokenBegin;
                        }
                        break;
                    }
                }
            }

            // If we reached the end of the string and didn't finish our last
            // token, we forcibly finish it here.
            if (state != ParserState.TokenEnd)
            {
                yield return new CommandPath(message, Range.StartAt(start));
            }
        }

        private static bool IsOpenQuotationCharacter(char c)
            => c == '"'
            || c == '\''
            || char.GetUnicodeCategory(c) == UnicodeCategory.InitialQuotePunctuation;

        // TODO: should we match quotation characters here?
        private static bool IsCloseQuotationCharacter(char c, char openQuote)
            => ((c == '"' || c == '\'') && c == openQuote)
            || char.GetUnicodeCategory(c) == UnicodeCategory.FinalQuotePunctuation;

        private static bool IsWhitespaceCharacter(char c)
            => char.IsWhiteSpace(c);

        public enum ParserState
        {
            TokenBegin,
            UnquotedToken,
            QuotedToken,
            TokenEnd
        }
    }
}
