using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal class CommandsBuilder : ICommandsBuilder
    {
        private readonly IList<Func<CommandCallback, CommandCallback>> _middlewareTypes;

        public CommandsBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            Services = services;

            _middlewareTypes = new List<Func<CommandCallback, CommandCallback>>();
        }

        public IServiceCollection Services { get; }

        public ICommandsBuilder Use(Func<CommandCallback, CommandCallback> middleware)
        {
            _middlewareTypes.Add(middleware);

            return this;
        }
    }
}
