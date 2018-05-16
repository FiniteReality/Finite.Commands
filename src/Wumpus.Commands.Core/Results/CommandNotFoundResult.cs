namespace Wumpus.Commands
{
    /// <summary>
    /// Struct used to represent command not found results.
    /// </summary>
    public struct CommandNotFoundResult : IResult
    {
        private static readonly CommandNotFoundResult _instance
            = new CommandNotFoundResult();

        public static CommandNotFoundResult Instance
            => _instance;

        public bool IsSuccess
            => false;
    }
}
