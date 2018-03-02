using System;

namespace Wumpus.Commands
{
    [AttributeUsage(AttributeTargets.Method,
        Inherited = true, AllowMultiple = false)]
    sealed class OnBuildingAttribute : Attribute
    { }
}
