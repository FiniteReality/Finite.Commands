namespace Wumpus.Commands
{
    public class ModuleBase<TContext>
        : IModule<TContext>
        where TContext : ICommandContext
    {
        public TContext Context { get; }

        ICommandContext IModule.Context
            => Context;
    }
}
