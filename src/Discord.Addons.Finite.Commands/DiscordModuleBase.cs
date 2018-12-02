using Finite.Commands;

namespace Discord.Addons.Finite.Commands
{
    /// <summary>
    /// A <see cref="ModuleBase{TContext}" /> specialised for use with
    /// Discord.Net.
    /// </summary>
    public abstract class DiscordModuleBase : ModuleBase<SocketCommandContext>
    {
        protected IResult Message(string body)
            => new MessageResult(content: body);
    }
}
