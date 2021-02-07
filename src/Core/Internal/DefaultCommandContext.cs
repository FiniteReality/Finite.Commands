namespace Finite.Commands
{
    internal sealed class DefaultCommandContext : CommandContext
    {
        public override CommandPath Path { get; set; } = CommandPath.Empty;
    }
}
