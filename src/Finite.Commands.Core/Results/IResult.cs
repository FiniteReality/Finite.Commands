namespace Finite.Commands
{
    /// <summary>
    /// Base interface for result types used by pipelines.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// The success value of this result.
        /// </summary>
        bool IsSuccess { get; }
    }
}
