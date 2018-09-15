using System;
using System.Collections.Generic;
using System.Text;

namespace Finite.Commands
{
    /// <summary>
    /// A set of reasons why the <see cref="DefaultCommandParser"/> may fail.
    /// </summary>
    public enum TokenizerFailureReason
    {
        /// <summary>
        /// An escape sequence was left unfinished
        /// </summary>
        UnfinishedEscapeSequence,
        /// <summary>
        /// An escape sequence was invalid
        /// </summary>
        InvalidEscapeSequence,
        /// <summary>
        /// A quoted string was left unfinished
        /// </summary>
        UnfinishedQuotedString,
        /// <summary>
        /// A quote was read where it was not expected
        /// </summary>
        UnexpectedQuote,
        /// <summary>
        /// The tokenizer finished in a state that wasn't expected
        /// </summary>
        InvalidState
    }

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
        protected virtual TokenizerResult Tokenize(string commandText,
            int prefixLength)
        {
            TokenizerResult Failure(TokenizerFailureReason reason,
                int position)
            {
                return new TokenizerResult((int)reason, commandText,
                    position);
            }

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
                        return Failure(
                            TokenizerFailureReason.UnfinishedEscapeSequence,
                            i);
                    case TokenizerState.Normal
                        when IsQuoteCharacter(c):
                        return Failure(
                            TokenizerFailureReason.UnexpectedQuote, i);
                    case TokenizerState.Normal
                        when IsEscapeCharacter(c):
                        state = TokenizerState.EscapeCharacter;
                        break;

                    case TokenizerState.EscapeCharacter
                        when IsEscapableCharacter(c):
                        state = TokenizerState.Normal;
                        goto default;
                    case TokenizerState.EscapeCharacter:
                        return Failure(
                            TokenizerFailureReason.InvalidEscapeSequence, i);

                    case TokenizerState.ParameterSeparator
                        when IsQuoteCharacter(c) && isLastCharacter:
                        return Failure(
                            TokenizerFailureReason.UnfinishedQuotedString, i);
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
                        return Failure(
                            TokenizerFailureReason.UnfinishedQuotedString, i);

                    default:
                        paramBuilder.Append(c);
                        break;
                }
            }

            // Add any final parameters
            result.Add(paramBuilder.ToString());

            if (state != TokenizerState.Normal)
                return Failure(TokenizerFailureReason.InvalidState,
                    commandText.Length);

            return new TokenizerResult(result.ToArray());
        }
    }
}
