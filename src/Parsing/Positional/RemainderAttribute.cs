using System;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines an attribute which can be used to mark a parameter as consuming
    /// the rest of the input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RemainderAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="RemainderAttribute"/>.
        /// </summary>
        public RemainderAttribute()
        { }
    }
}
