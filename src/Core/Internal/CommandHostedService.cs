using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Finite.Commands.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finite.Commands.Core
{
    internal class CommandHostedService : BackgroundService, ICommandExecutor
    {
        private static readonly Action<ILogger, string, Exception?> CommandExecuting
            = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(CommandExecuting)),
                "Executing command {comand}");

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
                CommandExecuting(_logger, "text", null);
                _contextFactory.ReleaseContext(context);
            }
        }
    }
}
