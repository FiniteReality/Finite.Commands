using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Finite.Commands.AttributedModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    [Group("hello")]
    public class HelloModule : Module
    {
        private readonly ILogger _logger;

        public HelloModule(ILogger<HelloModule> logger)
        {
            _logger = logger;
        }

        [Command("world")]
        public ValueTask<ICommandResult> HelloWorldCommand(
            int coolParameter, [Remainder]string coolerParameter)
        {
            _logger.LogInformation("Hello world from HelloModule!");

            return new ValueTask<ICommandResult>(new NoContentCommandResult());
        }
    }
}
