using System;
using System.Collections.Generic;
using System.Text;

namespace Finite.Commands
{
    public partial class DefaultCommandParser
    {
        private enum TokenizerState
        {
            Normal,
            EscapeCharacter,
            ParameterSeparator,
            QuotedString
        }

        /// <summary>
        /// Checks whether the given character is a quotation character, used
        /// to delimit quoted strings.
        /// </summary>
        /// <param name="quoteCharacter">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <code>true</code> when the parameter is a quotation character.
        /// </returns>
        protected virtual bool IsQuoteCharacter(char quoteCharacter)
        {
            return quoteCharacter == '\'' || quoteCharacter == '"';
        }

        /// <summary>
        /// Checks whether the given quotations are valid quotation characters
        /// for long strings in parameters.
        /// </summary>
        /// <param name="startQuote">
        /// The start quote to check.
        /// </param>
        /// <param name="endQuote">
        /// The end quote to check.
        /// </param>
        /// <returns>
        /// <code>true</code> when the quotes correspond to a valid long string
        /// parameter.
        /// </returns>
        protected virtual bool IsCompletedQuote(char startQuote,
            char endQuote)
        {
            return (startQuote == endQuote) &&
                (startQuote == '\'' || startQuote == '"');
        }

        /// <summary>
        /// Checks whether the given character is an escape character.
        /// </summary>
        /// <param name="escapeCharacter">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <code>true</code> when the character is an escape character.
        /// </returns>
        protected virtual bool IsEscapeCharacter(char escapeCharacter)
            => escapeCharacter == '\\';

        /// <summary>
        /// Checks whether the given character is an escapable character.
        /// </summary>
        /// <param name="escapedCharacter">
        /// The character to check.
        /// </param>
        /// <returns>
        /// <code>true</code> when the character is an escapable character.
        /// </returns>
        protected virtual bool IsEscapableCharacter(char escapedCharacter)
        {
            return
                // Parser characters
                IsEscapeCharacter(escapedCharacter) ||
                IsQuoteCharacter(escapedCharacter) ||

                // Markdown
                escapedCharacter == '*' || escapedCharacter == '_' ||
                escapedCharacter == '~' ||

                // Tags
                escapedCharacter == '<' || escapedCharacter == '@' ||
                escapedCharacter == '#';
        }


        /// <summary>
        /// Tokenizes a command string into a list of command segments.
        /// </summary>
        /// <param name="commandText">
        /// The command text to tokenize.
        /// </param>
        /// <param name="prefixLength">
        /// The number of characters of <paramref name="commandText"/> to skip.
        /// </param>
        /// <returns>
        /// An array of strings representing the individual tokens contained in
        /// <paramref name="commandText"/>.
        /// </returns>
        protected virtual string[] Tokenize(string commandText,
            int prefixLength)
        {
            if (prefixLength >= commandText.Length)
                throw new ArgumentOutOfRangeException(
                    nameof(prefixLength));

            var paramBuilder = new StringBuilder();
            var result = new List<string>();
            var state = TokenizerState.Normal;
            var beginQuote = default(char);

            for (int i = prefixLength; i < commandText.Length; i++)
            {
                char c = commandText[i];
                var isLastCharacter = i == commandText.Length - 1;

                switch (state)
                {
                    case TokenizerState.Normal
                        when char.IsWhiteSpace(c):
                        result.Add(paramBuilder.ToString());
                        state = TokenizerState.ParameterSeparator;
                        break;
                    case TokenizerState.Normal
                        when IsEscapeCharacter(c) && isLastCharacter:
                        throw new TokenizerException(
                            "An escape sequence was left unfinished",
                            commandText, i);
                    case TokenizerState.Normal
                        when IsQuoteCharacter(c):
                        throw new TokenizerException(
                            "A quote must either be escaped or " +
                            "preceeded by a space to begin a quoted string",
                            commandText, i);
                    case TokenizerState.Normal
                        when IsEscapeCharacter(c):
                        state = TokenizerState.EscapeCharacter;
                        break;

                    case TokenizerState.EscapeCharacter
                        when IsEscapableCharacter(c):
                        state = TokenizerState.Normal;
                        goto default;
                    case TokenizerState.EscapeCharacter:
                        throw new TokenizerException(
                            $"The character '{c}' cannot be escaped",
                            commandText, i);

                    case TokenizerState.ParameterSeparator
                        when IsQuoteCharacter(c) && isLastCharacter:
                        throw new TokenizerException(
                            "A quoted string was not finished",
                            commandText, i);
                    case TokenizerState.ParameterSeparator
                        when IsQuoteCharacter(c):
                        state = TokenizerState.QuotedString;
                        beginQuote = c;
                        paramBuilder.Clear();
                        break;
                    case TokenizerState.ParameterSeparator
                        when !char.IsWhiteSpace(c):
                        state = TokenizerState.Normal;
                        paramBuilder.Clear();
                        goto default;

                    case TokenizerState.QuotedString
                        when IsCompletedQuote(beginQuote, c):
                        state = TokenizerState.Normal;
                        break;
                    case TokenizerState.QuotedString
                        when isLastCharacter:
                        throw new TokenizerException(
                            "A quoted string was not finished",
                            commandText, i);

                    default:
                        paramBuilder.Append(c);
                        break;
                }
            }

            // Add any final parameters
            result.Add(paramBuilder.ToString());

            if (state != TokenizerState.Normal)
            {
                throw new TokenizerException(
                    "The tokenizer did not finish in the " +
                    $"{nameof(TokenizerState.Normal)} state.",
                    commandText, commandText.Length);
            }

            return result.ToArray();
        }
    }
}
