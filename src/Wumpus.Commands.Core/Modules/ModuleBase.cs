namespace Wumpus.Commands
{
    /// <summary>
    /// A base class for modules which can be built with
    /// <see cref="ClassBuilder"/>.
    /// </summary>
    /// <typeparam name="TContext">
    /// A type which implements <see cref="ICommandContext"/> used for
    /// command contextual data.
    /// </typeparam>
    public abstract class ModuleBase<TContext>
        where TContext : ICommandContext
    {
        /// <summary>
        /// The contextual data passed to the command, such as message author
        /// and message content.
        /// </summary>
        public TContext Context { get; private set; }

        internal void SetContext(ICommandContext context)
        {
            Context = (TContext)context;
        }
    }
}
