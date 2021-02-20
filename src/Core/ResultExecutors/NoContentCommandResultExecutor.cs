using System.Threading.Tasks;

namespace Finite.Commands
{
    internal sealed class NoContentCommandResultExecutor
        : ICommandResultExecutor<NoContentCommandResult>
    {
        public ValueTask ExecuteResultAsync(CommandContext context,
            NoContentCommandResult result)
            => default;
    }
}
