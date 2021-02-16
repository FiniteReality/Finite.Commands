using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

using static Finite.Commands.Parsing.ParameterHelper;

namespace Finite.Commands.Parsing
{
    internal sealed class PositionalCommandParser : ICommandParser
    {
        private readonly ICommandStore _commandStoreRoot;

        private static readonly char[] StandardGroupChars
            = new[] { '"', '\'', ' ' };

        public PositionalCommandParser(ICommandStore commandStore)
        {
            _commandStoreRoot = commandStore;
        }

        public void Parse(CommandContext context, string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message == string.Empty)
                throw new ArgumentException(null, nameof(message));

            var store = _commandStoreRoot;
            IEnumerable<ICommand>? commands = null;
            Index? start = null;
            int parameterCount = 0;

            foreach (var token in Lex(message))
            {
                if (start == null)
                    start = token.Portion.Start;

                var potentialGroup = store.GetCommandGroup(token);
                if (potentialGroup != null)
                {
                    if (token.Value.IndexOfAny(StandardGroupChars) >= 0 ||
                        HasUnexpectedQuotedPortion(token.Value))
                        throw new ArgumentException(
                            "Command group contains unexpected quoted portion",
                            nameof(message));

                    // The current token represents the name of a group
                    // e.g. "group" in "group name param1"
                    store = potentialGroup;
                }
                else if (!context.Path.HasValue)
                {
                    if (token.Value.IndexOfAny(StandardGroupChars) >= 0 ||
                        HasUnexpectedQuotedPortion(token.Value))
                        throw new ArgumentException(
                            "Command group contains unexpected quoted portion",
                            nameof(message));

                    // The current token represents the name of a command
                    // e.g. "name" in "group name param1"
                    Debug.Assert(start != null);

                    commands = store.GetCommands(token);

                    if (!commands.Any())
                        throw new ArgumentException(
                            $"Command store has no command '{token}'",
                            nameof(message));

                    context.Path = new CommandPath(message,
                        start.Value..token.Portion.End);
                }
                else
                {
                    // The current token represents a parameter to a command
                    // e.g. "param1" in "group name param1"

                    context.Items[GetParameterName(parameterCount)] = token;
                    parameterCount++;
                }
            }

            context.Items[TokenCount] = parameterCount;

            Debug.Assert(commands != null);

            // TODO: make this injectable somehow
            var binder = context.Services.GetRequiredService<ICommandBinder>();

            foreach (var command in commands)
            {
                if (binder.TryBind(context, command))
                {
                    context.Command = command;
                    return;
                }
            }

            throw new Exception("Failed to bind to any commands");

            static bool HasUnexpectedQuotedPortion(ReadOnlySpan<char> span)
            {
                foreach (var c in span)
                {
                    var category = char.GetUnicodeCategory(c);

                    if (category == UnicodeCategory.SpaceSeparator ||
                        category == UnicodeCategory.InitialQuotePunctuation ||
                        category == UnicodeCategory.FinalQuotePunctuation)
                        return true;
                }

                return false;
            }
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
