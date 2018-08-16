namespace Finite.Commands
{
    /// <summary>
    /// A class used for containing command matches.
    /// </summary>
    public class CommandMatch
    {
        /// <summary>
        /// The command matched by this match
        /// </summary>
        public CommandInfo Command { get; internal set; }

        /// <summary>
        /// The list of arguments to pass to this matched command
        /// </summary>
        public string[] Arguments { get; internal set; }
    }
}
