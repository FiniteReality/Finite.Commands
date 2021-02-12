using System;
using System.Collections.Generic;
using Finite.Commands;

namespace ConsoleCommands
{
    public class OtherCommand : ICommand
    {
        public CommandPath Name { get; } = new CommandPath("other");

        public IReadOnlyList<IParameter> Parameters
            => Array.Empty<IParameter>();
    }
}
