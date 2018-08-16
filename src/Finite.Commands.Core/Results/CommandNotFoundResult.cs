namespace Finite.Commands
{
    /// <summary>
    /// Struct used to represent command not found results.
    /// </summary>
    public struct CommandNotFoundResult : IResult
    {
        private static readonly CommandNotFoundResult _instance
            = new CommandNotFoundResult();

        /// <summary>
        /// The singleton instance for CommandNotFoundResult
        /// </summary>
        public static CommandNotFoundResult Instance
            => _instance;

        /// <inheritdoc/>
        public bool IsSuccess
            => false;
    }
}
