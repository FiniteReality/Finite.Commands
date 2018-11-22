namespace Finite.Commands
{
    // TODO: should this be sealed?

    /// <summary>
    /// A result returned when a tokenizer encounters an error.
    /// </summary>
    public sealed class TokenizerResult : IResult
    {
        /// <summary>
        /// The parsed token stream, when successful, or null otherwise.
        /// </summary>
        public string[] TokenStream { get; }

        /// <summary>
        /// The input string which caused the error, or null otherwise.
        /// </summary>
        public string InputString { get; }

        /// <summary>
        /// The position of the input string where the error occured, or null
        /// otherwise.
        /// </summary>
        public int? Position { get; }

        /// <summary>
        /// The reason why the error occured, or null otherwise.
        /// </summary>
        /// <remarks>
        /// If the current <see cref="TokenizerResult"/> was returned by the
        /// <see cref="DefaultCommandParser{TContext}"/>, then you should
        /// cast this to <see cref="TokenizerFailureReason"/> to retrieve a
        /// meaningful value from it.
        /// </remarks>
        public int? ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <summary>
        /// Constructs a new <see cref="TokenizerResult"/> which has failed.
        /// </summary>
        /// <param name="reason">
        /// The reason code the error occured.
        /// </param>
        /// <param name="input">
        /// The input string which caused the error.
        /// </param>
        /// <param name="position">
        /// The zero-based index of the <paramref name="input"/> where the
        /// error occured.
        /// </param>
        public TokenizerResult(int reason, string input, int position)
        {
            IsSuccess = false;

            ErrorReason = reason;
            InputString = input;
            Position = position;
        }

        /// <summary>
        /// Constructs a new <see cref="TokenizerResult"/> which has succeeded.
        /// </summary>
        /// <param name="tokenStream">
        /// The parsed token stream.
        /// </param>
        public TokenizerResult(string[] tokenStream)
        {
            IsSuccess = true;

            TokenStream = tokenStream;
        }
    }
}
