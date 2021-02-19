using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    public class HelloWorldCommand : ICommand
    {
        public CommandString Name { get; } = new CommandString("hello world");

        public IReadOnlyList<IParameter> Parameters { get; }
            = GetParameters().ToArray();

        public IReadOnlyDictionary<object, object?> Data { get; }
            = new Dictionary<object, object?>();

        private static IEnumerable<IParameter> GetParameters()
        {
            yield return new Parameter("message", typeof(int));
            yield return new RemainderParameter("cool", typeof(string));
        }

        public ValueTask ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken)
        {
            var logger = context.Services.GetRequiredService<ILogger<HelloWorldCommand>>();

            logger.LogInformation("Hello, world from the command!");

            return cancellationToken.IsCancellationRequested
                ? ValueTask.FromCanceled(cancellationToken)
                : default;
        }
    }

    internal record Parameter(string Name, Type Type) : IParameter
    {
        public IReadOnlyDictionary<object, object?> Data { get; }
            = new Dictionary<object, object?>();
    }

    internal record RemainderParameter(string Name, Type Type)
        : IParameter, IRemainderParameter
    {
        public IReadOnlyDictionary<object, object?> Data { get; }
            = new Dictionary<object, object?>();
    }
}
