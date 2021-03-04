using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Defines a function used to transfer control in the middleware pipeline.
    /// </summary>
    /// <returns>
    /// A task that represents the completion of request processing.
    /// </returns>
    public delegate ValueTask<ICommandResult> CommandMiddleware();
}
