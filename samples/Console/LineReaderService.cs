using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    public class LineReaderService : BackgroundService
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly ICommandParser _commandParser;
        private readonly ILogger _logger;

        public LineReaderService(
            ICommandParser commandParser,
            ICommandExecutor commandExecutor,
            ILogger<LineReaderService> logger)
        {
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
                    var context = await _commandParser.ParseAsync(command,
                        stoppingToken);
                    await _commandExecutor.ExecuteAsync(context,
                        stoppingToken);
                }
            }
        }
    }
}
