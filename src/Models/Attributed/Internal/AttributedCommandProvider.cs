using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Finite.Commands.AttributedModel
{
    internal class AttributedCommandProvider : ICommandProvider, IDisposable
    {
        private static readonly Type ICommandType = typeof(ICommand);

        private readonly List<CommandLoadContext> _loadContexts;
        private readonly IDisposable _optionsChangeMonitor;

        private CancellationTokenSource _assembliesReloaded;

        public AttributedCommandProvider(
            IOptionsMonitor<AttributedCommandOptions> options)
        {
            _loadContexts = new();

            _assembliesReloaded = new();

            _optionsChangeMonitor = options.OnChange(ReloadAssemblies);
            ReloadAssemblies(options.CurrentValue);
        }

        private void ReloadAssemblies(AttributedCommandOptions options)
        {
            foreach (var context in _loadContexts)
            {
                context.Dispose();
            }

            _loadContexts.Clear();

            foreach (var assembly in options.Assemblies)
            {
                var context = new CommandLoadContext(assembly);
                _loadContexts.Add(context);

                CommandBuilder.BuildCommandsFor(context);
            }

            Interlocked.Exchange(ref _assembliesReloaded, new())?.Cancel();
        }

        public void Dispose()
        {
            _optionsChangeMonitor.Dispose();
            foreach (var context in _loadContexts)
                context.Dispose();
        }

        public IEnumerable<ICommand> GetCommands()
        {
            foreach (var context in _loadContexts)
                foreach (var type in context.CommandsAssembly.GetTypes())
                    if (ICommandType.IsAssignableFrom(type))
                        yield return (ICommand)Activator.CreateInstance(type)!;
        }

        public IChangeToken GetChangeToken()
            => new CancellationChangeToken(_assembliesReloaded.Token);
    }
}
