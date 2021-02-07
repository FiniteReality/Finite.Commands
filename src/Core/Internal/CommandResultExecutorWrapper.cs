using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Finite.Commands
{
    internal sealed class CommandResultExecutorWrapper<TResult>
        : ICommandResultExecutor
        where TResult : ICommandResult
    {
        private readonly ICommandResultExecutor<TResult> _executor;

        public Type ResultType { get; } = typeof(TResult);

        public CommandResultExecutorWrapper(
            ICommandResultExecutor<TResult> executor)
        {
            _executor = executor;
        }

        public ValueTask ExecuteResultAsync(CommandContext context,
            object result)
        {
            Debug.Assert(result is TResult);

            return _executor.ExecuteResultAsync(context, (TResult)result);
        }
    }
}
