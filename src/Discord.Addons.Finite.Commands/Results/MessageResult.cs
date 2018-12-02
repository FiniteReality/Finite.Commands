using Finite.Commands;

namespace Discord.Addons.Finite.Commands
{
    /// <summary>
    /// An implementation of <see cref="IResult" /> representing a message to
    /// send to Discord.
    /// </summary>
    public class MessageResult : IResult
    {
        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MessageResult"/>
        /// </summary>
        /// <param name="content">
        /// The content to send
        /// </param>
        public MessageResult(string content)
        {
            Content = content;
        }

        /// <inheritdoc/>
        bool IResult.IsSuccess => true;
    }
}
