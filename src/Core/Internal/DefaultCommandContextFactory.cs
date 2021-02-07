using Finite.Commands;

namespace Finite.Commands
{
    internal sealed class DefaultCommandContextFactory : ICommandContextFactory
    {
        public CommandContext CreateContext()
            => new DefaultCommandContext();

        public void ReleaseContext(CommandContext context)
        {
            /* no op */
        }
    }
}
