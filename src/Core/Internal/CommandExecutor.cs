using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Finite.Commands
{
    internal class CommandExecutor : ICommandExecutor
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

        private readonly ILogger _logger;
        private readonly ICommandResultExecutorFactory _resultExecutorFactory;

        public CommandExecutor(
            ILogger<ICommandExecutor> logger,
            ICommandResultExecutorFactory resultExecutorFactory)
        {
            _logger = logger;
            _resultExecutorFactory = resultExecutorFactory;
        }

        public async ValueTask ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken)
        {
            CommandExecuting(_logger, context.Path, null);
            try
            {
                var result = await context.Command
                    .ExecuteAsync(context, cancellationToken);

                var executor = _resultExecutorFactory.GetExecutor(
                    result.GetType());

                await executor.ExecuteResultAsync(context, result);
            }
            catch (Exception e)
            {
                CommandFailed(_logger, context.Path, e);
            }
            finally
            {
                CommandExecuted(_logger, context.Path, null);
            }
        }
    }
}
