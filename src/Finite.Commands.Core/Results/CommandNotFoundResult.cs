namespace Finite.Commands
{
    /// <summary>
    /// Struct used to represent command not found results.
    /// </summary>
    public struct CommandNotFoundResult : IResult
    {
        /// <summary>
        /// The singleton instance for CommandNotFoundResult
        /// </summary>
        public static CommandNotFoundResult Instance { get; } = new CommandNotFoundResult();

        /// <inheritdoc/>
        public bool IsSuccess
            => false;
    }
}
