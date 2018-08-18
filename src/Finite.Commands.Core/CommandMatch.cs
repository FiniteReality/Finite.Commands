namespace Finite.Commands
{
    /// <summary>
    /// A class used for containing command matches.
    /// </summary>
    public sealed class CommandMatch
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandMatch" />
        /// </summary>
        /// <param name="command">
        /// The command which was matched
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to this command
        /// </param>
        /// <param name="path">
        /// The path of <paramref name="command" /> which was matched
        /// </path>
        internal CommandMatch(CommandInfo command, string[] arguments,
            string[] path)
        {
            Command = command;
            Arguments = arguments;
            CommandPath = path;
        }

        /// <summary>
        /// The command matched by this match
        /// </summary>
        public CommandInfo Command { get; }

        /// <summary>
        /// The list of arguments to pass to this matched command
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// The full path of the command which was matched
        /// </summary>
        public string[] CommandPath { get; }
    }
}
