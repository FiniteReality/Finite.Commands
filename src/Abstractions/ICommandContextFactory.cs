namespace Finite.Commands.Abstractions
{
    /// <summary>
    /// Defines an interface which can be used to create instances of
    /// <see cref="CommandContext"/>.
    /// </summary>
    public interface ICommandContextFactory
    {
        /// <summary>
        /// Creates a <see cref="CommandContext"/> which can be populated.
        /// </summary>
        /// <returns>
        /// The created <see cref="CommandContext"/>.
        /// </returns>
        CommandContext CreateContext();

        /// <summary>
        /// Releases any resources held by the <see cref="CommandContext"/>.
        /// </summary>
        /// <param name="context">
        /// The <see cref="CommandContext"/> to release.
        /// </param>
        void ReleaseContext(CommandContext context);
    }
}
