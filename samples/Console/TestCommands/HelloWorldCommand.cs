using System;
using System.Collections.Generic;
using System.Linq;
using Finite.Commands;

namespace ConsoleCommands
{
    public class HelloWorldCommand : ICommand
    {
        public CommandPath Name { get; } = new CommandPath("hello world");

        public IReadOnlyList<IParameter> Parameters { get; }
            = GetParameters().ToArray();

        private static IEnumerable<IParameter> GetParameters()
        {
            yield return new Parameter("message", typeof(int));
            yield return new Parameter("cool", typeof(string));
        }
    }

    internal record Parameter(string Name, Type Type) : IParameter
    { }
}
