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
        private static readonly Action<ILogger, CommandString, Exception?> CommandExecuting
            = LoggerMessage.Define<CommandString>(
                LogLevel.Information,
                new EventId(0, nameof(CommandExecuting)),
                "Executing command {command}");

        private static readonly Action<ILogger, CommandString, Exception?> CommandExecuted
            = LoggerMessage.Define<CommandString>(
                LogLevel.Information,
                new EventId(1, nameof(CommandExecuted)),
                "Executed command {command}");

        private static readonly Action<ILogger, CommandString, Exception?> CommandFailed
            = LoggerMessage.Define<CommandString>(
                LogLevel.Warning,
                new EventId(2, nameof(CommandFailed)),
                "Failed to execute {command}");

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
                try
                {
                    await context.Command.ExecuteAsync(context, stoppingToken);
                }
                catch (Exception e)
                {
                    CommandFailed(_logger, context.Path, e);
                }
                finally
                {
                    CommandExecuted(_logger, context.Path, null);
                    _contextFactory.ReleaseContext(context);
                }
            }
        }
    }
}
