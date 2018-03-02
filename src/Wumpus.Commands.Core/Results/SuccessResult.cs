namespace Wumpus.Commands
{
    /// <summary>
    /// Internal struct used to represent unknown success results.
    /// </summary>
    internal struct SuccessResult : ICommandResult
    {
        public static readonly SuccessResult Instance
            = new SuccessResult();

        public bool IsSuccess
            => true;
    }
}
