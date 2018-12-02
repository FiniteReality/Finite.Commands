using System;
using Discord.WebSocket;
using Finite.Commands;

namespace Discord.Addons.Finite.Commands
{
    /// <summary>
    /// An implementation of <see cref="ICommandContext" /> specialised for
    /// Discord.Net.
    /// </summary>
    public class SocketCommandContext : ICommandContext
    {
        /// <summary>
        /// Gets the client that the command is executed with.
        /// </summary>
        public DiscordSocketClient Client { get; }
        /// <summary>
        /// Gets the message that the command is interpreted from.
        /// </summary>
        public SocketMessage Message { get; }
        /// <summary>
        /// Gets the channel that the command was executed in.
        /// </summary>
        public ISocketMessageChannel Channel { get; }
        /// <summary>
        /// Gets the author of the command.
        /// </summary>
        public SocketUser Author { get; }
        /// <summary>
        /// Gets the guild that the command was executed in.
        /// </summary>
        public SocketGuild Guild { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        /// <summary>
        /// Initializes a new <see cref="SocketCommandContext" /> with the
        /// given client and message.
        /// </summary>
        /// <param name="client">
        /// The <see cref="Discord.WebSocket.DiscordSocketClient" /> to use.
        /// </param>
        /// <param name="message">
        /// The <see cref="Discord.WebSocket.SocketMessage" /> to use.
        /// </param>
        public SocketCommandContext(DiscordSocketClient client,
            SocketMessage message)
        {
            Client = client;
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (Channel as SocketGuildChannel)?.Guild;
        }

        /// <inheritdoc/>
        string ICommandContext.Message => throw new NotImplementedException();

        /// <inheritdoc/>
        string ICommandContext.Author => throw new NotImplementedException();
    }
}
