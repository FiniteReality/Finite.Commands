using System.Threading;
using System.Threading.Tasks;
using Finite.Commands.Abstractions;

namespace Finite.Commands.Core
{
    internal sealed class DefaultCommandParser : ICommandParser
    {
        private readonly ICommandContextFactory _contextFactory;

        public DefaultCommandParser(ICommandContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ValueTask<CommandContext> ParseAsync(string message,
            CancellationToken cancellationToken = default)
        {
            var context = _contextFactory.CreateContext();

            return new ValueTask<CommandContext>(context);
        }
    }
}
