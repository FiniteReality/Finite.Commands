using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Finite.Commands
{
    internal sealed class DefaultCommandStore
        : ICommandStore, IDisposable
    {
        private readonly ICommandProvider[] _commandProviders;
        private readonly IDisposable _commandChangeToken;
        private readonly List<ICommand> _currentCommands;

        private CancellationTokenSource _reloadCancellation;

        public DefaultCommandStore(
            IEnumerable<ICommandProvider> commandProviders)
        {
            _commandProviders = commandProviders.ToArray();

            _currentCommands = new();

            _commandChangeToken = ChangeToken.OnChange(
                () => new CompositeChangeToken(_commandProviders
                        .Select(static x => x.GetChangeToken())
                        .ToList()),
                OnCommandsChanged, this);

            OnCommandsChanged(this);
            Debug.Assert(_reloadCancellation != null);
        }

        public void Dispose()
        {
            _commandChangeToken.Dispose();
            _reloadCancellation.Dispose();
        }

        private static void OnCommandsChanged(object state)
        {
            var store = (DefaultCommandStore)state;

            var currentCancelTokenSource = new CancellationTokenSource();
            Interlocked.Exchange(
                ref store._reloadCancellation, currentCancelTokenSource)
                ?.Cancel();

            currentCancelTokenSource.Token.ThrowIfCancellationRequested();
            store._currentCommands.Clear();

            foreach (var provider in store._commandProviders)
            {
                currentCancelTokenSource.Token.ThrowIfCancellationRequested();
                store._currentCommands.AddRange(provider.GetCommands());
            }

            currentCancelTokenSource.Cancel();
        }

        public ICommandStoreSection? GetCommandGroup(CommandString prefix)
            => TryGetCommandGroup(prefix, out var section) ? section : null;

        public IEnumerable<ICommand> GetCommands(CommandString name)
        {
            foreach (var command in _currentCommands)
                if (name == command.Name)
                    yield return command;
        }

        private bool TryGetCommandGroup(CommandString prefix,
            [NotNullWhen(true)]
            out ICommandStoreSection? section)
        {
            foreach (var command in _currentCommands)
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
