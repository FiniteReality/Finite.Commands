namespace Wumpus.Commands
{
    /// <summary>
    /// A class used for containing command matches.
    /// </summary>
    public class CommandMatch
    {
        /// <summary>
        /// The command matched by this match
        /// </summary>
        /// <value>
        /// The Command property gets a <see cref="CommandInfo"/> which was
        /// matched by a command search.
        /// </value>
        public CommandInfo Command { get; internal set; }

        /// <summary>
        /// The list of arguments to pass to this matched command
        /// </summary>
        /// <value>
        /// The Arguments property gets a list of strings which should be
        /// deserialized and passed to <see cref="CommandMatch.Command"/> when
        /// executed.
        /// </value>
        public string[] Arguments { get; internal set; }
    }
}
