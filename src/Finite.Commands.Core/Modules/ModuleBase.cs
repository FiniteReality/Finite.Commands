namespace Finite.Commands
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
        where TContext : class, ICommandContext
    {
        /// <summary>
        /// The contextual data passed to the command, such as message author
        /// and message content.
        /// </summary>
        public TContext Context { get; private set; }

        /// <summary>
        /// The command service owning this module.
        /// </summary>
        public CommandService<TContext> Commands { get; private set; }

        /// <summary>
        /// A callback executed when the module is about to execute a command.
        /// </summary>
        /// <param name="command">
        /// The <see cref="CommandInfo"/> which is about to execute.
        /// </param>
        protected virtual void OnExecuting(CommandInfo command)
        { }

        internal void SetContext(ICommandContext context)
        {
            Context = (TContext)context;
        }

        internal void SetCommands(ICommandService commands)
        {
            Commands = (CommandService<TContext>)commands;
        }

        internal void CallOnExecuting(CommandInfo command)
        {
            OnExecuting(command);
        }
    }
}
