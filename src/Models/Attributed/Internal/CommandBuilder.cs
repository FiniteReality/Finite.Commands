using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Finite.Commands.AttributedModel
{
    internal class CommandBuilder : ICommandProvider, IDisposable
    {
        private static readonly Type ICommandType = typeof(ICommand);

        private readonly List<CommandLoadContext> _loadContexts;
        private readonly IDisposable _optionsChangeMonitor;

        private CancellationTokenSource _reloadCancellation;

        public CommandBuilder(
            IOptionsMonitor<AttributedCommandOptions> options)
        {
            _loadContexts = new();

            _optionsChangeMonitor = options.OnChange(ReloadAssemblies);
            ReloadAssemblies(options.CurrentValue);

            Debug.Assert(_reloadCancellation != null);
        }

        private void ReloadAssemblies(AttributedCommandOptions options)
        {
            var currentCancelTokenSource = new CancellationTokenSource();
            Interlocked.Exchange(
                ref _reloadCancellation, currentCancelTokenSource)
                ?.Cancel();

            foreach (var context in _loadContexts)
            {
                currentCancelTokenSource.Token.ThrowIfCancellationRequested();
                context.Dispose();
            }

            currentCancelTokenSource.Token.ThrowIfCancellationRequested();
            _loadContexts.Clear();

            foreach (var assembly in options.Assemblies)
            {
                currentCancelTokenSource.Token.ThrowIfCancellationRequested();
                _loadContexts.Add(new CommandLoadContext(assembly));
            }

            currentCancelTokenSource.Cancel();
        }

        public void Dispose()
        {
            _optionsChangeMonitor.Dispose();
            _reloadCancellation.Cancel();
            foreach (var context in _loadContexts)
                context.Dispose();
        }

        public IEnumerable<ICommand> GetCommands()
        {
            foreach (var context in _loadContexts)
                foreach (var type in context.EntryPoint.ExportedTypes)
                    if (ICommandType.IsAssignableFrom(type))
                        yield return (ICommand)Activator.CreateInstance(type)!;
        }

        public IChangeToken GetChangeToken()
            => new CancellationChangeToken(_reloadCancellation.Token);
    }
}
