using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finite.Commands
{
    internal class CommandHostedService : BackgroundService, ICommandExecutor
    {
        private static readonly Action<ILogger, CommandPath, Exception?> CommandExecuting
            = LoggerMessage.Define<CommandPath>(
                LogLevel.Information,
                new EventId(0, nameof(CommandExecuting)),
                "Executing command {comand}");

        private static readonly Action<ILogger, CommandPath, Exception?> CommandExecuted
            = LoggerMessage.Define<CommandPath>(
                LogLevel.Information,
                new EventId(1, nameof(CommandExecuted)),
                "Executed command {comand}");


        private readonly ICommandContextFactory _contextFactory;
        private readonly ILogger _logger;

        private readonly Channel<CommandContext> _executionRequests;

        public CommandHostedService(
            ICommandContextFactory factory,
            ILogger<ICommandExecutor> logger)
        {
            _contextFactory = factory;
            _logger = logger;

            _executionRequests = Channel.CreateUnbounded<CommandContext>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public ValueTask ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken)
            => _executionRequests.Writer.WriteAsync(context,
                cancellationToken);

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var reader = _executionRequests.Reader;
            await foreach (var context in reader.ReadAllAsync(stoppingToken))
            {
                CommandExecuting(_logger, context.Path, null);
                // TODO: execute command
                CommandExecuted(_logger, context.Path, null);
                _contextFactory.ReleaseContext(context);
            }
        }
    }
}
