using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Finite.Commands.Parsing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    public class LineReaderService : BackgroundService
    {
        private readonly ICommandContextFactory _commandContextFactory;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ICommandParser _commandParser;
        private readonly ICommandStore _commandStore;
        private readonly ILogger _logger;

        public LineReaderService(
            ICommandContextFactory commandContextFactory,
            ICommandExecutor commandExecutor,
            ICommandParser commandParser,
            ICommandStore commandStore,
            ILogger<LineReaderService> logger)
        {
            _commandContextFactory = commandContextFactory;
            _commandExecutor = commandExecutor;
            _commandParser = commandParser;
            _commandStore = commandStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogWarning(
                "Starting LineReaderService. Console output may appear "+
                "garbled.");

            // HACK: Since the command store has no way of getting *all*
            // registered commands, we have to do this currently
            var commandCount = ((List<ICommand>)_commandStore.GetType()
                .GetField("_currentCommands",
                    BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(_commandStore)!)
                .Count;

            _logger.LogInformation("Command store contains {count} commands",
                commandCount);

            // N.B. since Console.ReadLine() can't be cancelled or done
            // asynchronously, we forcibly yield here to ensure other services
            // can be started.
            await Task.Yield();

            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var command = Console.ReadLine();
                _logger.LogDebug("Command: {command}", command);

                if (command != null)
                {
                    var context = _commandContextFactory.CreateContext();

                    try
                    {
                        _commandParser.Parse(context, command);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            "Failed to parse {command}",
                            command);

                        continue;
                    }

                    foreach (var paramPair in context.Parameters)
                    {
                        _logger.LogInformation(
                            "Parameter {key} ({type}) = {value}",
                            paramPair.Key,
                            paramPair.Value?.GetType() ?? typeof(object),
                            paramPair.Value);
                    }

                    await _commandExecutor.ExecuteAsync(context,
                        stoppingToken);
                }
            }
        }
    }
}
