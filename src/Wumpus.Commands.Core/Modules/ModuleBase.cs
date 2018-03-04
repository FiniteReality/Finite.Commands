namespace Wumpus.Commands
{
    public abstract class ModuleBase<TContext>
        where TContext : ICommandContext
    {
        public TContext Context { get; private set; }

        internal void SetContext(ICommandContext context)
        {
            Context = (TContext)context;
        }
    }
}
