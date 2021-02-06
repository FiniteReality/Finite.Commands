using System;
using System.Diagnostics;
using Finite.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands.Core
{
    internal sealed class DefaultCommandResultExecutorFactory
        : ICommandResultExecutorFactory
    {
        private static readonly Type ICommandResult = typeof(ICommandResult);
        private static readonly Type CommandResultExecutorWrapper
            = typeof(CommandResultExecutorWrapper<>);
        private readonly IServiceProvider _services;

        public DefaultCommandResultExecutorFactory(IServiceProvider services)
        {
            _services = services;
        }

        public ICommandResultExecutor GetExecutor(Type resultType)
        {
            Debug.Assert(ICommandResult.IsAssignableFrom(resultType));

            var type = CommandResultExecutorWrapper.MakeGenericType(resultType);
            return (ICommandResultExecutor)_services.GetRequiredService(type);
        }

        public ICommandResultExecutor<TResult> GetExecutor<TResult>()
            where TResult : ICommandResult
            => _services.GetRequiredService<ICommandResultExecutor<TResult>>();
    }
}
