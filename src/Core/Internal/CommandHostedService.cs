using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finite.Commands
{
    internal class CommandHostedService : BackgroundService, ICommandExecutor
    {
        private readonly ICommandContextFactory _contextFactory;

        private readonly Channel<CommandContext> _executionRequests;

        public CommandHostedService(
            ICommandContextFactory contextFactory)
        {
            _contextFactory = contextFactory;

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
                var executor = context.Services
                    .GetRequiredService<CommandExecutor>();

                await executor.ExecuteAsync(context, stoppingToken);
                _contextFactory.ReleaseContext(context);
            }
        }
    }
}
