using System.Collections.Generic;
using Finite.Commands;

namespace ConsoleCommands
{
    public class HelloWorldCommand : ICommand
    {
        public CommandPath Name { get; } = new CommandPath("hello world");

        public IReadOnlyCollection<IParameter> Parameters => throw new System.NotImplementedException();
    }
}
