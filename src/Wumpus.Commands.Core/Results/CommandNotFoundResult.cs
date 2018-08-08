namespace Wumpus.Commands
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
        /// <value>
        /// Gets a static <see cref="CommandNotFoundResult"/> which represents
        /// a result indicating a command could not be found.
        /// </value>
        public static CommandNotFoundResult Instance
            => _instance;

        /// <inheritdoc/>
        public bool IsSuccess
            => false;
    }
}
