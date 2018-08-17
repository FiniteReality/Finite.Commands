using System;

namespace Finite.Commands
{
    /// <summary>
    /// Marks a method as a callback invoked when a module is being built.
    /// <seealso cref="OnBuildingCallback"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,
        Inherited = true, AllowMultiple = false)]
    public sealed class OnBuildingAttribute : Attribute
    { }
}
