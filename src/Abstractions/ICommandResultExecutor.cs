using System;
using System.Threading.Tasks;

namespace Finite.Commands.Abstractions
{
    /// <summary>
    /// Defines an interface for a service which can execute a particular kind
    /// of <see cref="ICommandResult"/>, without knowing the type at compile
    /// time.
    /// </summary>
    /// <remarks>
    /// This type exists to support command infrastructure, and it is not
    /// intended to be implemented in production code.
    /// </remarks>
    public interface ICommandResultExecutor
    {
        /// <summary>
        /// Gets a value indicating the type of <see cref="ICommandResult"/>.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        /// Executes the result operation of the command or middleware,
        /// potentially asynchronously.
        /// </summary>
        /// <param name="context">
        /// The context in which the result is executed.
        /// </param>
        /// <param name="result">
        /// The result which was produced by a previous command or middleware.
        /// </param>
        /// <returns>
        /// A ValueTask which represents the completion of the execute
        /// operation.
        /// </returns>
        ValueTask ExecuteResultAsync(CommandContext context, object result);
    }

    /// <summary>
    /// Defines an interface for a service which can execute a particular kind
    /// of <see cref="ICommandResult" />.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of <see cref="ICommandResult"/>.
    /// </typeparam>
    public interface ICommandResultExecutor<TResult>
        where TResult : ICommandResult
    {
        /// <summary>
        /// Executes the result operation of the command or middleware,
        /// potentially asynchronously.
        /// </summary>
        /// <param name="context">
        /// The context in which the result is executed.
        /// </param>
        /// <param name="result">
        /// The result which was produced by a previous command or middleware.
        /// </param>
        /// <returns>
        /// A ValueTask which represents the completion of the execute
        /// operation.
        /// </returns>
        ValueTask ExecuteResultAsync(CommandContext context, TResult result);
    }
}
