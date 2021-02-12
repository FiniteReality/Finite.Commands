using System;
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
        private readonly ILogger _logger;

        public LineReaderService(
            ICommandContextFactory commandContextFactory,
            ICommandParser commandParser,
            ICommandExecutor commandExecutor,
            ILogger<LineReaderService> logger)
        {
            _commandContextFactory = commandContextFactory;
            _commandExecutor = commandExecutor;
            _commandParser = commandParser;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogWarning(
                "Starting LineReaderService. Console output may appear "+
                "garbled.");

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
                        await _commandParser.ParseAsync(context, command,
                            stoppingToken);
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
                        _logger.LogInformation("Parameter {key} = {value}",
                            paramPair.Key, paramPair.Value);
                    }

                    await _commandExecutor.ExecuteAsync(context,
                        stoppingToken);
                }
            }
        }
    }
}
