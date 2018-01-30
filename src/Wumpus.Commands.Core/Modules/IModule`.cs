namespace Wumpus.Commands
{
    internal interface IModule<out TContext>
        : IModule
        where TContext : ICommandContext
    {
        new TContext Context { get; }
    }
}
