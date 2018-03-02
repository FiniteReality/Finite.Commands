namespace Wumpus.Commands
{
    public abstract class ModuleBase<TContext>
        : IModule<TContext>
        where TContext : ICommandContext
    {
        public TContext Context { get; }

        ICommandContext IModule.Context
            => Context;
    }
}
