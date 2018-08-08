using System;

namespace Wumpus.Commands
{
    /// <summary>
    /// An exception thrown when the default tokenizer encounters an error.
    /// </summary>
    public class TokenizerException : Exception
    {
        /// <summary>
        /// The input string which caused this error.
        /// </summary>
        /// <value>
        /// The InputString property gets the input string which caused the
        /// tokenizer error.
        /// </value>
        public string InputString { get; }

        /// <summary>
        /// The position of the input string which caused the error.
        /// </summary>
        /// <value>
        /// The Position property returns a zero-based index of
        /// <see cref="InputString"/> where the tokenizer error occured.
        /// </value>
        public int Position { get; }

        /// <summary>
        /// Constructs a new <see cref="TokenizerException"/>.
        /// </summary>
        /// <param name="message">
        /// The message of the exception.
        /// </param>
        /// <param name="input">
        /// The input string which caused the exception.
        /// </param>
        /// <param name="position">
        /// The zero-based index of the <paramref name="input"/> where the
        /// error occured.
        /// </param>
        public TokenizerException(string message, string input, int position)
            : base(message)
        {
            InputString = input;
            Position = position;
        }
    }
}
