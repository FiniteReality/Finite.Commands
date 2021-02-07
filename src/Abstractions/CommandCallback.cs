using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// A function that can process a command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="CommandContext"/>.
    /// </param>
    /// <returns>
    /// A task that represents the completion of request processing.
    /// </returns>
    public delegate ValueTask<ICommandResult> CommandCallback(CommandContext context);
}
