using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Finite.Commands.Parsing;

namespace Finite.Commands.Parsing
{
    internal sealed class DefaultCommandParser : ICommandParser
    {
        public ValueTask ParseAsync(CommandContext context, string message,
            CancellationToken cancellationToken = default)
        {
            context.Path = new CommandPath(message);

            return default;
        }
    }
}
