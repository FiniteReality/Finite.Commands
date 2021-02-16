using System;
using System.Globalization;

namespace Finite.Commands
{
    internal sealed class StringBinder : IParameterBinder<string>
    {
        private static readonly char[] StandardQuotes = { '"', '\'' };

        public string Bind(IParameter parameter, ReadOnlySpan<char> text,
            out bool success)
        {
            if (IsStandardQuoted(text) || IsQuotePair(text[1], text[^1]))
            {
                text = text[1..^1];
            }

            success = true;

            return text.ToString();
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
