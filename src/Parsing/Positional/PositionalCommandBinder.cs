using System;
using System.Diagnostics;
using System.Globalization;
using static Finite.Commands.Parsing.ParameterHelper;

namespace Finite.Commands.Parsing
{
    internal sealed class PositionalCommandBinder : ICommandBinder
    {
        private static readonly char[] StandardQuotes = { '"', '\'' };

        private readonly IParameterBinderFactory _binderFactory;

        public PositionalCommandBinder(IParameterBinderFactory binderFactory)
        {
            _binderFactory = binderFactory;
        }

        public bool TryBind(CommandContext context, ICommand command)
        {
            var tokenCount = (int)context.Items[TokenCount]!;

            for (int x = 0; x < command.Parameters.Count; x++)
            {
                var parameter = command.Parameters[x];
                var binder = _binderFactory.GetBinder(parameter.Type);
                CommandString token;

                if (parameter is IRemainderParameter &&
                    x == command.Parameters.Count - 1)
                {
                    if (x == tokenCount - 1)
                    {
                        // If we're the last token anyway, just bail out and
                        // reuse the existing token

                        token = (CommandString)context.Items[
                            GetParameterName(x)]!;
                    }
                    else
                    {
                        // Build a new token consisting of the rest of the
                        // tokens. Quotes here are preserved as remainder
                        // parameters treat everything like it was one string,
                        // with all the necessary escapes.

                        string rawToken = default!;
                        Index startIndex = default;
                        Index endIndex = default;

                        for (int pos = x; pos < tokenCount; pos++)
                        {
                            var originalToken = (CommandString)context.Items[
                                GetParameterName(pos)]!;

                            if (pos == x)
                            {
                                rawToken = originalToken.RawValue;
                                startIndex = originalToken.Portion.Start;
                                endIndex = originalToken.Portion.End;
                            }
                            else
                            {
                                Debug.Assert(rawToken == originalToken.RawValue);

                                endIndex = originalToken.Portion.End;
                            }
                        }

                        token = new CommandString(rawToken,
                            startIndex..endIndex);
                    }
                }
                else
                {
                    token = (CommandString)context.Items[GetParameterName(x)]!;

                    // Remove any quotation marks on normal parameters as
                    // they're not relevant
                    if (IsStandardQuoted(token.Value) ||
                        IsQuotePair(token.Value[0], token.Value[^1]))
                    {
                        var startIndex = token.Portion.Start.AddOffset(1);
                        var endIndex = token.Portion.End.AddOffset(-1);

                        token = new CommandString(token.RawValue,
                            startIndex..endIndex);
                    }
                }

                // Attempt to bind the parameter with the given value.
                var value = binder.Bind(parameter, token.Value,
                    out var success);

                if (!success)
                    return false;

                context.Parameters[parameter.Name] = value;
            }

            return true;
        }

        private static bool IsStandardQuoted(ReadOnlySpan<char> text)
            => text.IndexOfAny(StandardQuotes) == 0 &&
                text.LastIndexOfAny(StandardQuotes) == text.Length - 1;

        private static bool IsQuotePair(char first, char last)
            => char.GetUnicodeCategory(first)
                    == UnicodeCategory.InitialQuotePunctuation
                && char.GetUnicodeCategory(last)
                    == UnicodeCategory.FinalQuotePunctuation;
    }
}
