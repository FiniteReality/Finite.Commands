using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    public class OtherCommand : ICommand
    {
        private static readonly Random Rng = new();

        public CommandString Name { get; } = new CommandString("other");

        public IReadOnlyList<IParameter> Parameters
            => Array.Empty<IParameter>();

        public IReadOnlyDictionary<object, object?> Data { get; }
            = new Dictionary<object, object?>();

        public ValueTask<ICommandResult> ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken)
        {
            var logger = context.Services.GetRequiredService<ILogger<OtherCommand>>();

            logger.LogInformation("Hello, world from the command!");

            if (Rng.NextDouble() > 0.5)
                throw new Exception("This exception happens occasionally");

            return cancellationToken.IsCancellationRequested
                ? ValueTask.FromCanceled<ICommandResult>(cancellationToken)
                : new(new NoContentCommandResult());
        }
    }
}
