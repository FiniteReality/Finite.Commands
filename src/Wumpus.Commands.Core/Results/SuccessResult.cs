namespace Wumpus.Commands
{
    /// <summary>
    /// Internal struct used to represent unknown success results.
    /// </summary>
    internal struct SuccessResult : IResult
    {
        public static readonly SuccessResult Instance
            = new SuccessResult();

        /// <inheritdoc/>
        public bool IsSuccess
            => true;
    }
}
