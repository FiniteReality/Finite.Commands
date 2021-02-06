using System;

namespace Finite.Commands.Abstractions
{
    /// <summary>
    /// Defines a factory for instances of
    /// <see cref="ICommandResultExecutor{T}"/>.
    /// </summary>
    public interface ICommandResultExecutorFactory
    {
        /// <summary>
        /// Gets an executor for the given <paramref name="resultType"/>.
        /// </summary>
        /// <param name="resultType">
        /// The type of <see cref="ICommandResult"/> which should be executed.
        /// </param>
        /// <returns>
        /// A <see cref="ICommandResultExecutor"/> which can be used to execute
        /// results of type <paramref name="resultType"/>.
        /// </returns>
        ICommandResultExecutor GetExecutor(Type resultType);

        /// <summary>
        /// Gets an executor for the given <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of <see cref="ICommandResult"/> which should be executed.
        /// </typeparam>
        /// <returns>
        /// A <see cref="ICommandResultExecutor"/> which can be used to execute
        /// results of type <typeparamref name="TResult"/>.
        /// </returns>
        ICommandResultExecutor<TResult> GetExecutor<TResult>()
            where TResult : ICommandResult;
    }
}
