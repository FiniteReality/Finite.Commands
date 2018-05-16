namespace Wumpus.Commands
{
    public interface ICommandContext<TContext> : ICommandContext
        where TContext : class, ICommandContext<TContext>
    {
        CommandService<TContext> Commands { get; }
    }
}
