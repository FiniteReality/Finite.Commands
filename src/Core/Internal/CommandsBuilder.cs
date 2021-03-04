using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal class CommandsBuilder : ICommandsBuilder
    {
        private readonly CommandMiddlewareProvider _middlewareProvider;

        public CommandsBuilder(CommandMiddlewareProvider middlewareProvider,
            IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            Services = services;

            _middlewareProvider = middlewareProvider;
        }

        public IServiceCollection Services { get; }

        public ICommandsBuilder Use(Func<CommandMiddleware, CommandContext, CancellationToken, ValueTask<ICommandResult>> middleware)
        {
            _middlewareProvider.Add(middleware);

            return this;
        }
    }
}
