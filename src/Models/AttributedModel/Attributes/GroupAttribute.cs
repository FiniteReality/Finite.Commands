using System;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines an attribute which can be used to mark a group of commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        /// <summary>
        /// Gets or initializes the name of this command group.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Creates a new <see cref="GroupAttribute"/>.
        /// </summary>
        public GroupAttribute()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="GroupAttribute"/> with the given
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of this command group.
        /// </param>
        public GroupAttribute(string name)
        {
            Name = name;
        }
    }
}
