using System;

namespace Finite.Commands
{
    /// <summary>
    /// Marks a method as a callback invoked when a command is being invoked.
    /// <seealso cref="OnExecutingCallback"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,
        Inherited = true, AllowMultiple = false)]
    sealed class OnExecutingAttribute : Attribute
    { }
}
