using System;

namespace Wumpus.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter,
        Inherited = true, AllowMultiple = false)]
    public sealed class AliasAttribute : Attribute
    {
        public string[] Aliases { get; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
