namespace Wumpus.Commands
{
    public interface ICommandContext
    {
        string Message { get; }
        string Author { get; }
    }
}
