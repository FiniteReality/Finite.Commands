using System;

namespace Wumpus.Commands
{
    /// <summary>
    /// An attribute adding multiple aliases to a module or parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter,
        Inherited = true, AllowMultiple = false)]
    public sealed class AliasAttribute : Attribute
    {
        /// <summary>
        /// The aliases added to the current target.
        /// </summary>
        /// <value>
        /// The Aliases property gets the list of aliases added to the target.
        /// </value>
        public string[] Aliases { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AliasAttribute"/>.
        /// </summary>
        /// <param name="aliases">
        /// A list of aliases to add to the target.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="AliasAttribute.AliasAttribute(string[])"/>
        /// is called without any parameters.
        /// </exception>
        public AliasAttribute(params string[] aliases)
        {
            if (aliases.Length == 0)
                throw new InvalidOperationException(
                    "At least one alias must be given.");

            Aliases = aliases;
        }
    }
}
