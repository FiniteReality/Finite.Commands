using System;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines an attribute which can be used to mark a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets or initializes the name of this command group.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Creates a new <see cref="CommandAttribute"/>.
        /// </summary>
        public CommandAttribute()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="CommandAttribute"/> with the given
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of this command group.
        /// </param>
        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
