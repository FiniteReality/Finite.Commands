using System;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// Represents a typed pipeline.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Executes the pipeline.
        /// </summary>
        /// <param name="context">
        /// The execution context of the pipeline.
        /// </param>
        /// <param name="next">
        /// A callback to invoke the next pipeline in the sequence.
        /// </param>
        /// <returns>
        /// A <see cref="IResult"/> containing any results from this pipeline,
        /// or following pipelines.
        /// </returns>
        Task<IResult> ExecuteAsync(CommandExecutionContext context,
            Func<Task<IResult>> next);
    }
}
