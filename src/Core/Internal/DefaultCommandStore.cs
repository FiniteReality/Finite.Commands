using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Finite.Commands
{
    internal sealed class DefaultCommandStore
        : ICommandStore
    {
        private readonly IEnumerable<ICommand> _commands;

        public DefaultCommandStore(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommandStoreSection? GetCommandGroup(CommandString prefix)
            => TryGetCommandGroup(prefix, out var section) ? section : null;

        public IEnumerable<ICommand> GetCommands(CommandString name)
        {
            foreach (var command in _commands)
            {
                if (name == command.Name)
                {
                    yield return command;
                }
            }
        }

        private bool TryGetCommandGroup(CommandString prefix,
            [NotNullWhen(true)]
            out ICommandStoreSection? section)
        {
            foreach (var command in _commands)
            {
                var path = CommandPath.GetParentPath(command.Name);

                if (path == CommandString.Empty)
                    continue;

                if (prefix == CommandString.Empty &&
                    path != CommandString.Empty)
                {
                    section = new CommandStoreSection(this, prefix);
                    return true;
                }

                if (path.StartsWith(prefix))
                {
                    section = new CommandStoreSection(this, prefix);
                    return true;
                }
            }

            section = null;
            return false;
        }

        private sealed class CommandStoreSection : ICommandStoreSection
        {
            private readonly DefaultCommandStore _store;

            public CommandString Name { get; }

            public CommandStoreSection(DefaultCommandStore store,
                CommandString prefix)
            {
                _store = store;
                Name = prefix;
            }

            public ICommandStoreSection? GetCommandGroup(CommandString prefix)
                => _store.GetCommandGroup(CommandPath.Combine(Name, prefix));

            public IEnumerable<ICommand> GetCommands(CommandString name)
                => _store.GetCommands(CommandPath.Combine(Name, name));
        }
    }
}
