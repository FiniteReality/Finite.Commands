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

        public ICommandStoreSection? GetCommandGroup(CommandPath prefix)
            => TryGetCommandGroup(prefix, out var section) ? section : null;

        public IEnumerable<ICommand> GetCommands(CommandPath name)
        {
            foreach (var command in _commands)
            {
                if (name == command.Name)
                {
                    yield return command;
                }
            }
        }

        private bool TryGetCommandGroup(CommandPath prefix,
            [NotNullWhen(true)]
            out ICommandStoreSection? section)
        {
            foreach (var command in _commands)
            {
                var path = command.Name.GetParentPath();

                if (path == CommandPath.Empty)
                    continue;

                if (prefix == CommandPath.Empty &&
                    path != CommandPath.Empty)
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

            public CommandPath Name { get; }

            public CommandStoreSection(DefaultCommandStore store,
                CommandPath prefix)
            {
                _store = store;
                Name = prefix;
            }

            public ICommandStoreSection? GetCommandGroup(CommandPath prefix)
                => _store.GetCommandGroup(CommandPath.Combine(Name, prefix));

            public IEnumerable<ICommand> GetCommands(CommandPath name)
                => _store.GetCommands(CommandPath.Combine(Name, name));
        }
    }
}
