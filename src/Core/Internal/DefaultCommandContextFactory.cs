using Finite.Commands.Abstractions;

namespace Finite.Commands.Core
{
    internal sealed class DefaultCommandContextFactory : ICommandContextFactory
    {
        public CommandContext CreateContext()
            => new();

        public void ReleaseContext(CommandContext context)
        {
            /* no op */
        }
    }
}
