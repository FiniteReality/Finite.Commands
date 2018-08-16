namespace Finite.Commands
{
    /// <summary>
    /// A <see cref="ICommandContext"/> restricted to a certain context type,
    /// intended to be used publicly.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context type this type has been restricted to.
    /// </typeparam>
    public interface ICommandContext<TContext> : ICommandContext
        where TContext : class, ICommandContext<TContext>
    {
        /// <summary>
        /// The command service which contains a command being executed.
        /// </summary>
        CommandService<TContext> Commands { get; }
    }
}
