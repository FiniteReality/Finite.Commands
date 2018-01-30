namespace Wumpus.Commands
{
    internal interface IModule
    {
        ICommandContext Context { get; }
    }
}
