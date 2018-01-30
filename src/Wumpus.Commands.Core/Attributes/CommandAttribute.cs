using System;

namespace Wumpus.Commands
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false,
        AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
