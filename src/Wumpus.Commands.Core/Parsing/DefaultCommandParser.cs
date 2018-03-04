using System;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public class DefaultCommandParser : ICommandParser
    {
        public virtual string[] Tokenize(string commandText)
            => commandText.Split(" ".ToCharArray(),
                options: StringSplitOptions.RemoveEmptyEntries);

        public virtual async Task<ParseResult> ParseAsync<TContext>(
            CommandService<TContext> commands, string commandText)
            where TContext : class, ICommandContext<TContext>
        {
            string[] tokenStream = Tokenize(commandText);

            foreach (var match in commands.FindCommands(tokenStream))
            {
                //match.
            }

            return default(ParseResult);
        }
    }
}
