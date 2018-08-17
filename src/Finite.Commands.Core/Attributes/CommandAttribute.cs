using System;

namespace Finite.Commands
{
    /// <summary>
    /// Marks a method as a command suitable to be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true,
        AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// The aliases used to invoke this command.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="aliases">
        /// A list of aliases to add to the command.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when
        /// <see cref="CommandAttribute.CommandAttribute(string[])"/> is called
        /// without any parameters.
        /// </exception>
        public CommandAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
