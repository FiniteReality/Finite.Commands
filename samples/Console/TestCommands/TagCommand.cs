using System.Threading.Tasks;
using Finite.Commands;
using Finite.Commands.AttributedModel;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands
{
    public class TagModule : Module
    {
        private readonly ILogger _logger;

        public TagModule(ILogger<TagModule> logger)
        {
            _logger = logger;
        }

        [Command("tag")]
        public ValueTask<ICommandResult> TagCommand(
            [Remainder]string tag)
        {
            _logger.LogInformation("Getting tag {tag}", tag);

            return new ValueTask<ICommandResult>(new NoContentCommandResult());
        }
    }
}
